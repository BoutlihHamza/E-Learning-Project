using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using E_Learning.Server.Models;
using Microsoft.AspNetCore.Identity;

namespace E_Learning.Server.Models
{
    public class ApplicationDbContext : IdentityDbContext<Utilisateur, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Formation> Formations { get; set; }
        public DbSet<Formateur> Formateurs { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<ParticipantFormation> ParticipantFormations { get; set; }
        public DbSet<Panier> Paniers { get; set; }
        public DbSet<PanierItem> PanierItems { get; set; }
        public DbSet<Lesson> lessons { get; set; }
        public DbSet<ParticipantLesson> ParticipantLessons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Certificate relationships
            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.Formation)
                .WithMany(f => f.certificates)
                .HasForeignKey(c => c.FormationId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.Participant)
                .WithMany(p => p.Certificates)
                .HasForeignKey(c => c.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ParticipantFormation relationships
            modelBuilder.Entity<ParticipantFormation>()
                .HasOne(pf => pf.Formation)
                .WithMany(f => f.ParticipantFormations)
                .HasForeignKey(pf => pf.FormationId)
                .OnDelete(DeleteBehavior.NoAction);  // Prevent cascade delete from Formation

            modelBuilder.Entity<ParticipantFormation>()
                .HasOne(pf => pf.Participant)
                .WithMany(p => p.ParticipantFormations)
                .HasForeignKey(pf => pf.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);  // Allow cascade delete from Participant

            modelBuilder.Entity<Participant>()
                .HasOne(p => p.panier)
                .WithOne(p => p.Participant)
                .HasForeignKey<Panier>(p => p.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure PanierItem relationships
            modelBuilder.Entity<PanierItem>()
                .HasOne(pi => pi.Panier)
                .WithMany(p => p.PanierItems)
                .HasForeignKey(pi => pi.PanierId)
                .OnDelete(DeleteBehavior.Cascade);  // When cart is deleted, delete its items

            modelBuilder.Entity<PanierItem>()
                .HasOne(pi => pi.Formation)
                .WithMany()  // Formation doesn't need to track cart items
                .HasForeignKey(pi => pi.FormationId)
                .OnDelete(DeleteBehavior.NoAction);  // Don't delete cart items when formation is deleted

            // Ensure prices are handled correctly
            modelBuilder.Entity<PanierItem>()
                .Property(pi => pi.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PanierItem>()
                .Property(pi => pi.DiscountAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Formation>()
            .Property(f => f.price)
            .HasPrecision(18, 2);

            modelBuilder.Entity<Formation>()
                .Property(f => f.oldPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ParticipantLesson>()
               .HasOne(pl => pl.Participant)
               .WithMany()
               .HasForeignKey(pl => pl.ParticipantId)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ParticipantLesson>()
                .HasOne(pl => pl.Lesson)
                .WithMany()
                .HasForeignKey(pl => pl.LessonId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ParticipantLesson>()
                .HasOne(pl => pl.Formation)
                .WithMany()
                .HasForeignKey(pl => pl.FormationId)
                .OnDelete(DeleteBehavior.NoAction);
        }

    }
}