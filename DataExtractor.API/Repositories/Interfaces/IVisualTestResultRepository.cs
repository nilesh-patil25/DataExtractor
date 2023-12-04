
using DataExtractor.API.Models;

namespace DataExtractor.API.Repositories.Interfaces
{
    public interface IVisualTestResultRepository
    {
        Task BulkInsert(List<VisualTestResult> records);
    }
}
