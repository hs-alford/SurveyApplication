using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace SurveyApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<ProjectSite> ProjectSites { get; set; }
        public DbSet<SiteSurvey> SiteSurveys { get; set; }
        public DbSet<ImageModel> ImageModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProjectSite>()
                .HasMany(u => u.Surveys)
                .WithOne(u => u.ProjectSite);

            modelBuilder.Entity<ProjectSite>()
                .Property(b => b.DateCreated)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<SiteSurvey>()
                .Property(b => b.DateCreated)
                .HasDefaultValueSql("getdate()");

			modelBuilder.Entity<ProjectSite>()
			   .Property(b => b.LastModified)
			   .HasDefaultValueSql("getdate()");

			modelBuilder.Entity<SiteSurvey>()
				.Property(b => b.LastModified)
				.HasDefaultValueSql("getdate()");

		}
    }
}