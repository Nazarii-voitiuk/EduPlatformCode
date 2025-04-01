using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EduCodePlatform.Models.Identity;         // ApplicationUser
using EduCodePlatform.Data.Entities;           // Усі сутності (Badge, etc.)
using EduCodePlatform.Models.Entities;         // CodeSubmission, etc.
using System;

namespace EduCodePlatform.Data
{
    // Наслідуємо від IdentityDbContext<...> – підтримка користувачів, ролей.
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ========== DbSet-и (всі сутності зі схеми) ==========

        // Онлайн-редактор
        public DbSet<CodeSubmission> CodeSubmissions { get; set; }
        public DbSet<CodeSubmissionHistory> CodeSubmissionHistories { get; set; }
        public DbSet<CodeTestResult> CodeTestResults { get; set; }
        public DbSet<CodeAnalysisReport> CodeAnalysisReports { get; set; }

        public DbSet<ProgrammingLanguage> ProgrammingLanguages { get; set; }

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
            // Базовий виклик, щоб Identity могла налаштувати свої таблиці
            base.OnModelCreating(modelBuilder);

            // ====== (1) Перейменуємо таблиці Identity, якщо хочете ======
            // Перейменувати AspNetUsers -> AppUser
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("AppUser"); // Якщо хочете
            });
            // Перейменувати AspNetRoles -> AppRole
            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable("AppRole");
            });
            // Аналогічно AspNetUserRoles -> AppUserRole, якщо треба
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("AppUserRole");
            });
            // AspNetUserClaims -> AppUserClaim (за потреби)
            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("AppUserClaim");
            });
            // AspNetUserLogins -> AppUserLogin
            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("AppUserLogin");
            });
            // AspNetRoleClaims -> AppRoleClaim
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("AppRoleClaim");
            });
            // AspNetUserTokens -> AppUserToken
            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("AppUserToken");
            });


            // ====== (2) Каскадні обмеження (Restrict) =========
            // Уникаємо multiple cascade paths

            // Приклад: CodingBattle -> CreatedByUser
            modelBuilder.Entity<CodingBattle>()
                .HasOne(cb => cb.CreatedByUser)
                .WithMany()
                .HasForeignKey(cb => cb.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // BattleParticipant -> CodingBattle
            modelBuilder.Entity<BattleParticipant>()
                .HasOne(bp => bp.CodingBattle)
                .WithMany()
                .HasForeignKey(bp => bp.BattleId)
                .OnDelete(DeleteBehavior.Restrict);

            // BattleParticipant -> AppUser
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

            // CodeSubmission -> AppUser => (вже [Required], можете вирішити onDelete)
            // EditorSetting -> AppUser => (можна Restrict або Cascade, залежно від вашої логіки)

            // Приклади унікальних індексів, якщо треба
            // modelBuilder.Entity<ProgrammingLanguage>()
            //     .HasIndex(p => p.Name)
            //     .IsUnique();
        }
    }
}
