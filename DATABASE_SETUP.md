# Database Setup Guide

This document explains how to create the database for the E-Commerce application using either SQL scripts or EF Core migrations.

## Two Options Available

### Option 1: SQL Script (Standalone)
**Location**: `SQL/schema.sql`

A complete, standalone SQL script that can be executed directly in SQL Server Management Studio or via command line.

**Advantages**:
- ✅ No .NET tools required
- ✅ Can be reviewed and executed manually
- ✅ Works with any SQL Server client
- ✅ Single file contains everything

**See**: `SQL/README.md` for detailed instructions

---

### Option 2: Entity Framework Core Migrations
**Location**: `Migrations/` folder

EF Core migration files that can be applied using the `dotnet ef` command-line tool.

**Advantages**:
- ✅ Version-controlled database changes
- ✅ Can rollback migrations
- ✅ Tracks migration history
- ✅ Integrates with .NET development workflow

**See**: `Migrations/README.md` for detailed instructions

---

## Quick Start

### Using SQL Script

1. Open `SQL/schema.sql` in SQL Server Management Studio
2. Connect to your SQL Server instance
3. Execute the script (F5)

### Using EF Core Migrations

1. Open terminal in project root
2. Run: `dotnet ef database update`
3. Database will be created automatically

---

## Database Schema Overview

The database includes the following tables:

### Products
- Stores product information (Name, Description, Price, Category, ImageUrl)
- Primary Key: Id
- Timestamps: CreatedAt, UpdatedAt

### Orders
- Stores order transactions
- Primary Key: Id
- Fields: TotalAmount, CreatedAt

### OrderItems
- Stores individual items in each order
- Primary Key: Id
- Foreign Keys: OrderId → Orders, ProductId → Products
- Fields: PriceAtPurchase, Quantity

### Users
- Stores user accounts for API authentication
- Primary Key: Id
- Fields: Username, PasswordHash, Email

---

## Connection String

Default connection string (in `appsettings.json`):
```
Server=(localdb)\MSSQLLocalDB;Database=ECommerceDb;Trusted_Connection=True;
```

To use a different database, update the connection string before running migrations or SQL script.

---

## Which Option Should I Use?

- **Use SQL Script** if:
  - You want to review the exact SQL being executed
  - You're not using .NET/EF Core tools
  - You need to create the database manually
  - You're submitting the SQL script separately

- **Use EF Core Migrations** if:
  - You're developing with .NET
  - You want version-controlled database changes
  - You need to rollback changes
  - You want to integrate with CI/CD pipelines

Both options create the same database structure. Choose based on your preference and workflow.

