# SQL Script for Database Creation

This folder contains the SQL script to create the database schema.

## File

- **schema.sql** - Complete SQL script to create the entire database schema

## How to Use

### Option 1: Using SQL Server Management Studio (SSMS)

1. Open SQL Server Management Studio
2. Connect to your SQL Server instance (e.g., `(localdb)\MSSQLLocalDB`)
3. Open `schema.sql` file
4. Execute the script (Press F5 or click Execute)

### Option 2: Using Command Line (sqlcmd)

```bash
sqlcmd -S (localdb)\MSSQLLocalDB -i schema.sql
```

### Option 3: Using Azure Data Studio

1. Open Azure Data Studio
2. Connect to your SQL Server
3. Open `schema.sql`
4. Run the script

## What This Script Creates

The script creates the following database structure:

1. **Products Table**
   - Id (Primary Key)
   - Name, Description, Price, Category
   - ImageUrl (optional)
   - CreatedAt, UpdatedAt (timestamps)

2. **Orders Table**
   - Id (Primary Key)
   - TotalAmount
   - CreatedAt

3. **OrderItems Table**
   - Id (Primary Key)
   - OrderId (Foreign Key → Orders)
   - ProductId (Foreign Key → Products)
   - PriceAtPurchase
   - Quantity

4. **Users Table**
   - Id (Primary Key)
   - Username
   - PasswordHash
   - Email

5. **Migration History Table**
   - Tracks applied migrations

## Database Name

The script will create tables in the database specified in your connection string. By default, the application uses `ECommerceDb`.

## Notes

- This script includes all migrations combined into a single SQL file
- It can be run on a fresh database or an existing database
- The script uses transactions to ensure all-or-nothing execution
- Foreign key constraints are included with appropriate delete behaviors

