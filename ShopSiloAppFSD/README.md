# **ShopSilo App**

**ShopSilo** is an e-commerce platform built with a .NET Web API backend and a React frontend. The app features shopping cart functionality, product listings, payment gateway integration (using Razorpay), user authentication, and a rich customer profile section with order management, reviews, and more.

---

## **Table of Contents**
- [Project Overview](#project-overview)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Installation and Setup](#installation-and-setup)
- [Running the Project](#running-the-project)
- [API Endpoints](#api-endpoints)
- [Environment Variables](#environment-variables)
- [Testing](#testing)
- [Contributing](#contributing)
- [License](#license)

---

## **Project Overview**

**ShopSilo** is a complete e-commerce solution that includes the following features:
- **Frontend**: Built with React and Material-UI (MUI) components for responsive and interactive UI.
- **Backend**: Powered by .NET Web API with a focus on clean architecture and robust API design.
- **Payment Integration**: Razorpay is integrated for smooth and secure payment transactions.
- **Customer Profile**: Users can manage their profiles, including addresses, orders, and wishlist items.

### **Key Modules**
1. **Product Module**: Manage product listings, detailed product description pages, and user reviews.
2. **Cart and Checkout**: Add products to the cart, manage cart items, apply discounts, and proceed to checkout.
3. **Order Management**: View, cancel, or download invoices for orders.
4. **Payment Module**: Integrates Razorpay for transaction processing.
5. **Profile Management**: Users can update profile information, addresses, and view order/payment statuses.

---

## **Features**
- **JWT Authentication**: Secure user login and role-based authorization.
- **Razorpay Integration**: Seamless payment processing with transaction history.
- **Dynamic Cart and Wishlist**: Instant updates and user-specific cart/wishlist management.
- **Product Review System**: Customers can review and rate products.
- **Profile Dashboard**: A detailed profile page with sections for personal details, addresses, orders, payments, and wishlist.

---

## **Technologies Used**

### **Frontend**:
- React.js
- Material-UI (MUI)
- Tailwind CSS for custom styling
- i18n for localization

### **Backend**:
- ASP.NET Core Web API
- Entity Framework Core for database management
- SQL Server as the database
- JWT for authentication
- Razorpay for payment integration

### **Others**:
- Git for version control
- Google Drive for image hosting (optional)
- Docker (optional)


---

## **Installation and Setup**

### **Prerequisites**:
- **Node.js** (for frontend)
- **.NET SDK** (for backend)
- **SQL Server** (for database)
- **Razorpay account** (for payment integration)

### **Backend Setup**:
1. Clone the repository:
    ```bash
    git clone https://github.com/your-username/ShopSilo.git
    cd ShopSilo/server
    ```

2. Install dependencies:
    ```bash
    dotnet restore
    ```

3. Set up the database:
   - Configure your **connection string** in `appsettings.json`.
   - Run migrations:
     ```bash
     dotnet ef database update
     ```

4. Set up environment variables (for Razorpay integration):
   - Create a `.env` file and add your Razorpay credentials:
     ```
     RAZORPAY_KEY=your_razorpay_key
     RAZORPAY_SECRET=your_razorpay_secret
     ```

5. Run the backend server:
    ```bash
    dotnet run
    ```

### **Frontend Setup**:
1. Navigate to the client directory:
    ```bash
    cd ../client
    ```

2. Install dependencies:
    ```bash
    npm install
    ```

3. Start the development server:
    ```bash
    npm start
    ```

---

## **Running the Project**

To run the project:

1. Start the **backend server**:
   ```bash
   cd server
   dotnet run
   ```
2. Start the **frontend server**
   ```bash
   cd client
   npm start
   ```

## **Setting up the API credentials APPSETTINGS.JSON**

```bash
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "ShopSiloConStr": "server=SERVER_NAME;database=DATABASE_NAME;integrated security=True;TrustServerCertificate=True;"
  },
  "Razorpay": {
    "ApiKey": "RAZORPAY_APIKEY",
    "ApiSecret": "RAZORPAY_APISECRET"
  },
  "Cloudinary": {
    "CloudName": "CLOUDINARY_CLOUDNAME",
    "ApiKey": "CLOUDINARY_APIKEY",
    "ApiSecret": "CLOUDINARY_APISECRET"
  },
  "AllowedHosts": "*",
  "Jwt": {
    "Issuer": "Issuer",
    "Audience": "Audience",
    "Key": "JWT_RANDOM_KEY" //random key
  }
}
```



