# CompanyPrinters Management System (ASP.NET Core API)

The central backend for the CompanyPrinters system. This RESTful API provides the data layer and business logic for modern Angular frontend application.

## Tech Stack
* **Framework:** ASP.NET Core 8.0 
* **Database:**  SQL Server
* **ORM:** Entity Framework Core
* **Language:** C#
* **Architecture:** Repository Pattern with RESTful Controllers

##  Key Features
* **Full CRUD API:** Endpoints for Printers, Users, and Designations.
* **Complex Queries:** Support for filtering, sorting, and pagination.
* **Data Validation:** Server-side validation and consistent error handling.
* **SQL Error Management:** Graceful handling of database constraints and connection issues.

##  Setup & Installation
1. Clone the repo: `git clone https://github.com/Ziph0/CompanyPrinters-api.git`
2. Update `appsettings.json` with your MySQL/SQL Server connection string.
3. Run `dotnet restore` to install dependencies.
4. Run `dotnet run` to start the API.
