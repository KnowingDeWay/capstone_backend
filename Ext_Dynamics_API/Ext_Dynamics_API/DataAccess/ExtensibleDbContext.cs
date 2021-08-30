using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ext_Dynamics_API.Models;
using Ext_Dynamics_API.Models.CustomTabModels;
using Ext_Dynamics_API.Security.Models;
using Microsoft.EntityFrameworkCore;

namespace Ext_Dynamics_API.DataAccess
{
    public class ExtensibleDbContext : DbContext
    {
        public DbSet<ApplicationUserAccount> UserAccounts { get; set; }
        public DbSet<CanvasPersonalAccessToken> PersonalAccessTokens { get; set; }
        public DbSet<UserTokenEntry> UserTokenEntries { get; set; }
        public DbSet<DataColumnEntry> CustomDataColumns { get; set; }

        public ExtensibleDbContext()
        {

        }

        public ExtensibleDbContext(DbContextOptions<ExtensibleDbContext> options)
        : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUserAccount>().Property(x => x.RowVersion).IsRowVersion();
            modelBuilder.Entity<ApplicationUserAccount>().ToTable("App_Users").HasAlternateKey(x => x.AppUserName);

            modelBuilder.Entity<CanvasPersonalAccessToken>().Property(x => x.RowVersion).IsRowVersion();
            modelBuilder.Entity<CanvasPersonalAccessToken>().ToTable("Personal_Access_Tokens");

            modelBuilder.Entity<UserTokenEntry>().Property(x => x.RowVersion).IsRowVersion();
            modelBuilder.Entity<UserTokenEntry>().ToTable("Token_Entries");

            modelBuilder.Entity<DataColumnEntry>().Property(x => x.RowVersion).IsRowVersion();
            modelBuilder.Entity<DataColumnEntry>().HasAlternateKey(x => x.CourseId);
            modelBuilder.Entity<DataColumnEntry>().HasAlternateKey(x => x.RelatedDataId);
            modelBuilder.Entity<DataColumnEntry>().ToTable("Data_Column_Entries");
        }
    }
}
