using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MobileShopBilling.Domain.Entities;
using MobileShopBilling.Domain.Interfaces;

namespace MobileShopBilling.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly ITenantProvider _tenantProvider;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantProvider tenantProvider)
        : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<TaxConfiguration> TaxConfigurations => Set<TaxConfiguration>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Tenant>(e =>
        {
            e.HasIndex(t => t.Email).IsUnique();
            e.Property(t => t.ShopName).HasMaxLength(200);
            e.Property(t => t.Email).HasMaxLength(200);
        });

        builder.Entity<ApplicationUser>(e =>
        {
            e.HasOne(u => u.Tenant)
                .WithMany()
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Product>(e =>
        {
            e.HasIndex(p => p.TenantId);
            e.HasIndex(p => new { p.TenantId, p.Barcode }).IsUnique().HasFilter("\"Barcode\" IS NOT NULL");
            e.Property(p => p.SellingPrice).HasPrecision(18, 2);
            e.Property(p => p.PurchasePrice).HasPrecision(18, 2);
            e.HasQueryFilter(p => p.TenantId == _tenantProvider.GetCurrentTenantId());
        });

        builder.Entity<Customer>(e =>
        {
            e.HasIndex(c => c.TenantId);
            e.HasQueryFilter(c => c.TenantId == _tenantProvider.GetCurrentTenantId());
        });

        builder.Entity<Supplier>(e =>
        {
            e.HasIndex(s => s.TenantId);
            e.HasQueryFilter(s => s.TenantId == _tenantProvider.GetCurrentTenantId());
        });

        builder.Entity<TaxConfiguration>(e =>
        {
            e.HasIndex(t => t.TenantId);
            e.Property(t => t.Rate).HasPrecision(5, 2);
            e.HasQueryFilter(t => t.TenantId == _tenantProvider.GetCurrentTenantId());
        });

        builder.Entity<Invoice>(e =>
        {
            e.HasIndex(i => i.TenantId);
            e.HasIndex(i => new { i.TenantId, i.InvoiceNumber }).IsUnique();
            e.HasIndex(i => i.InvoiceDate);
            e.Property(i => i.SubTotal).HasPrecision(18, 2);
            e.Property(i => i.TaxAmount).HasPrecision(18, 2);
            e.Property(i => i.DiscountAmount).HasPrecision(18, 2);
            e.Property(i => i.TotalAmount).HasPrecision(18, 2);
            e.HasOne(i => i.Customer)
                .WithMany()
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasQueryFilter(i => i.TenantId == _tenantProvider.GetCurrentTenantId());
        });

        builder.Entity<InvoiceItem>(e =>
        {
            e.Property(i => i.UnitPrice).HasPrecision(18, 2);
            e.Property(i => i.DiscountPercent).HasPrecision(5, 2);
            e.Property(i => i.DiscountAmount).HasPrecision(18, 2);
            e.Property(i => i.TaxPercent).HasPrecision(5, 2);
            e.Property(i => i.TaxAmount).HasPrecision(18, 2);
            e.Property(i => i.TotalPrice).HasPrecision(18, 2);
            e.HasOne(i => i.Invoice)
                .WithMany(inv => inv.Items)
                .HasForeignKey(i => i.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Payment>(e =>
        {
            e.Property(p => p.Amount).HasPrecision(18, 2);
            e.HasIndex(p => p.InvoiceId).IsUnique();
            e.HasOne(p => p.Invoice)
                .WithOne(i => i.Payment)
                .HasForeignKey<Payment>(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<AuditLog>(e =>
        {
            e.HasIndex(a => a.TenantId);
            e.HasIndex(a => a.Timestamp);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantProvider.GetCurrentTenantId();

        foreach (var entry in ChangeTracker.Entries<TenantEntity>())
        {
            if (entry.State == EntityState.Added && tenantId.HasValue)
            {
                entry.Entity.TenantId = tenantId.Value;
            }
        }

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
