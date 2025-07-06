using DSAR.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;

namespace DSAR.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserId)
                .IsUnique(); // 👈 This makes Email unique

            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<RequestActions>()
            //    .HasOne(ra => ra.Request)
            //    .WithOne(ra => ra.RequestActions)
            //    .HasForeignKey(ra => ra.RequestId)
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestActions>()
                .HasOne(ra => ra.Department)
                .WithMany()
                .HasForeignKey(ra => ra.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CaseStudy>()
                .HasOne(cs => cs.User)
                .WithMany()
                .HasForeignKey(cs => cs.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Or DeleteBehavior.NoAction

            //modelBuilder.Entity<Request>()
            //    .HasOne(r => r.Manager)
            //    .WithMany()
            //    .HasForeignKey(r => r.ManagerId)
            //    .OnDelete(DeleteBehavior.Restrict); // disable cascade for manager
            modelBuilder.Entity<RequestActions>()
        .HasOne(ra => ra.Section)
        .WithMany()
        .HasForeignKey(ra => ra.SectionId)
        .OnDelete(DeleteBehavior.Restrict); // 👈 disables cascade

            modelBuilder.Entity<RequestActions>()
      .HasOne(ra => ra.Department)
      .WithMany()
      .HasForeignKey(ra => ra.DepartmentId)
      .OnDelete(DeleteBehavior.Restrict); // 👈 disables cascade

            //added

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserId)
                .IsUnique();

            modelBuilder.Entity<User>()
             .HasOne(u => u.Department)
              .WithMany(d => d.Users)
              .HasForeignKey(u => u.DepartmentId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
             .HasOne(u => u.Section)
             .WithMany(s => s.Users)
             .HasForeignKey(u => u.SectionId)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<User>()
            //    .HasOne(u => u.Sector)
            //    .WithMany(se => se.Users)
            //    .HasForeignKey(u => u.SectorId)
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.City)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestActions>()
             .HasOne(ra => ra.FormData)
             .WithOne(r => r.RequestActions)
           .HasForeignKey<RequestActions>(ra => ra.RequestId)
          .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CaseStudy>()
            .HasOne(cs => cs.Request)
             .WithOne(f => f.CaseStudy)
              .HasForeignKey<CaseStudy>(cs => cs.RequestId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CaseStudy>()
                .HasOne(cs => cs.User)
                .WithMany(u => u.CaseStudies)
                .HasForeignKey(cs => cs.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<History>()
          .HasOne(h => h.FormData)
           .WithMany(r => r.Histories)
            .HasForeignKey(h => h.RequestId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<History>()
             .HasOne(h => h.FormData)
             .WithMany(r => r.Histories)
              .HasForeignKey(h => h.RequestId)
              .OnDelete(DeleteBehavior.Restrict);


            //end of added

            //New Form
            base.OnModelCreating(modelBuilder);
            base.OnModelCreating(modelBuilder);

            // Configure SnapshotFormData
            modelBuilder.Entity<SnapshotFormData>()
                .Property(s => s.FormDataJson)
                .HasColumnType("nvarchar(max)");

            // Main form relationships
            ConfigureMainFormRelationships(modelBuilder);

            // Snapshot relationships
            ConfigureSnapshotRelationships(modelBuilder);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CaseStudyAttachmentMetadata>()
    .HasOne(m => m.CaseStudyAttachmentData)
    .WithOne(d => d.CaseStudyAttachmentMetadata)
    .HasForeignKey<CaseStudyAttachmentData>(d => d.Id);


            // 1:1 between Metadata and Data, shared PK
            modelBuilder.Entity<CaseStudyAttachmentMetadata>()
                .HasOne(meta => meta.CaseStudyAttachmentData)
                .WithOne(data => data.CaseStudyAttachmentMetadata)
                .HasForeignKey<CaseStudyAttachmentData>(data => data.Id);

            // Many metadata records belong to one CaseStudy
            modelBuilder.Entity<CaseStudyAttachmentMetadata>()
                .HasOne(meta => meta.CaseStudy)
                .WithMany(cs => cs.Attachments)
                .HasForeignKey(meta => meta.CaseStudyId)
                .OnDelete(DeleteBehavior.Restrict); // or your preferred delete behavior
        }
        public DbSet<User> User { get; set; }
        //public DbSet<Request> Request { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Section> Section { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Sector> Sector { get; set; }
        public DbSet<RequestActions> RequestActions { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Levels> Level { get; set; }
        public DbSet<CaseStudy> CaseStudy { get; set; }
        public DbSet<History> Histories { get; set; }

        // Main tables
        public DbSet<FormData> Forms { get; set; }
        public DbSet<AttachmentMetadata> AttachmentMetadata { get; set; }
        public DbSet<AttachmentData> AttachmentData { get; set; }
        public DbSet<DescriptionEntry> DescriptionEntries { get; set; }
        // Snapshot tables
        public DbSet<SnapshotFormData> SnapshotForms { get; set; }
        public DbSet<SnapshotAttachmentMetadata> SnapshotAttachmentMetadatas { get; set; }
        public DbSet<SnapshotAttachmentData> SnapshotAttachmentDatas { get; set; }
        public DbSet<SnapshotDescriptionEntry> SnapshotDescriptionEntries { get; set; }
        public DbSet<CaseStudyAttachmentMetadata> CaseStudyAttachmentMetadata { get; set; }
        public DbSet<CaseStudyAttachmentData> CaseStudyAttachmentData { get; set; }

        private void ConfigureMainFormRelationships(ModelBuilder modelBuilder)
        {
            // Existing configurations...

            modelBuilder.Entity<DescriptionEntry>()
                .Property(d => d.Description1)
                .HasMaxLength(500); // Add appropriate length limits

            modelBuilder.Entity<DescriptionEntry>()
                .Property(d => d.Description2)
                .HasMaxLength(500);
        }

        private void ConfigureSnapshotRelationships(ModelBuilder modelBuilder)
        {
            // Existing configurations...

            modelBuilder.Entity<SnapshotDescriptionEntry>()
                .Property(d => d.Description1)
                .HasMaxLength(500);

            modelBuilder.Entity<SnapshotDescriptionEntry>()
                .Property(d => d.Description2)
                .HasMaxLength(500);
        }
    }

}