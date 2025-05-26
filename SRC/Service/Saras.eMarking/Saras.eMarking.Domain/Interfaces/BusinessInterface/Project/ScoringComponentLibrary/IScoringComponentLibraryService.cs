using Saras.eMarking.Domain.ViewModels.Project.MarkScheme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Saras.eMarking.Domain.ViewModels.Project.ScoringComponentLibrary.ScoringComponentLibraryModel;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.ScoringComponentLibrary
{
    public interface IScoringComponentLibraryService
    {
        Task<IList<ScoreComponentLibraryName>> GetAllScoringComponentLibrary(long projectId);


        Task<string> CreateScoringComponentLibrary(ScoreComponentLibraryName markScheme, long projectId, long ProjectUserRoleID);

        Task<string> DeleteScoringComponentLibrary(long projectId, List<long> markSchemeids, long ProjectUserRoleID);
        Task<ScoreComponentLibraryName> ViewScoringComponentLibrary(long ComponentId);

    }
}
