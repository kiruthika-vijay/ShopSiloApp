using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Repository;
using ShopSiloAppFSD.Server.DTO;
using ShopSiloAppFSD.Server.Interfaces;
using ShopSiloAppFSD.Server.Services;
using ShopSiloAppFSD.Services;
using System.Text;
using DotNetEnv;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Load .env file
        DotNetEnv.Env.Load();

        // Load configuration from environment variables
        builder.Configuration.AddEnvironmentVariables();

        // ✅ Load environment variables from .env
        DotNetEnv.Env.Load();

        var configuration = builder.Configuration;

        // ✅ Database Configuration
        var DB_SERVER = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
        var DB_NAME = Environment.GetEnvironmentVariable("DB_NAME") ?? "master";

        // ✅ Razorpay API Keys
        var RAZORPAY_API_KEY = Environment.GetEnvironmentVariable("RAZORPAY_API_KEY") ?? "";
        var RAZORPAY_API_SECRET = Environment.GetEnvironmentVariable("RAZORPAY_API_SECRET") ?? "";

        // ✅ Cloudinary API Keys
        var CLOUDINARY_NAME = Environment.GetEnvironmentVariable("CLOUDINARY_NAME") ?? "";
        var CLOUDINARY_API_KEY = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY") ?? "";
        var CLOUDINARY_API_SECRET = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET") ?? "";

        // ✅ Google OAuth Credentials
        var GOOGLE_CLIENT_ID = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? "";
        var GOOGLE_CLIENT_SECRET = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET") ?? "";

        // ✅ JWT Secret Key
        var JWT_SECRET_KEY = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "";

        // ✅ React App URL
        var REACT_APP_URL = Environment.GetEnvironmentVariable("REACT_APP_URL") ?? "https://localhost:5173";

        // ✅ Construct the Connection String
        string connectionString = $"Server={DB_SERVER};Database={DB_NAME};Integrated Security=True;TrustServerCertificate=True;";

        // ✅ Inject connection string into Configuration
        builder.Configuration["ConnectionStrings:ShopSiloConStr"] = connectionString;

        builder.Configuration["Razorpay:ApiKey"] = RAZORPAY_API_KEY;
        builder.Configuration["Razorpay:ApiSecret"] = RAZORPAY_API_SECRET;

        builder.Configuration["Cloudinary:CloudName"] = CLOUDINARY_NAME;
        builder.Configuration["Cloudinary:ApiKey"] = CLOUDINARY_API_KEY;
        builder.Configuration["Cloudinary:ApiSecret"] = CLOUDINARY_API_SECRET;

        builder.Configuration["GoogleAuthSettings:ClientId"] = GOOGLE_CLIENT_ID;
        builder.Configuration["GoogleAuthSettings:ClientSecret"] = GOOGLE_CLIENT_SECRET;

        builder.Configuration["Jwt:Key"] = JWT_SECRET_KEY;

        builder.Configuration["ReactApp:REACT_APP_URL"] = REACT_APP_URL;

        Console.WriteLine($"Connection String: {connectionString}");

        // Add services to the container
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });

        // Add CORS support
        builder.Services.AddCors(options => {
            options.AddPolicy("AllowReactApp", policy =>
            {
                policy.WithOrigins(REACT_APP_URL ?? "https://localhost:5173")
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
        });

        // Register Cloudinary service
        builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

        builder.Services.AddScoped<IRazorpayService>(provider =>
        {
            var apiKey = RAZORPAY_API_KEY ?? "";
            var apiSecret = RAZORPAY_API_SECRET ?? "";
            return new RazorpayService(apiKey, apiSecret);
        });

        // Register services
        builder.Services.AddScoped<DashboardService>();
        builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IEmailServiceConfiguration, EmailServiceConfiguration>();
        builder.Services.AddScoped<InvoiceService>();

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ICustomerDetailsRepository, CustomerDetailsRepository>();
        builder.Services.AddScoped<ISellerRepository, SellerRepository>();
        builder.Services.AddScoped<ISellerDashboardRepository, SellerDashboardRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
        builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
        builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
        builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
        builder.Services.AddScoped<IShippingDetailRepository, ShippingDetailRepository>();
        builder.Services.AddScoped<IAddressRepository, ShippingAddressRepository>();
        builder.Services.AddScoped<IAdminRepository, AdminRepository>();
        builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();

        builder.Services.AddScoped<IAuditLogConfiguration, AuditLogConfiguration>();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddControllersWithViews();

        // Configure DbContext
        builder.Services.AddDbContext<ShopSiloDBContext>(options =>
            options.UseSqlServer(connectionString ?? ""));

        // JWT Authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT_SECRET_KEY ?? ""))
            };
        })
        .AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = GOOGLE_CLIENT_ID ?? "";
            googleOptions.ClientSecret = GOOGLE_CLIENT_SECRET ?? "";
        });

        // Setup for API Versioning
        builder.Services.AddApiVersioning(o =>
        {
            o.DefaultApiVersion = new ApiVersion(1, 0);
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.ReportApiVersions = true;
        });

        // Configure Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Shop Silo API", Version = "v1" });
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token in this format: Bearer {your token}",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });

        // Add logging services
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        var app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop Silo API v1");
            });
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("AllowReactApp");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}