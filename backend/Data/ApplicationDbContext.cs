using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Enrollment> Enrollments { get; set; } = null!;
}
