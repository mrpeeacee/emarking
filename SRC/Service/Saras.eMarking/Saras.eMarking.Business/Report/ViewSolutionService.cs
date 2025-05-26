using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Report;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report;
using Saras.eMarking.Domain.ViewModels.Report;
using Saras.eMarking.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Saras.eMarking.Domain.Entities;

namespace Saras.eMarking.Business.Report
{
    public class ViewSolutionService : BaseService<ViewSolutionService>, IViewSolutionService
    {
        readonly IViewSolutionRepository viewSolutionRepository;
        public static AppOptions _AppOptions { get; set; }
        public ViewSolutionService(IViewSolutionRepository _viewSolutionRepository, ILogger<ViewSolutionService> _logger, AppOptions _appOptions) : base(_logger, _appOptions)
        {
            viewSolutionRepository = _viewSolutionRepository;
            _AppOptions = _appOptions;
        }
        public async Task<ScheduleDetailsModel> GetUserScheduleDetails(long ProjectId, long UserId)
        {
            try
            {
                logger.LogInformation($"ViewSolutionService > GetUserScheduleDetails() started. ProjectId = {ProjectId} and UserId = {UserId}");

                ScheduleDetailsModel res = await viewSolutionRepository.GetUserScheduleDetails(ProjectId, UserId);
                if (res != null)
                {
                    res.Url = _AppOptions.AppSettings.ViewSolutionURL;
                    res.Solution = HttpUtility.UrlEncode(GetEncryptedValue("True"), Encoding.Default);
                    res.AssessmentId = HttpUtility.UrlEncode(GetEncryptedValue(Convert.ToString(res.AssessmentId)), Encoding.Default);
                    res.ScheduleUserId = HttpUtility.UrlEncode(GetEncryptedValue(Convert.ToString(res.ScheduleUserId)), Encoding.Default);
                    res.SectionId = HttpUtility.UrlEncode(GetEncryptedValue(Convert.ToString(res.SectionId)), Encoding.Default);
                    res.UserId = HttpUtility.UrlEncode(GetEncryptedValue(Convert.ToString(UserId)), Encoding.Default);
                    res.Theme = HttpUtility.UrlEncode(GetEncryptedValue("BrightIndigo"), Encoding.Default);
                    res.SumType = HttpUtility.UrlEncode(GetEncryptedValue("3"), Encoding.Default);
                    res.TestType = HttpUtility.UrlEncode(GetEncryptedValue("0"), Encoding.Default);
                    res.Page = HttpUtility.UrlEncode(GetEncryptedValue("TESTPLAYER"), Encoding.Default);
                    res.culture = HttpUtility.UrlEncode(GetEncryptedValue("en-us"), Encoding.Default);
                    res.Key = HttpUtility.UrlEncode(GetEncryptedValue("1"), Encoding.Default);
                    res.TestMode = HttpUtility.UrlEncode(GetEncryptedValue("0"), Encoding.Default);
                    res.TimeStamp = HttpUtility.UrlEncode(GetEncryptedValue(DateTime.Now.AddMinutes(5).ToString()), Encoding.Default);
                }
                logger.LogInformation($"ViewSolutionService > GetUserScheduleDetails() ended. ProjectId = {ProjectId} and ExamYear = {UserId}");

                return res;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ViewSolutionService > GetUserScheduleDetails(). ProjectId = {ProjectId} and ExamUserIdYear = {UserId}");
                throw;
            }
        }
        public string GetEncryptedValue(string StrString)
        {
            try
            {
                if (StrString.Length > 0)
                {
                    return EncryptDecryptBl.EncryptAes(StrString.Trim());
                }
                else
                {
                    return string.Empty;
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ViewSolutionService > GetEncryptedValue(). StrString = {StrString}");
                return string.Empty;
            }
        }
        public async Task<List<UserQuestionResponse>> GetUserResponse(long ProjectId, long UserId, bool Isfrommarkingplayer, long Testcentreid, bool reportstatus)
        {
            try
            {
                logger.LogInformation($"ViewSolutionService > GetUserScheduleDetails() started. ProjectId = {ProjectId} and UserId = {UserId} and TestcentreId = {Testcentreid} and ReportStatus = {reportstatus}");

                List<UserQuestionResponse> res = await viewSolutionRepository.GetUserResponse(ProjectId, UserId, Isfrommarkingplayer, Testcentreid, reportstatus);

                return res;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ViewSolutionService > GetUserScheduleDetails(). ProjectId = {ProjectId} and ExamUserIdYear = {UserId} and TestcentreId = {Testcentreid} and ReportStatus = {reportstatus}");
                throw;
            }
        }
        public async Task<List<AllUserQuestionResponses>> GetUserResponses(long ProjectId, int MaskingRequired,int PreOrPostMarking,SearchFilterModel searchFilterModel)
        {
            try
            {
                logger.LogInformation($"Error in ViewSolutionService > GetUserScheduleDetails()");

                List<AllUserQuestionResponses> res = await viewSolutionRepository.GetUserResponses(ProjectId, MaskingRequired, PreOrPostMarking, searchFilterModel);

                return res;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ViewSolutionService > GetUserScheduleDetails()");
                throw;
            }
        }
        public async Task<List<SchoolInfoDetails>> GetSchools(long ProjectId)
        {
            try
            {
                logger.LogInformation($"Error in ViewSolutionService > GetSchools()");

                List<SchoolInfoDetails> res = await viewSolutionRepository.GetSchools(ProjectId);
                return res;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ViewSolutionService > GetSchools()");
                throw;
            }
        }
    }
}
