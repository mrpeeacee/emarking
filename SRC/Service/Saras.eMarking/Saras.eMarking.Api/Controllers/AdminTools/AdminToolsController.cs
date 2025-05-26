using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.AdminTools;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using Saras.eMarking.Domain.ViewModels.AdminTools;
using System.Collections.Generic;
using Saras.eMarking.Domain.ViewModels.Banding;
using Saras.eMarking.Domain.ViewModels;
using DocumentFormat.OpenXml.Office2013.Excel;
using ClosedXML.Excel;
using System.IO;
using Saras.eMarking.Business.Global;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using System.Data;
using Azure;
using Saras.eMarking.Domain.ViewModels.Project.Setup;
using Nest;
using Newtonsoft.Json;

namespace Saras.eMarking.Api.Controllers.AdminTools
{
	[ApiController]
	[ApiVersion("1.0")]
	[Route("/api/v{version:apiVersion}/admin-tools")]
	public class AdminToolsController : BaseApiController<AdminToolsController>
	{
		private readonly IAdminToolsService admintoolsService;
		public AdminToolsController(IAdminToolsService _admintoolsService, ILogger<AdminToolsController> _logger, AppOptions appOptions, IAuditService _auditService, IAuthService _authService) : base(appOptions, _logger, _auditService)
		{
			admintoolsService = _admintoolsService;
		}

		/// <summary>
		/// Get All Projects.
		/// </summary> 
		/// <returns></returns> 
		[HttpGet]
		public async Task<IActionResult> BindAllProject()
		{
			long UserId = 0;
			long ProjectId = 0;
			List<BindAllProjectModel> resp = new List<BindAllProjectModel>();
			try
			{
				UserId = GetCurrentUserId();
				ProjectId = GetCurrentProjectId();

				logger.LogDebug("AdminToolsController > BindAllProject() started. ProjectId = {ProjectId} and UserId = {UserId}", ProjectId, UserId);

				resp = await admintoolsService.BindAllProject(UserId, ProjectId);

				logger.LogDebug("AdminToolsController > BindAllProject() completed. ProjectId = {ProjectId} and UserId = {UserId}", ProjectId, UserId);

				return Ok(resp);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in AdminToolsController > BindAllProject() method. ProjectId = {ProjectId} and UserId = {UserId}", ProjectId, UserId);
				throw;
			}
		}

		/// <summary>
		/// Get Live Marking Progress Report.
		/// </summary> 
		/// <param name="projectid"></param>
		/// <returns></returns> 
		[Route("{projectid}")]
		[HttpGet]
		public async Task<IActionResult> LiveMarkingProgressDetails(long projectid)
		{
			long UserId = 0;
			List<LiveMarkingProgressModel> result = new List<LiveMarkingProgressModel>();
			try
			{
				UserId = GetCurrentUserId();
				logger.LogDebug("AdminToolsController > LiveMarkingProgressDetails() started. ProjectID = {projectid} and UserId = {UserId}", projectid, UserId);

				result = await admintoolsService.LiveMarkingProgressDetails(projectid, UserId);

				logger.LogDebug("AdminToolsController > LiveMarkingProgressDetails() completed. ProjectID = {projectid} and UserId = {UserId}", projectid, UserId);

				return Ok(result);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in AdminToolsController > LiveMarkingProgressDetails() method. ProjectID = {projectid} and UserId = {UserId}", projectid, UserId);
				throw;
			}
		}

		/// <summary>
		/// Get Quality Check Report.
		/// </summary> 
		/// <param name="projectId"></param>
		/// <param name="selectedrpt"></param>
		/// <returns></returns> 
		[Route("{projectId}/{selectedrpt}")]
		[HttpGet]
		public async Task<IActionResult> QualityCheckDetails(long projectId, short selectedrpt)
		{
			long UserId = 0;
			AdminToolsModel result = new AdminToolsModel();
			try
			{
				UserId = GetCurrentUserId();
				logger.LogDebug("AdminToolsController > QualityCheckDetails() started. ProjectId = {projectId} and Selected Report = {selectedrpt} and UserId = {UserId}", projectId, selectedrpt, UserId);

				result = await admintoolsService.QualityCheckDetails(projectId, selectedrpt, UserId);

				logger.LogDebug("AdminToolsController > QualityCheckDetails() completed. ProjectId = {projectId} and Selected Report = {selectedrpt} and UserId = {UserId}", projectId, selectedrpt, UserId);

				return Ok(result);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in AdminToolsController > QualityCheckDetails() method. ProjectId = {projectId} and Selected Report = {selectedrpt} and UserId = {UserId}", projectId, selectedrpt, UserId);
				throw;
			}
		}

