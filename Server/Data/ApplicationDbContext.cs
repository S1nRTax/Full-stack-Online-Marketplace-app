using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Server.Models;

namespace Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<AccessToken> AccessTokens { get; set; }
        public DbSet<PostModel> Posts { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1-to-1 Relationship between User and Customer
            modelBuilder.Entity<User>()
                .HasOne(u => u.Customer)
                .WithOne(c => c.User)
                .HasForeignKey<Customer>(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            // 1-to-1 Relationship between User and Vendor
            modelBuilder.Entity<User>()
                .HasOne(u => u.Vendor)
                .WithOne(v => v.User)
                .HasForeignKey<Vendor>(v => v.Id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            // 1-to-many Relationship between Vendor and Posts
            modelBuilder.Entity<PostModel>()
                .HasOne(p => p.Vendor)
                .WithMany(v => v.Posts)
                .HasForeignKey(v => v.VendorId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);


                
                

            // Define unique constraint for RefreshToken's token
            modelBuilder.Entity<AccessToken>()
                .HasIndex(r => r.Token)  // Index on token for faster lookups
                .IsUnique();

            // 1-to-1 Relationship between User and RefreshToken
            modelBuilder.Entity<AccessToken>()
                .HasOne(rt => rt.User)          
                .WithOne(u => u.AccessToken)   
                .HasForeignKey<AccessToken>(rt => rt.UserId)  
                .OnDelete(DeleteBehavior.Cascade);


           
        }
    }
}
