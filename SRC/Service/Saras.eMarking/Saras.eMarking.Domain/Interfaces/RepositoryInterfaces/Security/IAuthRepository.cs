using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Auth;
using System.Data;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Security
{
    public interface IAuthRepository
    {
        UserContext Authenticate(AuthenticateRequestModel model, bool isSso = false);      
            UserContext GetuserDetails(AuthenticateRequestModel model);
        Task<string> ChangePassword(ChangePasswordRequestModel ObjCandidatesAnswerModel, string LoginId);
        UserContext GetUserContext(string LoginID, long projectid = 0, bool IsArchive = false);
        bool UpdateRefreshToken(UserLoginTokenModel token, string ipAddress, bool IsLogout, long projectId = 0);
        UserLoginTokenModel GetRefreshToken(string refreshtoken, string jwttoken);
        bool RevokeToken(string token);
        UserLoginToken IsTokenValid(string jwttoken, string refToken, long userid);

        bool IsValidProject(long ProjectId, long ProjectUserRoleId);
        bool IsValidProjectQig(long ProjectId, long ProjectUserRoleId, long QigId, bool? IsKp = null);
        bool IsValidProjectQigScript(long ProjectId, long QigId, long ScriptId);
        Task<ForgotPasswordModel> ForgotPassword(ForgotPasswordModel objForgotpassword, long CurrentProjUserRoleId);
        Task<string> ActivateorDeactivateUser(long userid, long activetype, long projectid);
        Task<CaptchaModel> CreateCaptcha(string CaptchText);
        Task<ForgotPasswordModel> IsValidateCaptcha(ForgotPasswordModel ObjForgotPasswordRequestModel);
        Task<ForgotPasswordModel> GetUserId(ForgotPasswordModel ObjForgotPasswordRequestModel);
        bool IsValidProjectQigUser(long ProjectId, long ProjectUserRoleId, long QigId);

        string  CheckEmarkingUser(EmarkingSsoRequest objUser);







    }
}
