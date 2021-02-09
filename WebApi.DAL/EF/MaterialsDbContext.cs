using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebApi.DAL.Entities;

namespace WebApi.DAL.EF
{
    public class MaterialsDbContext : DbContext
    {
        public DbSet<Material> Materialss { get; set; }
        public MaterialsDbContext(DbContextOptions<MaterialsDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Material>();
        }
    }
}
