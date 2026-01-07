# E-commerce ASP.NET Core MVC Application

## Project Overview

This is a full-featured e-commerce web application built with ASP.NET Core MVC. The application provides both an admin panel for product management and a customer-facing storefront with shopping cart and order processing capabilities.

## Tech Stack

- **Framework**: ASP.NET Core MVC (.NET 10.0)
- **Database**: SQL Server (LocalDB)
- **ORM**: Entity Framework Core 10.0
- **Frontend**: Bootstrap 5, Razor Views, Bootstrap Icons
- **Session Management**: In-memory session storage for shopping cart
- **Authentication**: 
  - Cookie-based authentication for MVC (browser-based login/register)
  - JWT (JSON Web Tokens) for API authentication
- **UI/UX**: Modern, responsive design with enhanced styling and animations

## Features

### Admin Panel
- **Product Management**: Full CRUD operations (Create, Read, Update, Delete)
- **Image Upload**: Support for product image uploads with file storage in `wwwroot/images/products/`
- **Image URL Support**: Alternative option to use external image URLs
- **Product Listing**: View all products in a table with images and actions
- **Order Management**: View all orders with details, item counts, and totals
- **Admin Navigation**: Dropdown menu for easy access to Products and Orders

### User Authentication (MVC)
- **User Registration**: 
  - Registration form with validation (username, email, password)
  - Duplicate username/email checking
  - Password hashing (SHA256)
  - Automatic login after registration
- **User Login**: 
  - Secure login with username and password
  - Password verification
  - Cookie-based authentication
  - Session management
- **User Logout**: Secure logout with session clearing
- **Protected Routes**: Automatic redirect to login for protected pages
- **User Session**: 30-minute session timeout with cookie expiration

### Storefront
- **Product Catalog**: Grid view of all available products with hover effects
- **Category Filtering**: Filter products by category using dropdown selector
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

### REST API
- **JWT Authentication**: Secure API access with JSON Web Tokens
- **User Registration & Login**: API endpoints for user management
- **Products API**: Full CRUD operations for products (with category filtering)
- **Cart API**: Shopping cart management via API
- **Orders API**: Order placement and retrieval via API
- **Authorization**: Protected endpoints require valid JWT token

## Project Structure

