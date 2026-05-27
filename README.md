# Mobile Shop Billing - SaaS Multi-Tenant Web Application

A production-ready, SaaS-based Mobile Shop Billing Web Application built with ASP.NET Core, Razor Views, and PostgreSQL.

## Architecture

```
┌──────────────────────────────────────────────────────┐
│                    Web Layer (MVC)                    │
│  Controllers │ Views (Razor) │ API Controllers       │
├──────────────────────────────────────────────────────┤
│               Application Layer                      │
│  Services │ DTOs │ Interfaces │ PDF Generator        │
├──────────────────────────────────────────────────────┤
│              Infrastructure Layer                    │
│  DbContext │ Repositories │ Middleware │ Identity     │
├──────────────────────────────────────────────────────┤
│                  Domain Layer                        │
│  Entities │ Enums │ Interfaces                       │
├──────────────────────────────────────────────────────┤
│                PostgreSQL Database                   │
└──────────────────────────────────────────────────────┘
```

## Tech Stack

- **Backend:** ASP.NET Core 10.0 (MVC + Web API)
- **Frontend:** Razor Views, Bootstrap 5, Bootstrap Icons
- **Database:** PostgreSQL with Entity Framework Core (Npgsql)
- **Authentication:** ASP.NET Core Identity (cookie-based)
- **PDF Generation:** QuestPDF
- **Excel Export:** ClosedXML
- **API Docs:** Swagger / OpenAPI

## Features

### Multi-Tenant Architecture
- Shared database with TenantId column on all tenant-specific tables
- Global query filters for automatic tenant data isolation
- Tenant middleware resolves tenant from authenticated user claims
- Zero cross-tenant data leakage

### Super Admin Module
- System-wide dashboard with stats
- Create new shops (tenants) with automatic provisioning
- View, activate, and deactivate shops
- Auto-creates admin user, roles, and default tax configurations

### Tenant (Shop) Features
- **Product Management:** Name, Brand, IMEI, Barcode, Price, Stock, Category
- **Customer Management:** Name, Email, Phone, Address, GST
- **Supplier Management:** Name, Contact, Email, Phone, GST
- **Tax/GST Configuration:** Multiple tax rates, default tax setting
- **User Management:** Add staff, assign roles (ShopAdmin, Staff)

### Billing Module
- Interactive invoice creation with product search
- Quantity, price, discount, and tax calculation
- Payment modes: Cash, Card, UPI
- Downloadable/printable PDF invoices
- Invoice cancellation with stock restoration

### Reports Module
- Daily/monthly sales reports with date range filters
- Product-wise sales breakdown
- Invoice history
- Excel/CSV export

### Additional Features
- Inventory tracking with stock management
- Low stock alerts
- Audit logging (created/updated timestamps)
- Responsive Bootstrap 5 UI with sidebar navigation
- Swagger API documentation

## Database Schema

### Tables
| Table | Description | Multi-Tenant |
|-------|------------|-------------|
| Tenants | Shop/tenant records | No (root) |
| AspNetUsers | Identity users with TenantId | Yes |
| AspNetRoles | SuperAdmin, ShopAdmin, Staff | No |
| Products | Product catalog | Yes |
| Customers | Customer records | Yes |
| Suppliers | Supplier records | Yes |
| TaxConfigurations | GST/tax rates | Yes |
| Invoices | Sales invoices | Yes |
| InvoiceItems | Line items | Via Invoice |
| Payments | Payment records | Via Invoice |
| AuditLogs | System audit trail | Yes |

## Folder Structure

