namespace API.Data;

public class StoreContext : IdentityDbContext<User, API.Entities.Role, int>
{
    public StoreContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<API.Entities.Product> Products { get; set; }
    public DbSet<Basket> Baskets { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>()
                .HasOne(a => a.Address)
                .WithOne()
                .HasForeignKey<UserAddress>(a => a.Id)
                .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<API.Entities.Role>()
        .HasData(
            new API.Entities.Role {Id = 1, Name = "Member", NormalizedName = "MEMBER" },
            new API.Entities.Role {Id = 2, Name = "Admin", NormalizedName = "ADMIN" }
        );
    }
}