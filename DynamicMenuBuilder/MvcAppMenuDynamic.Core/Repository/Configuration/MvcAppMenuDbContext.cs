using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcAppMenuDynamic.Core.Entities;

namespace MvcAppMenuDynamic.Core.Repository.Configuration
{
    public class MvcAppMenuDbContext : DbContext
    {
        public MvcAppMenuDbContext() : base("MvcAppMenuConn")
        {

        }

        public DbSet<Menu> Menus { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        
    }
}
