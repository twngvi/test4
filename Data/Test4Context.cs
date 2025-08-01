using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace test4.Data;

public partial class Test4Context : DbContext
{
    public Test4Context()
    {
    }

    public Test4Context(DbContextOptions<Test4Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Destination> Destinations { get; set; }

    public virtual DbSet<Tour> Tours { get; set; }

  
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Destination>(entity =>
        {
            entity.HasKey(e => e.DestinationId).HasName("PK__Destinat__DB5FE4CCA2B736A5");

            entity.ToTable("Destination");

            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.DestinationName).HasMaxLength(100);
            entity.Property(e => e.PhotoPath).HasMaxLength(200);

            entity.HasOne(d => d.Tour).WithMany(p => p.Destinations)
                .HasForeignKey(d => d.TourId)
                .HasConstraintName("FK__Destinati__TourI__4BAC3F29");
        });

        modelBuilder.Entity<Tour>(entity =>
        {
            entity.HasKey(e => e.TourId).HasName("PK__Tour__604CEA30C43282D3");

            entity.ToTable("Tour");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TourName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
