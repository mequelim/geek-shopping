# Fixing Authentication And Authorization on Delete a Product

---

## Summary

The `DELETE /api/Products/DeleteProduct/{id}` endpoint was returning `403 Forbidden` when called from `GeekShopping.Web`. The root cause was a chain of independent problems tha needed to be resolved in sequence.

---

## Problems & Solutions

### 1. `invalid_scope` in Duende IdentityServer

+ **Problem:** the web was requesting the `role` scope in `AddOpenIdConnect`, but it was not registered in the IdentityServer.
+ **Solution:** register the `role` `IdentityResource` and include it in the client's `AllowedScopes`:

  ```csharp
  // IdentityServerConfiguration.cs
  new IdentityResource("role", "User role", [ "role" ])

  // Client `geek_shopping`:
  AllowedScopes = [ ..., "role" ]
  ```

---

### 2. `invalid_grant` in Duende IdentityServer

+ **Problem:** the `role` claim was not included in the `access_token` because an `ApiResource` with `UserClaims` declaring that the role should be included was missing;
+ **Solution:**  declare an `ApiResource` with `UserClaims = { role }`:

  ```csharp
  new ApiResource("geek_shopping", "GeekShopping API")
  {
      Scopes = { "geek_shopping" },
      UserClaims = { JwtClaimTypes.Role, "role" }
  }
  ```

And, register it in IdentityServer:

```csharp
.AddInMemoryApiResources(IdenityServerConfiguration.ApiResources);
```

---

### 3. `TestUserStore` not registered — Login broken

+ **Problem:** the Duende Razor Pages (Login) depended on TestUserStore injected in the `Index.cshtml.cs` constructor, but it was not registered in the DI container;
+ **Solution:** remove the `TestUserStore` dependency from the `Login` constructor, since the project already used `UserManager<ApplicationUser>` and `SignInManager<ApplicationUser>` directly.

---

### 4. Conflict between AddIdentity and AddAspNetIdentity

+ **Problem:** both `AddIdentity` and AddAspNetIdentity were registering `IUserClaimsPrincipalFactory`, causing `InvalidOperationException: Decorator already registered`;
+ **Solution:** use only AddIdentity with `AddProfileService<ProfileService>()` in IdentityServer, without `AddAspNetIdentity`.

---

### 5. `ProfileService` required to inject the role claim into tokens

+ **Problem:** without a custom `ProfileService`, IdentityServer did not include the role claim in tokens even when the user had the role assigned;
+ **Solution:** implement `IProfileService` to fetch user roles via UserManager and add them to the issued claims:

  ```csharp
  public async Task GetProfileDataAsync(ProfileDataRequestContext context)
  {
      ApplicationUser? user = await _userManager.GetUserAsync(context.Subject);
      IList<string> roles = await _userManager.GetRolesAsync(user);
      context.IssuedClaims.AddRange(
          roles.Select(role => new Claim(JwtClaimTypes.Role, role))
      );
  }
  ```

---

### 6. `[Timestamp]` with byte[] does not work natively in PostgreSQL

+ **Problem:** the RowVersion field on the `Product` entity was `byte[]` with `[Timestamp]`, which is a SQL Server mechanism;
  + In PostgreSQL, the generated values were empty, causing `Convert.FromBase64String` to fail and returning 400 Bad Request.
+ **Solution:** migrate to PostgreSQL's xmin system column, which is automatically updated on every row modification:

  ```csharp
  // Entity:
  [Timestamp]
  public uint Version { get; set; }

  // AppDbContext:
  modelBuilder.Entity<Product>()
      .Property(p => p.Version)
      .HasColumnName("xmin")
      .HasColumnType("xid")
      .ValueGeneratedOnAddOrUpdate()
      .IsConcurrencyToken();
  ```

The migration should only drop the physical row_version column — xmin is a PostgreSQL system column and must not be created manually:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropColumn(name: "row_version", table: "products");
}
```

---

### 7. Missing `MapInboundClaims = false` in ProductAPI

+ **Problem (root cause of the persistent 403):** the .NET JWT Bearer middleware, by default, remaps received claims to the long WS-Federation namespaces;
  + The `role` claim was arriving as `http://schemas.microsoft.com/ws/2008/06/identity/claims/role`, but `RoleClaimType = "role"` was looking for the short name — so `[Authorize(Roles = "Admin")]` always failed.
+ **Solution:** add `MapInboundClaims = false` to AddJwtBearer in ProductAPI:

```csharp
.AddJwtBearer((options) =>
{
    options.Authority = configuration["ServicesUrls:IdentityServer"];
    options.MapInboundClaims = false; // ← prevents remapping to long namespaces
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        RoleClaimType = "role"
    };
});
```

---

## Key Takeaway

The hardest problem to identify was `MapInboundClaims`. The token was correct, `RoleClaimType` was configured, but the middleware was silently remapping the `role` claim to the long namespace before the authorization check — making `[Authorize(Roles = "Admin")]` fail even when everything looked correct.

> [!IMPORTANT]
>
> Always add `MapInboundClaims = false` when configuring JWT Bearer with Duende IdentityServer.
