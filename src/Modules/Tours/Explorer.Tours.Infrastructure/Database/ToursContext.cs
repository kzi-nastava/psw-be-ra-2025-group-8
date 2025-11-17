using Explorer.Stakeholders.Core.Domain;
using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database;

public class ToursContext : DbContext
{
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<ReportProblem> ReportProblem { get; set; }
    public DbSet<PersonEquipment> PersonEquipment { get; set; }

    public ToursContext(DbContextOptions<ToursContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tours");

        modelBuilder.Ignore<Person>();

        modelBuilder.Entity<PersonEquipment>(builder =>
        {
            builder.HasKey(pe => pe.Id);

            builder.HasOne(pe => pe.Equipment)
                   .WithMany()
                   .HasForeignKey(pe => pe.EquipmentId)
                   .OnDelete(DeleteBehavior.Cascade);
        });
    }
}