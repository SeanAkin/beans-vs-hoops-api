using HoopsVsBeans.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HoopsVsBeans.Data;

public class HoopsVsBeansContext(DbContextOptions<HoopsVsBeansContext> options) : DbContext(options)
{
    public DbSet<Vote> Votes { get; set; }
    public DbSet<VoteOptions> VoteOptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<VoteOptions>().HasData(new VoteOptions
        {
            Id = 1,
            Hoops = 0,
            Beans = 0
        });
    }
}