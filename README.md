# .NET Core with Duende IdentityServer and JWT Authentication

This is a **learning-focused project** built with **ASP.NET Core** that demonstrates how to secure a **Web API** using **Duende IdentityServer** (the modern, actively maintained successor to IdentityServer4), JWT-based authentication, and OpenID Connect/OAuth 2.0 flows.

The project shows how a client application (or external clients) can authenticate users, obtain access tokens and refresh tokens, and securely consume protected API endpoints.

### Official Documentation
- Duende IdentityServer: [https://docs.duendesoftware.com/identityserver/v6/](https://docs.duendesoftware.com/identityserver/v6/)

Repo URL: https://github.com/Sumbal-Ayaz/MultiTenantsWeb

---
## 📌 Project Overview

This solution consists of multiple projects (or a single API project with IdentityServer hosted in-process):

### Main Components
- **API Project** (ASP.NET Core Web API)
  - Hosts **Duende IdentityServer** for authentication and token issuance
  - Secured with **JWT Bearer Authentication**
  - Uses **NSwag** for OpenAPI/Swagger documentation and client generation
  - Uses **ReDoc** and **Swagger UI** for interactive API documentation
  - Persists refresh tokens and operational data in PostgreSQL

- **Authentication Flow**
  - Supports **Resource Owner Password Credentials (ROPC)** grant (for learning/demo purposes)
  - Issues **access tokens** (JWT) and **refresh tokens**
  - Refresh tokens are persisted in the database

---
## 🎯 Learning Objectives

- Understand **OAuth 2.0**, **OpenID Connect**, and token-based security
- Implement **Duende IdentityServer** with ASP.NET Core Identity
- Issue and validate **JWT access tokens**
- Persist and manage **refresh tokens** using the operational store
- Secure APIs with `[Authorize]` and JWT Bearer authentication
- Configure clients, API scopes, and identity resources
- Integrate **NSwag** with selective OAuth2 security (lock icons only on protected endpoints)
- Explore in-process IdentityServer hosting and EF Core persistence

---
## 🛠 Technologies Used

- ASP.NET Core (.NET 10 or later)
- Duende IdentityServer
- ASP.NET Core Identity (with Entity Framework Core)
- PostgreSQL (via Npgsql)
- Duende.IdentityServer.EntityFramework (for persisted grants and configuration)
- NSwag (OpenAPI documentation & Swagger UI)
- ReDoc (alternative clean documentation UI)
- JWT Bearer Authentication

---
## 🚧 Current Status

- Solution and projects created
- ASP.NET Core Web API with controllers
- ASP.NET Core Identity configured with PostgreSQL
- Duende IdentityServer integrated (in-process hosting)
- Clients, API scopes, and identity resources configured
- JWT Bearer authentication implemented
- Refresh tokens persisted using operational store
- NSwag configured with selective OAuth2 security (locks only on `[Authorize]` endpoints)
- Swagger UI with OAuth2 client (ROPC flow) and ReDoc enabled
- Database migrations for both Identity and IdentityServer applied

---
## 🗄 Database Setup & Migrations

The project uses **PostgreSQL** with two separate contexts sharing the same database:

1. **ASP.NET Core Identity tables** (`ApplicationDbContext`)
   ```bash
   Add-Migration "IdentityTables" -Context ApplicationDbContext -OutputDir Data/Migrations/Identity
   Update-Database -Context ApplicationDbContext

2. **Duende IdentityServer operational tables** (`PersistedGrantDbContext` – for refresh tokens, codes, etc.)
   ```bash
   Add-Migration "InitialIdentityServerPersistedGrants" -Context PersistedGrantDbContext -OutputDir Data/Migrations/IdentityServer
   Update-Database -Context PersistedGrantDbContext

These commands create tables such as AspNetUsers, AspNetRoles, PersistedGrants, DeviceCodes, and Keys.

## 🚀 Running the Project
1. Update appsettings.json with your PostgreSQL connection string

   ```bash
   "ConnectionStrings": {
      "DefaultConnection": "Host=localhost;Database=YourDbName;Username=postgres;Password=yourpassword"
   }

2. Apply migrations (commands above)
3. Run the API project
   - IdentityServer runs at https://localhost:5001, https://localhost:7164/
   - Get token using Postman https://localhost:7164/content/token
   - Swagger UI available at /swagger
   - ReDoc available at /redoc
4. Use Swagger UI to:
   - Login via /Account/Login (or directly request token at /connect/token using OAuth2 settings)

## 🔐 Security Notes

- Resource Owner Password flow is used for demonstration/learning only (not recommended for production public clients)
- In production: prefer Authorization Code with PKCE, or external identity providers
- Replace AddDeveloperSigningCredential() with proper key management in production

## 📖 Future Plans

- Add a separate client web application (MVC/Razor Pages/Blazor) consuming the API
- Implement multi-tenancy features
- Add proper external login providers
- Move configuration data (clients/scopes) to database store

## 🤝 Contributions
This is a personal learning project, but suggestions, issues, and improvements are very welcome!

## 📄 License
This project is for learning and educational purposes. Feel free to fork and experiment.