		/// <summary>
		/// Get Candidate Script Details
		/// </summary> 
		/// <param name="projectId"></param>
		/// <param name="searchFilterModel"></param>
		/// <param name="IsDownload"></param>
		/// <returns></returns> 
		[Route("{projectId}/candidatescript/{IsDownload}")]
		[HttpPost]
		public async Task<IActionResult> CandidateScriptDetails(long projectId, SearchFilterModel searchFilterModel, bool IsDownload = false)
		{
			long UserId = 0;
			List<CandidateScriptModel> result = new List<CandidateScriptModel>();
			try
			{
				UserId = GetCurrentUserId();
				logger.LogDebug("AdminToolsController > CandidateScriptDetails() started. ProjectId = {projectId} and searchFilterModel = {searchFilterModel} and UserId = {UserId} and IsDownload = {IsDownload}.", projectId, searchFilterModel, UserId, IsDownload);

				result = await admintoolsService.CandidateScriptDetails(projectId, searchFilterModel, UserId, IsDownload);

				logger.LogDebug("AdminToolsController > CandidateScriptDetails() completed. ProjectId = {projectId} and searchFilterModel = {searchFilterModel} and UserId = {UserId} and IsDownload = {IsDownload}.", projectId, searchFilterModel, UserId, IsDownload);

				return Ok(result);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in AdminToolsController > CandidateScriptDetails() method. ProjectId = {projectId} and searchFilterModel = {searchFilterModel} and UserId = {UserId} and IsDownload = {IsDownload}.", projectId, searchFilterModel, UserId, IsDownload);
				throw;
			}
		}

		/// <summary>
		/// Get Frequency Distribution Report
		/// </summary> 
		/// <param name="projectId"></param>
		/// <param name="questionType"></param>
		/// <param name="searchFilterModel"></param>
		/// <param name="IsDownload"></param>
		/// <returns></returns> 
		[Route("{projectId}/frequencydistribution/{questionType}/{IsDownload}")]
		[HttpPost]
		public async Task<IActionResult> FrequencyDistributionReport(long projectId, SearchFilterModel searchFilterModel, long questionType, bool IsDownload = false)
		{
			long UserId = 0;
			List<FrequencyDistributionModel> result = new List<FrequencyDistributionModel>();
			try
			{
				UserId = GetCurrentUserId();
				logger.LogDebug("AdminToolsController > FrequencyDistributionReport() started. ProjectID = {projectId} and QuestionType = {questionType} and SearchFilterModel = {searchFilterModel} and UserId = {UserId} and IsDownload = {IsDownload}", projectId, questionType, searchFilterModel, UserId, IsDownload);

				result = await admintoolsService.FrequencyDistributionReport(projectId, questionType, searchFilterModel, UserId, IsDownload);

				logger.LogDebug("AdminToolsController > FrequencyDistributionReport() completed. ProjectID = {projectId} and QuestionType = {questionType} and SearchFilterModel = {searchFilterModel} and UserId = {UserId} and IsDownload = {IsDownload}", projectId, questionType, searchFilterModel, UserId, IsDownload);

				return Ok(result);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in AdminToolsController > FrequencyDistributionReport() method. ProjectID = {projectId} and QuestionType = {questionType} and SearchFilterModel = {searchFilterModel} and UserId = {UserId} and IsDownload = {IsDownload}", projectId, questionType, searchFilterModel, UserId, IsDownload);
				throw;
			}
		}


