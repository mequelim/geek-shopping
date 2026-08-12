# -------------------------------------
# Stage 01: Build
# -------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build

# Set working directory at repository root:
WORKDIR /app

# Copy entire repository:
COPY . .

# Move to the solution directory:
WORKDIR /app/src/GeekShopping

# Restore dependencies:
RUN dotnet restore GeekShopping.slnx

# Build the solution:
RUN dotnet build GeekShopping.slnx -c Release

# Publish the main application project:
RUN dotnet publish GeekShopping.ProductAPI/GeekShopping.ProductAPI.csproj \
    -c Release \
    -o /app/publish \
    --no-build

# -------------------------------------
# Stage 02: Runtime
# -------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

# Set runtime working directory:
WORKDIR /app

# Copy published output:
COPY --from=build /app/publish .

# Expose application port:
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "GeekShopping.ProductAPI.dll"]
