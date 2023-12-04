using DataExtractor.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DataExtractor.API.Data
{
    public class VisualAcuityDbContext : DbContext
    {
        public VisualAcuityDbContext(DbContextOptions<VisualAcuityDbContext> options) : base(options)
        {

        }

        public DbSet<VisualTestResult> VisionTestResults { get; set; }
    }
}
