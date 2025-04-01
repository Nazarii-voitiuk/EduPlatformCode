using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EduCodePlatform.Models.Entities;
using EduCodePlatform.Data.Entities;

namespace EduCodePlatform.Data
{
    // Тепер контекст успадковується від IdentityDbContext<AppUser>
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ========== DbSet-и ==========

        // Таблиця користувачів (Identity автоматично створить AspNetUsers,
        // але ми можемо змінити це у OnModelCreating, щоб була AppUser)
        // Однак, якщо вам потрібно явно з нею працювати, лишаємо або видаляємо:
        public DbSet<AppUser> AppUsers { get; set; }

        // Таблиця мов програмування
        public DbSet<ProgrammingLanguage> ProgrammingLanguages { get; set; }

        // Онлайн-редактор
        public DbSet<CodeSubmission> CodeSubmissions { get; set; }
        public DbSet<CodeSubmissionHistory> CodeSubmissionHistories { get; set; }
        public DbSet<CodeTestResult> CodeTestResults { get; set; }
        public DbSet<CodeAnalysisReport> CodeAnalysisReports { get; set; }

        // Завдання та тестові кейси
        public DbSet<TaskDifficulty> TaskDifficulties { get; set; }
        public DbSet<CodingTask> Tasks { get; set; }
        public DbSet<TaskTestCase> TaskTestCases { get; set; }
        public DbSet<TaskSubmission> TaskSubmissions { get; set; }

        // Гейміфікація
        public DbSet<UserProgress> UserProgresses { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<CodingBattle> CodingBattles { get; set; }
        public DbSet<BattleParticipant> BattleParticipants { get; set; }
        public DbSet<DailyChallenge> DailyChallenges { get; set; }
        public DbSet<DailyChallengeSubmission> DailyChallengeSubmissions { get; set; }

        // Спільні проєкти та Pull Request’и
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectCollaborator> ProjectCollaborators { get; set; }
        public DbSet<PullRequest> PullRequests { get; set; }
        public DbSet<PullRequestComment> PullRequestComments { get; set; }

        // Налаштування редактора
        public DbSet<EditorSetting> EditorSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Базовий виклик: тепер важливо викликати base.OnModelCreating(),
            // щоб Identity змогла налаштувати себе.
            base.OnModelCreating(modelBuilder);

            // --- 1) Мапимо AppUser на таблицю "AppUser" (замість AspNetUsers)
            // Якщо хочете, щоб ваша сутність AppUser дійсно відображалась у табл. "AppUser"
            // (а не в "AspNetUsers"), можна додати такий код:
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.ToTable("AppUser");  // замість AspNetUsers

                // Якщо треба переназвати поля Identity:
                // entity.Property(u => u.Id).HasColumnName("UserId");
                // entity.Property(u => u.UserName).HasColumnName("UserName").IsRequired();
                // entity.Property(u => u.Email).HasColumnName("Email");
                // entity.Property(u => u.PasswordHash).HasColumnName("PasswordHash");
                // ...та інші поля, якщо потрібно.

                // Якщо у вас вже поля в AppUser: UserId, Email, CreatedAt:
                // не забудьте додати відповідні .HasColumnName(...) тут.
            });

            // ======================
            // 2) Обмеження каскадного видалення (Restrict) 
            //    — щоб уникнути multiple cascade paths
            // ======================

            // CodingBattle -> CreatedByUser
            modelBuilder.Entity<CodingBattle>()
                .HasOne(cb => cb.CreatedByUser)
                .WithMany()
                .HasForeignKey(cb => cb.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // BattleParticipant -> CodingBattle, -> AppUser
            modelBuilder.Entity<BattleParticipant>()
                .HasOne(bp => bp.CodingBattle)
                .WithMany()
                .HasForeignKey(bp => bp.BattleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BattleParticipant>()
                .HasOne(bp => bp.User)
                .WithMany()
                .HasForeignKey(bp => bp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // DailyChallenge -> AppUser
            modelBuilder.Entity<DailyChallenge>()
                .HasOne(dc => dc.CreatedByUser)
                .WithMany()
                .HasForeignKey(dc => dc.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // DailyChallengeSubmission -> CodeSubmission, -> DailyChallenge
            modelBuilder.Entity<DailyChallengeSubmission>()
                .HasOne(dcs => dcs.CodeSubmission)
                .WithMany()
                .HasForeignKey(dcs => dcs.CodeSubmissionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DailyChallengeSubmission>()
                .HasOne(dcs => dcs.DailyChallenge)
                .WithMany()
                .HasForeignKey(dcs => dcs.DailyChallengeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Project -> AppUser
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Owner)
                .WithMany()
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProjectCollaborator -> Project, -> AppUser
            modelBuilder.Entity<ProjectCollaborator>()
                .HasOne(pc => pc.Project)
                .WithMany()
                .HasForeignKey(pc => pc.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectCollaborator>()
                .HasOne(pc => pc.User)
                .WithMany()
                .HasForeignKey(pc => pc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // PullRequest -> Project, -> AppUser
            modelBuilder.Entity<PullRequest>()
                .HasOne(pr => pr.Project)
                .WithMany()
                .HasForeignKey(pr => pr.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PullRequest>()
                .HasOne(pr => pr.User)
                .WithMany()
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // PullRequestComment -> PullRequest, -> AppUser
            modelBuilder.Entity<PullRequestComment>()
                .HasOne(prc => prc.PullRequest)
                .WithMany()
                .HasForeignKey(prc => prc.PullRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PullRequestComment>()
                .HasOne(prc => prc.User)
                .WithMany()
                .HasForeignKey(prc => prc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Task (CodingTask) -> TaskDifficulty, -> AppUser
            modelBuilder.Entity<CodingTask>()
                .HasOne(t => t.Difficulty)
                .WithMany()
                .HasForeignKey(t => t.DifficultyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CodingTask>()
                .HasOne(t => t.CreatedByUser)
                .WithMany()
                .HasForeignKey(t => t.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // TaskTestCase -> Task
            modelBuilder.Entity<TaskTestCase>()
                .HasOne(ttc => ttc.Task)
                .WithMany()
                .HasForeignKey(ttc => ttc.TaskId)
                .OnDelete(DeleteBehavior.Restrict);

            // TaskSubmission -> Task, -> CodeSubmission
            modelBuilder.Entity<TaskSubmission>()
                .HasOne(ts => ts.Task)
                .WithMany()
                .HasForeignKey(ts => ts.TaskId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskSubmission>()
                .HasOne(ts => ts.CodeSubmission)
                .WithMany()
                .HasForeignKey(ts => ts.CodeSubmissionId)
                .OnDelete(DeleteBehavior.Restrict);

            // UserProgress -> AppUser
            modelBuilder.Entity<UserProgress>()
                .HasOne(up => up.User)
                .WithMany()
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // UserBadge -> AppUser, -> Badge
            modelBuilder.Entity<UserBadge>()
                .HasOne(ub => ub.User)
                .WithMany()
                .HasForeignKey(ub => ub.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Можна аналогічно налаштувати .WithMany(...) для колекцій,
            // якщо потрібні навігаційні властивості "List<...>"

            // ======================
            // Приклади індексів, унікальності, тощо
            // ======================
            // modelBuilder.Entity<ProgrammingLanguage>()
            //     .HasIndex(p => p.Name)
            //     .IsUnique();

            // modelBuilder.Entity<TaskDifficulty>()
            //     .HasIndex(d => d.DifficultyName)
            //     .IsUnique();
        }
    }
}
