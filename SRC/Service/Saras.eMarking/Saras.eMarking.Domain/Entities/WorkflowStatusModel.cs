using Saras.eMarking.Domain.ViewModels;

namespace Saras.eMarking.Domain.Entities
{
    public class WorkflowStatusModel
    {
        public int WorkflowID { get; set; }
        public string WorkflowCode { get; set; }
        public string WorkflowName { get; set; }
        public EnumWorkflowType WorkflowType { get; set; }
    }

}
