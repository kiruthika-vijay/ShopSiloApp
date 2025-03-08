# **ShopSilo App**

**ShopSilo** is an e-commerce platform built with a .NET Web API backend and a React frontend. The app features shopping cart functionality, product listings, payment gateway integration (using Razorpay), user authentication, and a rich customer profile section with order management, reviews, and more.

---

## **Table of Contents**
- [Project Overview](#project-overview)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Installation and Setup](#installation-and-setup)
- [Running the Project](#running-the-project)
- [Environment Variables](#setting-up-env-files-for-secure-access-of-credentials)
- [Setting up the API credentials APPSETTINGS.JSON](#setting-up-the-api-credentials-appsettingsjson)

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
    "ShopSiloConStr": "Server=${DB_SERVER};Database=${DB_NAME};integrated security=True;TrustServerCertificate=True;"
  },
  "Razorpay": {
    "ApiKey": "${RAZORPAY_API_KEY}",
    "ApiSecret": "${RAZORPAY_API_SECRET}"
  },
  "Cloudinary": {
    "CloudName": "${CLOUDINARY_NAME}",
    "ApiKey": "${CLOUDINARY_API_KEY}",
    "ApiSecret": "${CLOUDINARY_API_SECRET}"
  },
  "GoogleAuthSettings": {
    "ClientId": "${GOOGLE_CLIENT_ID}",
    "ClientSecret": "${GOOGLE_CLIENT_SECRET}"
  },
  "AllowedHosts": "*",
  "Jwt": {
    "Issuer": "Issuer",
    "Audience": "Audience",
    "Key": "${JWT_SECRET_KEY}"
  },
  "ReactApp": {
    "REACT_APP_URL": "https://localhost:0000"
  }
}
```

## **Setting Up Env Files for Secure Access of Credentials**
### **Client .env file**

```bash
VITE_GOOGLE_CLIENT_ID={client_id}.apps.googleusercontent.com
```

### **Server .env file**

```bash
# Database Configuration
DB_SERVER=DB_SERVER
DB_NAME=DB_NAME
DB_USER=your_db_username
DB_PASS=your_db_password

# Razorpay API Keys
RAZORPAY_API_KEY=rzp_test_key
RAZORPAY_API_SECRET=razorpay secret

# Cloudinary API Keys
CLOUDINARY_NAME=cloud name
CLOUDINARY_API_KEY=cloud api key
CLOUDINARY_API_SECRET=cloud api secret

# Google OAuth Credentials
GOOGLE_CLIENT_ID={client_id}.apps.googleusercontent.com
GOOGLE_CLIENT_SECRET=client secret

# JWT Secret Key
JWT_SECRET_KEY={jwt token}

#React App URL
REACT_APP_URL=https://localhost:0000
```




