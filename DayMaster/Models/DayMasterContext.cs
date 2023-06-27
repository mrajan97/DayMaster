using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using DayMaster.Models;

namespace DayMaster.Models;

public partial class DayMasterContext : DbContext
{
    public DayMasterContext()
    {
    }

    public DayMasterContext(DbContextOptions<DayMasterContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DayMaster;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public DbSet<DayMaster.Models.User>? User { get; set; }
    public DbSet<DayMaster.Models.Notification>? Notification { get; set; }
    public DbSet<DayMaster.Models.Priority>? Priority { get; set; }
    public DbSet<DayMaster.Models.Report>? Report { get; set; }
    public DbSet<DayMaster.Models.Task>? Task { get; set; }
    public DbSet<DayMaster.Models.TaskHistory>? TaskHistory { get; set; }
    public DbSet<DayMaster.Models.Audit>? Audit { get; set; }
}
