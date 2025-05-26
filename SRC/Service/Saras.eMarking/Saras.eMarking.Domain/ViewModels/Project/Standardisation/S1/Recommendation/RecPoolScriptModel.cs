using Saras.eMarking.Domain.Extensions;
using System;

namespace Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Recommendation
{
    public class RecPoolScriptModel
    {
        public RecPoolScriptModel()
        {

        }
        public long? ProjectID { get; set; }
        public long ScriptID { get; set; }
        [XssTextValidation]
        public string ScriptName { get; set; }
        [XssTextValidation]
        public string RoleCode { get; set; }
        public bool? IsRecommended { get; set; }
        public bool? IsNullResponse { get; set; }
        
        [XssTextValidation]
        public string RecommendedBy { get; set; }
        public bool IsRecommendedByMe { get; set; }
        public WorkflowProcessStatus ProcessStatus { get; set; }
        [XssTextValidation]
        public string WorkFlowStatusCode { get; set; }
        public long CenterID { get; set; }
        public string CenterName { get; set; }
        public string CenterCode { get; set; }
        public Boolean IscenterSelected { get; set; }
    }
}
