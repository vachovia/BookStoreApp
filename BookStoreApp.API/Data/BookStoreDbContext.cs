using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.API.Data;

// Switched base class From DbContext to IdentityDbContext
// To Migrate changes don't forget to remove already created tables: Authors and Books
// Also added code on line 23
public partial class BookStoreDbContext : IdentityDbContext<ApiUser>
{
    public BookStoreDbContext() { }

    public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options): base(options) { }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Added after switching DbContext by IdentityDbContext
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Authors__3214EC079E81F942");

            entity.Property(e => e.Bio).HasMaxLength(250);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Books__3214EC07A41576A2");

            entity.HasIndex(e => e.Isbn, "UQ__Books__447D36EACA7334D5").IsUnique();

            entity.Property(e => e.Image).HasMaxLength(250);
            entity.Property(e => e.Isbn)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("ISBN");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Summary).HasMaxLength(250);
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK_Books_ToAuthors");
        });

        // Added this code to seed initial Users and Roles
        // Then Add-Migration with Update-Database
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Name = "User",
                NormalizedName = "USER",
                Id = "70b4f51c-f566-4ea9-aa48-612759bbafec" // used online Guid generator
            },
            new IdentityRole
            {
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR",
                Id = "78a9a3b6-9647-49a1-b953-49bd82a175e6" // used online Guid generator
            }
        );

        var hasher = new PasswordHasher<ApiUser>();

        modelBuilder.Entity<ApiUser>().HasData(
            new ApiUser
            {
                Id = "8383f68f-764c-4427-92b7-6e98902f5f4e", // used online Guid generator
                Email = "admin@bookstore.com",
                NormalizedEmail = "ADMIN@BOOKSTORE.COM",
                UserName = "admin@bookstore.com",
                NormalizedUserName = "ADMIN@BOOKSTORE.COM",
                FirstName = "System",
                LastName = "Admin",
                PasswordHash = hasher.HashPassword(null, "P@ssword1")
            },
            new ApiUser
            {
                Id = "10851584-7191-4dee-a2b5-305ae30a9777", // used online Guid generator
                Email = "user@bookstore.com",
                NormalizedEmail = "USER@BOOKSTORE.COM",
                UserName = "user@bookstore.com",
                NormalizedUserName = "USER@BOOKSTORE.COM",
                FirstName = "System",
                LastName = "User",
                PasswordHash = hasher.HashPassword(null, "P@ssword1")
            }
        );

        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = "78a9a3b6-9647-49a1-b953-49bd82a175e6",
                UserId = "8383f68f-764c-4427-92b7-6e98902f5f4e"
            },
            new IdentityUserRole<string>
            {
                RoleId = "70b4f51c-f566-4ea9-aa48-612759bbafec",
                UserId = "10851584-7191-4dee-a2b5-305ae30a9777"
            }
        );

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
