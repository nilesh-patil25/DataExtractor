using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataExtractor.API.Models
{
    public class VisualTestResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Laterality { get; set; }

        public string VisionType { get; set; }

        public string Qualifier { get; set; }

        public string Measurement { get; set; }

        public string Method { get; set; }
    
        public string RecordedBy { get; set; }
    }
}
