using EduCodePlatform.Models;
using Microsoft.EntityFrameworkCore;
using EduCodePlatform.Models;

namespace EduCodePlatform.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CodeSubmission> CodeSubmissions { get; set; }
        public DbSet<ProgrammingLanguage> ProgrammingLanguages { get; set; }
        public DbSet<EditorSetting> EditorSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // унікальність імені для ProgrammingLanguage:
            modelBuilder.Entity<ProgrammingLanguage>()
                        .HasIndex(pl => pl.Name)
                        .IsUnique();
        }
    }
}
