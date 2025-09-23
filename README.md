# E-Commerce Inventory Management System (Backend)

![E-Commerce Inventory Banner](https://static.vecteezy.com/system/resources/thumbnails/012/494/550/small_2x/inventory-control-system-concept-professional-manager-and-worker-are-checking-goods-and-stock-supply-inventory-management-with-goods-demand-vector.jpg)

[![GitHub Repo stars](https://img.shields.io/github/stars/novelkhan/InventoryWebAPI?style=social)](https://github.com/novelkhan/InventoryWebAPI)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com/novelkhan/InventoryWebAPI/actions)
[![Deployed on Render](https://img.shields.io/badge/deployed_on-Render-blueviolet)](https://inventorywebapi.onrender.com)

A robust and scalable backend for the **E-Commerce Inventory Management System**, built with **.NET Core Web API**. This backend powers secure authentication, comprehensive CRUD operations, and efficient data management for products and categories, integrated with a modern Angular 16 frontend.

## Quick Links

| 🔗 **Information**           | 📍 **Details**                                                         |
|------------------------------|-------------------------------------------------------------------------|
| 🌐 **Live Demo**             | [View Live Application](https://inventory3.netlify.app)                 |
| 📂 **Frontend Repository**   | [GitHub Repo](https://github.com/novelkhan/InventoryFrontend.git)       |
| 📚 **Backend Repository**    | [GitHub Repo](https://github.com/novelkhan/InventoryWebAPI.git)         |
| 📖 **API Documentation**     | [Swagger (Dev Only)](http://localhost:7011/swagger/index.html)          |
| 🛠️ **Issues & Contributions** | [Report Issues](https://github.com/novelkhan/InventoryWebAPI/issues)   |

> **Note**: Swagger API documentation is available only in the development environment (`http://localhost:7011/swagger/index.html`). In production, Swagger is disabled for security reasons.

## Project Overview

The **E-Commerce Inventory Management System Backend** is a .NET Core Web API that provides the core functionality for managing e-commerce inventory. It follows **Domain-Driven Design (DDD)** principles with a layered architecture (Application, Domain, Infrastructure, Presentation) for clean and scalable code. The backend integrates with an Angular 16 frontend, deployed on Netlify, to deliver a seamless user experience.

### Key Highlights
- **Secure Authentication**: Implements JWT (JSON Web Tokens) for user registration, login, and authorization, with refresh token support for seamless session management.
- **Inventory Management**: Supports CRUD operations for products and categories, including filtering, pagination, search by name/description, and image upload (stored as Base64).
- **Architecture**: Uses DDD layered style with Repository Pattern and Unit of Work for efficient data access and transaction management.
- **ORM & Database**: Entity Framework Core for ORM, with Local MSSQL for development and PostgreSQL for production (hosted on Render's free tier).
- **API Documentation**: Swagger integrated for API exploration in development mode (disabled in production).
- **File Handling**: Product images are stored as Base64 strings in the database.
- **Code Quality**: Follows SOLID principles, clean code practices, and Git for version control.
- **Deployment**: Automated CI/CD pipeline using GitHub Actions for deployment to Render.

The backend is deployed on **Render**, while the frontend is hosted on **Netlify**, both with automated deployments for seamless updates.

## Features

### User Authentication
- **Register**: Create a new user with username, email, and hashed password (`POST /api/auth/register`).
- **Login**: Authenticate and return a JWT token (`POST /api/auth/login`).
- **Authorization**: All endpoints are protected with JWT; unauthorized requests return 403.
- **Refresh Token**: Automatically refreshes expired tokens for uninterrupted sessions.

### Product Management
- **Create Product**: Add a new product with name, description, price, stock, category, and optional image (`POST /api/products`).
- **List Products**: Retrieve a paginated list with filters (category, price range) and search by name/description (`GET /api/products`).
- **Get Single Product**: Fetch product details by ID, including category and image (`GET /api/products/{id}`).
- **Update Product**: Edit product details (`PUT /api/products/{id}`).
- **Delete Product**: Remove a product (`DELETE /api/products/{id}`).
- **Search Products**: Search products by name or description with pagination (`GET /api/products/search?q=keyword`).

### Category Management
- **Create Category**: Add a unique category with name and description (`POST /api/categories`).
- **List Categories**: Retrieve all categories with product counts (`GET /api/categories`).
- **Get Single Category**: Fetch category details by ID (`GET /api/categories/{id}`).
- **Update Category**: Edit category details (`PUT /api/categories/{id}`).
- **Delete Category**: Remove a category if no linked products exist (`DELETE /api/categories/{id}`); returns 409 if linked products are present.

### Additional Features
- **Error Handling**: Proper HTTP status codes (e.g., 404 for not found, 409 for conflicts).
- **Pagination & Filtering**: Efficient data retrieval with pagination and dynamic filtering.
- **Image Handling**: Supports image uploads stored as Base64 strings.
- **Session Management**: Supports frontend session timeout with countdown modal.

## Testing Credentials

To quickly test the application, use the following credentials in the live demo:

| Field       | Value                     |
|-------------|---------------------------|
| **Email**   | `novel4004@gmail.com`     |
| **Password**| `123456`                  |

> **Warning**: These credentials are provided solely for testing in the live demo environment. Do not use them in a production environment, and avoid sharing sensitive credentials in a real-world application.

## Tech Stack

### Backend
- **Framework**: .NET Core Web API (v8.0)
- **Architecture**: Domain-Driven Design (DDD) with layered structure (Application, Domain, Infrastructure, Presentation)
- **Design Patterns**: Repository Pattern, Unit of Work
- **ORM**: Entity Framework Core
- **Database**:
  - **Development**: Local MSSQL
  - **Production**: PostgreSQL (hosted on Render's free tier)
- **Authentication**: JWT with refresh tokens
- **Documentation**: Swagger (available in development only)
- **Other**: SOLID principles, clean code, Git for version control

### Frontend (Integrated)
- **Framework**: Angular 16 (standalone components, reactive forms, modern Angular features)
- **Styling**: SCSS with Bootstrap 5 for responsive design
- **State Management**: RxJS for asynchronous operations
- **Routing**: Lazy loading with route guards
- **Deployment**: Hosted on Netlify with automated CI/CD

## Installation

### Prerequisites
- **Node.js**: v16 or higher (for frontend)
- **.NET SDK**: 8.0 or higher
- **Database**:
  - Local MSSQL for development (LocalDB or full instance)
  - PostgreSQL for production (optional, if deploying to Render)
- **Git**: For cloning repositories
- **Angular CLI**: v16 or higher (`npm install -g @angular/cli`) (for frontend)

### Backend Setup
1. Clone the backend repository:
   ```bash
   git clone https://github.com/novelkhan/InventoryWebAPI.git
