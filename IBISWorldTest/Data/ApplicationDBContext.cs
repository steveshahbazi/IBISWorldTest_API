using IBISWorld_API.Models;
using IBISWorld_API.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace IBISWorld_API.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }
        public DbSet<Term> Terms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Term>().HasData(
                new TermDTO { Id = 1, Name = "Coding", Definition = "Practice of writing program" },
                new TermDTO { Id = 2, Name = "Ibis", Definition = "A great company to work" },
                new TermDTO { Id = 3, Name = "Web API application", Definition = "Good way of making web apps" });
        }
    }
}
