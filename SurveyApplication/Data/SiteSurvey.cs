using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SurveyApplication.Data
{
	public class SiteSurvey
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		[Display(Name = "Survey Number")]
		public int SurveyNumber { get; set; }
		[Display(Name = "Sign Type")]
		public string? SignType { get; set; }
		[Display(Name = "Sign Quantity")]
		public int SignQuantity { get; set; } = 1;
		public string? Lightining { get; set; }
		public string? Faces { get; set; }
		public string? Height { get; set; }
		public string? Width { get; set; }
		public string? Depth { get; set; }
		public string? Voltage { get; set; }
		[Display(Name = "Height From Grade")]
		public string? Height_FromGrade { get; set; }
		[Display(Name = "Sign Availabe Area")]
		public string? AvaiableArea { get; set; }
		[Display(Name = "Wall Color")]
		public string? WallColor { get; set; }
		[Display(Name = "Wall Height")]
		public string? WallHeight { get; set; }
		[Display(Name = "Mounting Details")]
		public string? MountingDetails { get; set; }
		[Display(Name = "Reuse Flooring?")]
		public bool? ResuseFooting { get; set; }
		[Display(Name = "Survey Notes")]
		public string? Notes { get; set; }
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
		[Display(Name = "Date Last Modifited")]
		public DateTime? LastModified { get; set; }


		public int? ProjectSiteId { get; set; }
		public ProjectSite? ProjectSite { get; set; }
		public List<ImageModel>? Images { get; set; }
		 
	}
}
