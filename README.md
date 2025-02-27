# Core Banking API

A lightweight core banking API built with **.NET 9**, featuring authentication, transaction management, and logging.  

## Features
- **Authentication**: JWT-based authentication.
- **Bank Accounts**: Create and manage user accounts.
- **Transactions**: Deposit, withdraw, and transfer funds.
- **Logging & Monitoring**: Integrated with **Serilog** and **HealthChecks**.
- **Testing**: Uses **xUnit**, **Moq**, and **InMemory database**.

## Tech Stack
- **.NET 9** (OpenAPI + Scalar for replacing Swagger)
- **SQL Server** (Supports SQLite if needed)
- **Entity Framework Core**
- **AutoMapper** (Object mapping)
- **FluentResults** (Error handling)
- **Serilog** (Logging)
- **MailKit / MimeKit** (For future email confirmation)
