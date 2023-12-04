

using DataExtractor.API.Data;
using DataExtractor.API.Models;
using DataExtractor.API.Repositories.Interfaces;

namespace DataExtractor.API.Repositories
{
    // Data Access logic
    public class VisualTestResultRepository : IVisualTestResultRepository
    {
        private readonly VisualAcuityDbContext _context;
        public VisualTestResultRepository(VisualAcuityDbContext context)
        {
            _context = context;
        }

        // Method to save all the data in one go
        public async Task BulkInsert(List<VisualTestResult> records)
        {
            await _context.VisionTestResults.AddRangeAsync(records);
            await _context.SaveChangesAsync();
        }
    }
}
