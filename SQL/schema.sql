IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Products] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Category] nvarchar(max) NOT NULL,
    [ImageUrl] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251129180958_InitialCreate', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [Products] ADD [CreatedAt] datetime2 NULL;

ALTER TABLE [Products] ADD [UpdatedAt] datetime2 NULL;

CREATE TABLE [Orders] (
    [Id] int NOT NULL IDENTITY,
    [TotalAmount] decimal(18,2) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id])
);

CREATE TABLE [OrderItems] (
    [Id] int NOT NULL IDENTITY,
    [OrderId] int NOT NULL,
    [ProductId] int NOT NULL,
    [PriceAtPurchase] decimal(18,2) NOT NULL,
    [Quantity] int NOT NULL,
    CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);

CREATE INDEX [IX_OrderItems_ProductId] ON [OrderItems] ([ProductId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251129232231_AddOrdersAndOrderItems', N'10.0.0');

COMMIT;
GO

