# E-commerce ASP.NET Core MVC Application

## Project Overview

This is a full-featured e-commerce web application built with ASP.NET Core MVC. The application provides both an admin panel for product management and a customer-facing storefront with shopping cart and order processing capabilities.

## Tech Stack

- **Framework**: ASP.NET Core MVC (.NET 10.0)
- **Database**: SQL Server (LocalDB)
- **ORM**: Entity Framework Core 10.0
- **Frontend**: Bootstrap 5, Razor Views, Bootstrap Icons
- **Session Management**: In-memory session storage for shopping cart
- **UI/UX**: Modern, responsive design with enhanced styling and animations

## Features

### Admin Panel
- **Product Management**: Full CRUD operations (Create, Read, Update, Delete)
- **Image Upload**: Support for product image uploads with file storage in `wwwroot/images/products/`
- **Image URL Support**: Alternative option to use external image URLs
- **Product Listing**: View all products in a table with images and actions
- **Order Management**: View all orders with details, item counts, and totals
- **Admin Navigation**: Dropdown menu for easy access to Products and Orders

### Storefront
- **Product Catalog**: Grid view of all available products with hover effects
- **Product Details**: Detailed product view with image and description
- **Shopping Cart**: Session-based shopping cart with:
  - Add items to cart (stays on store page for continued shopping)
  - Update quantities
  - Remove items
  - View cart total with tax calculation preview
  - Order summary sidebar
- **Order Processing**: Place orders with automatic pricing calculation and transaction safety

### Order Management
- **Order Creation**: Automatic order generation from cart items with database transaction support
- **Order Summary**: Detailed order confirmation page with itemized breakdown
- **Orders List**: View all orders with details (Order ID, date, items count, total amount)
- **Pricing Logic**: 10% tax applied to cart subtotal
- **Order History**: Orders stored in database with order items (persistent storage)
- **Transaction Safety**: Database transactions ensure all-or-nothing order creation

## Project Structure

```
EcommerceApp/
├── Controllers/
│   ├── HomeController.cs
│   ├── ProductsController.cs    # Admin product management
│   ├── StoreController.cs         # Customer storefront
│   └── OrdersController.cs        # Order processing
├── Models/
│   ├── Product.cs
│   ├── Order.cs
│   └── OrderItem.cs
├── ViewModels/
│   ├── CartItemViewModel.cs
│   └── OrderViewModel.cs
├── Services/
│   ├── ICartService.cs
│   ├── CartService.cs             # Session-based cart management
│   ├── IOrderService.cs
│   └── OrderService.cs            # Order creation and retrieval
├── ViewComponents/
│   └── CartCountViewComponent.cs  # Cart badge in navigation
├── Data/
│   └── ApplicationDbContext.cs     # EF Core DbContext
├── Views/
│   ├── Products/                  # Admin product views
│   ├── Store/                     # Storefront views
│   ├── Orders/                    # Order views
│   └── Shared/                    # Layout and components
├── wwwroot/
│   └── images/
│       └── products/              # Uploaded product images
└── Migrations/                    # EF Core migrations
```

## Architecture

The application follows a layered architecture:

1. **Controllers**: Handle HTTP requests, call services, return views
2. **Services**: Business logic layer (cart operations, order processing, pricing calculations)
3. **Models**: Entity models representing database tables
4. **ViewModels**: Data transfer objects for views
5. **DbContext**: Entity Framework Core for database operations
6. **Views**: Razor views for UI rendering

### Data Flow

- **Storefront Flow**: StoreController → CartService (Session) → Views
- **Order Flow**: OrdersController → OrderService (with Transaction) → CartService → DbContext → Database
- **Admin Flow**: ProductsController → DbContext → Database
- **Orders List Flow**: OrdersController → OrderService → DbContext → Database → Views

## Database Schema

### Products Table
- `Id` (int, Primary Key)
- `Name` (string, Required)
- `Description` (string, Required)
- `Price` (decimal, Required)
- `Category` (string, Required)
- `ImageUrl` (string, Optional)

### Orders Table
- `Id` (int, Primary Key)
- `TotalAmount` (decimal, Required)
- `CreatedAt` (datetime2, Required)

### OrderItems Table
- `Id` (int, Primary Key)
- `OrderId` (int, Foreign Key → Orders)
- `ProductId` (int, Foreign Key → Products)
- `PriceAtPurchase` (decimal, Required) - Stores price at time of purchase
- `Quantity` (int, Required)

## How to Run the Project

