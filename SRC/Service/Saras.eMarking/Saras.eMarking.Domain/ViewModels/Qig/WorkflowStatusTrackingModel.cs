using Saras.eMarking.Domain.Extensions;

namespace Saras.eMarking.Domain.ViewModels
{
    public class WorkflowStatusTrackingModel
    {
        [XssTextValidation]
        public string WorkflowStatusCode { get; set; }
        public WorkflowProcessStatus ProcessStatus { get; set; }
        [XssTextValidation]
        public string Remark { get; set; }
        public byte ProjectStatus { get; set; }
    }
}
