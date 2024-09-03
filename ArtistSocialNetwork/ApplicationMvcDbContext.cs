using Business;
using Microsoft.EntityFrameworkCore;

namespace ArtistSocialNetwork
{
    public class ApplicationMvcDbContext : DbContext
    {
        public ApplicationMvcDbContext(DbContextOptions<ApplicationMvcDbContext> options)
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
    }
}
