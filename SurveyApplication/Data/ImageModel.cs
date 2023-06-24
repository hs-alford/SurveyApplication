using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SurveyApplication.Data
{
    public class ImageModel
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string? Title { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Image Name")]
        public string? ImageName { get; set; }

        [NotMapped]
        [DisplayName("Upload File")]
        public IFormFile? ImageFile { get; set; }

        public int? SiteSurveyId { get; set; }
        public SiteSurvey? Survey { get; set; }
        
    }
}
