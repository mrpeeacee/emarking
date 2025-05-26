using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Report;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report;
using Saras.eMarking.Domain.ViewModels.Report;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Report
{
    public class MarkersPerformanceService : BaseService<MarkersPerformanceService>, IMarkersPerformanceService
    {
        readonly IMarkersPerformanceRepository markingOverviewsRepository;
        public MarkersPerformanceService(IMarkersPerformanceRepository _markingOverviewsRepository, ILogger<MarkersPerformanceService> _logger) : base(_logger)
        {
            markingOverviewsRepository = _markingOverviewsRepository;
        }
        public async Task<List<SchoolDetails>> GetSchoolDetails(long ProjectId)
        {
            try
            {
                logger.LogInformation($"MarkersPerformanceService > GetSchoolDetails() started. ProjectId = {ProjectId}");

                List<SchoolDetails> res = await markingOverviewsRepository.GetSchoolDetails(ProjectId);

                logger.LogInformation($"MarkersPerformanceService > GetSchoolDetails() ended. ProjectId = {ProjectId}");

                return res;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkersPerformanceService > GetSchoolDetails(). ProjectId = {ProjectId}");
                throw;
            }
        }
        public async Task<MarkerPerformanceStatistics> GetMarkerPerformanceDetails(MarkerDetails markerDetails)
        {
            try
            {
                logger.LogInformation($"MarkersPerformanceService > GetMarkerPerformanceDetails() started. ProjectId = {markerDetails.ProjectId} and QigId = {markerDetails.QigID} and ExamYear = {markerDetails.ExamYear} and MarkerName = {markerDetails.MarkerName} and SchoolCode = {markerDetails.SchoolCode}");

                MarkerPerformanceStatistics res = await markingOverviewsRepository.GetMarkerPerformanceDetails(markerDetails);

                logger.LogInformation($"MarkersPerformanceService > GetMarkerPerformanceDetails() ended. ProjectId = {markerDetails.ProjectId} and QigId = {markerDetails.QigID} and ExamYear = {markerDetails.ExamYear} and MarkerName = {markerDetails.MarkerName} and SchoolCode = {markerDetails.SchoolCode}");

                return res;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkersPerformanceService > GetMarkerPerformanceDetails(). ProjectId = {markerDetails.ProjectId} and QigId = {markerDetails.QigID} and ExamYear = {markerDetails.ExamYear} and MarkerName = {markerDetails.MarkerName} and SchoolCode = {markerDetails.SchoolCode}");
                throw;
            }
        }
        public async Task<List<MarkerPerformance>> GetMarkerPerformance(MarkerDetails markerDetails)
        {
            try
            {
                logger.LogInformation($"MarkersPerformanceService > GetMarkerPerformance() started. ProjectId = {markerDetails.ProjectId} and QigId = {markerDetails.QigID} and ExamYear = {markerDetails.ExamYear} and MarkerName = {markerDetails.MarkerName} and SchoolCode = {markerDetails.SchoolCode}");

                List<MarkerPerformance> res = await markingOverviewsRepository.GetMarkerPerformance(markerDetails);

                logger.LogInformation($"MarkersPerformanceService > GetMarkerPerformance() ended. ProjectId = {markerDetails.ProjectId} and QigId = {markerDetails.QigID} and ExamYear = {markerDetails.ExamYear} and MarkerName = {markerDetails.MarkerName} and SchoolCode = {markerDetails.SchoolCode}");

                return res;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkersPerformanceService > GetMarkerPerformance(). ProjectId = {markerDetails.ProjectId} and QigId = {markerDetails.QigID} and ExamYear = {markerDetails.ExamYear} and MarkerName = {markerDetails.MarkerName} and SchoolCode = {markerDetails.SchoolCode}");
                throw;
            }
        }
    }
}
