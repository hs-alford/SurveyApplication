using LazZiya.ImageResize;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using SurveyApplication.Data;
using SurveyApplication.Utilities;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml.Linq;

namespace SurveyApplication.Pages
{
	public class CreateSiteSurveyModel : PageModel
	{
		public ApplicationDbContext _context { get; set; }
		private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;
		public UserManager<IdentityUser> _userManager { get; set; }

		public CreateSiteSurveyModel(ApplicationDbContext context, UserManager<IdentityUser> userManager, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
		{
			_context = context;
			_userManager = userManager;
			Environment = environment;
		}

		// ** Page Model Properites ** //

		[BindProperty(SupportsGet = true)]
		public string Id { get; set; }

		[BindProperty]
		public ProjectSite ProjectSite { get; set; } = default!;
	   
		[BindProperty]
		public List<SiteSurvey> SiteSurveys { get; set; } = default!;
	  
		[BindProperty]
		public List<FileItem>? SurveyImages { get; set; } = new List<FileItem>();
		
		[BindProperty]
		public int? SurveyID { get; set; }

		[TempData]
		public bool SaveConfirmation { get; set; } = false;
		
		[TempData]
		public bool SurveyDeleteConfirmation { get; set; } = false;

		[TempData]
		public string CurrentForm { get; set; } = "1";


		// ** Page Methods ** //

		// Http GET Action - Retrives the list of project sites, surveys, images, etc. and displays
		// the site survey page
		// Route: /CreateSiteSurvey/{id}
		public void OnGet(string id)
		{
			ProjectSite projSite = _context.ProjectSites.Where(u => u.ProjectID == id).FirstOrDefault();
			List<SiteSurvey> sites = _context.SiteSurveys.Where(u => u.ProjectSiteId == projSite.Id).ToList();
			ProjectSite = projSite;
			SiteSurveys = sites;
			Id = id;

			// Getting list of site numbers to retrive each site's photos
			List<int> siteIds = sites.Select(u => u.Id).ToList();
			//siteNums.Sort();
			OnGetListFolderContents(id, siteIds);
		}


		// Http GET Action - Triggered by the "Add New Page" button on the page. This action adds a new site
		// survey to the project and fixes the numbers
		// the site survey page
		// Route: /CreateSiteSurvey/{id}
		public async Task<IActionResult> OnGetAddSurvey(string? id)
		{
			Id = id;
			Console.WriteLine("ID:" + Id);
			ProjectSite = _context.ProjectSites.Include(u => u.Surveys).ToList().Where(u => u.ProjectID == Id).FirstOrDefault();
			List<SiteSurvey> surveys = ProjectSite.Surveys;
			for (int i = 0, x = 1; i < surveys.Count; i++, x++)
			{
				surveys[i].SurveyNumber = x;
			}
			SiteSurvey survey = new SiteSurvey();
			survey.ProjectSiteId = ProjectSite.Id;
			survey.ProjectSite = ProjectSite;
			survey.SurveyNumber = ProjectSite.Surveys.Count + 1;

			_context.SiteSurveys.Add(survey);
			ProjectSite.Surveys.Add(survey);
			await _context.SaveChangesAsync();
			return RedirectToPage("./CreateSiteSurvey/", Id);

		}

		public async Task<IActionResult> OnPost(string id)
		{
			Id= id;

			var errors = ModelState.Values.SelectMany(x => x.Errors.Select(c => c.ErrorMessage)).ToList();
			foreach (var error in errors) { 
				Console.WriteLine($"Error: {error}");
			}

			if (!ModelState.IsValid)
			{
				Console.WriteLine("Model State: false");

				return Page();
			}

			for(int i = 0; i < SiteSurveys.Count; i++)
			{
				SiteSurveys[i].LastModified= DateTime.Now;
				_context.Attach(SiteSurveys[i]).State = EntityState.Modified;


				if (!SiteExists(SiteSurveys[i].Id))
				{
					return NotFound();
				}
			}

			_context.Attach(ProjectSite).State = EntityState.Modified;
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ProjectExists(ProjectSite.Id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			SaveConfirmation = true;
			return RedirectToPage("./CreateSiteSurvey/", Id);
		}


		public async Task<IActionResult> OnGetDeleteSiteSurvey(int? surveyId)
		{
			if (surveyId == null || _context.SiteSurveys == null)
			{
				return NotFound();
			}

			SiteSurvey sitesurvey = await _context.SiteSurveys.FindAsync(surveyId);

			int projectSiteId = (int)sitesurvey.ProjectSiteId;

			if (sitesurvey != null)
			{
				_context.SiteSurveys.Remove(sitesurvey);
				await _context.SaveChangesAsync();
				SurveyDeleteConfirmation = true;

			}
			List<SiteSurvey> remainingSurveys = _context.SiteSurveys.Where(u => u.ProjectSiteId == projectSiteId).OrderBy(u => u.SurveyNumber).ToList();

			if (SurveyDeleteConfirmation)
			{
				for(int i = 0, x = 1; i < remainingSurveys.Count; i++, x++)
				{
					remainingSurveys[i].SurveyNumber = x;
				}
				await _context.SaveChangesAsync();
			}

			return RedirectToPage("./CreateSiteSurvey/", Id);
		}


		private bool SiteExists(int id)
		{
			return _context.SiteSurveys.Any(e => e.Id == id);
		}


		private bool ProjectExists(int id)
		{
			return _context.ProjectSites.Any(e => e.Id == id);
		}



		public async Task<IActionResult> OnPostUploadPhotosAsync(List<IFormFile> DocumentPhotos)
		{
			string surveyId = "Site-" + SurveyID;
			string id = Id;
			string path = @"C:" + id + "\\" + surveyId;  // Give the specific path  
			if (!(Directory.Exists(path)))
			{
				Directory.CreateDirectory(path);
			}

			var imgFiles = new[] { ".jpg", ".png", ".gif" };
			foreach (var file in DocumentPhotos)
			{
				//get uploaded file name: true to create temp name, false to get real name
				var fileName = file.TempFileName(false);
				var fileExt = fileName.Substring(fileName.LastIndexOf('.'));
				//_logger.LogInformation("File extension : " + fileExt);
				if (file.Length > 0)
				{
					// optional : server side resize create image with watermark
					// these steps requires LazZiya.ImageResize package from nuget.org
					if (imgFiles.Any(ext => ext.Equals(fileExt, StringComparison.OrdinalIgnoreCase)))
					{
						using (var stream = file.OpenReadStream())
						{
							// Create image file from uploaded file stream
							// Then resize, and add text/image watermarks
							// And save

							using (var img = Image.FromStream(stream))
							{
								img.ScaleByWidth(800)                                  
									.SaveAs($"wwwroot/upload/{id}/{surveyId}/{fileName}");
							}
						}
					}
					else
					{
						// upload and save files to upload folder
						using (var stream = new FileStream($"wwwroot\\upload\\{id}\\{surveyId}\\{fileName}", FileMode.Create))
						{
							await file.CopyToAsync(stream);
							return RedirectToPage("./CreateSiteSurvey/", Id);
						}
					}
				}
			}
			return RedirectToPage("./CreateSiteSurvey/", Id);

		}

		public void OnGetListFolderContents(string id, List<int> siteNumbers)
		{

			var folderPath = $"wwwroot\\upload\\{id}";

			if (!Directory.Exists(folderPath))
				return;

			SurveyImages = new List<FileItem>();

			foreach (var snum in siteNumbers) 
			{
				var surveyFolderPath = $"wwwroot\\upload\\{id}\\Site-{snum}";
				if (!Directory.Exists(surveyFolderPath))
					continue;

				var folderItems = Directory.GetFiles(surveyFolderPath);

				if (folderItems.Length == 0)
					continue;


				foreach (var file in folderItems)
				{
					var fileInfo = new FileInfo(file);
					SurveyImages.Add(new FileItem
					{
						Name = fileInfo.Name,
						FilePath = $"https://localhost:50843//upload/{id}/Site-{snum}/{fileInfo.Name}",
						FileSize = fileInfo.Length,
						SurveyId = snum
					});
				}
			}
		}

		public FileResult OnGetDownloadFile(string file)
		{
			file = file.Substring(file.IndexOf("upload"));
			var path = Path.Combine($"wwwroot\\{file}");
			byte[] bytes = System.IO.File.ReadAllBytes(path);
			string fileName = file.Substring(file.LastIndexOf("/") + 1);
			Console.WriteLine(fileName); 
			return File(bytes, "application/octet-stream", fileName);
		}

		public async Task<IActionResult> OnGetDeleteFile(string file)
		{
			file = file.Substring(file.IndexOf("upload"));
			var filePath = Path.Combine($"wwwroot\\{file}");
			try
			{
				System.IO.File.Delete(filePath);
			}
			catch
			{
				return new JsonResult(false) { StatusCode = (int)HttpStatusCode.InternalServerError };
			}

			return RedirectToPage("./CreateSiteSurvey/", Id);
		}
	}
}


