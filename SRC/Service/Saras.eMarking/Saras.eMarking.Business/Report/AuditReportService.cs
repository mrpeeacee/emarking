using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Saras.eMarking.Domain;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Report;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using Saras.eMarking.Domain.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Saras.eMarking.Business.Report
{
    public class AuditReportService : BaseService<AuditReportService>, IAuditReportService  
    {
        private readonly IAuditReportRepository auditReportRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditReportService"/> class.
        /// </summary>
        /// <param name="_logger">The _logger.</param>
        /// <param name="_auditReportRepository">The _audit report repository.</param>
        /// <param name="_appOptions">The _app options.</param>
        public AuditReportService(ILogger<AuditReportService> _logger, IAuditReportRepository _auditReportRepository, AppOptions _appOptions = null) : base(_logger, _appOptions)
        {
            auditReportRepository = _auditReportRepository;
        }
        /// <summary>
        /// Get all application module names
        /// </summary>
        /// <param name="localizedStrings"></param>
        /// <returns></returns>
        public async Task<List<ApplicationModuleModel>> GetAppModules(List<LocalizedString> localizedStrings)
        {
            logger.LogDebug("AuditReportService GetAppModules() method started.");

            //Get all application module names
            List<ApplicationModuleModel> moduleData = await auditReportRepository.GetAppModules();

            if (moduleData != null && moduleData.Count > 0)
            {
                //Check the Module code from the Localized resource file and get the the text from it
                moduleData.ForEach(mItem =>
                {
                    LocalizedString searchedItem = localizedStrings.Find(a => a.Name == mItem.ModuleCode);
                    if (searchedItem != null)
                    {
                        mItem.ModuleName = searchedItem.Value;
                    }
                });

                //Order by module name to display data in the list in one order
                moduleData = moduleData.OrderBy(a => a.ModuleName).ToList();
            }

            logger.LogDebug("AuditReportService GetAppModules() method completed.");

            return moduleData;
        }

        /// <summary>
        /// This method is to get the Audit details for given filters and parse it to given user defined template text
        /// {LoginName} single property.
        /// {LoginName} {MarkSchemeName}. ['Bands' {BandName} {BandFrom} {BandTo}] Array of bands display BandName,BandFrom, BandTo with 'Bands' class
        /// [{BandName} {BandFrom} {BandTo}] Array of data witout class
        /// </summary>
        /// <param name="objaudit"></param>
        /// <param name="projectuserroleID"></param>
        /// <param name="LoginId"></param>
        /// <param name="TimeZone"></param>
        /// <returns></returns>
        public async Task<List<AuditReportDataModel>> GetAuditReport(AuditReportRequestModel objaudit, UserTimeZone TimeZone, List<LocalizedString> localizedStrings)
        {
            try
            {
                logger.LogDebug("AuditReportService GetAuditReport() method started. ");

                List<AuditReportDataModel> result = new();

                //Get Audit data from data base for given criteria.
                List<AuditReportModel> reportdata = await auditReportRepository.GetAuditReport(objaudit);

                if (reportdata != null && reportdata.Count > 0)
                {
                    //Audit data Order by audit date to list it in order by date time.
                    ////reportdata = reportdata.OrderBy(a => a.EventDateTime).ToList();

                    //Group audit data by session id to group single session data and add function performed in the single session.
                   
                    foreach (var grpProject in reportdata.GroupBy(f => f.SessionId))
                    {
                        result.Add(BuildAuditData(grpProject, TimeZone, localizedStrings));
                    }
                }

                if (result != null)
                {
                    //Group session on a user in a order and give SlNo for each group.
                    foreach (var group in result.OrderBy(f => f.LogInDateTime).GroupBy(f => f.UserName))
                    {
                        int serialNo = 1;
                        foreach (var item in group)
                        {
                            item.SlNo = serialNo;
                            serialNo++;
                        }
                    }
                }

                logger.LogDebug("AuditReportService GetAuditReport() method completed.");

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AuditReportService > GetAuditReport().");
                throw;
            }
        }

        private static AuditReportDataModel BuildAuditData(IGrouping<string, AuditReportModel> grpProject, UserTimeZone timeZone, List<LocalizedString> localizedStrings)
        {
            var firstRow = grpProject.FirstOrDefault();

            var auditdata = new AuditReportDataModel
            {
                UserName = firstRow?.LoginId,
                IPAddress = firstRow?.IPAddress,
                LogInDateTime = (grpProject.FirstOrDefault(a =>  a.Status == 1||a.Status==4 || a.Status == 8 || a.Status == 2 || a.Status == 23 )?.EventDateTime).UtcToProfileDateTime(timeZone),
              ////  LogInDateTime = (grpProject.FirstOrDefault(a => a.EventCode == "ENABLE" && a.Status == 1)?.EventDateTime).UtcToProfileDateTime(timeZone),
               ////LogOutDateTime = grpProject.FirstOrDefault(a =>  a.Status == 1)?.EventDateTime.UtcToProfileDateTime(timeZone),
                LogOutDateTime = grpProject.FirstOrDefault(a => a.EventCode == "LOGOUT" && a.Status == 1)?.EventDateTime.UtcToProfileDateTime(timeZone),
                TotalRows = firstRow?.TotalRows
            };
            if (auditdata.LogOutDateTime != null && auditdata.LogInDateTime != null)
            {
                //auditdata.Duration = auditdata.LogOutDateTime.Value.Subtract(auditdata.LogInDateTime.Value).ToString(@"hh\:mm\:ss");
                TimeSpan timeDifference = auditdata.LogOutDateTime.Value.Subtract(auditdata.LogInDateTime.Value);

                // The ToString method with @"hh\:mm\:ss" format will display the time difference in hours, minutes, and seconds
                auditdata.Duration = timeDifference.ToString(@"hh\:mm\:ss");
            }
            else
            {
                auditdata.Duration = null;
            }

            auditdata.FunctionPerformed = new List<string>();

            foreach (var projectModule in grpProject)
            {
                //Genereate function performed for single session and give proper text message.
                string functionPerformed = GenerateFunctionPerformed(projectModule, localizedStrings, timeZone);
                if (!string.IsNullOrEmpty(functionPerformed))
                {
                    auditdata.FunctionPerformed.Add(functionPerformed);
                }
            }
            return auditdata;
        }

        /// <summary>
        /// Get the action performed text
        /// </summary>
        /// <param name="projectModule"></param>
        /// <param name="localizedStrings"></param>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        private static string GenerateFunctionPerformed(AuditReportModel projectModule, List<LocalizedString> localizedStrings, UserTimeZone timeZone)
        {
            //Get the template text from the Localization resource file by Audit report model.
            string templateText = GetTemplateText(projectModule, localizedStrings);

            string parsedData = string.Empty;
            if (!string.IsNullOrEmpty(templateText))
            {
                parsedData = ParseAuditTemplate(projectModule, templateText, timeZone);
            }
            return parsedData;
        }

        /// <summary>
        /// Parse audit template from Remark data in audit table
        /// </summary>
        /// <param name="remarks"></param>
        /// <param name="templateText"></param>
        /// <returns></returns>
        private static string ParseAuditTemplate(AuditReportModel projectModule, string templateText, UserTimeZone timeZone)
        {
            JToken jsonObject = JToken.Parse(projectModule.Remarks);
            Dictionary<string, string> stringValues = new();
            List<JObject> arrayValues = new();

            //Convert audit report data to JObject and Separate JArray and Jsting to separate list.
            SeparateValues(jsonObject, stringValues, arrayValues);

            //Get template keys from the template text and group it.
            List<AuditTemplateModel> templateKeys = GetAuditTemplateKeys(templateText);

            templateKeys.ForEach(tempKey =>
            {
                switch (tempKey.TemplateType)
                {
                    case 3:
                        {
                            JArray jObjArray = (JArray)jsonObject[tempKey.JObjectName];

                            jObjArray.ForEach(jo =>
                            {
                                arrayValues.Add((JObject)jo);
                            });
                            var arryText = FindAndReplaceTemplatesArray(arrayValues, tempKey);
                            templateText = templateText.Replace(tempKey.Key, arryText).Replace("'" + tempKey.JObjectName + "'", string.Empty);
                            break;
                        }

                    case 2:
                        {
                            var arryText = FindAndReplaceTemplatesArray(arrayValues, tempKey);
                            templateText = templateText.Replace(tempKey.Key, arryText);
                            break;
                        }

                    default:
                        //Replace the the audit template common templates like LoginName, ProjectName and Audit Date time which is outside remarks json.
                        if (tempKey.Key == "LoginName" || tempKey.Key == "ProjectName" || tempKey.Key == "EventDateTime")
                        {
                            if (tempKey.Key == "LoginName" && projectModule.Status==8)
                            {
                                JObject jsonObject = JObject.Parse(projectModule.Remarks);

                               
                                string loginName = (string)jsonObject["Loginname"];

                                templateText = templateText.Replace("{LoginName}", loginName)
                                                          .Replace("{ProjectName}", projectModule.ProjectName)
                                                          .Replace("Username", projectModule.FirstName)
                                                          .Replace("{EventDateTime}", Convert.ToString(projectModule.EventDateTime.UtcToProfileDateTime(timeZone)));
                            }
                            else
                            {
                                templateText = templateText.Replace("{LoginName}", projectModule.LoginId)
                                                           .Replace("{ProjectName}", projectModule.ProjectName)
                                                           .Replace("Username", projectModule.FirstName)
                                                           .Replace("{EventDateTime}", Convert.ToString(projectModule.EventDateTime.UtcToProfileDateTime(timeZone)));
                            }
                        }
                        templateText = ReplaceTemplateKeys(templateText, stringValues);
                        break;
                }
            });

            templateText = templateText.Replace("[", "").Replace("]", "");

            return templateText;
        }

        /// <summary>
        /// Get template keys from the template text and group it
        /// </summary>
        /// <param name="templateText"></param>
        /// <returns></returns>
        private static List<AuditTemplateModel> GetAuditTemplateKeys(string templateText)
        {
            List<AuditTemplateModel> keys = new();
            // Define a regular expression pattern to match the template strings
            string pattern1 = @"\{([^{}]+)\}"; //// Pattern to match {UserName}
            string pattern2 = @"\[([^[\]]+)\]"; //// Pattern to match [LoginName]
            Regex regex1 = new(pattern1);
            Regex regex2 = new(pattern2);

            MatchCollection matches2 = regex2.Matches(templateText);
            foreach ((Match match, string jObjName) in from Match match in matches2
                                                       let groupContent = match.Groups[1].Value
                                                       let jObjName = CheckJObjName(groupContent)
                                                       select (match, jObjName))
            {
                if (string.IsNullOrEmpty(jObjName))
                {
                    keys.Add(new AuditTemplateModel
                    {
                        Key = match.Groups[1].Value,
                        TemplateType = 2
                    });
                }
                else
                {
                    keys.Add(new AuditTemplateModel
                    {
                        Key = match.Groups[1].Value,
                        TemplateType = 3,
                        JObjectName = jObjName
                    });
                }

                templateText = templateText.Replace(match.Groups[1].Value, string.Empty);
            }

            MatchCollection matches1 = regex1.Matches(templateText);

            keys.AddRange(from Match match in matches1.Cast<Match>()
                          select new AuditTemplateModel
                          {
                              Key = match.Groups[1].Value,
                              TemplateType = 1
                          });
            return keys;
        }

        /// <summary>
        /// Get the JObject array name given in the template text like 'Bands'
        /// </summary>
        /// <param name="groupContent"></param>
        /// <returns></returns>
        private static string CheckJObjName(string groupContent)
        {
            string pattern = "'([^']+)'"; //Pattern Matching for 'Bands'

            Match match = Regex.Match(groupContent, pattern);

            return match.Success && match.Groups.Count > 1 ? match.Groups[1].Value : null;
        }

        /// <summary>
        /// Get the teplate text from resource file for given audit model
        /// </summary>
        /// <param name="projectModule"></param>
        /// <param name="localizedStrings"></param>
        /// <returns></returns>
        private static string GetTemplateText(AuditReportModel projectModule, List<LocalizedString> localizedStrings)
        {
            string templateText = string.Empty;
            string keyToSearch = $"{projectModule.ModuleCode.Replace(" ", "").ToUpper()}.{projectModule.EventCode.ToUpper()}.{projectModule.Status}";

            //Find template text from localization resource file. Template name will be formatted as "ModuleCode.EventCode.Status"
            LocalizedString searchedItem = localizedStrings.Find(a => a.Name == keyToSearch);
            if (searchedItem != null)
            {
                templateText = searchedItem.Value;
            }
            return templateText;
        }

        /// <summary>
        /// Replace template array in the template text with the actual value
        /// </summary>
        /// <param name="jsonData"></param>
        /// <param name="templateKeys"></param>
        /// <returns></returns>
        private static string FindAndReplaceTemplatesArray(List<JObject> jsonData, AuditTemplateModel templateKeys)
        {
            templateKeys.Key = templateKeys.Key.Replace("[", "").Replace("]", "");

            StringBuilder parsedText = new();

            foreach (JObject json in jsonData)
            {
                Dictionary<string, string> stringValues = new();
                List<JObject> arrayValues = new();

                SeparateValues(json, stringValues, arrayValues);

                parsedText.Append(ReplaceTemplateKeys(templateKeys.Key, stringValues)).Append(',');
            }

            return parsedText.ToString();
        }

        /// <summary>
        /// Replace template text with the remark value
        /// </summary>
        /// <param name="input"></param>
        /// <param name="replacements"></param>
        /// <returns></returns>
        private static string ReplaceTemplateKeys(string input, Dictionary<string, string> replacements)
        {
            string pattern = @"\{([^{}]+)\}"; //// Pattern to match {LoginName}

            return Regex.Replace(input, pattern, match =>
            {
                string key = match.Groups[1].Value;
                if (replacements.TryGetValue(key, out string replacement))
                {
                    return replacement;
                }
                return match.Value; // Keep the original if no replacement found
            });
        }

        /// <summary>
        /// Convert audit report data to JObject and Separate JArray and Jsting to separate list.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="stringValues"></param>
        /// <param name="arrayValues"></param>
        private static void SeparateValues(JToken token, Dictionary<string, string> stringValues, List<JObject> arrayValues)
        {
            if (token.Type == JTokenType.Object)
            {
                foreach (JProperty property in from JProperty property in token.OfType<JProperty>()
                                               where property.Value.Type is not JTokenType.Array and not JTokenType.Object
                                               select property)
                {
                    stringValues.Add(property.Name, (string)property.Value);
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                arrayValues.AddRange(from JToken item in token.ToList()
                                     select (JObject)item);
            }
        }
    }
}
