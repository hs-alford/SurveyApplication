using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data;
using Microsoft.Data.SqlClient;
using SurveyApplication.Utilities;

namespace SurveyApplication.Data
{
	public class EpicorQueryHelper
	{
		private static string connStr = "";

		public static async Task<ProjectSite> ExecuteQueryAsync_GetProjectData(ProjectSite projectSite)
		{

			Dictionary<string, string> siteProps = new Dictionary<string, string>();

			string commandText = @"SELECT Proj.Company, Proj.ProjectID, PM.Name, PM.Phone, PM.EMailAddress, Cust.Name, Cust.Address1, Cust.Address2,
				Cust.Address3, Cust.City, Cust.State, Cust.Zip, Cust.Country, SO.OrderNum, SubQuery1.Calculated_MaxQuote as QuoteNum, Contact.Name,
				IIf(Contact.PhoneNum = null, Contact.CellPhoneNum, Contact.PhoneNum) as SiteContactPhone, Contact.EMailAddress, Proj.SurveyDate_c as SurveyDate
				FROM [dbo].[Project] as Proj inner join [dbo].[EmpBasic] as PM on Proj.Company = PM.Company And Proj.ConProjMgr = PM.EmpID
				left outer join dbo.OrderHed as SO on Proj.ProjectID = SO.ProjectID_c and Proj.Company = SO.Company
				left outer join dbo.Customer as Cust on Proj.ConCustNum = Cust.CustNum and Proj.Company = Cust.Company

				left outer join  (select 
					Quote.[Company] as [Quote_Company],
					Quote.[ProjectID_c] as [Quote_ProjectID_c],
					(MAX(Quote.QuoteNum)) as [Calculated_MaxQuote]
				from dbo.QuoteHed as Quote
				group by Quote.[Company],
					Quote.[ProjectID_c])  as SubQuery1 on 
					SubQuery1.Quote_Company = Proj.Company
				And
					SubQuery1.Quote_ProjectID_c = Proj.ProjectID

				left outer join Erp.CustCnt as Contact on 
					Proj.Company = Contact.Company 
				and 
					Proj.ConCustNum = Contact.CustNum 
				and 
					Contact.ConNum = (select 
						MAX(Con.ConNum) 
					from Erp.CustCnt as Con 
						where Contact.Company = Con.Company 
					and
						Contact.CustNum = Con.CustNum)
				WHERE Proj.Company = 'Colite' and Proj.ProjectID = @PROJID";

			using (var connection = new SqlConnection(connStr))
			{
				await connection.OpenAsync();   //vs  connection.Open();
				using (var tran = connection.BeginTransaction())
				{
					bool error = false;
					using (var command = new SqlCommand(commandText, connection, tran))
					{
						try
						{
							command.Parameters.Add("@PROJID", SqlDbType.NVarChar);
							command.Parameters["@PROJID"].Value = projectSite.ProjectID;

							SqlDataReader rdr = await command.ExecuteReaderAsync();  // vs also alternatives, command.ExecuteReader();  or await command.ExecuteNonQueryAsync();

							while (rdr.Read())
							{
								projectSite.PM_Name = rdr.SafeGetString(2);
								projectSite.PM_Phone = rdr.SafeGetString(3);
								projectSite.PM_Email = rdr.SafeGetString(4);
								projectSite.Customer = rdr.SafeGetString(5);
								for (int i = 0, x = 6; i < 7; i++, x++)
								{
									projectSite.Location += rdr.SafeGetString(x);
									projectSite.Location += i < 6 ? " " : "";
								}
								projectSite.OrderNum = rdr.SafeGetInt(13).ToString();
								projectSite.QuoteNum= rdr.SafeGetInt(14).ToString();
								projectSite.SiteContact_Name = rdr.SafeGetString(15);
								projectSite.SiteContact_Phone = rdr.SafeGetString(16);
								projectSite.SiteContact_Email = rdr.SafeGetString(17);
								projectSite.SurveyDate = rdr.SafeGetNullableDateTime(18);
							}
							await rdr.CloseAsync();
						}
						catch (Exception Ex)
						{
							await connection.CloseAsync();
							string msg = Ex.Message.ToString();
							error = true;
							
							throw;
						}
					}
					if (error)
					{
						tran.Rollback();

					}
				}

			}

			
			return projectSite;

		
	}


		

	}
}
