using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TransportCabinet.Models;

public partial class TransportCabinetContext : DbContext
{
    public TransportCabinetContext()
    {
    }

    public TransportCabinetContext(DbContextOptions<TransportCabinetContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Driver> Drivers { get; set; }

    public virtual DbSet<Purchase> Purchases { get; set; }

    public virtual DbSet<Models.Route> Routes { get; set; }

    public virtual DbSet<Transport> Transports { get; set; }

    public virtual DbSet<TransportCard> TransportCards { get; set; }

    public virtual DbSet<TransportVehicle> TransportVehicles { get; set; }

    public virtual DbSet<Trip> Trips { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=TransportCabinet;Username=postgres;Password=dOOr007");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.pk_id_driver).HasName("driver_pkey");

            entity.ToTable("driver");

            entity.Property(e => e.pk_id_driver).HasColumnName("pk_id_driver");
            entity.Property(e => e.name)
                .HasMaxLength(30)
                .HasColumnName("name");
            entity.Property(e => e.patronymic)
                .HasMaxLength(30)
                .HasColumnName("patronymic");
            entity.Property(e => e.surname)
                .HasMaxLength(30)
                .HasColumnName("surname");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.pk_num_purchase).HasName("purchase_pkey");

            entity.ToTable("purchase");

            entity.Property(e => e.pk_num_purchase).HasColumnName("pk_num_purchase");
            entity.Property(e => e.amount).HasColumnName("amount");
            entity.Property(e => e.fk_id_card).HasColumnName("fk_id_card");
            entity.Property(e => e.name_purchase)
                .HasMaxLength(30)
                .HasColumnName("name_purchase");
            entity.Property(e => e.price)
                .HasPrecision(6, 2)
                .HasColumnName("price");
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.HasKey(e => e.pk_id_route).HasName("route_pkey");

            entity.ToTable("route");

            entity.Property(e => e.pk_id_route)
                .ValueGeneratedNever()
                .HasColumnName("pk_id_route");
            entity.Property(e => e.end_point)
                .HasMaxLength(30)
                .HasColumnName("end_point");
            entity.Property(e => e.start_point)
                .HasMaxLength(30)
                .HasColumnName("start_point");
        });

        modelBuilder.Entity<Transport>(entity =>
        {
            entity.HasKey(e => e.pk_car_num).HasName("transport_pkey");

            entity.ToTable("transport");

            entity.Property(e => e.pk_car_num)
                .HasMaxLength(10)
                .HasColumnName("pk_car_num");
            entity.Property(e => e.fk_id_vehicle).HasColumnName("fk_id_vehicle");
        });

        modelBuilder.Entity<TransportCard>(entity =>
        {
            entity.HasKey(e => e.pk_id_card).HasName("transport_card_pkey");

            entity.ToTable("transport_card");

            entity.Property(e => e.pk_id_card).HasColumnName("pk_id_card");
            entity.Property(e => e.balance)
                .HasPrecision(9, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("balance");
            entity.Property(e => e.data_issue).HasColumnName("data_issue");
            entity.Property(e => e.fk_id_owner)
                .HasMaxLength(30)
                .HasColumnName("fk_id_owner");
            entity.Property(e => e.num_days)
                .HasDefaultValueSql("0")
                .HasColumnName("num_days");
        });

        modelBuilder.Entity<TransportVehicle>(entity =>
        {
            entity.HasKey(e => e.pk_id_vehicle).HasName("transport_vehicle_pkey");

            entity.ToTable("transport_vehicle");

            entity.Property(e => e.pk_id_vehicle).HasColumnName("pk_id_vehicle");
            entity.Property(e => e.brand)
                .HasMaxLength(30)
                .HasColumnName("brand");
            entity.Property(e => e.model)
                .HasMaxLength(30)
                .HasColumnName("model");
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.pk_num_trip).HasName("trip_pkey");

            entity.ToTable("trip");

            entity.Property(e => e.pk_num_trip).HasColumnName("pk_num_trip");
            entity.Property(e => e.fk_id_card).HasColumnName("fk_id_card");
            entity.Property(e => e.fk_id_driver).HasColumnName("fk_id_driver");
            entity.Property(e => e.fk_id_tr)
                .HasMaxLength(10)
                .HasColumnName("fk_id_tr");
            entity.Property(e => e.num_route).HasColumnName("num_route");
            entity.Property(e => e.time_pay)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time_pay");

            entity.HasOne(d => d.fk_id_card_navigation).WithMany()
                .HasForeignKey(d => d.fk_id_card)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("trip_fk_id_card_fkey");

            entity.HasOne(d => d.fk_id_driver_navigation).WithMany()
                .HasForeignKey(d => d.fk_id_driver)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("trip_fk_id_driver_fkey");

            entity.HasOne(d => d.fk_id_tr_navigation).WithMany()
                .HasForeignKey(d => d.fk_id_tr)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("trip_fk_id_tr_fkey");

            entity.HasOne(d => d.num_route_navigation).WithMany()
                .HasForeignKey(d => d.num_route)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("trip_num_route_fkey");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.pk_login).HasName("user_account_pkey");

            entity.ToTable("user_account");

            entity.HasIndex(e => e.email, "user_account_email_key").IsUnique();

            entity.HasIndex(e => e.passport_num, "user_account_passport_num_key").IsUnique();

            entity.Property(e => e.pk_login)
                .HasMaxLength(30)
                .HasColumnName("pk_login");
            entity.Property(e => e.birthday).HasColumnName("birthday");
            entity.Property(e => e.email)
                .HasMaxLength(30)
                .HasColumnName("email");
            entity.Property(e => e.name)
                .HasMaxLength(30)
                .HasColumnName("name");
            entity.Property(e => e.passport_num)
                .HasMaxLength(30)
                .HasColumnName("passport_num");
            entity.Property(e => e.password)
                .HasMaxLength(30)
                .HasColumnName("password");
            entity.Property(e => e.patronymic)
                .HasMaxLength(30)
                .HasColumnName("patronymic");
            entity.Property(e => e.role)
                .HasMaxLength(10)
                .HasColumnName("role");
            entity.Property(e => e.surname)
                .HasMaxLength(30)
                .HasColumnName("surname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
