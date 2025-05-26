using Saras.eMarking.Domain.ViewModels.Report;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Report
{
    public interface IMarkersPerformanceService
    {
        public Task<List<SchoolDetails>> GetSchoolDetails(long ProjectId);
        public Task<MarkerPerformanceStatistics> GetMarkerPerformanceDetails(MarkerDetails markerDetails);
        public Task<List<MarkerPerformance>> GetMarkerPerformance(MarkerDetails markerDetails);
    }
}
