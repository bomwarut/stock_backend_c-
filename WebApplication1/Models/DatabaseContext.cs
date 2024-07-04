using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace WebApplication1.Models;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<product> products { get; set; }
    public virtual DbSet<stock> stocks { get; set; }
    public virtual DbSet<stock_history> stock_histories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost,3306;database=stock_manage;user id=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.32-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8_general_ci")
            .HasCharSet("utf8");

        modelBuilder.Entity<product>(entity =>
        {
            entity.HasKey(e => e.product_id).HasName("PRIMARY");

            entity.ToTable("product");

            entity.Property(e => e.product_id).HasColumnType("int(11)");
            entity.Property(e => e.product_code).HasMaxLength(255);
            entity.Property(e => e.product_name).HasMaxLength(255);
            entity.Property(e => e.product_price).HasPrecision(10, 2);
            entity.Property(e => e.product_update_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<stock>(entity =>
        {
            entity.HasKey(e => e.stock_id).HasName("PRIMARY");

            entity.ToTable("stock");

            entity.HasIndex(e => e.product_id, "stock_fk");

            entity.Property(e => e.stock_id).HasColumnType("int(11)");
            entity.Property(e => e.product_id).HasColumnType("int(11)");
            entity.Property(e => e.product_quantity).HasColumnType("int(11)");
            entity.Property(e => e.stock_update_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");

            entity.HasOne(d => d.product).WithMany(p => p.stocks)
                .HasForeignKey(d => d.product_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("stock_fk");
        });

        modelBuilder.Entity<stock_history>(entity =>
        {
            entity.HasKey(e => e.stock_history_id).HasName("PRIMARY");

            entity.ToTable("stock_history");

            entity.HasIndex(e => e.stock_id, "stock_history_fk");

            entity.Property(e => e.stock_history_id).HasColumnType("int(11)");
            entity.Property(e => e.quantity).HasColumnType("int(11)");
            entity.Property(e => e.stock_history_update_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.stock_id).HasColumnType("int(11)");
            entity.Property(e => e.stock_type).HasComment("1 = รับสินค้าเข้าคลัง, 2=เบิกสินค้าออกคลัง");

            entity.HasOne(d => d.stock).WithMany(p => p.stock_histories)
                .HasForeignKey(d => d.stock_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("stock_history_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
