using Saras.eMarking.Outbound.Services.Interfaces.BusinessInterface;
using Saras.eMarking.Outbound.Services.Model;
using Microsoft.Extensions.Logging;
using iExamSecurity.Util;
using iExamSecurity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Options = iExamSecurity.Util.Options;
using System.Text;
using static com.sun.tools.@internal.xjc.reader.xmlschema.bindinfo.BIConversion;
using Newtonsoft.Json.Linq;
using System.Net;
using static com.sun.tools.javac.tree.JCTree;
using System.Net.Cache;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
using sun.swing;
using java.nio.file.attribute;

namespace Saras.eMarking.Outbound.Services.Services
{
    public class SyncReportService : ISyncReportService
    {
        private readonly ILogger logger;

        private readonly AppSettings _appSettings;
        readonly string iEXAMS2_EG_app_id;
        readonly string Nonce = string.Empty;
        readonly string TimeStamp = string.Empty;
        readonly string ApiKey;
        readonly string RandSymmKey = string.Empty;

        public SyncReportService(ILogger<SyncReportService> _logger, IOptionsMonitor<AppSettings> appSettings)
        {
            logger = _logger;
            _appSettings = appSettings.CurrentValue;


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

        public SyncResponseModel SyncReportToiExam(SyncReportModel syncReportModel)
        {
            SyncResponseModel syncResponseModel = new SyncResponseModel();

            logger.LogDebug("Service layer SyncReportToiExam started");
            syncResponseModel = UploadFile(syncReportModel);

            return syncResponseModel;
        }
        public SyncResponseModel SyncEMSReportToiExam(SyncReportModel syncReportModel)
        {
            SyncResponseModel syncResponseModel = new SyncResponseModel();

            logger.LogDebug("Service layer SyncReportToiExam started");
            syncResponseModel = UploadEMSFile(syncReportModel);

            return syncResponseModel;
        }
        #region Get Claims
        private async Task<string> GetiExamClaims(ClaimsApiRequest emsBody)
        {
            logger.LogDebug("Service layer GetiExamClaims started");
            string content = string.Empty;
            try
            {
                Dictionary<string, string> requestHeader = new Dictionary<string, string>();
                emsBody.request.header.ForEach(header => requestHeader.Add(header.key, header.value));

                logger.LogDebug("Service layer GetiExamClaims started to post api");

                HttpResponseMessage getClaimsResponse = await HttpClientRequestHandler.PostAsync(emsBody.request.url.raw, emsBody.request.body.raw, requestHeader, _appSettings.ProxyUrl);
                logger.LogDebug("getClaimsResponse HttpResponseMessage : " + JsonConvert.SerializeObject(getClaimsResponse));
                if (getClaimsResponse.IsSuccessStatusCode)
                {
                    content = getClaimsResponse.Content.ReadAsStringAsync().Result;
                    logger.LogDebug("getClaimsResponse plain Content : " + content);
                    GetClaimResponseHeader ClaimResponseHeader = new();
                    getClaimsResponse.Headers.ToList().ForEach(hd =>
                    {
                        logger.LogDebug("getClaimsResponse Headers : " + JsonConvert.SerializeObject(hd));
                        switch (hd.Key)
                        {
                            case "iEXAMS2_EG_APPID":
                                ClaimResponseHeader.APPID = hd.Value.FirstOrDefault();
                                logger.LogDebug("getClaimsResponse ClaimResponseHeader.APPID : " + ClaimResponseHeader.APPID);
                                break;
                            case "iEXAMS2_EG_nonce":
                                ClaimResponseHeader.nonce = hd.Value.FirstOrDefault();
                                logger.LogDebug("getClaimsResponse ClaimResponseHeader.nonce : " + ClaimResponseHeader.nonce);
                                break;
                            case "iEXAMS2_EG_timestamp":
                                ClaimResponseHeader.timestamp = hd.Value.FirstOrDefault();
                                logger.LogDebug("getClaimsResponse ClaimResponseHeader.timestamp : " + ClaimResponseHeader.timestamp);
                                break;
                            case "iEXAMS2_EG_datakey_enc":
                                ClaimResponseHeader.datakey = hd.Value.FirstOrDefault();
                                logger.LogDebug("getClaimsResponse ClaimResponseHeader.datakey : " + ClaimResponseHeader.datakey);
                                break;
                            case "iEXAMS2_EG_signature":
                                ClaimResponseHeader.signature = hd.Value.FirstOrDefault();
                                logger.LogDebug("getClaimsResponse ClaimResponseHeader.signature : " + ClaimResponseHeader.signature);
                                break;
                            default:
                                break;

                        }
                    });

                    logger.LogDebug("getClaimsResponse GetClaimDecryptedResponse started : " + content);
                    content = GetClaimDecryptedResponse(content, ClaimResponseHeader);
                    logger.LogDebug("getClaimsResponse GetClaimDecryptedResponse completed : " + content);
                }

                logger.LogDebug("Service layer GetiExamClaims completed");
            }
            catch (Exception ex)
            {
                logger.LogError("Error in Service layer GetiExamClaims", ex);
                logger.LogError("Error in Service layer GetiExamClaims", ex.Message);
            }
            return content;
        }

        private string GetClaimDecryptedResponse(string content, GetClaimResponseHeader getClaimResponse)
        {
            string decrypted = string.Empty;
            try
            {
                logger.LogDebug("Service layer GetClaimDecryptedResponse started");
                string key = IexamToken.eexamsDecrypt(getClaimResponse.datakey);
                logger.LogDebug("Service layer GetClaimDecryptedResponse started Key : " + key);
                decrypted = IexamToken.bodyDecrypt(content, key);
                logger.LogDebug("Service layer GetClaimDecryptedResponse completed decrypted: " + decrypted);
            }
            catch (Exception ex)
            {
                logger.LogError("Error in Service layer GetClaimDecryptedResponse Method", ex);
                logger.LogError("Error in Service layer GetClaimDecryptedResponse Method", ex.Message);
            }
            return decrypted;
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

        #endregion

        #region Create Jwt

        private string CreateJwt(string claims)
        {
            SeabJwtToken? seabJwtToken = JsonConvert.DeserializeObject<SeabJwtToken>(claims);
            string jwttoken = string.Empty;
            if (seabJwtToken != null)
            {
                seabJwtToken.Exp = DateTime.UtcNow.AddMinutes(10);
                seabJwtToken.Iat = DateTime.UtcNow.AddMinutes(0);
                seabJwtToken.SelectedUserRoleType = "SEAB";
                seabJwtToken.AppId = _appSettings.APPId;
                seabJwtToken.UserId = _appSettings.UserId;
                seabJwtToken.Sub = _appSettings.userLoginId;

                jwttoken = JwtToken.Create(seabJwtToken, _appSettings.JWTSecret);
            }

            return jwttoken;
        }

        #endregion 

        #region Upload File

        private SyncResponseModel UploadFile(SyncReportModel syncReportModel)
        {
            SyncResponseModel syncResponseModel = new SyncResponseModel();
            logger.LogInformation("UploadFile method started.");
            try
            {
                string UploadFileApiUrl = _appSettings.UploadFileApiUrl;

                switch (syncReportModel.ReportType)
                {
                    case 1:
                        UploadFileApiUrl = UploadFileApiUrl.Replace("{type}", "EMS1");
                        UploadFileApiUrl = UploadFileApiUrl.Replace("{year}", Convert.ToString(syncReportModel.Year));
                        break;
                    case 2:
                        UploadFileApiUrl = UploadFileApiUrl.Replace("{type}", "EMS2");
                        UploadFileApiUrl = UploadFileApiUrl.Replace("{year}", Convert.ToString(syncReportModel.Year));
                        break;
                    case 3:
                        UploadFileApiUrl = UploadFileApiUrl.Replace("{type}", "OMS");
                        UploadFileApiUrl = UploadFileApiUrl.Replace("{year}", Convert.ToString(syncReportModel.Year));
                        break;
                }

                string readfilepath = syncReportModel.RootFolder + @"\\" + syncReportModel.FileId + "~" + syncReportModel.FileName;

                logger.LogInformation("File name : " + readfilepath);

                UploadFileApiUrl = UploadFileApiUrl.Replace("{correlationId}", Convert.ToString(System.Guid.NewGuid()));

                //string bodydata = File.ReadAllText(readfilepath);
                //string bodydata = string.Empty;
                //foreach (string line in System.IO.File.ReadLines(readfilepath))
                //{
                //    bodydata += line + "\r\n";
                //}

                RequestSampleFinal(UploadFileApiUrl, readfilepath, syncReportModel.FileName);

                logger.LogInformation("UploadFile method completed ");
            }
            catch (Exception ex)
            {
                syncResponseModel.Status = SyncResponseStatus.Error;
                syncResponseModel.Message = "Error in UploadFile method. Error : " + ex.Message;
                logger.LogError("Error in UploadFile method. Error :" + ex);
            }
            return syncResponseModel;
        }

        private SyncResponseModel UploadEMSFile(SyncReportModel syncReportModel)
        {
            SyncResponseModel syncResponseModel = new SyncResponseModel();
            logger.LogInformation("UploadFile method started.");
            try
            {
                string UploadFileApiUrl = _appSettings.UploadFileApiUrl;

                switch (syncReportModel.ReportType)
                {
                    case 1:
                        UploadFileApiUrl = UploadFileApiUrl.Replace("{type}", "EMS1");
                        UploadFileApiUrl = UploadFileApiUrl.Replace("{year}", Convert.ToString(syncReportModel.Year));
                        break;
                    case 2:
                        UploadFileApiUrl = UploadFileApiUrl.Replace("{type}", "EMS2");
                        UploadFileApiUrl = UploadFileApiUrl.Replace("{year}", Convert.ToString(syncReportModel.Year));
                        break;
                    case 3:
                        UploadFileApiUrl = UploadFileApiUrl.Replace("{type}", "OMS");
                        UploadFileApiUrl = UploadFileApiUrl.Replace("{year}", Convert.ToString(syncReportModel.Year));
                        break;
                }

                string readfilepath = syncReportModel.RootFolder + @"\\" + syncReportModel.FileName;

                logger.LogInformation("File name : " + readfilepath);

                UploadFileApiUrl = UploadFileApiUrl.Replace("{correlationId}", Convert.ToString(System.Guid.NewGuid()));

                //string bodydata = File.ReadAllText(readfilepath);
                //string bodydata = string.Empty;
                //foreach (string line in System.IO.File.ReadLines(readfilepath))
                //{
                //    bodydata += line + "\r\n";
                //}

                RequestSampleFinal1(UploadFileApiUrl, readfilepath, syncReportModel.FileName);

                logger.LogInformation("UploadFile method completed ");
            }
            catch (Exception ex)
            {
                syncResponseModel.Status = SyncResponseStatus.Error;
                syncResponseModel.Message = "Error in UploadFile method. Error : " + ex.Message;
                logger.LogError("Error in UploadFile method. Error :" + ex);
            }
            return syncResponseModel;
        }

        private void RequestSampleFinal(string PostUrl, string reportfilepath, string FileName)
        {
            try
            {

                var emsBody = ConstructEmsRequest();


                string jwttoken = string.Empty;
                if (_appSettings.IsFileEncRequired)
                {
                    //string claims = GetiExamClaims(emsBody).Result;

                    //jwttoken = CreateJwt(claims);
                    jwttoken = CreateJwt();
                }
                else
                {
                    jwttoken = CreateJwt();
                }


                string strResponseText = string.Empty;
                logger.LogInformation("Started RequestSampleFinal");
                string url = PostUrl;
                //string boundary = "------------------------------" + DateTime.UtcNow.Ticks; //638175687184405782";
                string boundary = "------------------------------638175687184405782";

                logger.LogInformation("URL Info" + url);
                logger.LogInformation("multipart/form-data; boundary=" + boundary);

                WebProxy proxy = new WebProxy(_appSettings.ProxyUrl);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = proxy;
                request.Method = "POST";
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.Headers["iEXAMS2_EG_app_id"] = "EEXAMS2";

                if (emsBody.request != null && emsBody.request.body != null && emsBody.request.header.Count > 0)
                {
                    var headerVal = emsBody.request.header.FirstOrDefault(a => a.key == "iEXAMS2_EG_APIKey_enc");
                    request.Headers["iEXAMS2_EG_APIKey_enc"] = headerVal.value;
                    headerVal = emsBody.request.header.FirstOrDefault(a => a.key == "iEXAMS2_EG_nonce");
                    request.Headers["iEXAMS2_EG_nonce"] = headerVal.value;
                    headerVal = emsBody.request.header.FirstOrDefault(a => a.key == "iEXAMS2_EG_timestamp");
                    request.Headers["iEXAMS2_EG_timestamp"] = headerVal.value;
                    headerVal = emsBody.request.header.FirstOrDefault(a => a.key == "iEXAMS2_EG_datakey_enc");
                    request.Headers["iEXAMS2_EG_datakey_enc"] = headerVal.value;
                    headerVal = emsBody.request.header.FirstOrDefault(a => a.key == "iEXAMS2_EG_signature");
                    request.Headers["iEXAMS2_EG_signature"] = headerVal.value;
                }

                request.Headers["iEXAMS2_EG_token"] = CreateJwt();

                request.Timeout = 120000;

                using (Stream requestStream = request.GetRequestStream())
                {
                    string requestBody = "------------------------------638175687184405782\r\n"
                      + "Content-Disposition: form-data; name=\"file\"; filename=\"" + FileName + "\"\r\n"
                      + "Content-Type: text/plain\r\n"
                      + "\r\n";

                    foreach (string line in System.IO.File.ReadLines(reportfilepath))
                    {
                        requestBody += line + "\r\n";
                    }
                    requestBody += "------------------------------638175687184405782--\r\n";

                    logger.LogInformation("IsEncryption Required : " + _appSettings.IsFileEncRequired);

                    logger.LogInformation("Before Encryption : " + requestBody);
                    if (_appSettings.IsFileEncRequired)
                    {
                        requestBody = IexamToken.bodyEncrypt(requestBody, emsBody.RandSymmKey);
                    }
                    logger.LogInformation("After Encryption : " + requestBody);

                    byte[] requestData = System.Text.Encoding.ASCII.GetBytes(requestBody);
                    requestStream.Write(requestData, 0, requestData.Length);
                }
                request.Headers.AllKeys.ToList().ForEach(header =>
                {
                    logger.LogInformation("UploadFileEncAsync Headers  Key " + header + " value " + Convert.ToString(request.Headers[header]));
                });
                using (WebResponse response = request.GetResponse())
                {
                    logger.LogInformation("Status code : " + Convert.ToString(((HttpWebResponse)response).StatusCode));
                    logger.LogInformation("Status Description : " + Convert.ToString(((HttpWebResponse)response).StatusDescription));
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        strResponseText = reader.ReadToEnd();
                        logger.LogInformation(strResponseText);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex.Message.ToString());
            }
            logger.LogInformation("Completed RequestSampleFinal");
        }


        public void RequestSampleFinal1(string PostUrl, string reportfilepath, string FileName)
        {
            try
            {
                //string reportfilepath = string.Concat(_appSettings.ReportFolderRoot, "\\ReportsFile\\EMS1\\XE2_EMS1_A_9572_01_WRITTN_YE_1_1_02122020_100002.txt");

                var emsBody = ConstructEmsRequest();
                //Item emsBody = ConstructEmsRequest();

                string strResponseText = string.Empty;
                logger.LogInformation("Started RequestSampleFinal1");
                string url = PostUrl;//"https://uat.iexams.seab.gov.sg/iexams2/eexams/scheduler/eexams2/fileupload/2019/EMS1/" + Convert.ToString(System.Guid.NewGuid());
                string boundary = "----------------------------638175687184405782";
                logger.LogInformation("URL: " + url);
                logger.LogInformation("boundary: " + boundary);
                logger.LogInformation("multipart/form-data; boundary=" + boundary);

                WebProxy proxy = new WebProxy("http://100.114.159.157:3128");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = proxy;
                request.Method = "POST";
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.Headers["iEXAMS2_EG_app_id"] = "EEXAMS2";
                if (!_appSettings.IsFileEncRequired)
                {
                    request.Headers["iEXAMS2_EG_APIKey_enc"] = "pYXzPOkCVKDpiVd9Lutgk2VBL/VmO/vQ6+g2fSh8heExJd8bQtYhcDVpa8u0E7hTYX6CAlIUBEpq7H4xupvjadqoeN2TmOB+tj4y/HLVnsh5an1sAe55G4i01RQOcw+E8lhITMEBuZyerUg2+wAsfSOFkNxVgKh5OSPXpDoIxPLM1XDgETTj80tvVLs5v4QplDpi0rw/hCg0CT67jOK8HMzogeQUr1bc0WTm/uznetZFgSpJ+vF79NraiM7TSLdl0RfgtX5emkWlUAfbU6HCnBcy9m9vQCvNP0NIKphRGdV1ueQ8geUJjV9viD7AEJOoCzZz2aoP1cKofc0zmDE+dQ==";
                    request.Headers["iEXAMS2_EG_nonce"] = "-5470052299808636001";
                    request.Headers["iEXAMS2_EG_timestamp"] = "1681971916320";
                    request.Headers["iEXAMS2_EG_datakey_enc"] = "L/ZZWehcZu0TebO6sGOF67ARBLWz71qltoDWpTq8l4+LClku/q3/qqJRPPD1QbHGh3idZ9YZO/4/YgRd3Xhdb0RBl0+9Ar03jabo4cvdPGhQ8zEhIELq+4ae0aK0hUSrjEJzASKHObmvl9juoNq5KPztf+z3QJyG76E41KrP2SuZQFy59oBuS3AO4vx4OQZvfGBScJiDxj7y0YIjtaSUI0NJS6fy/smMtTc5RrI6tYGnoeLsKnOZqIDDRd6TtZx+nS7HVsso9LEL4pG6gvGXlPOhCxiO7lRH+Dh+F5va4ozwKwe0bFHnP9Hvjo5hnQoTvpFUR1APNPgyakVitTK9Kw==";
                    request.Headers["iEXAMS2_EG_signature"] = "QAXFKI2JhijriJkf3IlSiy+nt4d3XwHJySfIwXKsqszL0nSapavgQvq7DHvgIbfKOSdoFi4J7870baJ5Mu820qNpfnhKr0lbD/AzwdXbyKnoZvD1GtBoU5/Vu/jXbWZZf/6jGL/lV3LtL+5D9jfOF4mQfa1h2LLrCJn/gjvYVSXt9HZHgFrziBiy3pkkzH2NTFPOenMu38tj0zYUZn07HzwPml43AwWlJI9fnG4IEF5Gdj19jfAl8Ox6epod6XsAtPmIGixyCqe5owAYZrMX0M38bKU5SKXSh4L2tXvttIjemiY/pM3+jRoBsrV3vHpjIk5n6oFpTtJ+yKTcYRajbA==";
                }
                else
                {
                    if (emsBody.request != null && emsBody.request.body != null && emsBody.request.header.Count > 0)
                    {
                        var headerVal = emsBody.request.header.FirstOrDefault(a => a.key == "iEXAMS2_EG_APIKey_enc");
                        request.Headers["iEXAMS2_EG_APIKey_enc"] = headerVal.value;
                        headerVal = emsBody.request.header.FirstOrDefault(a => a.key == "iEXAMS2_EG_nonce");
                        request.Headers["iEXAMS2_EG_nonce"] = headerVal.value;
                        headerVal = emsBody.request.header.FirstOrDefault(a => a.key == "iEXAMS2_EG_timestamp");
                        request.Headers["iEXAMS2_EG_timestamp"] = headerVal.value;
                        headerVal = emsBody.request.header.FirstOrDefault(a => a.key == "iEXAMS2_EG_datakey_enc");
                        request.Headers["iEXAMS2_EG_datakey_enc"] = headerVal.value;
                        headerVal = emsBody.request.header.FirstOrDefault(a => a.key == "iEXAMS2_EG_signature");
                        request.Headers["iEXAMS2_EG_signature"] = headerVal.value;
                    }
                }
                request.Headers["iEXAMS2_EG_token"] = CreateJwt();

                request.Timeout = 120000;

                using (Stream requestStream = request.GetRequestStream())
                {
                    string requestBody = "------------------------------638175687184405782\r\n"
                        + "Content-Disposition: form-data; name=\"file\"; filename=\"" + FileName + "\"\r\n"
                        + "Content-Type: text/plain\r\n"
                        + "\r\n";

                    foreach (string line in System.IO.File.ReadLines(reportfilepath))
                    {
                        requestBody += line + "\r\n";
                    }

                    //+ System.IO.File.ReadAllText(reportfilepath)
                    //+ "00E-EXAMS             EMS1_TXT            0212202010:16:06\r\n"
                    //+ "2019GCEA  YE    957201ENGLSHWRITTN                                       30500610  1  00610.003.0013.0 X\r\n"
                    //+ "9900000003\r\n"
                    requestBody += "------------------------------638175687184405782--\r\n";

                    logger.LogInformation("IsEncryption Required : " + _appSettings.IsFileEncRequired);

                    logger.LogInformation("Before Encryption :" + requestBody);
                    if (_appSettings.IsFileEncRequired)
                    {
                        requestBody = IexamToken.bodyEncrypt(requestBody, emsBody.RandSymmKey);
                    }
                    logger.LogInformation(requestBody);

                    byte[] requestData = System.Text.Encoding.ASCII.GetBytes(requestBody);
                    requestStream.Write(requestData, 0, requestData.Length);
                }
                request.Headers.AllKeys.ToList().ForEach(header =>
                {
                    logger.LogInformation("UploadFileEncAsync Headers  Key " + header + " value " + Convert.ToString(request.Headers[header]));
                });
                using (WebResponse response = request.GetResponse())
                {
                    logger.LogInformation(Convert.ToString(((HttpWebResponse)response).StatusCode));
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        strResponseText = reader.ReadToEnd();
                        logger.LogInformation(strResponseText);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex.Message.ToString());
            }
            logger.LogInformation("Completed RequestSampleFinal");
        }
        private string CreateJwt()
        {
            string JwtToken = string.Empty;
            SeabJwtToken seabJwtToken = new SeabJwtToken
            {
                Exp = DateTime.UtcNow.AddMinutes(2),
                Iat = DateTime.UtcNow.AddMinutes(0),
                Role = new List<SeabJwtRole> {
                    new SeabJwtRole { authority= "eexams2UploadFile" },
                },
                ApiOrigin = "internet",
                SelectedUserRoleType = "SEAB",
                AppId = "EEXAMS2",
                //AclData = { DATA_ORGID = new List<string> { "3" } },
                SelectedOrgId = "3",
                UserId = 1202196,
                Sub = _appSettings.userLoginId
            };

            JwtToken = iExamSecurity.JwtToken.Create(seabJwtToken, _appSettings.JWTSecret);

            return JwtToken;
        }

        #endregion


    }
}
