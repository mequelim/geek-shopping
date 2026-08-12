# `GeekShopping` project

#### .NET 10 • C# 14 • PostgreSQL • Docker

---

## Overview

+ The `GeekShopping` is a project created in the Microservices Architecture course, developed from scratch using C#, .NET, PostgreSQL, and Docker.

---

## 🛠 Development Environment

### Requirements

+ .NET 10 SDK;
+ Docker & Docker Compose;
+ PostgreSQL (if running outside Docker);
+ GitHub account (for CI/CD).

### Build project

```shell
dotnet build --configuration Release
```

### Restore dependencies

```shell
dotnet restore
```

---

## 🐳 Running with Docker and `.env`

### To run containers

```shell
docker compose up -d
```

### To stop containers

```shell
docker compose down
```

### To start API + PostgreSQL

```shell
docker compose up --build
```

### API will run on

```shell
http://localhost:8080
```

---

## 🗃 Migrations

### Update Entity Framework Core (EF Core)

```powershell
dotnet tool update --global dotnet-ef
```

### To add migration

```powershell
cd GeekShopping.ProductAPI
dotnet ef migrations add {MigrationName} --output-dir .\Infrastructure\Persistence\Migrations\
```

> [!NOTE]
>
> Change the `{MigrationName}` field to the name you want to give your migration!

### To apply migration

```powershell
cd GeekShopping.ProductAPI
dotnet ef database update
```

or

```powershell
dotnet ef database update --project .\GeekShopping.ProductAPI\GeekShopping.ProductAPI.csproj
```

### To remove a migration

```powershell
# cd GeekShopping.ProductAPI
dotnet ef migrations remove
```

---

## 📄 License

+ MIT License;
+ Free to use, modify, and distribute.

---

## 👨‍💻 Author

+ Pedro Henrique Miquelin da Silva;
+ Mobile and FullStack .NET Developer & Software Architecture;
+ Brazil, 🇧🇷