```
MobileShopBilling/
├── MobileShopBilling.slnx
├── Dockerfile
├── docker-compose.yml
├── README.md
└── src/
    ├── MobileShopBilling.Domain/          # Entities, Enums, Interfaces
    │   ├── Entities/
    │   │   ├── BaseEntity.cs
    │   │   ├── TenantEntity.cs
    │   │   ├── Tenant.cs
    │   │   ├── ApplicationUser.cs
    │   │   ├── Product.cs
    │   │   ├── Customer.cs
    │   │   ├── Supplier.cs
    │   │   ├── TaxConfiguration.cs
    │   │   ├── Invoice.cs
    │   │   ├── InvoiceItem.cs
    │   │   ├── Payment.cs
    │   │   └── AuditLog.cs
    │   ├── Enums/
    │   │   ├── PaymentMode.cs
    │   │   └── InvoiceStatus.cs
    │   └── Interfaces/
    │       ├── ITenantProvider.cs
    │       └── IRepository.cs
    ├── MobileShopBilling.Application/     # Services, DTOs
    │   ├── DTOs/
    │   ├── Interfaces/
    │   └── Services/
    ├── MobileShopBilling.Infrastructure/  # Data access, Middleware
    │   ├── Data/
    │   │   └── ApplicationDbContext.cs
    │   ├── Repositories/
    │   │   └── Repository.cs
    │   ├── Middleware/
    │   │   └── TenantMiddleware.cs
    │   └── Services/
    │       └── TenantProvider.cs
    └── MobileShopBilling.Web/             # MVC Controllers & Views
        ├── Controllers/
        │   ├── AccountController.cs
        │   ├── HomeController.cs
        │   ├── SuperAdminController.cs
        │   ├── ProductsController.cs
        │   ├── CustomersController.cs
        │   ├── SuppliersController.cs
        │   ├── BillingController.cs
        │   ├── ReportsController.cs
        │   ├── TaxConfigController.cs
        │   ├── UsersController.cs
        │   └── Api/
        │       ├── ProductsApiController.cs
        │       └── InvoicesApiController.cs
        ├── Views/
        │   ├── Shared/_Layout.cshtml
        │   ├── Account/
        │   ├── Home/
        │   ├── SuperAdmin/
        │   ├── Products/
        │   ├── Customers/
        │   ├── Suppliers/
        │   ├── TaxConfig/
        │   ├── Billing/
        │   ├── Reports/
        │   └── Users/
        └── wwwroot/
            └── css/site.css
```

## Setup Instructions

### Prerequisites
- .NET 10.0 SDK
- PostgreSQL 14+

### Local Development

1. **Clone and configure database:**
   ```bash
   # Update connection string in appsettings.json
   # Default: Host=localhost;Port=5432;Database=MobileShopBilling;Username=postgres;Password=postgres
   ```

2. **Run the application:**
   ```bash
   cd src/MobileShopBilling.Web
   dotnet run
   ```
   The app automatically runs EF Core migrations and seeds:
   - Roles: SuperAdmin, ShopAdmin, Staff
   - Super Admin: `superadmin@mobileshop.com` / `Admin@123`

3. **Access:**
   - Web UI: https://localhost:5001
   - Swagger: https://localhost:5001/swagger

### Docker

```bash
docker-compose up --build
# Access at http://localhost:8080
```

## Default Credentials

| Role | Email | Password |
|------|-------|----------|
| Super Admin | superadmin@mobileshop.com | Admin@123 |

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/ProductsApi | List all products |
| GET | /api/ProductsApi/{id} | Get product by ID |
| GET | /api/ProductsApi/search?term= | Search products |
| GET | /api/ProductsApi/low-stock | Low stock products |
| POST | /api/ProductsApi | Create product |
| PUT | /api/ProductsApi/{id} | Update product |
| DELETE | /api/ProductsApi/{id} | Delete product |
| GET | /api/InvoicesApi | List all invoices |
| GET | /api/InvoicesApi/{id} | Get invoice by ID |
| POST | /api/InvoicesApi | Create invoice |
| GET | /api/InvoicesApi/{id}/pdf | Download invoice PDF |
| POST | /api/InvoicesApi/{id}/cancel | Cancel invoice |
| GET | /api/InvoicesApi/dashboard | Dashboard stats |
| GET | /api/InvoicesApi/reports/sales | Sales report |
| GET | /api/InvoicesApi/reports/products | Product sales report |

## Tenant Provisioning Flow

When Super Admin creates a new shop:
1. Tenant record created in `Tenants` table
2. Admin user created with `ShopAdmin` role
3. TenantId assigned to the user
4. Default GST configurations seeded (5%, 12%, 18%, 28%)
5. Login credentials provided to shop owner

## Security

- Cookie-based authentication with ASP.NET Core Identity
- Role-based authorization (SuperAdmin, ShopAdmin, Staff)
- Global query filters enforce tenant isolation
- Anti-forgery tokens on all forms
- Tenant validation in middleware on every request
