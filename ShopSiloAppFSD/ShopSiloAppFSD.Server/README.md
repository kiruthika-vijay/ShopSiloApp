# ShopSilo App

## Overview

**ShopSilo** is a powerful and user-friendly e-commerce platform built with .NET and React. It provides a seamless shopping experience, allowing users to browse products, manage their cart, and complete purchases with ease.

## Table of Contents

- [Features](#features)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Features

- User-friendly interface
- Product browsing and filtering
- Cart management and checkout
- User authentication with JWT
- Review and rating system
- Secure payment processing integration
- Responsive design for mobile and desktop

## Getting Started

### Prerequisites

Before you begin, ensure you have the following installed on your machine:

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/)
- [Git](https://git-scm.com/)

### Clone the Repository

### Ignore appsettings.json

Since it holds confidential secret keys for API integrations.

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
    "ApiKey": "RAZORPAY_API_KEY",
    "ApiSecret": "RAZORPAY_API_SECRET"
  },
  "Cloudinary": {
    "CloudName": "CLOUD_NAME",
    "ApiKey": "CLOUD_API_KEY",
    "ApiSecret": "CLOUD_API_SECRET"
  },
  "AllowedHosts": "*",
  "Jwt": {
    "Issuer": "Issuer",
    "Audience": "Audience",
    "Key": "RANDOM_JWT_KEY" //random key
  }
}
```

```bash
git clone https://github.com/kiruthika-vijay/ShopSiloApp.git
cd ShopSiloApp
```
