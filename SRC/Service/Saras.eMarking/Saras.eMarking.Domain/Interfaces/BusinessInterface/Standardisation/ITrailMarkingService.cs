using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Recommendation;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Standardisation
{
    public interface ITrailMarkingService
    {
        Task<bool> ResponseMarking(List<QuestionUserResponseMarkingDetailModel> markingResponseDetails, long projectid, long ProjectUserRoleID, long qigid,bool IsAutoSave);
        Task<RecQuestionModel> GetScriptQuestionResponse(long ProjectId, long ScriptId, long ProjectQuestionId, bool IsDefault);

        Task<IList<UserScriptResponseModel>> UserScriptMarking(UserScriptMarkingDetails UsersScriptMarkingDetails);
        Task<QuestionUserResponseMarkingDetail> ResponseMarkingDetails(long Scriptid, long ProjectQuestionResponseID, Nullable<long> ProjectUserRoleID, long? workflowstatusid, long? UserScriptMarkingRefId);
        Task<bool> MarkingSubmit(long scriptid, long projectid, long ProjectUserRoleID, long? workflowstatusid);
        Task<bool> MarkingScriptTimeTracking(MarkingScriptTimeTrackingModel MarkingScriptTimeTracking);
        Task<ValidateAnnotationsanddescreteModel> validateannotation(long projectid, long qigid, byte EntityType, string ProjectUserRoleCode, long ProjectUserRoleID);
        Task<List<Trialmarkingannotationmodel>> Getannoatationdetails(long qigid);

        Task<CatagarizedandS1Completedmodel> Getcatagarizedands1configureddetails(long? qigid, long? scriptid, long? workflowid);
        Task<DownloadMarkschemeModel> Viewanddownloadmarkscheme(long projectquestionId, long projectId, long? markschemeid = null);
        Task<List<ViewScriptModel>> ViewScript(long projectID, long projectUserRoleID, ViewScriptModel objView, UserTimeZone userTimeZone);

    }
       
        
}
