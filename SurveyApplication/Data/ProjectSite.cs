using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SurveyApplication.Data
{
    public class ProjectSite
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "Project ID")]
        [StringLength(25)]
        public string? ProjectID { get; set; }
        public string? Customer { get; set; }
        public string? Location { get; set; }
        [Display(Name = "Order Number")]
        public string? OrderNum { get; set; }
        [Display(Name = "Quote Number")]
        public string? QuoteNum { get; set; }
        [Display(Name = "PM Phone")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(15)]
        public string? PM_Phone { get; set; }
        [Display(Name = "PM Email")]
        [DataType(DataType.EmailAddress)]
        public string? PM_Email { get; set; }
        [Display(Name = "PM Name")]
        public string? PM_Name { get; set; }
        [Display(Name = "Contact Phone")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(15)]
        public string? SiteContact_Phone { get; set; }
        [Display(Name = "Contact Email")]
        [DataType(DataType.EmailAddress)]
        public string? SiteContact_Email { get; set; }
        [Display(Name = "Contact Name")]
        public string? SiteContact_Name { get; set;  }
        [Display(Name = "Surveyor Name")]
        public string? Surveyor_Name { get; set; }
        [Display(Name = "Surveyor Company")]
        public string? Surveyor_Company { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Survey Date")]
        public DateTime? SurveyDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Creation Date")]
        public DateTime? DateCreated { get; set; }
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }
        [Display(Name = "Last Modified By")]
        public string? LastModifiedBy { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Last Modified Date")]
        public DateTime? LastModified { get; set; }


        public List<SiteSurvey>? Surveys { get; set; } = new List<SiteSurvey>();
    
    }
}
