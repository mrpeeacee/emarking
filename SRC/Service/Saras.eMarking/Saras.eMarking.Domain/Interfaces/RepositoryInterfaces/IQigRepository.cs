using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saras.eMarking.Domain.ViewModels;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces
{
    public interface IQigRepository
    {
        Task<IList<QigModel>> GetAllQIGs(Int64 ProjectId);
        Task<bool> UpdateQigSetting(QigModel objQigModel, long projectId, long ProjectUserRoleID);
        Task<IList<QigQuestionModel>> GetAllQigQuestions(long ProjectId, long QigId);
        Task<List<WorkflowStatusTrackingModel>> GetQigWorkflowTracking(long projectId, long entityid, EnumAppSettingEntityType entitytype);
        Task<IList<UserQigModel>> GetQIGs(long ProjectId, long ProjectUserRoleID, long? Qigtype);
    }
}
