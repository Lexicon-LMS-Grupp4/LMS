using Domain.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMS.Blazor.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CourseTeacher>(entity =>
        {
            entity.HasKey(ct => new { ct.CourseId, ct.TeacherId });
        });

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.HasOne(u => u.Course)
                .WithMany(c => c.Students)
                .HasForeignKey(u => u.CourseId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
