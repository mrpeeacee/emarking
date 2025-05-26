using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Report;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Setup;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Report
{
    public class TestcentrerwiseesponseReportSevice : BaseService<TestcentrerwiseesponseReportSevice>, ITestcenterReportService
    {
        readonly ITestcenterReportRepository TestcenterReportRepository;
        public static AppOptions _AppOptions { get; set; }

        public TestcentrerwiseesponseReportSevice(ITestcenterReportRepository _TestcenterReportRepository, ILogger<TestcentrerwiseesponseReportSevice> _logger, AppOptions _appOptions) : base(_logger, _appOptions)
        {
            TestcenterReportRepository = _TestcenterReportRepository;
            _AppOptions = _appOptions;
        }
        public async Task<IList<ExamCenterModel>> ProjectCenters(long ProjectId)
        {

            try
            {
                logger.LogInformation($"TestcentrerwiseesponseReportSevice > ProjectCenters() started. ProjectId = {ProjectId}");


                logger.LogInformation($"TestcentrerwiseesponseReportSevice > ProjectCenters() ended. ProjectId = {ProjectId}");

                return await TestcenterReportRepository.ProjectCenters(ProjectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in TestcentrerwiseesponseReportSevice > ProjectCenters(). ProjectId = {ProjectId} ");
                throw;
            }
        }
        public async Task<string> getquestiondetails(long questionid, long projectid)
        {

            try
            {
                logger.LogInformation($"TestcentrerwiseesponseReportSevice > getquestiondetails() started. questionid = {questionid}");


                logger.LogInformation($"TestcentrerwiseesponseReportSevice > getquestiondetails() ended. questionid = {questionid}");

                return await TestcenterReportRepository.getquestiondetails(questionid, projectid);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in TestcentrerwiseesponseReportSevice > getquestiondetails(). questionid = {questionid} ");
                throw;
            }
        }

    }
}
