using Saras.eMarking.Domain.Configuration;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.ViewModels.Auth;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Account
{
    public interface IAuthService
    {
        AuthenticateResponseModel Authenticate(AuthenticateRequestModel model, string ipAddress, bool isSso);
        Task<string> ChangePassword(ChangePasswordRequestModel ObjCandidatesAnswerModel, string LoginId);
        AuthenticateResponseModel RefreshToken(string refToken, string jwttoken, string ipAddress, long projectid, string csrftoken);
        bool RevokeToken(string token, string jwttoken, string ipAddress, bool IsLogout);
        UserLoginToken IsTokenValid(string jwttoken, string refToken, string refAccessToken);
        bool IsValidProject(long ProjectId, long ProjectUserRoleId);
        bool IsValidProjectQig(long ProjectId, long ProjectUserRoleId, long QigId, bool? IsKp = null);
        bool IsValidProjectQigScript(long ProjectId, long QigId, long ScriptId);
        Task<ForgotPasswordModel> ForgotPassword(ForgotPasswordModel objForgotpassword, long CurrentProjUserRoleId);
        Task<string> ActivateorDeactivateUser(long userid, long activetype, long projectId);
        Task<CaptchaModel> CreateCaptcha();
        bool IsValidProjectQigUser(long ProjectId, long ProjectUserRoleId, long QigId);


        
        string ValidateSsoArchiveToken(string SsoJwtToken, SsoIntegrationOptions ssoIntegrationOptions);

        string EmarkingSSOArchive(EmarkingSsoRequest emarkingSsoRequest);

        string EmarkingSSOLive(EmarkingSsoRequest emarkingSsoRequest);












    }
}

