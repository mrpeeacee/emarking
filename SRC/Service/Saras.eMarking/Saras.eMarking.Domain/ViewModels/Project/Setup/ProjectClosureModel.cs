using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels.Project.Setup
{
    public class ProjectClosureModel
    {
        public ProjectClosureModel()
        {
            QigModels = new List<ProjectClosureQigModel>();
            DiscrepancyModels = new List<CheckDiscrepancyModel>();
        }
        public string Remarks { get; set; }
        public string ReopenRemarks { get; set; }
        public string ProjectStatus { get; set; }
        public bool Rpackexist { get; set; }
        public List<ProjectClosureQigModel> QigModels { get; set; }
        public List<CheckDiscrepancyModel> DiscrepancyModels { get; set; }
        
    }
    public class ProjectClosureQigModel
    {
        public string QigName { get; set; }
        public long TotalScriptCount { get; set; }
        public long ManualMarkingCount { get; set; }
        public long LivePoolScriptCount { get; set; }
        public long SubmittedScriptCount { get; set; }
        public long Rc1UnApprovedCount { get; set; }
        public long Rc2UnApprovedCount { get; set; }
        public long CheckedOutScripts { get; set; }
        public long QuestionsType { get; set; }
        public long ToBeSampledForRC2 { get; set; }
        public long RC2Exists { get; set; }
        public long ToBeSampledForRC1 { get; set; }
        public long TotalSubmitted { get; set; }
        public long QigId { get; set; }
    }
    public class CheckDiscrepancyModel
    {
        public string QigName { get; set; }
        public bool IsDiscrepancyExist { get; set; }
    }
    public enum SyncResponseStatus
    {
        Success = 1,
        Error = 2
    }

    public class SyncResponseModel
    {
        public string Message { get; set; }
        public SyncResponseStatus Status { get; set; } = SyncResponseStatus.Error;
        public string Content { get; set; }
    }
}
