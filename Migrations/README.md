# Entity Framework Core Migrations

This folder contains all EF Core migrations used to create and update the database schema.

## Migration Files

### 1. InitialCreate (20251129180958)
- **File**: `20251129180958_InitialCreate.cs`
- **Purpose**: Creates the initial `Products` table
- **Creates**:
  - Products table with Id, Name, Description, Price, Category, ImageUrl

### 2. AddOrdersAndOrderItems (20251129232231)
- **File**: `20251129232231_AddOrdersAndOrderItems.cs`
- **Purpose**: Adds Orders and OrderItems tables, and timestamps to Products
- **Creates**:
  - Orders table (Id, TotalAmount, CreatedAt)
  - OrderItems table (Id, OrderId, ProductId, PriceAtPurchase, Quantity)
  - Foreign key relationships
  - Indexes for performance
  - Adds CreatedAt and UpdatedAt to Products table

### 3. AddUserTable (20251204002326)
- **File**: `20251204002326_AddUserTable.cs`
- **Purpose**: Adds Users table for API authentication
- **Creates**:
  - Users table (Id, Username, PasswordHash, Email)
  - Makes ImageUrl nullable in Products table

### 4. FixDecimalPrecision (20251204002523)
- **File**: `20251204002523_FixDecimalPrecision.cs`
- **Purpose**: Ensures decimal precision (18,2) for Price and TotalAmount columns
- **Note**: This migration is handled in `ApplicationDbContext.OnModelCreating()` method

## How to Use Migrations

### Prerequisites

1. Install EF Core Tools (if not already installed):
   ```bash
   dotnet tool install --global dotnet-ef
   ```

2. Ensure your connection string in `appsettings.json` is correct:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ECommerceDb;Trusted_Connection=True;"
   }
   ```

### Apply All Migrations

To create/update the database with all migrations:

```bash
dotnet ef database update
```

This will:
- Create the database if it doesn't exist
- Apply all pending migrations in order
- Create all tables, indexes, and constraints

### Apply Specific Migration

To apply migrations up to a specific point:

```bash
dotnet ef database update AddUserTable
```

### Rollback Migration

To rollback to a previous migration:

```bash
dotnet ef database update PreviousMigrationName
```

### Generate SQL Script from Migrations

To generate a SQL script from all migrations:

```bash
dotnet ef migrations script -o ../SQL/schema.sql
```

## Migration Files Structure

Each migration consists of:

- **`.cs` file** - Contains the `Up()` and `Down()` methods
  - `Up()` - Applies the migration (creates/modifies database)
  - `Down()` - Reverses the migration (rollback)
- **`.Designer.cs` file** - Auto-generated metadata (do not edit manually)
- **`ApplicationDbContextModelSnapshot.cs`** - Current state of the database model

## Migration Order

Migrations are applied in chronological order based on the timestamp in the filename:

1. `20251129180958_InitialCreate`
2. `20251129232231_AddOrdersAndOrderItems`
3. `20251204002326_AddUserTable`
4. `20251204002523_FixDecimalPrecision`

## Important Notes

- **Never delete migration files** after they've been applied to a production database
- **Always test migrations** on a development database first
- **Backup your database** before applying migrations in production
- The `ApplicationDbContextModelSnapshot.cs` file tracks the current model state
- Designer files (`.Designer.cs`) are auto-generated and should not be edited manually

## Troubleshooting

### Migration Already Applied

If you get an error that a migration is already applied:
```bash
dotnet ef database update --force
```

### Reset Database (Development Only)

⚠️ **Warning**: This will delete all data!

```bash
dotnet ef database drop
dotnet ef database update
```

### Check Migration Status

```bash
dotnet ef migrations list
```

