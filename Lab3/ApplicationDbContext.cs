using Lab3.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab3
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        :base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<UserBook>()
                .HasOne(userBook => userBook.Book)
                .WithMany(book => book.UserBooks)
                .HasForeignKey(userBook => userBook.BookID);

            modelBuilder
                .Entity<UserBook>()
                .HasOne(userBook => userBook.User)
                .WithMany(user => user.UserBooks)
                .HasForeignKey(book => book.UserID);


        }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<UserBook> UserBooks { get; set; }
    }
}