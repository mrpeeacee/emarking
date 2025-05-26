using iExamSecurity;
using iExamSecurity.Util;
using iExamSync;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using Saras.eMarking.Outbound.Services.Interfaces.BusinessInterface;
using Saras.eMarking.Outbound.Services.Model;

using System.Net;
using System.Text;
using Options = iExamSecurity.Util.Options;

namespace Saras.eMarking.Outbound.Controllers
{
    [Route("/api/reports")]
    public class ReportsController : Controller
    {
        private readonly ISyncReportService syncReportService;
        private readonly ILogger<ReportsController> logger;
        private readonly AppSettings _appSettings;
        private readonly IWebHostEnvironment env;
        readonly string iEXAMS2_EG_app_id;
        readonly string Nonce = string.Empty;
        readonly string TimeStamp = string.Empty;
        readonly string ApiKey;
        readonly string RandSymmKey = string.Empty;
        public ReportsController(ILogger<ReportsController> _logger, IOptionsMonitor<AppSettings> appSettings, IWebHostEnvironment env, ISyncReportService _syncReportService)
        {
            logger = _logger;
            syncReportService = _syncReportService;
            _appSettings = appSettings.CurrentValue;
            this.env = env;

            IexamToken.UAT_EEXAM_PRI_KEY = _appSettings.eExamsPriKey;
            IexamToken.UAT_EEXAM_PUB_KEY = _appSettings.eExamsPubKey;
            IexamToken.UAT_PUB_KEY = _appSettings.iExamsPubKey;
            IexamToken.UAT_PRI_KEY = _appSettings.iExamsPriKey;
            IexamToken.KEY_ALGORITHM = _appSettings.KEY_ALGORITHM;
            IexamToken.AESGCMALGORTITHM = _appSettings.AESGCMALGORTITHM;
            IexamToken.INITVECTOR = _appSettings.INITVECTOR;
            IexamToken.SIGNING = _appSettings.SIGNING;
            IexamToken.RandomGenerator = _appSettings.RANDOMGENERATOR;
            IexamToken.ALGORITHM = _appSettings.ALGORITHM;
            iEXAMS2_EG_app_id = _appSettings.APPId;
            ApiKey = _appSettings.APIKey;
            if (String.IsNullOrEmpty(Nonce))
                Nonce = Convert.ToString(IexamToken.generateNonceRandom());
            if (String.IsNullOrEmpty(TimeStamp))
                TimeStamp = Convert.ToString(TimeUtils.GetUnixTimstamp(DateTime.UtcNow));
            if (String.IsNullOrEmpty(RandSymmKey))
                RandSymmKey = IexamToken.GenRandSymmKey32();
        }

