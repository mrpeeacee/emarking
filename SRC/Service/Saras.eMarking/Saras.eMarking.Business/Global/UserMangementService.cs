using MediaLibrary.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.GlobalBusinessInterface;
using Saras.eMarking.Domain.Interfaces.GlobalRepositoryInterfaces;
using Saras.eMarking.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Saras.eMarking.Domain.ViewModels.Auth;
using Newtonsoft.Json;
using Saras.eMarking.Domain.Extensions;

namespace Saras.eMarking.Business.Global
{
    public class UserMangementService : BaseService<UserMangementService>, IUserMangementService
    {
        private readonly IUserManagementRepository _usermanagementRepository;
        public UserMangementService(IUserManagementRepository usermanagementRepository, ILogger<UserMangementService> _logger, AppOptions appOptions) : base(_logger, appOptions)
        {
            _usermanagementRepository = usermanagementRepository;
        }

        public async Task<GetAllApplicationUsersModel> GetAllUsers(SearchFilterModel searchFilterModel, long CuurentUserId)
        {
            logger.LogInformation("User Management Service >> GetAllUsers() started");
            try
            {
                return await _usermanagementRepository.GetAllUsers(searchFilterModel, CuurentUserId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while getting All users list:Method Name:GetAllUsers()");
                throw;
            }
        }
        public async Task<GetCreateEditUserModel> GetCreateEditUserdetails(long UserId, long CuurentUserId)
        {
            logger.LogInformation("User Management Service >> GetCreateEditUserdetails() started");
            try
            {
                return await _usermanagementRepository.GetCreateEditUserdetails(UserId, CuurentUserId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while getting Create and edit user details:Method Name:GetCreateEditUserdetails()");
                throw;
            }
        }
        public async Task<string> CreateEditUser(CreateEditUser model, long CurrentUserRoleId)
        {
            logger.LogInformation("User Management Service >> CreateEditUser() started");
            try
            {
                return await _usermanagementRepository.CreateEditUser(model, CurrentUserRoleId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while creating or editing users:Method Name:CreateEditUser()");
                throw;
            }
        }
        public SendMailRequestModel Resetpwd(long UserId, long CurrentUserRoleId)
        {
            SendMailRequestModel sendMailRequestModel = new SendMailRequestModel();
            logger.LogInformation("User Management Service >> Resetpwd() started");
            try
            {
                UserContext usercontext = _usermanagementRepository.Resetpwd(UserId, CurrentUserRoleId);
                if (usercontext == null) return null;

                sendMailRequestModel.Status = usercontext.Status;
                sendMailRequestModel.QueueID = usercontext.MailQueueId;
                sendMailRequestModel.IsMailSent = false;

                if (usercontext.Status == "SUCC001")
                {
                    sendMailRequestModel.Status = usercontext.Status;
                    sendMailRequestModel.QueueID = usercontext.MailQueueId;

                    var ApiUrl = AppOptions.AppSettings.OutboundApiUrl + "api/eMarking/SendEmail";
                    var result = ApiHandler.PostApiHandler(ApiUrl, JsonConvert.SerializeObject(sendMailRequestModel), "application/json", 0);

                    // Deserialize the JSON string
                    List<SendMailResponseModel> jsonObject = JsonConvert.DeserializeObject<List<SendMailResponseModel>>(result);
                    if (jsonObject.Count > 0)
                    {
                        // Access the ResponseData string
                        string isMailSentString = jsonObject[0].IsMailSent.ToString();
                        bool isMailSent = bool.Parse(isMailSentString);
                        sendMailRequestModel.IsMailSent = isMailSent;
                    }
                    else
                    {
                        sendMailRequestModel.IsMailSent = false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while Resetting password :Method Name:Resetpwd()");
                throw;
            }
            return sendMailRequestModel;
        }


        public async Task<UserCreations> UserCreation(IFormFile file, long ProjectUserRoleId, int type)
        {
            logger.LogInformation("User Management Service >> UserCreation() started");
            try
            {
                UserCreations uc = new UserCreations();
                string status = ProcessUploadedFile(file, uc);

                if (status == "SUCCESS")
                {
                    if (uc.users.Any(a => !a.Status))
                    {
                        return uc;
                    }
                    else
                    {
                        return await _usermanagementRepository.UserCreation(uc, ProjectUserRoleId, type);
                    }
                }
                else
                {
                    uc.status = status;
                    return uc;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while creating users: Method Name:UserCreation()");
                throw;
            }
        }

        private string ProcessUploadedFile(IFormFile file, UserCreations uc)
        {
            if (file.Length > 0 && file.ContentType == "text/csv")
            {
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                if (sFileExtension == ".csv")
                {
                    var folderName = Path.Combine("Resources", "BulkUser");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var dir = @"\";
                    string fullPathWithFileName = pathToSave + dir + Path.GetFileNameWithoutExtension(fileName) + "_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + Path.GetExtension(fileName);

                    if (!Directory.Exists(pathToSave))
                    {
                        Directory.CreateDirectory(pathToSave);
                    }

                    using (var stream = new FileStream(fullPathWithFileName, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    DataTable validatedTable = FileRead(fullPathWithFileName, out string status);

                   //// var duplicates = validatedTable.AsEnumerable().GroupBy(i => new { Name = i.Field<string>("LoginName"), Subject = i.Field<string>("NRIC") }).Where(g => g.Count() > 1).Select(g => new { g.Key.Name, g.Key.Subject }).ToList();

                    var logiddup = validatedTable.AsEnumerable().GroupBy(i => new { Name = i.Field<string>("LoginName")}).Where(g => g.Count() > 1).Select(g => new { g.Key.Name }).ToList();

                    var nricdup = validatedTable.AsEnumerable().GroupBy(i => new { NRIC = i.Field<string>("NRIC") }).Where(g => g.Count() > 1).Select(g => new { g.Key.NRIC }).ToList();

                    if(logiddup.Count > 0 || nricdup.Count > 0)
                    {
                        uc.status = "Duplicate";
                        uc.users = ExtractUserDetails(validatedTable);

                        List<UserDetails> details = uc.users.GroupBy(a => new { a.LoginName, a.NRIC }).Select(ud => new UserDetails() { LoginName = ud.Key.LoginName, NRIC = ud.Key.NRIC }).ToList();

                        details = details.Distinct().ToList();
                        StringBuilder error = new StringBuilder();
                        foreach (var (item, count) in from item in details
                                                      let count = uc.users.Count(a => a.LoginName == item.LoginName || a.NRIC == item.NRIC)
                                                      select (item, count))
                        {
                            if(count > 1)
                            {
                                error.Clear();

                                error.AppendLine("Duplicate Login Name/NRIC,");

                                var err = error.ToString().Split(',').ToList();
                                err.RemoveAll(s => string.IsNullOrEmpty(s));

                                foreach (var items in uc.users.Where(a => a.LoginName == item.LoginName || a.NRIC == item.NRIC).ToList())
                                {
                                    items.Status = false;
                                    items.Error = err;
                                }
                            }
                        }

                        return "Duplicate";
                    }

                    if (validatedTable.Rows.Count > 0)
                    {
                        uc.status = status;
                        uc.users = ExtractUserDetails(validatedTable);
                        
                        return "SUCCESS";
                    }
                    else
                    {
                        uc.status = "NODATA";
                        return "NODATA";
                    }
                }
                else
                {
                    uc.status = "NOTCSV";
                    return "NOTCSV";
                }
            }
            else
            {
                uc.status = "INVALIDFILE";
                return "INVALIDFILE";
            }
        }

        private static List<UserDetails> ExtractUserDetails(DataTable table)
        {
            List<UserDetails> userlist = new List<UserDetails>();

            foreach (DataRow dr in table.Rows)
            {
                var err = Convert.ToString(dr["Error"]).Split(',').ToList();
                err.RemoveAll(s => string.IsNullOrEmpty(s));

                userlist.Add(new UserDetails
                {
                    LoginName = dr["LoginName"].ToString(),
                    RoleCode = dr["BaseRole"].ToString(),
                    NRIC = dr["NRIC"].ToString(),
                    FirstName = Convert.ToString(dr["Name"].ToString()),
                    SchoolCode = Convert.ToString(dr["SchoolCode"].ToString()),
                    SchoolName = Convert.ToString(dr["SchoolName"].ToString()),
                    Status = Convert.ToBoolean(dr["Status"]),
                    Error = err
                });
            }

            return userlist;
        }


        private DataTable FileRead(string filePath, out string sts)
        {
            DataTable ValidatedTable = new();
            try
            {
                DataTable ExcelDataTable = new();

                ExcelDataTable = GetDataTableFromExcelFile(filePath, out sts);

                if (!ExcelDataTable.Columns.Contains("Error"))
                {
                    ValidatedTable = ValidateImportUsers(ExcelDataTable, out sts);

                }
                else
                {
                    ValidatedTable = ExcelDataTable;
                }

                //Remove uploaded file from directory folder              
                try
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error in User Management page while deleting Uploaded file for specific Qig:Method Name:FileRead()");
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while Uploading file for specific Qig:Method Name:FileRead()");
                throw;
            }
            return ValidatedTable;
        }

        private static DataTable GetDataTableFromExcelFile(string csvFilePath, out string sts)
        {
            DataTable csvData = new DataTable();
            sts = "";

            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(csvFilePath))
                {
                    ConfigureCsvReader(csvReader);
                    string[] colFields = ReadAndValidateColumnFields(csvReader);

                    if (colFields.Length == 6)
                    {
                        if (AreColumnFieldsValid(colFields))
                        {
                            CreateColumns(csvData, colFields);

                            ReadAndAddDataRows(csvReader, csvData);
                        }
                        else
                        {
                            csvData = CreateInvalidFormatDataTable();
                            sts = "INVALIDFORMAT";
                        }
                    }
                    else if (colFields.Length == 0)
                    {
                        csvData = CreateEmptyFileDataTable();
                        sts = "EMPTYFILE";
                    }
                    else if (colFields.Length > 0 && colFields.Length != 6)
                    {
                        csvData = CreateInvalidFormatDataTable();
                        sts = "INVALIDFORMAT";
                    }
                }
            }
            catch (Exception)
            {
                csvData = CreateInvalidFileDataTable();
                sts = "INVALIDFILE";
            }

            return csvData;
        }

        private static void ConfigureCsvReader(TextFieldParser csvReader)
        {
            csvReader.SetDelimiters(new string[] { "," });
            csvReader.HasFieldsEnclosedInQuotes = true;
            csvReader.TrimWhiteSpace = true;
            csvReader.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
        }

        private static string[] ReadAndValidateColumnFields(TextFieldParser csvReader)
        {
            string[] colFields = csvReader.ReadFields().Take(6).ToArray();

            if (colFields.Length == 0)
                return colFields;

            if (!AreColumnFieldsValid(colFields))
            {
                colFields = null;
            }

            return colFields;
        }

        private static bool AreColumnFieldsValid(string[] colFields)
        {
            return colFields[0].Equals("Name") && colFields[1].Equals("LoginName") && colFields[2].Equals("NRIC")
                && colFields[3].Equals("SchoolCode") && colFields[4].Equals("SchoolName") && colFields[5].Equals("BaseRole");
        }

        private static void CreateColumns(DataTable csvData, string[] colFields)
        {
            foreach (string column in colFields)
            {
                DataColumn datecolumn = new(column)
                {
                    AllowDBNull = true
                };
                csvData.Columns.Add(datecolumn);
            }
        }

        private static void ReadAndAddDataRows(TextFieldParser csvReader, DataTable csvData)
        {
            while (!csvReader.EndOfData)
            {
                string[] fieldData = csvReader.ReadFields().Take(6).ToArray();

                if (string.Join(",", fieldData).Trim(',') != string.Empty)
                {
                    csvData.Rows.Add(fieldData);
                }
            }
        }


        private static DataTable CreateInvalidFormatDataTable()
        {
            DataTable csvData = CreateColumnRemark();
            var activityRow = csvData.NewRow();
            activityRow.SetField(7, "Invalid data format, Please refer sample csv template,");
            activityRow.SetField(6, false);
            csvData.Rows.Add(activityRow.ItemArray);
            return csvData;
        }

        private static DataTable CreateEmptyFileDataTable()
        {
            DataTable csvData = CreateColumnRemark();
            DataRow dr = csvData.NewRow();
            dr["Error"] = "EmptyData,";
            dr["Status"] = false;
            csvData.Rows.Add(dr);
            return csvData;
        }

        private static DataTable CreateInvalidFileDataTable()
        {
            DataTable csvData = CreateColumnRemark();
            DataRow dr = csvData.NewRow();
            dr["Status"] = false;
            dr["Error"] = "Invalid file format,";
            csvData.Rows.Add(dr);
            return csvData;
        }


        private DataTable ValidateImportUsers(DataTable CSVDataTable, out string sts)
        {
            DataTable Table = new DataTable();
            sts = "SU001";
            var index = 0;

            try
            {
                Table = CreateColumnRemark();
                var activityRow = Table.NewRow();

                var strName = @"^(?!ds+$)(?:[a-zA-Z][a-zA-Z0-9 &@.,()'-_]*)?$";
                var stremail = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
                Regex Name = new Regex(strName, RegexOptions.IgnoreCase);
                Regex Email = new Regex(stremail, RegexOptions.IgnoreCase);
                Regex rg = new Regex(@"^[a-zA-Z0-9\s,]*$", RegexOptions.IgnoreCase);

                CSVDataTable.Columns.Add("Status", typeof(bool));
                CSVDataTable.Columns.Add("Error", typeof(string));

                StringBuilder sbstatus = new StringBuilder();

                if (CSVDataTable.Rows.Count > 0)
                {
                    foreach (DataRow excelrowdata in CSVDataTable.Rows)
                    {
                        sbstatus.Clear();

                        index++;
                        activityRow.SetField(7, "Valid");//Remarks
                        activityRow.SetField(6, true);
                        activityRow = AddRowRemark(excelrowdata, activityRow);

                        CheckEmptyField(excelrowdata, sbstatus, activityRow);
                        CheckFormatFields(excelrowdata, sbstatus, activityRow, Name, Email, rg);

                        Table.Rows.Add(activityRow.ItemArray);
                    }

                    if (Table.AsEnumerable().Any(row => !row.Field<bool>("Status")))
                    {
                        sts = "FAILED";
                    }
                }
                else
                {
                    SetEmptyRowError(activityRow, sbstatus);
                    activityRow.SetField(6, false);
                    sts = "EMPTYROW";
                    Table.Rows.Add(activityRow.ItemArray);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while Uploading file for specific Qig:Method Name:ValidateImportUsers()");
                throw;
            }

            return Table;
        }

        private static void CheckEmptyField(DataRow excelrowdata, StringBuilder sbstatus, DataRow activityRow)
        {
            if (IsFieldEmpty(excelrowdata, 0))
            {
                activityRow.SetField(7, sbstatus.Append("Name is Empty,"));
                activityRow.SetField(6, false);
            }
            if (IsFieldEmpty(excelrowdata, 1))
            {
                activityRow.SetField(7, sbstatus.Append("Login name is empty,"));
                activityRow.SetField(6, false);
            }
            if (IsFieldEmpty(excelrowdata, 2))
            {
                activityRow.SetField(7, sbstatus.Append("NRIC is empty,"));
                activityRow.SetField(6, false);
            }
            if (IsFieldEmpty(excelrowdata, 3))
            {
                activityRow.SetField(7, sbstatus.Append("School code is empty,"));
                activityRow.SetField(6, false);
            }
            if (IsFieldEmpty(excelrowdata, 4))
            {
                activityRow.SetField(7, sbstatus.Append("School name is empty,"));
                activityRow.SetField(6, false);
            }
            if (IsFieldEmpty(excelrowdata, 5))
            {
                activityRow.SetField(7, sbstatus.Append("Role is empty,"));
                activityRow.SetField(6, false);
            }
        }

        private static void CheckFormatFields(DataRow excelrowdata, StringBuilder sbstatus, DataRow activityRow, Regex Name, Regex Email, Regex rg)
        {
            if (!IsFieldEmpty(excelrowdata, 0) && !Name.IsMatch(excelrowdata[0].ToString().ToLower()))
            {
                activityRow.SetField(7, sbstatus.Append("Name is not in correct format,"));
                activityRow.SetField(6, false);
            }
            if (!IsFieldEmpty(excelrowdata, 1) && !Email.IsMatch(excelrowdata[1].ToString().ToLower()))
            {
                activityRow.SetField(7, sbstatus.Append("Login name is not in correct format,"));
                activityRow.SetField(6, false);
            }
            if (!IsFieldEmpty(excelrowdata, 2) && excelrowdata[2].ToString().ToLower().Length != 9)
            {
                activityRow.SetField(7, sbstatus.Append("NRIC must be 9 digits,"));
                activityRow.SetField(6, false);
            }
            if (!IsFieldEmpty(excelrowdata, 2) && !rg.IsMatch(excelrowdata[2].ToString()))
            {
                activityRow.SetField(7, sbstatus.Append("NRIC should not contain special characters,"));
                activityRow.SetField(6, false);
            }
            if (excelrowdata[5].ToString().ToLower() != "markingpersonnel" && !IsFieldEmpty(excelrowdata, 5))
            {
                activityRow.SetField(7, sbstatus.Append("Only MarkingPersonnel role is allowed,"));
                activityRow.SetField(6, false);
            }
        }

        private static bool IsFieldEmpty(DataRow row, int columnIndex)
        {
            return row.ItemArray[columnIndex].ToString().ToLower() == "";
        }

        private static void SetEmptyRowError(DataRow activityRow, StringBuilder sbstatus)
        {
            sbstatus.Append("Name is empty,");
            sbstatus.Append("Login name is empty,");
            sbstatus.Append("NRIC is empty,");
            sbstatus.Append("School code is empty,");
            sbstatus.Append("School name is empty,");
            sbstatus.Append("Role is empty,");

            activityRow.SetField(7, sbstatus.ToString());
        }


        private static DataTable CreateColumnRemark()
        {
            DataTable ImportData = new();
            ImportData.Columns.Add("Name", typeof(string));
            ImportData.Columns.Add("LoginName", typeof(string));

            ImportData.Columns.Add("NRIC", typeof(string));
            ImportData.Columns.Add("SchoolCode", typeof(string));
            ImportData.Columns.Add("SchoolName", typeof(string));
            ImportData.Columns.Add("BaseRole", typeof(string));

            ImportData.Columns.Add("Status", typeof(bool));

            ImportData.Columns.Add("Error", typeof(string));
            return ImportData;

        }

        private static DataRow AddRowRemark(DataRow excelrowdata, DataRow excelcolumn)
        {
            excelcolumn["LoginName"] = excelrowdata["LoginName"];
            excelcolumn["BaseRole"] = excelrowdata["BaseRole"];
            excelcolumn["Name"] = excelrowdata["Name"];
            excelcolumn["SchoolCode"] = excelrowdata["SchoolCode"];
            excelcolumn["SchoolName"] = excelrowdata["SchoolName"];
            excelcolumn["NRIC"] = excelrowdata["NRIC"];
            return excelcolumn;
        }


        public async Task<List<GetAllUsersModel>> GetBlockedUsers()
        {
            try
            {
                return await _usermanagementRepository.GetBlockedUsers();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while getting All Blocked users list:Method Name:GetBlockedUsers()");
                throw;
            }
        }
        public async Task<string> unblockUser(List<GetAllUsersModel> objunblockUsers)
        {

            try
            {
                return await _usermanagementRepository.unblockUser(objunblockUsers);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while unblocking users list:Method Name:unblockUser()");
                throw;
            }
        }

        public async Task<List<PassPharseModel>> GetPassPhrase(long ModifiedId)
        {

            try
            {
                return await _usermanagementRepository.GetPassPhrase(ModifiedId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while getting All Pass Phrase list:Method Name:GetPassPhrase()");
                throw;
            }
        }

        public async Task<string> AddPassPhrase(PassPharseModel passPhraseObject, long ModifiedId)
        {

            try
            {
                string status = ValidateAddPassPhrase(passPhraseObject);
                if (status == "success")
                {
                    return await _usermanagementRepository.AddPassPhrase(passPhraseObject, ModifiedId);
                }
                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while adding Add Pass Phrase:Method Name:AddPassPhrase()");
                throw;
            }
        }
        private static string ValidateAddPassPhrase(PassPharseModel passPhraseObject)
        {
            string status = "";
            if (passPhraseObject.PassPharseCode == null || passPhraseObject.PassPharseCode == "" || passPhraseObject.PassPharseCode.Length < 8 || passPhraseObject.PassPharseCode.Length > 8)
            {
                status = "failed";
            }
            else if (string.IsNullOrWhiteSpace(passPhraseObject.PassPharseCode.Trim()) || passPhraseObject.PassPharseCode.Trim().Contains(' '))
            {
                status = "failed";
            }
            else
            {
                status = "success";
            }
            return status;
        }

        public async Task<string> ValidateRemoveUser(long UserId)
        {

            try
            {

                return await _usermanagementRepository.ValidateRemoveUser(UserId);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while getting All users list:Method Name:ValidateRemoveUser()");
                throw;
            }
        }

        public async Task<string> ScriptExists(int StatusId, long UserId)
        {

            try
            {

                return await _usermanagementRepository.ScriptExists(StatusId, UserId);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while getting All users list:Method Name:ScriptExists()");
                throw;
            }
        }
        public async Task<SendMailRequestModel> ChangeStatusUsers(int StatusId, long UserId, long currentUserId)
        {
            SendMailRequestModel sendMailRequestModel = new SendMailRequestModel();

            try
            {
                UserContext usercontext = await _usermanagementRepository.ChangeStatusUsers(StatusId, UserId, currentUserId);
                if (usercontext == null) return null;

                sendMailRequestModel.Status = usercontext.Status;
                sendMailRequestModel.IsMailSent = false;


                if (usercontext.Status == "EN001")
                {
                    sendMailRequestModel.Status = usercontext.Status;
                    sendMailRequestModel.QueueID = usercontext.MailQueueId;

                    var ApiUrl = AppOptions.AppSettings.OutboundApiUrl + "api/eMarking/SendEmail";
                    var result = ApiHandler.PostApiHandler(ApiUrl, JsonConvert.SerializeObject(sendMailRequestModel), "application/json", 0);

                    // Deserialize the JSON string
                    List<SendMailResponseModel> jsonObject = JsonConvert.DeserializeObject<List<SendMailResponseModel>>(result);

                    if (jsonObject.Count > 0)
                    {
                        // Access the ResponseData string
                        string isMailSentString = jsonObject[0].IsMailSent.ToString();
                        bool isMailSent = bool.Parse(isMailSentString);
                        sendMailRequestModel.IsMailSent = isMailSent;
                    }
                    else
                    {
                        sendMailRequestModel.IsMailSent = false;
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while changing users status :Method Name:ChangeStatusUsers()");
                throw;
            }
            return sendMailRequestModel;
        }

        public async Task<CreateEditUser> GetMyProfileDetails(long UserId, long? ProjectuserroleId = null)
        {
            logger.LogInformation("UserManagement Service >> GetMyProfileDetails() started");
            try
            {
                return await _usermanagementRepository.GetMyProfileDetails(UserId, ProjectuserroleId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Usermanagement page while getting my profile details for specific Users : Method Name : GetMyProfileDetails()");
                throw;
            }
        }

        public async Task<RoleSchooldetails> GetApplicationLevelUserRoles()
        {
            logger.LogInformation("User Management Service >> GetApplicationLevelUserRoles() started");
            try
            {
                return await _usermanagementRepository.GetApplicationLevelUserRoles();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while getting All Roles :Method Name:GetApplicationLevelUserRoles()");
                throw;
            }
        }
    }
}
