using Microsoft.EntityFrameworkCore;
using ShopSiloAppFSD.Enums;
using ShopSiloAppFSD.Server.Models;
using System.Security.Claims;

namespace ShopSiloAppFSD.Models
{
    public class ShopSiloDBContext : DbContext
    {
        public ShopSiloDBContext()
        {
        }

        public ShopSiloDBContext(DbContextOptions<ShopSiloDBContext> options) : base(options) { }

        // User Management
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<SubscribedUsers> SubscribedUsers { get; set; }
        public virtual DbSet<CustomerDetail> CustomerDetails { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<Address> Address { get; set; }

        // Seller Management
        public virtual DbSet<Seller> Sellers { get; set; }
        public virtual DbSet<SalesData> SalesData { get; set; }
        public virtual DbSet<SellerDashboard> SellerDashboards { get; set; }

        // Product Management
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public DbSet<ColorVariation> ColorVariations { get; set; }
        public virtual DbSet<ProductReview> ProductReviews { get; set; }

        // Shopping Cart
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Wishlist> Wishlists { get; set; }
        public virtual DbSet<WishlistItem> WishlistItems { get; set; }

        // Orders and Payments
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<PaymentTransaction> Payments { get; set; }

        // Inventory
        public virtual DbSet<Inventory> Inventories { get; set; }

        // Discounts and Promotions
        public virtual DbSet<Discount> Discounts { get; set; }
        public virtual DbSet<UserDiscount> UserDiscounts { get; set; }

        // Order Tracking and Shipping
        public virtual DbSet<ShippingDetail> ShippingDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Address-CustomerDetail relationship
            modelBuilder.Entity<Address>()
                .HasOne(a => a.CustomerDetail)
                .WithMany(cd => cd.Addresses)
                .HasForeignKey(a => a.CustomerID)
                .OnDelete(DeleteBehavior.Cascade);  // Cascades delete if a CustomerDetail is deleted

            // Order-User relationship
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserID)
                .OnDelete(DeleteBehavior.Restrict);  // Prevents User deletion if associated with an Order

            // Order-Seller relationship
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Seller)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.SellerID)
                .OnDelete(DeleteBehavior.Restrict);  // Prevents Seller deletion if associated with an Order

            // Order-BillingAddress relationship
            modelBuilder.Entity<Order>()
                .HasOne(o => o.BillingAddress)
                .WithMany()
                .HasForeignKey(o => o.BillingAddressID)
                .OnDelete(DeleteBehavior.ClientSetNull);  // Set BillingAddressID to null in application code

            // Order-ShippingAddress relationship
            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingAddress)
                .WithMany()
                .HasForeignKey(o => o.ShippingAddressID)
                .OnDelete(DeleteBehavior.ClientSetNull);  // Set ShippingAddressID to null in application code

            // CartItem-Product relationship
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductID)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent Product deletion if associated with a CartItem

            // Category self-referencing relationship (Parent-SubCategory)
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent deletion of ParentCategory if it has subcategories

            // OrderItem-Order relationship
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderID)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete if Order is deleted

            // OrderItem-Product relationship
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductID)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent Product deletion if associated with an OrderItem

            // Payment-Order relationship
            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderID)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete if Order is deleted

            // Product-Seller relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Seller)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SellerID)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent Seller deletion if associated with Products

            // Make DiscountedPrice required during flash sale
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.DiscountedPrice).HasColumnType("decimal(18,2)");
                entity.Property(p => p.FlashSaleStart).HasColumnType("datetime");
                entity.Property(p => p.FlashSaleEnd).HasColumnType("datetime");
            });

            // ProductReview-Product relationship
            modelBuilder.Entity<ProductReview>()
                .HasOne(pr => pr.Product)
                .WithMany(p => p.ProductReviews)
                .HasForeignKey(pr => pr.ProductID)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete if Product is deleted

            // ProductReview-User relationship
            modelBuilder.Entity<ProductReview>()
                .HasOne(pr => pr.User)
                .WithMany(u => u.ProductReviews)
                .HasForeignKey(pr => pr.UserID)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent User deletion if associated with ProductReview

            // SalesData-Order relationship
            modelBuilder.Entity<SalesData>()
                .HasOne(sd => sd.Order)
                .WithMany(o => o.SalesData)
                .HasForeignKey(sd => sd.OrderID)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete if Order is deleted

            // ShippingDetail-Order relationship
            modelBuilder.Entity<ShippingDetail>()
                .HasOne(sd => sd.Order)
                .WithMany(o => o.ShippingDetails)  // This specifies that an Order can have many ShippingDetails
                .HasForeignKey(sd => sd.OrderID)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent Order deletion if associated with ShippingDetail

            // SellerDashboard-Seller relationship
            modelBuilder.Entity<SellerDashboard>()
                .HasOne(sd => sd.Seller)
                .WithOne(s => s.SellerDashboard)
                .HasForeignKey<SellerDashboard>(sd => sd.DashboardID)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete if Seller is deleted

            // CustomerDetail-User relationship
            modelBuilder.Entity<CustomerDetail>()
                .HasOne(cd => cd.User)
                .WithOne(u => u.CustomerDetail)
                .HasForeignKey<CustomerDetail>(cd => cd.CustomerID)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete if User is deleted

            modelBuilder.Entity<User>()
                .Property(p => p.Role)
                .HasConversion(
                    v => v.ToString(),
                    v => (UserRole)Enum.Parse(typeof(UserRole), v));

            modelBuilder.Entity<PaymentTransaction>()
                .Property(p => p.PaymentStatus)
                .HasConversion(
                    v => v.ToString(),
                    v => (PaymentStatus)Enum.Parse(typeof(PaymentStatus), v));

            modelBuilder.Entity<Category>()
                .Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (ApprovalStatus)Enum.Parse(typeof(ApprovalStatus), v));

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderStatus)
                .HasConversion(
                    v => v.ToString(),
                    v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v));

            modelBuilder.Entity<ShippingDetail>()
                .Property(s => s.ShippingStatus)
                .HasConversion(
                    v => v.ToString(),
                    v => (ShippingStatus)Enum.Parse(typeof(ShippingStatus), v));

            // Allow null values
            modelBuilder.Entity<User>()
                .Property(u => u.ResetToken)
                .HasMaxLength(100)
                .IsRequired(false); // This allows null values

            modelBuilder.Entity<User>()
                .Property(u => u.TokenExpiration)
                .HasMaxLength(100)
                .IsRequired(false); // This allows null values
        }
    }
}
