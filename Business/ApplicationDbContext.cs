using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<AccountDetail> AccountDetails { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<TypeOfArtwork> TypeOfArtworks { get; set; }
        public virtual DbSet<Artwork> Artworks { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Reaction> Reactions { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Follow> Follows { get; set; }
        public virtual DbSet<EventParticipants> EventParticipants { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectParticipant> ProjectParticipants { get; set; }
        public virtual DbSet<DocumentInfo> DocumentInfos { get; set; }


        // nếu thấy lỗi https thì hãy kiểm tra ở dbcontext đã có virtual
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", true, true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("Project"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure primary keys and relationships
            //role
            modelBuilder.Entity<Role>().HasData(
                new Role { IdRole = 1, RoleName = "Admin" },
                new Role { IdRole = 2, RoleName = "User" }
            );

            // Account
            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    IdAccount = 1,
                    Email = "Quang111420@gmail.com",
                    Password = "quang111420", // Note: In a real application, this should be hashed
                    IdRole = 1,
                    CreatedWhen = DateTime.Now,
                    LastUpdateWhen = DateTime.Now
                },
                new Account
                {
                    IdAccount = 2,
                    Email = "khang2007@gmail.com",
                    Password = "khang2007", // Note: In a real application, this should be hashed
                    IdRole = 2,
                    CreatedWhen = DateTime.Now,
                    LastUpdateWhen = DateTime.Now
                }
            );

            // AccountDetail
            modelBuilder.Entity<AccountDetail>().HasData(
                new AccountDetail
                {
                    IdAccountDt = 1,
                    Active = true,
                    Fullname = "Nguyễn Ngọc Quang",
                    IdAccount = 1,
                    CCCD = 123456789,
                    Nationality = "Vietnam",
                    Gender = "Nam",
                    CreatedWhen = DateTime.Now,
                    LastUpdateWhen = DateTime.Now,
                    Description = "Admin account",
                    CreatedBy = 1,// Assuming the admin updates their own account
                    LastUpdateBy = 1
                },
                new AccountDetail
                {
                    IdAccountDt = 2,
                    Active = true,
                    Fullname = "Minh Khang",
                    IdAccount = 2,
                    CCCD = 987654321,
                    Nationality = "Vietnam",
                    Gender = "Nam",
                    CreatedWhen = DateTime.Now,
                    LastUpdateWhen = DateTime.Now,
                    Description = "User account",
                    CreatedBy = 2,
                    LastUpdateBy = 1,// Assuming the admin (ID 1) updates this account
                }
            ) ;

            modelBuilder.Entity<Account>()
                .HasOne(a => a.AccountDetail)
                .WithOne(ad => ad.account)
                .HasForeignKey<AccountDetail>(ad => ad.IdAccount)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
               .HasMany(a => a.CreatedAd)
               .WithOne(ad => ad.Creator)
               .HasForeignKey(ad => ad.CreatedBy)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
               .HasMany(a => a.UpdatedAd)
               .WithOne(ad => ad.Updator)
               .HasForeignKey(ad => ad.LastUpdateBy)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Artworks)
                .WithOne(ar => ar.Account)
                .HasForeignKey(ar => ar.IdAc)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Comments)
                .WithOne(c => c.Account)
                .HasForeignKey(c => c.IdAc)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.CreatedDocuments)
                .WithOne(d => d.CreatedBy)
                .HasForeignKey(d => d.Created_by)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.UpdatedDocuments)
                .WithOne(d => d.LastUpdatedBy)
                .HasForeignKey(d => d.Last_update_by)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Reactions)
                .WithOne(ra => ra.Account)
                .HasForeignKey(ra => ra.IdAc)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Followers)
                .WithOne(f => f.Follower)
                .HasForeignKey(f => f.IdFollower)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Following)
                .WithOne(f => f.Following)
                .HasForeignKey(f => f.IdFollowing)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Projects)
                .WithOne(p => p.Account)
                .HasForeignKey(p => p.IdAc)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.CreatedArt)
                .WithOne(ar => ar.Creator)
                .HasForeignKey(ar => ar.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.UpdatedArt)
                .WithOne(ar => ar.Updater)
                .HasForeignKey(ar => ar.LastUpdateBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Events)
                .WithOne(e => e.Account)
                .HasForeignKey(e => e.IdAc)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Account>()
                .HasMany(a => a.CreatedE)
                .WithOne(e => e.Creator)
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.UpdatedE)
                .WithOne(e => e.Updater)
                .HasForeignKey(e => e.LastUpdateBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.EPAccDt)
                .WithOne(ep => ep.Account)
                .HasForeignKey(ep => ep.IdAc)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Account>()
                .HasMany(a => a.DocumentInfos)
                .WithOne(d => d.Account)
                .HasForeignKey(d => d.IdAc)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Account>()
                .HasMany(a => a.PjAccDt)
                .WithOne(pp => pp.Account)
                .HasForeignKey(pp => pp.IdAc)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>()
                .HasMany(r => r.RoleAccount)
                .WithOne(a => a.AccountRole)
                .HasForeignKey(a => a.IdRole)
                .OnDelete(DeleteBehavior.Restrict);
            // artwork
            modelBuilder.Entity<Artwork>()
                .HasMany(a => a.Comments)
                .WithOne(c => c.Artwork)
                .HasForeignKey(c => c.IdArtwork)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Artwork>()
                .HasMany(a => a.DocumentInfos)
                .WithOne(d => d.IdArtworkNavigation)
                .HasForeignKey(d => d.IdArtwork)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Artwork>()
                .HasMany(a => a.Reactions)
                .WithOne(ra => ra.Artwork)
                .HasForeignKey(ra => ra.IdArtwork)
                .OnDelete(DeleteBehavior.Restrict);
            //event
            modelBuilder.Entity<Event>()
                .HasMany(e => e.DocumentInfos)
                .WithOne(d => d.IdEventNavigation)
                .HasForeignKey(d => d.IdEvent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Event>()
                .HasMany(e => e.EventParticipants)
                .WithOne(ep => ep.Event)
                .HasForeignKey(ep => ep.IdEvent)
                .OnDelete(DeleteBehavior.Restrict);
            //project
            modelBuilder.Entity<Project>()
                .HasMany(p => p.DocumentInfos)
                .WithOne(d => d.IdProjectNavigation)
                .HasForeignKey(d => d.IdProject)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.ProjectParticipants)
                .WithOne(pp => pp.Project)
                .HasForeignKey(pp => pp.IdProject)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Creator)
                .WithMany(ad => ad.CreatedPj)
                .HasForeignKey(p => p.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Updater)
                .WithMany(ad => ad.UpdatedPj)
                .HasForeignKey(p => p.LastUpdateBy)
                .OnDelete(DeleteBehavior.Restrict);
            // typeOfArtwork

            modelBuilder.Entity<TypeOfArtwork>()
                .HasMany(to => to.Artworks)
                .WithOne(a => a.TypeOfArtwork)
                .HasForeignKey(a => a.IdTypeOfArtwork)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
