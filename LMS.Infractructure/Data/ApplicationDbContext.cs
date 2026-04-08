using Domain.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<CourseTeacher> CourseTeachers { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Course
            builder.Entity<Course>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(c => c.Description)
                    .HasMaxLength(2000);
            });

            // Module
            builder.Entity<Module>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.Property(m => m.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(m => m.Description)
                    .HasMaxLength(2000);

                entity.HasOne(m => m.Course)
                    .WithMany(c => c.Modules)
                    .HasForeignKey(m => m.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Activity
            builder.Entity<Activity>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(a => a.Description)
                    .HasMaxLength(2000);

                entity.HasOne(a => a.Module)
                    .WithMany(m => m.Activities)
                    .HasForeignKey(a => a.ModuleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ApplicationUser -> Course (student belongs to course)
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasOne(u => u.Course)
                    .WithMany(c => c.Students)
                    .HasForeignKey(u => u.CourseId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // CourseTeacher (explicit many-to-many between Course and ApplicationUser)
            builder.Entity<CourseTeacher>(entity =>
            {
                entity.HasKey(ct => new { ct.CourseId, ct.TeacherId });

                entity.HasOne(ct => ct.Course)
                    .WithMany(c => c.CourseTeachers)
                    .HasForeignKey(ct => ct.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ct => ct.Teacher)
                    .WithMany(u => u.TeachingCourses)
                    .HasForeignKey(ct => ct.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Document
            builder.Entity<Document>(entity =>
            {
                entity.HasKey(d => d.Id);

                entity.Property(d => d.StoredFileName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(d => d.DisplayName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(d => d.Description)
                    .HasMaxLength(2000);

                entity.HasOne(d => d.UploadedByUser)
                    .WithMany(u => u.UploadedDocuments)
                    .HasForeignKey(d => d.UploadedByUserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict); // User must exist, can't delete user without handling documents first

                entity.HasOne(d => d.Course)
                    .WithMany(c => c.Documents)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.NoAction); // Document must be handled manually if course is deleted

                entity.HasOne(d => d.Module)
                    .WithMany(m => m.Documents)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.NoAction); // Document must be handled manually if module is deleted

                entity.HasOne(d => d.Activity)
                    .WithMany(a => a.Documents)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.NoAction); // Document must be handled manually if activity is deleted

                entity.HasOne(d => d.Submission)
                    .WithOne(s => s.Document)
                    .HasForeignKey<Document>(d => d.SubmissionId)
                    .OnDelete(DeleteBehavior.NoAction); // Document must be handled manually if submission is deleted

                // Ensure that exactly one of CourseId, ModuleId, ActivityId, SubmissionId is non-null
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Document_ExactlyOneOwner",
                    """
                    (
                        CASE WHEN CourseId IS NOT NULL THEN 1 ELSE 0 END +
                        CASE WHEN ModuleId IS NOT NULL THEN 1 ELSE 0 END +
                        CASE WHEN ActivityId IS NOT NULL THEN 1 ELSE 0 END +
                        CASE WHEN SubmissionId IS NOT NULL THEN 1 ELSE 0 END
                    ) = 1
                    """));
            });

            // Submission
            builder.Entity<Submission>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.Body)
                    .HasMaxLength(5000);


                entity.HasOne(s => s.Activity)
                    .WithMany(a => a.Submissions)
                    .HasForeignKey(s => s.ActivityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Student)
                    .WithMany(u => u.Submissions)
                    .HasForeignKey(s => s.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // SubmissionComment
            builder.Entity<SubmissionComment>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Text)
                    .IsRequired()
                    .HasMaxLength(5000);

                entity.HasOne(c => c.Submission)
                    .WithMany(s => s.Comments)
                    .HasForeignKey(c => c.SubmissionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Author)
                    .WithMany(u => u.SubmissionComments)
                    .HasForeignKey(c => c.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

            });
        }

    }
}
