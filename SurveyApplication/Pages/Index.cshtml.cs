using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SurveyApplication.Data;
using System.Linq;

namespace SurveyApplication.Pages
{
	public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;

		// Database context (for accessing the application database)
		public ApplicationDbContext _context { get; set; }

		public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
		{
			_logger = logger;
			_context = context;
		}

		// ** Page Model Properties ** //

		[BindProperty]
		public List<ProjectSite> ProjectSites { get; set; }

		[BindProperty]
		public ProjectSite NewProjectSite { get; set; }

		[TempData]
		public bool SiteDeleteConfirmation { get; set; } = false;


		// ** Page Methods ** //

		// Http GET Action - Retrives the list of project sites from the database and calls the home page.
		// Route: /Index
		public void OnGet()
		{
			ProjectSites = _context.ProjectSites.ToList();
		}

		// Http POST Action - Creates a new site docuemnt for a project site, saves data to the database, 
		// and takes the user to the new page to begin editing.
		public async Task<IActionResult> OnPostAsync()
		{

			if (!ModelState.IsValid)
			{
				return RedirectToPage("./Index");
			}

			_context.ProjectSites.Add(NewProjectSite);
			
			/* Using the Projet ID entered into the form, a query is run through Epicor to fill in the
			 respective fields associated with it that have values. */
			await EpicorQueryHelper.ExecuteQueryAsync_GetProjectData(NewProjectSite);

			await _context.SaveChangesAsync();

			// New site survey is added to the project site 
			SiteSurvey survey = new SiteSurvey();
			survey.ProjectSite = NewProjectSite;
			survey.ProjectSiteId = NewProjectSite.Id;
			survey.SurveyNumber = 1;
			survey.SignQuantity = 1;

			_context.SiteSurveys.Add(survey);
			NewProjectSite.Surveys.Add(survey);
			await _context.SaveChangesAsync();

			// User is redirected to the project site's document page for editing
			return RedirectToPage("./CreateSiteSurvey/", new { id = NewProjectSite.ProjectID.ToString() });
		}

		// Http GET Action - Page handler method to delete a project site
		// Route: /Index/DeleteProjectSite/{id?}
		public async Task<IActionResult> OnGetDeleteProjectSite(int? id)
		{
			if (id == null || _context.ProjectSites == null)
			{
				return NotFound();
			}

			ProjectSite projectSite = _context.ProjectSites.Where(u => u.Id == id).FirstOrDefault();


			if (projectSite != null)
			{
				// Surveys are deleted
				foreach (var survey in _context.SiteSurveys.Where(u => u.ProjectSiteId == id).ToList())
				{
					DeleteSiteSurvey(survey.Id);
				}

				// Project is deleted
				_context.ProjectSites.Remove(projectSite);
				await _context.SaveChangesAsync();
				SiteDeleteConfirmation = true;

			}

			return RedirectToPage("./Index");
		}

		// Method to handle deleting project surveys
		public async void DeleteSiteSurvey(int surveyId)
		{
			if (surveyId == null || _context.SiteSurveys == null)
			{
				return;
			}

			SiteSurvey sitesurvey = await _context.SiteSurveys.FindAsync(surveyId);

			if (sitesurvey != null)
			{
				_context.SiteSurveys.Remove(sitesurvey);
			}
		}
	}

}