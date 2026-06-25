# Loan Management System

A full-stack loan management application with a .NET 6 REST API and an Angular frontend.

## Architecture

- **Fundo.Domain** – domain entities and result types
- **Fundo.Application** – CQRS handlers, validation, and DTOs (MediatR + FluentValidation)
- **Fundo.Infrastructure** – EF Core, SQL Server, JWT authentication
- **Fundo.Applications.WebApi** – REST API, Serilog structured logging, Swagger
- **Fundo.Services.Tests** – unit and integration tests (xUnit)

## Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Node.js 20+](https://nodejs.org/) (for the Angular frontend)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (optional, for containerized setup)

## Quick Start with Docker

From the `backend/src` directory:

```sh
docker compose up --build
```

This starts:

| Service | URL |
|---------|-----|
| API | http://localhost:5000 |
| Swagger | http://localhost:5000/swagger |
| SQL Server | localhost:1433 |
| Seq (logs) | http://localhost:8081 |

Default SQL credentials: `sa` / `TestPassword123`

Seq admin password: `LoanPassword123`

## Local Development (without Docker)

### 1. Start SQL Server and Seq

```sh
cd backend/src
docker compose up fundo.database fundo.seq -d
```

### 2. Run the API

```sh
cd backend/src/Fundo.Applications.WebApi
dotnet run
```

The API listens on http://localhost:5000. Structured logs are sent to Seq at http://localhost:5341 and the console.

### 3. Run the Frontend

```sh
cd frontend
npm install
npm start
```

Open http://localhost:4200. The app authenticates automatically and loads seed data from the API.

## API Endpoints

All loan endpoints require a JWT Bearer token. Obtain a token via:

```http
POST /auth/token
Content-Type: application/json

{
  "username": "demo-user",
  "roles": ["LoanAdmin"]
}
```

| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| POST | `/loans` | LoanAdmin | Create a loan |
| GET | `/loans` | LoanManager, LoanAdmin | List all loans |
| GET | `/loans/{id}` | LoanManager, LoanAdmin | Get loan by id |
| POST | `/loans/{id}/payment` | LoanAdmin | Register a payment |

### Example loan response

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "amount": 1500.00,
  "currentBalance": 500.00,
  "applicantName": "Maria Silva",
  "status": "active"
}
```

## Running Tests

```sh
cd backend/src
dotnet test
```

Integration tests use an in-memory database and do not require SQL Server. The suite covers:

- **Authentication** (`/auth/token`) – token generation and role-based access
- **GET /loans** – list, authorization (401/403), seed data
- **GET /loans/{id}** – retrieve, not found (404), authorization
- **POST /loans** – create, validation (400), authorization (401/403)
- **POST /loans/{id}/payment** – balance reduction, paid status, validation, not found, authorization

## Logging (Serilog + Seq)

Structured logs are configured in `appsettings.json`:

- **Console** – always enabled
- **Seq** – `http://localhost:5341` (local) or `http://fundo.seq:5341` (Docker)

View logs in the Seq UI at http://localhost:8081 after starting the Seq container.

## CI/CD

GitHub Actions workflow (`.github/workflows/backend-ci.yml`) builds and tests the backend on every push/PR to `main`.

## Roles

| Role | Permissions |
|------|-------------|
| LoanManager | Read loans |
| LoanAdmin | Read and write loans, register payments |
