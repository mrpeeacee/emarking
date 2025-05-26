using Saras.eMarking.Domain.ViewModels.Project.MarkScheme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Saras.eMarking.Domain.ViewModels.Project.ScoringComponentLibrary.ScoringComponentLibraryModel;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.ScoringComponentLibrary
{
    public interface IScoringComponentLibraryRepository
    {
        Task<IList<ScoreComponentLibraryName>> GetAllScoringComponentLibrary(long projectId);
        Task<string> CreateScoringSomponentLibrary(ScoreComponentLibraryName markScheme, long projectId, long ProjectUserRoleID);

        Task<string> DeleteScoringComponentLibrary(long projectId, List<long> ScoreComponentID, long ProjectUserRoleID);

        Task<ScoreComponentLibraryName> ViewScoringComponentLibrary(long ComponentId);

    }
}
