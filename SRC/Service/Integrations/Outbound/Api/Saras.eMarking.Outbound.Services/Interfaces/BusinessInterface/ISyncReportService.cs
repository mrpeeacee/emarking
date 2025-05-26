using Saras.eMarking.Outbound.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saras.eMarking.Outbound.Services.Interfaces.BusinessInterface
{
    public interface ISyncReportService
    {
        SyncResponseModel SyncReportToiExam(SyncReportModel syncReportModel);


        SyncResponseModel SyncEMSReportToiExam(SyncReportModel syncReportModel);
    }
}