```
EcommerceApp/
├── Controllers/
│   ├── HomeController.cs
│   ├── AuthController.cs         # MVC user authentication (login/register)
│   ├── ProductsController.cs      # Admin product management (MVC)
│   ├── StoreController.cs         # Customer storefront
│   ├── OrdersController.cs        # Order processing
│   └── Api/                       # REST API Controllers
│       ├── AuthController.cs      # API user registration & login (JWT)
│       ├── ProductsApiController.cs # Products CRUD API
│       ├── CartApiController.cs    # Cart management API
│       └── OrdersApiController.cs  # Orders API
├── Models/
│   ├── Product.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   ├── User.cs                    # User model for API auth
│   ├── LoginRequest.cs            # API DTO
│   └── RegisterRequest.cs         # API DTO
├── ViewModels/
│   ├── CartItemViewModel.cs
│   └── OrderViewModel.cs
├── Services/
│   ├── ICartService.cs
│   ├── CartService.cs             # Session-based cart management
│   ├── IOrderService.cs
│   ├── OrderService.cs            # Order creation and retrieval
│   ├── IJwtService.cs
│   └── JwtService.cs             # JWT token generation
├── ViewComponents/
│   └── CartCountViewComponent.cs  # Cart badge in navigation
├── Data/
│   └── ApplicationDbContext.cs     # EF Core DbContext
├── Views/
│   ├── Auth/                      # Authentication views (Login, Register)
│   ├── Products/                  # Admin product views
│   ├── Store/                     # Storefront views
│   ├── Orders/                    # Order views
│   └── Shared/                    # Layout and components
├── wwwroot/
│   └── images/
│       └── products/              # Uploaded product images
├── Migrations/                    # EF Core migrations
└── SQL/
    └── schema.sql                 # Database schema script
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

### Users Table
- `Id` (int, Primary Key)
- `Username` (string, Required) - User login name
- `PasswordHash` (string, Required) - Hashed password (SHA256)
- `Email` (string, Required) - User email address

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
     - **Login**: `/Auth/Login` - User login page
     - **Register**: `/Auth/Register` - User registration page
     - **Store**: `/Store` - Browse products (with category filtering)
     - **Cart**: `/Store/Cart` - View shopping cart
     - **Admin → Products**: `/Products` - Manage products
     - **Admin → Orders**: `/Orders` - View all orders
   - **API Endpoints**: Available at `/api/*` (see API section below)

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

## User Authentication (MVC)

The application provides both MVC-based authentication (for browser users) and API-based authentication (for external clients).

### MVC Authentication (Browser)

**Login Page**: `http://localhost:5079/Auth/Login`
- Username and password login
- Cookie-based authentication
- 30-minute session timeout
- Automatic redirect to Store after login

**Registration Page**: `http://localhost:5079/Auth/Register`
- User registration with:
  - Username (3-50 characters, unique)
  - Email (valid format, unique)
  - Password (minimum 6 characters)
- Password hashing (SHA256)
- Duplicate username/email validation
- Automatic login after successful registration

**Security Features**:
- CSRF protection with anti-forgery tokens
- Password hashing before storage
- Encrypted authentication cookies
- Session management
- Input validation

**Authentication Flow**:
1. User registers/logs in via MVC pages
2. Password is hashed and stored in database
3. Authentication cookie is created with user claims
4. User session is established (30 minutes)
5. Protected routes automatically redirect to login if not authenticated

## REST API

The application provides a RESTful API for external clients (mobile apps, Postman, etc.). All API endpoints are prefixed with `/api`.

### Authentication

**Base URL**: `http://localhost:5079/api`

#### Register User
- **Endpoint**: `POST /api/auth/register`
- **Auth Required**: No
- **Request Body**:
  ```json
  {
    "username": "john_doe",
    "password": "password123",
    "email": "john@example.com"
  }
  ```
- **Response**: `200 OK` with user data

#### Login
- **Endpoint**: `POST /api/auth/login`
- **Auth Required**: No
- **Request Body**:
  ```json
  {
    "username": "john_doe",
    "password": "password123"
  }
  ```
- **Response**: `200 OK` with JWT token
  ```json
  {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "username": "john_doe"
  }
  ```

### Products API

#### Get All Products
- **Endpoint**: `GET /api/products`
- **Auth Required**: No
- **Query Parameters**: 
  - `category` (optional) - Filter by category (e.g., `?category=Electronics`)
- **Response**: Array of products

#### Get Single Product
- **Endpoint**: `GET /api/products/{id}`
- **Auth Required**: No
- **Response**: Product object

#### Create Product
- **Endpoint**: `POST /api/products`
- **Auth Required**: Yes (Bearer token)
- **Request Body**: Product object (Name, Description, Price, Category, ImageUrl)
- **Response**: Created product

#### Update Product
- **Endpoint**: `PUT /api/products/{id}`
- **Auth Required**: Yes (Bearer token)
- **Request Body**: Product object (without Id)
- **Response**: `204 No Content`

#### Delete Product
- **Endpoint**: `DELETE /api/products/{id}`
- **Auth Required**: Yes (Bearer token)
- **Response**: `204 No Content` or `400 Bad Request` if product is referenced in orders

### Cart API

All cart endpoints require authentication (Bearer token).

- **Get Cart**: `GET /api/cart`
- **Add to Cart**: `POST /api/cart/add` (Body: `{ "productId": 1, "quantity": 2 }`)
- **Update Cart Item**: `PUT /api/cart/update` (Body: `{ "productId": 1, "quantity": 3 }`)
- **Remove from Cart**: `DELETE /api/cart/remove/{productId}`
- **Clear Cart**: `POST /api/cart/clear`

### Orders API

All orders endpoints require authentication (Bearer token).

- **Place Order**: `POST /api/orders/place`
- **Get All Orders**: `GET /api/orders`
- **Get Single Order**: `GET /api/orders/{id}`

### Using the API

1. **Register/Login** to get a JWT token
2. **Include token** in Authorization header: `Authorization: Bearer {token}`
3. **Make requests** to protected endpoints with the token

Example with curl:
```bash
# Login
curl -X POST http://localhost:5079/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"john_doe","password":"password123"}'

# Use token in subsequent requests
curl -X GET http://localhost:5079/api/products \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

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
- ✅ User registration with validation
- ✅ Secure login with password hashing
- ✅ User logout functionality
- ✅ Cookie-based authentication
- ✅ Session management (30-minute timeout)
- ✅ Browse product catalog (grid view)
- ✅ Filter products by category
- ✅ View product details
- ✅ Add products to shopping cart (stays on store page)
- ✅ Update cart item quantities
- ✅ Remove items from cart
- ✅ View cart total with tax preview
- ✅ Place orders with transaction safety
- ✅ View order confirmation with detailed breakdown

### Technical Features
- ✅ Cookie-based authentication for MVC
- ✅ JWT authentication for API
- ✅ Password hashing (SHA256)
- ✅ CSRF protection with anti-forgery tokens
- ✅ Session-based shopping cart
- ✅ Session-based user authentication
- ✅ Image file upload and storage
- ✅ Entity Framework Core migrations
- ✅ Responsive Bootstrap UI with modern design
- ✅ Bootstrap Icons for enhanced visual elements
- ✅ Cart item count badge in navigation
- ✅ User authentication in navigation menu
- ✅ Order history in database
- ✅ Database transaction support for order creation
- ✅ Enhanced UI with hover effects and animations
- ✅ Admin dropdown navigation menu
- ✅ REST API with JWT authentication
- ✅ Category-based product filtering
- ✅ Foreign key constraint handling
- ✅ Comprehensive error handling
- ✅ Input validation with data annotations

## Assumptions

1. **Shopping Cart**: Uses in-memory session storage (not persisted in database)
2. **Image Storage**: Uploaded images stored in `wwwroot/images/products/` directory
3. **Pricing Logic**: 10% tax applied to cart subtotal (as specified in requirements)
4. **Order Items**: Product prices are stored at purchase time (`PriceAtPurchase`) to preserve historical pricing
5. **MVC Authentication**: 
   - User registration and login available via `/Auth/Login` and `/Auth/Register`
   - Cookie-based authentication with 30-minute session timeout
   - Protected routes redirect to login page if not authenticated
6. **API Authentication**: REST API endpoints require JWT authentication (except public endpoints like GetProducts)
7. **Category Filtering**: Case-insensitive category filtering available in both MVC storefront and API
8. **Password Security**: Passwords are hashed using SHA256 before storage (consider upgrading to bcrypt/PBKDF2 for production)

## UI/UX Features

- **Modern Design**: Clean, professional interface with Bootstrap 5
- **Icons**: Bootstrap Icons throughout the application for better visual communication
- **Responsive Layout**: Works seamlessly on desktop, tablet, and mobile devices
- **Hover Effects**: Interactive product cards with smooth animations
- **Success Messages**: User-friendly feedback when actions are completed
- **Empty States**: Helpful messages when carts or lists are empty
- **Navigation**: Intuitive navigation with cart badge and admin dropdown menu

## Future Enhancements (Optional)

- Upgrade password hashing to bcrypt or PBKDF2 for enhanced security
- Role-based authorization (Admin, Customer roles)
- Order history page for individual customers
- Password reset functionality
- Email verification for registration
- Product search functionality
- Pagination for product and order listings
- Order status tracking (Pending, Shipped, Delivered)
- Email notifications for order confirmations
- Payment integration (Stripe, PayPal, etc.)
- Product reviews and ratings
- Order export (PDF, CSV)
- Sales analytics and reporting
- API rate limiting
- Swagger/OpenAPI documentation

## License

This project is created for educational/demonstration purposes.

