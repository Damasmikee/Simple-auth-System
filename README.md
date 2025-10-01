# AuthOnion - Minimal Authentication (Onion Architecture)

This project is a minimal authentication system built with ASP.NET Core Web API using an Onion-style folder organization.

## What it includes
- User Registration (secure password hashing with BCrypt)
- User Login (JWT token generation)
- Token Validation (JWT middleware to protect endpoints)
- Repository Pattern (IUserRepository implemented)
- Onion-style folders: Domain, Application, Infrastructure, Presentation (Controllers)
- Swagger UI for testing APIs
- EF Core InMemory provider for quick testing

## Requirements
- .NET 9 SDK
- `dotnet` CLI

## How to run
1. Open a terminal and navigate to the project root:
   ```bash
   cd /path/to/auth-onion/AuthOnion.Api
   ```
2. Restore, build and run:
   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```
3. Open Swagger UI:
   - `http://localhost:5000/` (Swagger UI)

## Postman
A `postman_collection.json` is included in the repository root. Import into Postman and set `{baseUrl}` to `http://localhost:5000`.

## Notes
- This project is intentionally minimal for learning purposes. For production:
  - Use a persistent database (SQL Server/Postgres).
  - Store JWT secrets in environment variables or secret manager.
  - Restrict Swagger in production environments.