        [Route("report-sync")]
        [HttpPost]
        public IActionResult ReportSync([FromForm] SyncRequestModel syncRequestModel)
        {
            SyncResponseModel result = new SyncResponseModel();
            try
            {
                logger.LogDebug($"ReportsController > ReportSync() started.");

                if (syncRequestModel != null && !string.IsNullOrEmpty(syncRequestModel.Payload))
                {
                    SyncReportModel? syncReportModel = JsonConvert.DeserializeObject<SyncReportModel>(syncRequestModel.Payload);
                    if (syncReportModel != null)
                    {

                        string url = _appSettings.UploadFileApiUrl;

                        switch (syncReportModel.ReportType)
                        {
                            case 1:
                                url = url.Replace("{type}", "EMS1");
                                url = url.Replace("{year}", Convert.ToString(syncReportModel.Year));
                                break;
                            case 2:
                                url = url.Replace("{type}", "EMS2");
                                url = url.Replace("{year}", Convert.ToString(syncReportModel.Year));
                                break;
                            case 3:
                                url = url.Replace("{type}", "OMS");
                                url = url.Replace("{year}", Convert.ToString(syncReportModel.Year));
                                break;
                        }

                        string readfilepath = syncReportModel.RootFolder + @"\\" + syncReportModel.FileId + "~" + syncReportModel.FileName;

                        logger.LogInformation("File name : " + readfilepath);

                        url = url.Replace("{correlationId}", syncReportModel.FileId);

                        ClaimsApiRequest emsBody = ConstructEmsRequest();

                        string strResponseText = string.Empty;
                        string boundary = "----------------------------638175687184405782";

                        logger.LogInformation("URL: " + url);

                        logger.LogInformation("boundary: " + boundary);
                        logger.LogInformation("multipart/form-data; boundary=" + boundary);

                        string filecontent = string.Empty;
                        foreach (string line in System.IO.File.ReadLines(readfilepath))
                        {
                            filecontent += line + "\r\n";
                        }

                        try
                        {
                            WebProxy proxy = new WebProxy(_appSettings.ProxyUrl);

                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                            request.Proxy = proxy;
                            request.Method = "POST";
                            request.ContentType = "multipart/form-data; boundary=" + boundary;
                            request.Headers["iEXAMS2_EG_app_id"] = _appSettings.APPId;

                            if (emsBody.request != null && emsBody.request.body != null && emsBody.request.header.Count > 0)
                            {
                                var headerVal = emsBody.request.header.FirstOrDefault(a => a.key == "iEXAMS2_EG_APIKey_enc");
                                request.Headers["iEXAMS2_EG_APIKey_enc"] = headerVal?.value;
                                headerVal = emsBody.request.header.FirstOrDefault(a => a.key == "iEXAMS2_EG_nonce");
                                request.Headers["iEXAMS2_EG_nonce"] = headerVal?.value;
                                headerVal = emsBody.request.header.FirstOrDefault(a => a.key == "iEXAMS2_EG_timestamp");
                                request.Headers["iEXAMS2_EG_timestamp"] = headerVal?.value;
                                headerVal = emsBody.request.header.FirstOrDefault(a => a.key == "iEXAMS2_EG_datakey_enc");
                                request.Headers["iEXAMS2_EG_datakey_enc"] = headerVal?.value;
                                headerVal = emsBody.request.header.FirstOrDefault(a => a.key == "iEXAMS2_EG_signature");
                                request.Headers["iEXAMS2_EG_signature"] = headerVal?.value;
                            }
                            request.Headers["iEXAMS2_EG_token"] = _appSettings.IsJwtFromClaims ? CreateJwt(emsBody) : CreateJwt();

                            request.Timeout = 120000;

                            using (Stream requestStream = request.GetRequestStream())
                            {
                                string requestBody = "------------------------------638175687184405782\r\n"
                                    + "Content-Disposition: form-data; name=\"file\"; filename=\"" + syncReportModel.FileName + "\"\r\n"
                                    + "Content-Type: text/plain\r\n"
                                    + "\r\n";

                                requestBody += filecontent;

                                requestBody += "------------------------------638175687184405782--\r\n";

                                logger.LogInformation("Before Encryption :" + requestBody);
                                requestBody = IexamToken.bodyEncrypt(requestBody, emsBody.RandSymmKey);
                                logger.LogInformation("After Encryption :" + requestBody);

                                byte[] requestData = System.Text.Encoding.ASCII.GetBytes(requestBody);
                                requestStream.Write(requestData, 0, requestData.Length);
                            }
                            request.Headers.AllKeys.ToList().ForEach(header =>
                            {
                                logger.LogInformation("UploadFileEncAsync Headers  Key " + header + " value " + Convert.ToString(request.Headers[header]));
                            });
                            using (WebResponse response = request.GetResponse())
                            {
                                logger.LogInformation("Status Message ReportSync() : " + result.Message);
                                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                                {
                                    strResponseText = reader.ReadToEnd();
                                    result.Content = strResponseText;
                                    logger.LogInformation("Api response from iExams2 " + strResponseText);
                                }
                                logger.LogInformation("Completed ReportSync()");
                                if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                                {
                                    result.Status = SyncResponseStatus.Success;
                                    result.Message = "Data uploaded successfully.";

                                    logger.LogInformation("Data uploaded successfully. Status code :" + ((HttpWebResponse)response).StatusCode);

                                }
                                else
                                {
                                    result.Status = SyncResponseStatus.Error;
                                    result.Message = "Error while updating the data. Status code: " + (int)((HttpWebResponse)response).StatusCode + ". " + ((HttpWebResponse)response).StatusDescription;

                                    logger.LogError("Error while updating the data. Status code :" + ((HttpWebResponse)response).StatusCode);

                                    return StatusCode((int)((HttpWebResponse)response).StatusCode, ((HttpWebResponse)response).StatusDescription);
                                }
                            }
                        }
                        catch (WebException ex)
                        {
                            logger.LogError(message: ex.Message.ToString());
                            if (ex.Status == WebExceptionStatus.ProtocolError)
                            {
                                if (ex.Response is HttpWebResponse response && response.StatusCode != HttpStatusCode.OK)
                                {
                                    result.Status = SyncResponseStatus.Error;
                                    result.Message = "Error while updating the data. Status code: " + (int)(int?)response.StatusCode + ". " + response.StatusDescription;
                                }
                                else
                                {
                                    throw;
                                }
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                    else
                    {
                        result.Status = SyncResponseStatus.Error;
                        result.Message = "Invalid input parameter";
                    }
                }
                else
                {
                    result.Status = SyncResponseStatus.Error;
                    result.Message = "Invalid input parameter";
                }
            }
            catch (Exception ex)
            {
                result.Status = SyncResponseStatus.Error;
                result.Message = "Error while updating the data. Error : " + ex.Message;
                logger.LogError($"Error in ReportsController > ReportSync().", ex);
            }
            finally
            {
                logger.LogDebug($"ReportsController > ReportSync() completed.");
            }
            return Ok(result);
        }
        private string CreateJwt()
        {
            List<SeabJwtRole> jroles = new List<SeabJwtRole>();
            _appSettings.ClaimsAuthority.ForEach(a =>
            {
                jroles.Add(new SeabJwtRole
                {
                    authority = a
                });

            });
            string JwtToken = string.Empty;
            SeabJwtToken seabJwtToken = new SeabJwtToken
            {
                Exp = DateTime.UtcNow.AddMinutes(2),
                Iat = DateTime.UtcNow.AddMinutes(0),
                Role = jroles,
                ApiOrigin = _appSettings.ApiOrigin,
                SelectedUserRoleType = _appSettings.SelectedUserRoleType,
                AppId = _appSettings.APPId,
                AclData = { DATA_ORGID = new List<string> { _appSettings.SelectedOrgId } },
                SelectedOrgId = _appSettings.SelectedOrgId,
                UserId = _appSettings.UserId,
                Sub = _appSettings.userLoginId
            };

            JwtToken = iExamSecurity.JwtToken.Create(seabJwtToken, _appSettings.JWTSecret);

            return JwtToken;
        }
        private ClaimsApiRequest ConstructEmsRequest()
        {

            GetClainRequestBody requestBody = new GetClainRequestBody
            {
                roleProfile = "SEAB",
                currentWorkingOrganisation = "",
                userLoginId = _appSettings.userLoginId
            };
            ClaimsApiRequest emsBody = new ClaimsApiRequest
            {
                name = "apiClaims",
                request = new Request
                {
                    method = "POST",
                    header = new List<Header> {
                        new Header { key = "iEXAMS2_EG_app_id",
                            value = "EEXAMS2",
                            type = "text"
                        },
                        new Header {
                        key = "iEXAMS2_EG_APIKey_enc",
                        value = IexamToken.iexamsEncrypt(string.Concat(ApiKey, Nonce, TimeStamp)), type = "text"
                        },
                        new Header { key = "iEXAMS2_EG_nonce",
                        value = Nonce,
                        type = "text"
                        },
                        new Header { key = "iEXAMS2_EG_timestamp",
                            value = TimeStamp,
                            type = "text"
                        },
                        new Header { key = "iEXAMS2_EG_datakey_enc",
                            value = IexamToken.iexamsEncrypt(RandSymmKey), type = "text"
                        },
                        new Header { key = "iEXAMS2_EG_signature",
                            value =  IexamToken.eexamsSign(iEXAMS2_EG_app_id + Nonce + TimeStamp), type = "text"
                        },
                    },
                    body = new Body
                    {
                        mode = "raw",
                        raw = IexamToken.bodyEncrypt(JsonConvert.SerializeObject(requestBody), RandSymmKey),
                        options = new Options
                        {
                            raw = new Raw
                            {
                                language = "json"
                            }
                        }
                    },
                    url = new Url
                    {
                        raw = _appSettings.ClaimURL,
                        host = new List<string> { "dev", "iexams", "seab", "gov", "sg" },
                        path = new List<string> { "iexams2", "api", "portal", "user", "get-claims" }
                    }
                },
                RandSymmKey = RandSymmKey
            };
            return emsBody;
        }

        #region Get Claims
        private string CreateJwt(ClaimsApiRequest emsbody)
        {
            logger.LogInformation("CreateJwt Claims method started");
            string JwtToken = string.Empty;

            GetClaimResponse? GetClaimResponse = GetClaimsAsync(emsbody).Result;
            if (GetClaimResponse != null)
            {
                logger.LogInformation("CreateJwt Claims  method plain text " + GetClaimResponse.ClaimsResponse.PlainBodyText);

                IexamRoot? claimRoot = JsonConvert.DeserializeObject<IexamRoot>(GetClaimResponse.ClaimsResponse.PlainBodyText);

                if (claimRoot != null)
                {
                    logger.LogInformation("CreateJwt Claims  >> ClaimsToken value after serialize " + JsonConvert.SerializeObject(claimRoot));

                    List<SeabJwtRole> seabJwtRoles = new();

                    claimRoot.Claims.Role.ForEach(a =>
                    {
                        seabJwtRoles.Add(new SeabJwtRole
                        {
                            authority = a
                        });

                    });

                    JwtAclDataOrgId jwtAclDataOrgId = new()
                    {
                        DATA_ORGID = claimRoot.Claims.AclData.DataOrgId.Select(a => a.Id).ToList(),
                    };


                    SeabJwtToken claimsToken = new()
                    {
                        Exp = DateTime.UtcNow.AddMinutes(2),
                        Iat = DateTime.UtcNow.AddMinutes(0),
                        Role = seabJwtRoles,
                        ApiOrigin = _appSettings.ApiOrigin,
                        SelectedUserRoleType = _appSettings.SelectedUserRoleType,
                        AppId = _appSettings.APPId,
                        AclData = jwtAclDataOrgId,
                        SelectedOrgId = _appSettings.SelectedOrgId,
                        UserId = _appSettings.UserId,
                        Sub = _appSettings.userLoginId
                    };

                    logger.LogInformation("CreateJwt Claims  >> claimsToken " + JsonConvert.SerializeObject(claimsToken));

                    JwtToken = iExamSecurity.JwtToken.Create(claimsToken, _appSettings.JWTSecret);

                    logger.LogInformation("CreateJwt Claims  >> claimsToken " + JwtToken);
                }
                else
                {
                    logger.LogInformation("CreateJwt Claims >> GetClaimResponse Empty Claims Response after decrypt and serialize");
                }
            }
            else
            {
                logger.LogInformation("CreateJwt Claims >> Empty response from get claims method");
            }
            return JwtToken;
        }

        private async Task<GetClaimResponse?> GetClaimsAsync(ClaimsApiRequest emsBody)
        {
            logger.LogInformation("GetClaimsAsync method started");
            GetClaimResponse? getClaimResponse = new();
            try
            {
                string ApiUrl = emsBody.request.url.raw;

                string requestobj = emsBody.request.body.raw;

                HttpClientHandler handler = new()
                {
                    UseProxy = true,
                    Proxy = WebClientRequestHandler.GetProxy(_appSettings.ProxyUrl)
                };

                var client = new HttpClient(handler);
                var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl);
                emsBody.request.header.ForEach(header =>
                {
                    request.Headers.Add(header.key, header.value);
                });
                var content = new StringContent(requestobj, null, "application/json");
                request.Content = content;
                HttpResponseMessage response = client.Send(request);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    getClaimResponse.Body = await response.Content.ReadAsStringAsync();
                    GetClaimResponseHeader ClaimResponseHeader = new();
                    response.Headers.ToList().ForEach(hd =>
                    {
                        switch (hd.Key)
                        {
                            case "iEXAMS2_EG_APPID":
                                ClaimResponseHeader.APPID = hd.Value.First();
                                break;
                            case "iEXAMS2_EG_nonce":
                                ClaimResponseHeader.nonce = hd.Value.First();
                                break;
                            case "iEXAMS2_EG_timestamp":
                                ClaimResponseHeader.timestamp = hd.Value.First();
                                break;
                            case "iEXAMS2_EG_datakey_enc":
                                ClaimResponseHeader.datakey = hd.Value.First();
                                break;
                            case "iEXAMS2_EG_signature":
                                ClaimResponseHeader.signature = hd.Value.First();
                                break;
                            default:
                                break;
                        }
                    });

                    getClaimResponse.Header = ClaimResponseHeader;
                    getClaimResponse.ClaimsResponse = GetClaimDecryptedResponse(getClaimResponse);
                }
                logger.LogInformation("GetClaimsAsync method completed " + JsonConvert.SerializeObject(getClaimResponse));

            }
            catch (Exception ex)
            {
                getClaimResponse = null;
                logger.LogInformation("Error in GetClaimsAsync method ", ex);
            }
            return getClaimResponse;
        }

        private ClaimsDecryptedResponse GetClaimDecryptedResponse(GetClaimResponse getClaimResponse)
        {
            ClaimsDecryptedResponse ObjClaimsDecryptedResponse = new();
            try
            {
                ObjClaimsDecryptedResponse.IsSignVerified = true;
                ObjClaimsDecryptedResponse.DataKeyPlain = IexamToken.eexamsDecrypt(getClaimResponse.Header.datakey);
                ObjClaimsDecryptedResponse.PlainBodyText = IexamToken.bodyDecrypt(getClaimResponse.Body, ObjClaimsDecryptedResponse.DataKeyPlain);
            }
            catch (Exception ex)
            {
                ObjClaimsDecryptedResponse.ExceptionMessage = ex.Message;
            }
            return ObjClaimsDecryptedResponse;
        }

        #endregion
    }
}
