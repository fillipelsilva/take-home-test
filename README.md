# Loan Management System

A simple loan management application built with **.NET 6** and **Angular 19** as a take-home project. The goal was to implement the requested features while keeping the solution organized, testable and easy to maintain.

---

## Features

### Backend

- Loan CRUD
- Loan payment registration
- JWT authentication
- Role-based authorization
- FluentValidation
- Swagger documentation
- Structured logging with Serilog and Seq
- Unit and integration tests

### Frontend

- Loan listing
- Create, edit and delete loans
- Register loan payments
- Responsive UI with Angular Material
- Automatic JWT authentication

---

## Architecture

The backend follows a Clean Architecture approach, separating the solution into four projects:

- **Domain** – Business entities and business rules.
- **Application** – Use cases, commands, queries and validations.
- **Infrastructure** – Database access, authentication and external services.
- **Web API** – Controllers, middleware and application configuration.

### Design Patterns

The project uses a few common patterns to keep the code organized and maintainable.

#### CQRS

Commands are responsible for changing the application's state, while queries are responsible only for reading data. This keeps each use case focused on a single responsibility.

#### MediatR

MediatR is used to dispatch commands and queries to their respective handlers, keeping controllers small and moving business logic into the application layer.

#### Repository Pattern

Repositories abstract the data access layer so the application remains independent from Entity Framework Core and is easier to test.

#### Result Pattern

Business operations return success or failure results instead of using exceptions for expected validation scenarios. This makes the execution flow more explicit.

#### Pipeline Behaviors

Validation, request logging and exception handling are implemented through MediatR pipeline behaviors, avoiding duplicated code across handlers.

#### Dependency Injection

ASP.NET Core's built-in dependency injection container is used to register services, repositories and application components, keeping the layers loosely coupled.

### Frontend

The frontend was developed with Angular 19 using:

- Standalone Components
- Reactive Forms
- Angular Material
- RxJS
- HTTP Interceptor for JWT authentication

---

## Tech Stack

### Backend

- .NET 6
- ASP.NET Core
- Entity Framework Core
- SQL Server
- MediatR
- FluentValidation
- AutoMapper
- Serilog
- Seq
- xUnit

### Frontend

- Angular 19
- TypeScript
- RxJS
- Angular Material

### DevOps

- Docker
- Docker Compose
- GitHub Actions

---

## Running the Project

### Backend

```bash
cd backend/src
docker compose up --build
```

Available services:

- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger
- Seq: http://localhost:8081

### Frontend

```bash
cd frontend
npm install
npm start
```

Application:

```
http://localhost:4200
```

---

## Authentication

The API is protected with JWT Bearer authentication.

Generate a token using:

```http
POST /auth/token
```

Use the returned token in the `Authorization` header when calling protected endpoints.

---

## Running Tests

Execute all backend tests:

```bash
dotnet test
```

The project includes:

- Unit tests for domain logic
- Integration tests for the API
- Authentication and authorization tests

---

## Future Improvements

Some improvements I would consider if the project continued to evolve:

- Refresh token support
- Payment history
- Pagination and filtering
- Health Check endpoint
- End-to-end tests
- Docker support for the frontend
- CI pipeline with code coverage reports