		/// <summary>
		/// Get All Answer Key Report.
		/// </summary>
		/// <param name="projectId"></param>
		/// <param name="searchFilterModel"></param>
		/// <param name="IsDownload"></param>
		/// <returns></returns>
		[Route("{projectId}/answerkeyreport/{IsDownload}")]
		[HttpPost]
		public async Task<IActionResult> AllAnswerKeysReport(long projectId, SearchFilterModel searchFilterModel, bool IsDownload = false)
		{
			long UserId = 0;
			List<AllAnswerKeysModel> result = new List<AllAnswerKeysModel>();
			try
			{
				UserId = GetCurrentUserId();
				logger.LogDebug("AdminToolsController > AllAnswerKeysReport() started. ProjectID = {projectId}, ", projectId);

				result = await admintoolsService.AllAnswerKeysReport(projectId, UserId, searchFilterModel, IsDownload);

				logger.LogDebug("AdminToolsController > AllAnswerKeysReport() completed. ProjectID = {projectId} ", projectId);

				return Ok(result);

			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in AdminToolsController > AllAnswerKeysReport() method. ProjectID = {projectId} ", projectId);
				throw;
			}
		}

		/// <summary>
		/// Gets All Answer Key Reports
		/// </summary>
		/// <param name="projectId"></param>
		/// <returns></returns>
		[Route("answerkeycompletereport/{projectId}")]
		[HttpGet]
		public async Task<IActionResult> GetAnswerKeyCompelteReport(long projectId)
		{
			logger.LogDebug($"AdminToolsController > Method Name: GetAnswerKeyCompelteReport() started. ProjectId={GetCurrentProjectId()}");

			Boolean IsDownload = false;
			long UserId = 0;
			List<AllAnswerKeysModel> Alluserslst = new List<AllAnswerKeysModel>();

			SearchFilterModel searchFilterModel = new SearchFilterModel();
			searchFilterModel.PageNo = 0;
			searchFilterModel.PageSize = 0;
			searchFilterModel.SearchText = string.Empty;
			try
			{
				UserId = GetCurrentUserId();

				logger.LogInformation($"AdminToolsController > GetAnswerKeyCompelteReport() started.");

				Alluserslst = await admintoolsService.AllAnswerKeysReport(projectId, UserId, searchFilterModel, IsDownload);
				DataTable dt = null;

				//// DataTable dt = ToGetUserDataTable(Alluserslst);

				if (Alluserslst != null || Alluserslst.Count < 0)
				{
					dt = ToGetAnswerDataTable(Alluserslst);
				}

				using (XLWorkbook wb = new XLWorkbook())
				{

					wb.Worksheets.Add(dt, "All Answers Key list");
					wb.Worksheet("All Answers Key list").Columns("1").Width = 30;
					wb.Worksheet("All Answers Key list").Columns("2").Width = 30;
					wb.Worksheet("All Answers Key list").Columns("3").Width = 30;
					wb.Worksheet("All Answers Key list").Columns("4").Width = 30;
					wb.Worksheet("All Answers Key list").Columns("5").Width = 30;
					wb.Worksheet("All Answers Key list").Columns("6").Width = 30;
					wb.Worksheet("All Answers Key list").Columns("7").Width = 30;
					using (MemoryStream stream = new())
					{
						wb.SaveAs(stream);

						logger.LogDebug($"UserMangementController > Method Name: GetusermaagemetCompleteReport() completed. ProjectId={GetCurrentProjectId()}");

						return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");

					}
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error in UserManagementController > GetusermaagemetCompleteReport().");
				throw;
			}

		}

		private static DataTable ToGetAnswerDataTable(List<AllAnswerKeysModel> userdatalist)
		{
			DataTable dt = new DataTable();

			dt.Columns.Add("QuestionCode", typeof(string));
			dt.Columns.Add("Blank Code", typeof(string));
			dt.Columns.Add("Question Marks", typeof(string));
			dt.Columns.Add("QuestionType", typeof(string));
			dt.Columns.Add("ChoiceText", typeof(string));
			dt.Columns.Add("OptionText", typeof(string));
			if (userdatalist.Count > 0)
			{
				userdatalist.ForEach(uc =>

				{

					dt.Rows.Add(uc.ParentQuestionCode, uc.QuestionCode, uc.QuestionMarks, uc.QuestionName, uc.ChoiceText, uc.OptionText);

				});
			}

			return dt;
		}

		/// <summary>
		/// Get mail sent details.
		/// </summary> 
		/// <returns></returns> 
		[Route("MailSentDetails")]
		[HttpPost]
		public async Task<IActionResult> MailSentDetails(ClsMailSent clsMailSent)
		{
			long UserId = 0;
			List<Mailsentdetails> result = new List<Mailsentdetails>();
			try
			{
				UserId = GetCurrentUserId();
				clsMailSent.ProjectUserRoleID = GetCurrentProjectUserRoleID();
				clsMailSent.UserTimeZone = GetCurrentContextTimeZone();
				logger.LogDebug($"AdminToolsController > MailSentDetails() started. MailSentModel = {clsMailSent} ");

				result = await admintoolsService.MailSentDetails(clsMailSent);

				logger.LogDebug($"AdminToolsController > MailSentDetails() completed. MailSentModel = {clsMailSent}");

				return Ok(result);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error in AdminToolsController > MailSentDetails() method.MailSentModel = {clsMailSent}");
				throw;
			}
		}

		/// <summary>
		/// Get api for export pdf mail sent details
		/// </summary> 
		/// <returns></returns> 
		[Route("Export-mail-sent-report")]
		[HttpGet]
		[ApiVersion("1.0")]
		//  [Authorize(Roles = "SUPERADMIN,SERVICEADMIN")]
		public async Task<IActionResult> ExportMailSentDetails()
		{

			List<Mailsentdetails> result = new List<Mailsentdetails>();
			ClsMailSent clsMailSent = new ClsMailSent();
			clsMailSent.PageNo = 0;
			clsMailSent.PageSize = 0;
			clsMailSent.SearchText = "";
			clsMailSent.IsEnabled = 2;
			clsMailSent.UserTimeZone = GetCurrentContextTimeZone();
			try
			{
				logger.LogInformation($"AdminToolsController > ExportMailSentDetails() started. ProjectId={GetCurrentProjectId()}");

				clsMailSent.ProjectUserRoleID = GetCurrentProjectUserRoleID();

				result = await admintoolsService.MailSentDetails(clsMailSent);
				DataTable dt = ToGetUserDataTable(result);

				using (XLWorkbook wb = new XLWorkbook())
				{

					wb.Worksheets.Add(dt, "Mail Sent Details");
					wb.Worksheet("Mail Sent Details").Columns("1").Width = 30;
					wb.Worksheet("Mail Sent Details").Columns("2").Width = 30;
					wb.Worksheet("Mail Sent Details").Columns("3").Width = 30;
					wb.Worksheet("Mail Sent Details").Columns("4").Width = 30;
					wb.Worksheet("Mail Sent Details").Columns("5").Width = 30;
					using (MemoryStream stream = new MemoryStream())
					{
						wb.SaveAs(stream);

						logger.LogDebug($"AdminToolsController > Method Name: ExportMailSentDetails() completed. ProjectId={GetCurrentProjectId()}");

						return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");

					}
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error in AdminToolsController > ExportMailSentDetails(). ProjectId={GetCurrentProjectId()}");
				throw;
			}

		}

		private static DataTable ToGetUserDataTable(List<Mailsentdetails> md)
		{
			DataTable dt = new DataTable();

			dt.Columns.Add("UserName", typeof(string));
			dt.Columns.Add("LoginName", typeof(string));
			dt.Columns.Add("Status", typeof(string));
			dt.Columns.Add("Invite Sent Status", typeof(string));
			dt.Columns.Add("Date of Mail", typeof(string));

			if (md.Count > 0)
			{
				md.ForEach(uc =>
				{
					dt.Rows.Add(uc.UserName, uc.LoginName, uc.IsActive ? "Disabled" : "Enabled", uc.IsMailSent ? "Mail Sent" : "Mail Not Sent", uc.MailSentDate);
				});
			}

			return dt;
		}

		/// <summary>
		/// Get FIDI Report Details
		/// </summary> 
		/// <param name="projectId"></param>
		/// <param name="searchFilterModel"></param>
		/// <param name="IsDownload"></param>
		/// <returns></returns> 
		[Route("{projectId}/fidireport/{IsDownload}/{syncMetaData}")]
		[Authorize(Roles = "SUPERADMIN,SERVICEADMIN,EO,EM,AO")]
		[HttpPost]
		public async Task<IActionResult> FIDIReportDetails(long projectId, SearchFilterModel searchFilterModel, bool syncMetaData, bool IsDownload = false)
		{
			try
			{
				logger.LogDebug("AdminToolsController > FIDIReportDetails() started. ProjectID = {projectId} and SearchFilterModel = {searchFilterModel} and IsDownload = {IsDownload}", projectId, searchFilterModel, IsDownload);
				FIDIReportModel result = await admintoolsService.FIDIReportDetails(projectId, searchFilterModel, IsDownload, syncMetaData);
				logger.LogDebug("AdminToolsController > FIDIReportDetails() completed. ProjectID = {projectId} and SearchFilterModel = {searchFilterModel} and IsDownload = {IsDownload}", projectId, searchFilterModel, IsDownload);
				return Ok(result);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in AdminToolsController > FIDIReportDetails() method. ProjectID = {projectId} and SearchFilterModel = {searchFilterModel} and IsDownload = {IsDownload}", projectId, searchFilterModel, IsDownload);
				throw;
			}
		}

		/// <summary>
		/// Get ProjectStatus
		/// </summary> 
		/// <param name="projectId"></param>
		/// <returns></returns> 
		[Route("{projectId}")]
		[HttpPost]
		public async Task<byte> ProjectStatus(long projectId)
		{
			byte result;
			try
			{
				logger.LogDebug("AdminToolsController > ProjectStatus() started. ProjectID = {projectId}", projectId);
				result = await admintoolsService.ProjectStatus(projectId);
				logger.LogDebug("AdminToolsController > ProjectStatus() completed. ProjectID = {projectId}", projectId);
				return result;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in AdminToolsController > ProjectStatus() method. ProjectID = {projectId}", projectId);
				throw;
			}
		}

		/// <summary>
		/// GetProjectAssessmentInfo : Method to Get Project Assessment Info Details
		/// </summary>
		/// <returns></returns>
		[Route("marker-performance-analysis/{projectId}/{IsDownload}")]
		[HttpPost]
		[Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
		public async Task<IActionResult> GetMarkerPerformanceReport(long projectId, SearchFilterModel searchFilterModel, bool IsDownload = false)
		{
			////long projectId = 0;
			long UserId = 0;
			List<BindAllMarkerPerformanceModel> result = new List<BindAllMarkerPerformanceModel>();
			try
			{
				////projectId = GetCurrentProjectId();
				////if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
				////{
				////    return new ForbidResult();
				////}
				UserId = GetCurrentUserId();
				result = await admintoolsService.GetMarkerPerformanceReport(projectId, searchFilterModel, UserId, GetCurrentContextTimeZone(), IsDownload);
				return Ok(result);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in AdminToolsController : Method Name: GetMarkerPerformanceReport() and ProjectID = {projectId}", projectId);
				throw;
			}
		}
		[Route("SyncMetaData")]
		[Authorize(Roles = "SUPERADMIN,SERVICEADMIN,EO,EM,AO")]
		[HttpPost]
		public async Task<IActionResult> SyncMetaData(List<SyncMetaDataModel> SyncMetaData)
		{
			SyncMetaDataResult result = null;
			try
			{
				logger.LogDebug("AdminToolsController > SyncMetaData() started. ProjectID = {projectId} and SyncMetaData = {SyncMetaData}", GetCurrentProjectId(), JsonConvert.SerializeObject(SyncMetaData));
				result = await admintoolsService.SyncMetaData(SyncMetaData);
				logger.LogDebug("AdminToolsController > SyncMetaData() completed. ProjectID = {projectId} and SyncMetaData = {SyncMetaData} and result ", GetCurrentProjectId(), JsonConvert.SerializeObject(SyncMetaData), JsonConvert.SerializeObject(result));
				return Ok(result);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in AdminToolsController > SyncMetaData() method. ProjectID = {projectId} and SyncMetaData = {SyncMetaData}", GetCurrentProjectId(), SyncMetaData);
				throw;
			}
			//finally
			//{
			//	#region Insert Audit Trail

			//	_ = InsertAuditLogs(new AuditTrailData
			//	{
			//		Event = AuditTrailEvent.UPLOAD,
			//		Module = AuditTrailModule.REPORTS,
			//		Entity = AuditTrailEntity.FIDIREPORT,
			//		UserId = GetCurrentUserId(),
			//		ProjectUserRoleID = GetCurrentProjectUserRoleID(),
			//		Remarks = result,
			//		//ResponseStatus = AuditTrailResponseStatus.Success,P001
			//		ResponseStatus = result?.StatusCode == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
			//		Response = result?.StatusCode
			//	});
			//	#endregion
			//}
		}


	}
}
