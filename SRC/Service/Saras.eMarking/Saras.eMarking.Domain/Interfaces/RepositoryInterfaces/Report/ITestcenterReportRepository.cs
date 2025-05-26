using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Setup;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report
{
    public interface ITestcenterReportRepository
    {
        public Task<IList<ExamCenterModel>> ProjectCenters(long ProjectId);

        public Task<string> getquestiondetails(long questionid, long projectid);
    
    }
}