### Prerequisites
- .NET 10.0 SDK
- SQL Server LocalDB (or SQL Server Express)
- Visual Studio 2022 or VS Code (optional)

### Setup Steps

1. **Clone or download the project**

2. **Update Connection String** (if needed)
   - Open `appsettings.json`
   - Verify the connection string matches your SQL Server instance:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ECommerceDb;Trusted_Connection=True;"
   }
   ```

3. **Apply Database Migrations**
   ```bash
   dotnet ef database update
   ```
   This will create the database and apply all migrations.

4. **Run the Application**
   ```bash
   dotnet run
   ```
   Or press F5 in Visual Studio.

5. **Access the Application**
   - Open browser to `http://localhost:5079` (or the port shown in console)
   - Navigate to:
     - **Store**: `/Store` - Browse products
     - **Cart**: `/Store/Cart` - View shopping cart
     - **Admin → Products**: `/Products` - Manage products
     - **Admin → Orders**: `/Orders` - View all orders

## How to Apply Migrations

### Create a New Migration
```bash
dotnet ef migrations add MigrationName
```

### Apply Migrations to Database
```bash
dotnet ef database update
```

### Generate SQL Script (for submission)
```bash
dotnet ef migrations script -o SQL/schema.sql
```

### Rollback Migration
```bash
dotnet ef database update PreviousMigrationName
```

## Pricing Logic

The application implements **Option A: Add 10% Tax** to the cart subtotal.

### Implementation Details
- Location: `Services/OrderService.cs` in the `CreateOrderAsync` method
- Calculation:
  ```csharp
  var subtotal = cartItems.Sum(item => item.Subtotal);
  var tax = subtotal * 0.10m;
  var totalAmount = subtotal + tax;
  ```

### Alternative Options (commented in code)
- **Option B**: Apply fixed 5% discount
  ```csharp
  var totalAmount = subtotal * 0.95m;
  ```
- **Option C**: Add +20 EGP markup per product
  ```csharp
  var totalAmount = subtotal + (cartItems.Count * 20);
  ```

To change the pricing logic, modify the `CreateOrderAsync` method in `Services/OrderService.cs`.

## Features Summary

### Admin Features
- ✅ Create new products
- ✅ Edit existing products
- ✅ Delete products
- ✅ Upload product images
- ✅ View all products in a table
- ✅ View all orders with details
- ✅ Access orders list with item counts and totals

### Customer Features
- ✅ Browse product catalog (grid view)
- ✅ View product details
- ✅ Add products to shopping cart (stays on store page)
- ✅ Update cart item quantities
- ✅ Remove items from cart
- ✅ View cart total with tax preview
- ✅ Place orders with transaction safety
- ✅ View order confirmation with detailed breakdown

### Technical Features
- ✅ Session-based shopping cart
- ✅ Image file upload and storage
- ✅ Entity Framework Core migrations
- ✅ Responsive Bootstrap UI with modern design
- ✅ Bootstrap Icons for enhanced visual elements
- ✅ Cart item count badge in navigation
- ✅ Order history in database
- ✅ Database transaction support for order creation
- ✅ Enhanced UI with hover effects and animations
- ✅ Admin dropdown navigation menu

## Assumptions

1. **Shopping Cart**: Uses in-memory session storage (not persisted in database)
2. **Image Storage**: Uploaded images stored in `wwwroot/images/products/` directory
3. **Pricing Logic**: 10% tax applied to cart subtotal (as specified in requirements)
4. **Order Items**: Product prices are stored at purchase time (`PriceAtPurchase`) to preserve historical pricing
5. **No Authentication**: Admin and customer features are accessible without login (for simplicity)

## UI/UX Features

- **Modern Design**: Clean, professional interface with Bootstrap 5
- **Icons**: Bootstrap Icons throughout the application for better visual communication
- **Responsive Layout**: Works seamlessly on desktop, tablet, and mobile devices
- **Hover Effects**: Interactive product cards with smooth animations
- **Success Messages**: User-friendly feedback when actions are completed
- **Empty States**: Helpful messages when carts or lists are empty
- **Navigation**: Intuitive navigation with cart badge and admin dropdown menu

## Future Enhancements (Optional)

- User authentication and authorization
- Order history page for individual customers
- Product search and filtering
- Category-based product filtering
- Pagination for product and order listings
- Order status tracking (Pending, Shipped, Delivered)
- Email notifications for order confirmations
- Payment integration (Stripe, PayPal, etc.)
- Product reviews and ratings
- Order export (PDF, CSV)
- Sales analytics and reporting

## License

This project is created for educational/demonstration purposes.

