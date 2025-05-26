using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using Saras.eMarking.Domain;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.ProjectUsers;
using Saras.eMarking.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Setup.ProjectUsers
{
    public class ProjectUsersRepository : BaseRepository<ProjectUsersRepository>, IProjectUsersRepository
    {
        private readonly ApplicationDbContext context;
        public AppOptions AppOptions { get; set; }

        public ProjectUsersRepository(ApplicationDbContext context, ILogger<ProjectUsersRepository> _logger, AppOptions appOptions) : base(_logger)
        {
            this.context = context;
            AppOptions = appOptions;
        }

        /// <summary>
        /// Userscount : This API method is to Get Project users data view
        /// </summary>
        /// <param name="QigId"></param>
        /// <returns></returns>
        public async Task<ProjectUserCountModel> Userscount(long ProjectID, long? QigId)
        {
            ProjectUserCountModel usercount = null;
            try
            {
                if (ProjectID != 0 && QigId != 0)
                {
                    List<RolesModel> userscount = (await (from uri in context.ProjectUserRoleinfos
                                                          join u in context.UserInfos on uri.UserId equals u.UserId
                                                          join pqh in context.ProjectQigteamHierarchies on uri.ProjectUserRoleId equals pqh.ProjectUserRoleId
                                                          join r in context.Roleinfos on uri.RoleId equals r.RoleId
                                                          where !r.Isdeleted && !uri.Isdeleted && uri.IsActive == true && !u.IsDeleted && uri.ProjectId == ProjectID
                                                          && pqh.Qigid == QigId && !pqh.Isdeleted && (r.RoleCode != "EO" && r.RoleCode != "Admin" && r.RoleCode != "SUPERADMIN" && r.RoleCode != "SERVICEADMIN" && r.RoleCode != "EM")
                                                          select new RolesModel
                                                          {
                                                              RoleCode = r.RoleCode,
                                                              ProjectUserRoleId = uri.ProjectUserRoleId,
                                                              RoleId = uri.RoleId,
                                                              ProjectId = uri.ProjectId
                                                          }).Distinct().ToListAsync()).ToList();

                    usercount = BindUserCountModel(userscount);
                }
                else
                {
                    List<RolesModel> userscount = (await (from uri in context.ProjectUserRoleinfos
                                                          join u in context.UserInfos on uri.UserId equals u.UserId
                                                          join p in context.ProjectInfos on uri.ProjectId equals p.ProjectId
                                                          join r in context.Roleinfos on uri.RoleId equals r.RoleId
                                                          where !r.Isdeleted && !uri.Isdeleted && uri.IsActive == true && !u.IsDeleted && uri.ProjectId == ProjectID && (r.RoleCode != "EO" && r.RoleCode != "Admin" && r.RoleCode != "SUPERADMIN" && r.RoleCode != "SERVICEADMIN" && r.RoleCode != "EM")
                                                          select new RolesModel
                                                          {
                                                              RoleCode = r.RoleCode,
                                                              ProjectUserRoleId = uri.ProjectUserRoleId,
                                                              RoleId = uri.RoleId,
                                                              ProjectId = uri.ProjectId
                                                          }).Distinct().ToListAsync()).ToList();

                    usercount = BindUserCountModel(userscount);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while getting Project user count for specific project:Method Name:Projectusercount() and ProjectID: ProjectID=" + ProjectID.ToString());
                throw;
            }
            return usercount;
        }

        private static ProjectUserCountModel BindUserCountModel(List<RolesModel> userscount)
        {
            return new ProjectUserCountModel()
            {
                Totuserscount = userscount.Count,
                Aocount = userscount.Count(x => x.RoleCode == "AO"),
                Cmcount = userscount.Count(x => x.RoleCode == "CM"),
                Acmcount = userscount.Count(x => x.RoleCode == "ACM"),
                Tlcount = userscount.Count(x => x.RoleCode == "TL"),
                Atlcount = userscount.Count(x => x.RoleCode == "ATL"),
                Markercount = userscount.Count(x => x.RoleCode == "MARKER")
            };
        }

        /// <summary>
        /// GetProjectUserslist : This API method is to Get Project users list
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProjectUsersviewModel>> GetProjectUserslist(long ProjectID, Domain.ViewModels.UserTimeZone userTimeZone)
        {
            List<ProjectUsersviewModel> userscount = new();
            try
            {
                userscount = (await (from uri in context.ProjectUserRoleinfos
                                     join u in context.UserInfos on uri.UserId equals u.UserId
                                     join p in context.ProjectInfos on uri.ProjectId equals p.ProjectId
                                     join r in context.Roleinfos on uri.RoleId equals r.RoleId
                                     join school in context.SchoolInfos on uri.SendingSchoolId equals school.SchoolId into school
                                     from schoolinfo in school.DefaultIfEmpty()
                                     where uri.IsActive == true && !uri.Isdeleted && !u.IsDeleted && !p.IsDeleted && !r.Isdeleted
                                     && uri.ProjectId == ProjectID && (r.RoleCode != "EO" && r.RoleCode != "Admin" && r.RoleCode != "SUPERADMIN" && r.RoleCode != "SERVICEADMIN" && r.RoleCode != "EM")
                                     select new ProjectUsersviewModel
                                     {
                                         UserName = u.FirstName + " " + u.LastName,
                                         LoginName = u.LoginId,
                                         SendingSchoolID = schoolinfo.SchoolName,
                                         RoleID = r.RoleCode,
                                         Role = r.RoleId,
                                         AppointStartDate = uri.AppointStartDate != null ? uri.AppointStartDate.UtcToProfileDateTime(userTimeZone) : null,
                                         AppointEndDate = uri.AppointEndDate != null ? uri.AppointEndDate.UtcToProfileDateTime(userTimeZone) : null,
                                         ProjectUserRoleID = uri.ProjectUserRoleId,
                                         isBlock = u.IsBlock,
                                         userId = u.UserId
                                     }).ToListAsync()).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while getting Project user count for specific project:Method Name:Projectusercount() and ProjectID: ProjectID=" + ProjectID.ToString());
                throw;
            }
            return userscount.OrderBy(m => m.Role).ThenBy(m => m.userId).ToList();
        }

        /// <summary>
        /// GetQiguserDatalist : This API method is to Get Qig users data list
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="FilteredBy"></param>
        /// <param name="ProjectUserRoleId"></param>
        /// <returns></returns>
        public async Task<List<QiguserDataviewModel>> GetQiguserDatalist(long ProjectId, long QigId, long ProjectUserRoleID, string FilteredBy, UserTimeZone userTimeZone)
        {
            List<QiguserDataviewModel> userscount = new();
            try
            {
                if (ProjectUserRoleID == 0)
                {
                    userscount = (await (from uri in context.ProjectUserRoleinfos
                                         join u in context.UserInfos on uri.UserId equals u.UserId
                                         join pqh in context.ProjectQigteamHierarchies on uri.ProjectUserRoleId equals pqh.ProjectUserRoleId
                                         join r in context.Roleinfos on uri.RoleId equals r.RoleId
                                         where uri.IsActive == true && pqh.ProjectId == ProjectId && pqh.Qigid == QigId &&
                                         !pqh.Isdeleted && !uri.Isdeleted && !u.IsDeleted && !r.Isdeleted && (r.RoleCode != "EO" && r.RoleCode != "Admin" && r.RoleCode != "SUPERADMIN" && r.RoleCode != "SERVICEADMIN" && r.RoleCode != "EM")
                                         select new QiguserDataviewModel
                                         {
                                             UserName = u.FirstName + " " + u.LastName,
                                             LoginName = u.LoginId,
                                             RoleID = r.RoleCode,
                                             Role = r.RoleId,
                                             ReportingTo = pqh.ReportingTo,
                                             AppointStartDate = uri.AppointStartDate.UtcToProfileDateTime(userTimeZone),
                                             AppointEndDate = uri.AppointEndDate.UtcToProfileDateTime(userTimeZone),
                                             ProjectUserRoleID = uri.ProjectUserRoleId,
                                             IsKP = pqh.IsKp,
                                             Isactive = pqh.IsActive,
                                             Rolelevel = (int)r.RoleLevelId,
                                             ProjectQIGTeamHierarchyID = pqh.ProjectQigteamHierarchyId
                                         }).ToListAsync()).OrderBy(p => p.Rolelevel).ToList();
                }
                else
                {
                    await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                    using SqlCommand sqlCmd = new("[Marking].[UspGetQIGProjectUserReportees]", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = QigId;
                    sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
                    sqlCmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = ProjectUserRoleID;
                    sqlCon.Open();
                    SqlDataReader reader = sqlCmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        int i = 0;
                        while (reader.Read())
                        {
                            QiguserDataviewModel objQualitycheck = new QiguserDataviewModel();
                            objQualitycheck.ProjectUserRoleID = Convert.ToInt64(reader["ProjectUserRoleID"]);

                            if (i == 0)
                            {
                                objQualitycheck.UserName = "";
                            }
                            else
                            {
                                objQualitycheck.UserName = reader["ReportingToName"] is DBNull ? "" : Convert.ToString(reader["ReportingToName"]);
                                objQualitycheck.ReportingToLoginName = reader["ReportingToLoginName"] is DBNull ? "" : Convert.ToString(reader["ReportingToLoginName"]);
                            }

                            objQualitycheck.LoginName = Convert.ToString(reader["ProjectUserName"]);
                            objQualitycheck.LoginID = Convert.ToString(reader["LoginID"]);
                            objQualitycheck.RoleID = Convert.ToString(reader["RoleCode"]);
                            objQualitycheck.Rolelevel = reader["RoleLevelID"] is DBNull ? 0 : Convert.ToInt16(reader["RoleLevelID"]);
                            if (objQualitycheck.ProjectUserRoleID != ProjectUserRoleID)
                            {
                                objQualitycheck.ReportingTo = reader["ReportingTo"] is DBNull ? 0 : Convert.ToInt64(reader["ReportingTo"]);
                            }
                            objQualitycheck.IsKP = Convert.ToBoolean(reader["IsKP"]);
                            userscount.Add(objQualitycheck);

                            i++;
                        }
                    }

                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                    if (sqlCon.State != ConnectionState.Closed)
                    {
                        sqlCon.Close();
                    }
                }

                QigstandardizationScriptSetting qigstdsetting = context.QigstandardizationScriptSettings.FirstOrDefault(qsss => !qsss.Isdeleted && qsss.Qigid == QigId);

                var s2 = qigstdsetting?.IsS2available ?? false;
                var s3 = qigstdsetting?.IsS3available ?? false;

                var ltrc1 = context.ScriptMarkingPhaseStatusTrackings.Where(s => s.ProjectId == ProjectId && s.Qigid == QigId &&
                           s.Status == (int)MarkingScriptStauts.Approved && !s.IsDeleted && s.Phase == (int)MarkingScriptPhase.RC1).ToList();

                var ltrc2 = context.ScriptMarkingPhaseStatusTrackings.Where(s => s.ProjectId == ProjectId && s.Qigid == QigId &&
                           s.Status == (int)MarkingScriptStauts.Approved && !s.IsDeleted && s.Phase == (int)MarkingScriptPhase.RC2).ToList();

                var ltadhoc = context.ScriptMarkingPhaseStatusTrackings.Where(s => s.ProjectId == ProjectId && s.Qigid == QigId &&
                           s.Status == (int)MarkingScriptStauts.Approved && !s.IsDeleted && s.Phase == (int)MarkingScriptPhase.Adhoc).ToList();

                var StandardisationSummary = context.MpstandardizationSummaries.Where(sse => sse.ProjectId == ProjectId && sse.Qigid == QigId && !sse.IsDeleted).ToList();

                foreach (var user in userscount)
                {
                    user.ReportingTousernamename = userscount.FirstOrDefault(u => u.ProjectUserRoleID == user.ReportingTo)?.UserName;

                    if (user.RoleID == "MARKER")
                    {
                        user.IsKPVal = "NA";
                    }
                    else
                    {
                        if (user.IsKP)
                        {
                            user.IsKPVal = "Yes";
                        }
                        else
                        {
                            user.IsKPVal = "No";
                        }
                    }

                    if (user.RoleID == "AO" || user.RoleID == "CM" || user.RoleID == "ACM" || user.IsKP)
                    {
                        user.S2S3Clear = "NA";

                        user.RC1Count = ltrc1.Count(r1 => r1.ActionBy == user.ProjectUserRoleID).ToString();
                        user.RC2Count = ltrc2.Count(r1 => r1.ActionBy == user.ProjectUserRoleID).ToString();
                        user.Adhoc = ltadhoc.Count(r1 => r1.ActionBy == user.ProjectUserRoleID).ToString();
                    }
                    else if ((user.RoleID == "TL" || user.RoleID == "ATL") && !user.IsKP)
                    {
                        if (s2)
                        {
                            if (StandardisationSummary.Any(s => s.ApprovalStatus == (int)AssessmentApprovalStatus.Approved && s.ProjectUserRoleId == user.ProjectUserRoleID && !s.IsDeleted))
                            {
                                user.S2S3Clear = "Yes";
                            }
                            else
                            {
                                user.S2S3Clear = "No";
                            }
                            user.RC1Count = ltrc1.Count(r1 => r1.ActionBy == user.ProjectUserRoleID).ToString();
                            user.RC2Count = ltrc2.Count(r1 => r1.ActionBy == user.ProjectUserRoleID).ToString();
                            user.Adhoc = ltadhoc.Count(r1 => r1.ActionBy == user.ProjectUserRoleID).ToString();
                        }
                        else
                        {
                            user.S2S3Clear = "NA";
                            user.RC1Count = ltrc1.Count(r1 => r1.ActionBy == user.ProjectUserRoleID).ToString();
                            user.RC2Count = ltrc2.Count(r1 => r1.ActionBy == user.ProjectUserRoleID).ToString();
                            user.Adhoc = ltadhoc.Count(r1 => r1.ActionBy == user.ProjectUserRoleID).ToString();
                        }
                    }
                    else if (user.RoleID == "MARKER")
                    {
                        if (s3)
                        {
                            if (StandardisationSummary.Any(s => s.ApprovalStatus == (int)AssessmentApprovalStatus.Approved && s.ProjectUserRoleId == user.ProjectUserRoleID && !s.IsDeleted))
                            {
                                user.S2S3Clear = "Yes";
                            }
                            else
                            {
                                user.S2S3Clear = "No";
                            }
                        }
                        else
                        {
                            user.S2S3Clear = "NA";
                        }

                        user.RC1Count = "NA";
                        user.RC2Count = "NA";
                        user.Adhoc = "NA";
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while getting Project user count for specific project:Method Name:Projectusercount() and ProjectID: ProjectID=" + ProjectId.ToString());
                throw;
            }
            return userscount.OrderBy(m => m.Rolelevel).ThenBy(m => m.LoginName).ToList();
        }

        /// <summary>
        /// QigUsersImportFile : This API method is to Qig users import file
        /// </summary>
        /// <param name="QigId"></param>
        /// <returns></returns>
        public Task<List<QigUserModel>> QigUsersImportFile(IFormFile file, long QigId, long ProjectID, long ProjectUserRoleID)
        {
            try
            {
                IFormFile Ifile = file;
                List<QigUserModel> userlist = new();
                DataTable validatedTable = new();
                if (Ifile.Length > 0 && Ifile.ContentType == "text/csv")
                {
                    string sFileExtension = Path.GetExtension(Ifile.FileName).ToLower();
                    if (sFileExtension == ".csv")
                    {
                        var folderName = Path.Combine("Resources", "CSVFiles");
                        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var dir = @"\";
                        string FullPathWithFileName = pathToSave + dir + Path.GetFileNameWithoutExtension(fileName) + "_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + Path.GetExtension(fileName);

                        // create folder if not exists

                        if (!Directory.Exists(pathToSave))
                        {
                            Directory.CreateDirectory(pathToSave);
                        }

                        using (var stream = new FileStream(FullPathWithFileName, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        validatedTable = FileRead(FullPathWithFileName, QigId, ProjectID, ProjectUserRoleID);

                        if (validatedTable.Rows.Count > 0)
                        {
                            foreach (DataRow dr in validatedTable.Rows)
                            {
                                userlist.Add(new QigUserModel
                                {
                                    LoginName = dr["LoginName"].ToString(),
                                    Role = dr["Role"].ToString(),
                                    ReportingTo = dr["ReportingTo"].ToString(),
                                    Rownum = Convert.ToInt64(dr["Rownum"].ToString()),
                                    status = Convert.ToBoolean(dr["status"].ToString()),
                                    dbstatus = dr["dbstatus"].ToString(),
                                    loginnamemsg = dr["loginnamemsg"].ToString(),
                                    roleidmsg = dr["roleidmsg"].ToString(),
                                    reportingtomsg = dr["reportingtomsg"].ToString(),
                                    Remarks = dr["Remarks"].ToString(),
                                    returnstatus = dr["returnstatus"].ToString()
                                });
                            }
                        }

                        // status = false;contains invalid data
                        if (validatedTable.AsEnumerable().Any(a => !a.Field<bool>("status") || a.Field<string>("dbstatus") != "S001"))
                        {
                            return Task.FromResult(userlist);
                        }
                        else
                        {
                            userlist = new List<QigUserModel>();
                            return Task.FromResult(userlist);
                        }
                    }
                }
                return Task.FromResult(userlist);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while Uploading file for specific Qig:Method Name:QigUsersImportFile() and ProjectID: ProjectID=" + ProjectID.ToString());
                throw;
            }
        }

        private DataTable FileRead(string filePath, long QigId, long ProjectID, long ProjectUserRoleID)
        {
            DataTable ValidatedTable = new();
            bool Isvalid = true;
            try
            {
                DataTable ExcelDataTable = new();
                ExcelDataTable = GetDataTableFromExcelFile(filePath);

                if (!ExcelDataTable.Columns.Contains("returnstatus"))
                {
                    ValidatedTable = ValidateImportUsers(ExcelDataTable, ProjectID);

                    var Projqigteamusers = (from pqth in context.ProjectQigteamHierarchies
                                            where pqth.ProjectId == ProjectID && pqth.Qigid == QigId
                                            && !pqth.Isdeleted && pqth.IsActive == true
                                            select pqth).ToList();

                    var res = Projqigteamusers.Where(p => ValidatedTable.AsEnumerable().Any(x => x.Field<long>("loginprojuserroleid") == p.ProjectUserRoleId)).ToList();

                    if (ValidatedTable.Rows.Count > 0)
                    {
                        if (ValidatedTable.AsEnumerable().Any(a => !a.Field<bool>("status")))
                        {
                            Isvalid = false;
                        }
                        else
                        {
                            Isvalid = true;
                        }

                        if (Isvalid)
                        {
                            ProjectQigteamHierarchy qigUsersListModel = new();

                            if (res.Count == 0)
                            {
                                foreach (DataRow user in ValidatedTable.Rows)
                                {
                                    // insert record
                                    qigUsersListModel = new ProjectQigteamHierarchy()
                                    {
                                        ProjectUserRoleId = user.Field<long>("loginprojuserroleid"),
                                        ReportingTo = user.Field<long>("reportingprojuserroleid"),
                                        ProjectId = ProjectID,
                                        Qigid = QigId,
                                        Isdeleted = false,
                                        IsActive = true,
                                        CreatedBy = ProjectUserRoleID,
                                        CreatedDate = DateTime.UtcNow,
                                        IsKp = (user.Field<string>("Role") == "AO" || user.Field<string>("Role") == "CM" || user.Field<string>("Role") == "ACM")
                                    };
                                    context.ProjectQigteamHierarchies.Add(qigUsersListModel);
                                    user["dbstatus"] = "S001";
                                }
                                context.SaveChanges();
                            }
                            else
                            {
                                foreach (var d in res)
                                {
                                    var dr = ValidatedTable.AsEnumerable().Where(s => s.Field<long>("loginprojuserroleid") == d.ProjectUserRoleId).Select(d => d).First();
                                    dr["loginnamemsg"] = "User already mapped to this QIG";
                                    dr["dbstatus"] = "User Already Exists";
                                    dr["Remarks"] = "Entry Exists more than once";
                                    dr["returnstatus"] = "Fail";
                                }
                            }
                        }
                        else
                        {
                            foreach (var d in res)
                            {
                                var dr = ValidatedTable.AsEnumerable().Where(s => s.Field<long>("loginprojuserroleid") == d.ProjectUserRoleId).Select(d => d).First();
                                dr["loginnamemsg"] = "User already mapped to this QIG";
                                dr["dbstatus"] = "User Already Exists";
                                dr["Remarks"] = "User already mapped to this QIG";
                                dr["returnstatus"] = "Fail";
                            }
                        }
                    }
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
                    logger.LogError(ex, "Error in User Management page while deleting Uploaded file for specific Qig:Method Name:FileRead() and ProjectID: ProjectID=" + ProjectID.ToString());
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while Uploading file for specific Qig:Method Name:FileRead() and ProjectID: ProjectID=" + ProjectID.ToString());
                throw;
            }
            return ValidatedTable;
        }

        private static DataTable GetDataTableFromExcelFile(string csv_file_path)
        {
            DataTable csvData = new();
            try
            {
                using (TextFieldParser csvReader = new(csv_file_path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    csvReader.TrimWhiteSpace = true;
                    csvReader.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                    string[] colFields = csvReader.ReadFields().Take(3).ToArray();
                    var index = 0;

                    if (colFields.Length == 3)
                    {
                        if (colFields[0].Equals("LoginName") && colFields[1].Equals("Role") && colFields[2].Equals("ReportingTo"))
                        {
                            foreach (string column in colFields)
                            {
                                DataColumn datecolumn = new(column);
                                datecolumn.AllowDBNull = true;
                                csvData.Columns.Add(datecolumn);
                            }

                            while (!csvReader.EndOfData)
                            {
                                string[] fieldData = csvReader.ReadFields().Take(3).ToArray();

                                if (string.Join(",", fieldData).Trim(',') == string.Empty)
                                    continue;
                                else
                                    csvData.Rows.Add(fieldData);
                            }
                        }
                        else if (!colFields[0].Equals("LoginName") || !colFields[1].Equals("Role") || !colFields[2].Equals("ReportingTo"))
                        {
                            csvData = CreateColumnRemark();
                            var activityRow = csvData.NewRow();
                            index++;
                            activityRow.SetField(4, index);
                            activityRow.SetField(8, false);
                            activityRow.SetField(12, "Invaliddata");
                            csvData.Rows.Add(activityRow.ItemArray);
                        }
                    }
                    else if (colFields.Length == 0)
                    {
                        csvData = CreateColumnRemark();
                        DataRow dr = csvData.NewRow();
                        dr["Rownum"] = 0;
                        dr["status"] = false;
                        dr["returnstatus"] = "Emptyfile";
                        csvData.Rows.Add(dr);
                    }
                    else if (colFields.Length > 0 && colFields.Length != 3)
                    {
                        csvData = CreateColumnRemark();
                        var activityRow = csvData.NewRow();
                        index++;
                        activityRow.SetField(4, index);
                        activityRow.SetField(8, false);
                        activityRow.SetField(12, "Invaliddata");
                        csvData.Rows.Add(activityRow.ItemArray);
                    }
                }
            }
            catch (Exception)
            {
                csvData = CreateColumnRemark();
                DataRow dr = csvData.NewRow();
                dr["Rownum"] = 0;
                dr["status"] = false;
                dr["returnstatus"] = "Emptyfile";
                csvData.Rows.Add(dr);
            }
            return csvData;
        }

        private DataTable ValidateImportUsers(DataTable CSVDataTable, long ProjectID)
        {
            var index = 0;
            DataTable Table = new();
            try
            {
                Table = CreateColumnRemark();
                var activityRow = Table.NewRow();

                var userslist = (from user in context.UserInfos
                                 join projuserrole in context.ProjectUserRoleinfos
                                 on user.UserId equals projuserrole.UserId
                                 join ri in context.Roleinfos
                                 on projuserrole.RoleId equals ri.RoleId
                                 join rolelevel in context.RoleLevels
                                 on ri.RoleLevelId equals rolelevel.RoleLevelId
                                 where projuserrole.ProjectId == ProjectID && !user.IsDeleted && !ri.Isdeleted
                                 && !projuserrole.Isdeleted && projuserrole.IsActive == true
                                 select new
                                 {
                                     LoginId = user.LoginId.ToLower(),
                                     projuserrole.ProjectUserRoleId,
                                     user.UserId,
                                     rolelevel.Order,
                                     RoleCode = ri.RoleCode.ToLower()
                                 }).ToList();
                if (CSVDataTable.Rows.Count > 0)
                {
                    foreach (DataRow excelrowdata in CSVDataTable.Rows)
                    {
                        index++;
                        activityRow = AddRowRemark(excelrowdata, activityRow);
                        activityRow.SetField(4, index);// count or Rowid
                        activityRow.SetField(3, "");//Remarks

                        StringBuilder sbstatus = new();

                        // check empty values of login name,Role & reporting to
                        if (activityRow.ItemArray[0].ToString().ToLower() != "" && activityRow.ItemArray[1].ToString().ToLower() != "" && activityRow.ItemArray[2].ToString().ToLower() != "")
                        {
                            // check user existence
                            var checkusers = userslist.FirstOrDefault(a => a.LoginId == activityRow.ItemArray[0].ToString().ToLower());
                            if (checkusers?.LoginId != null)
                            {
                                activityRow.SetField(10, checkusers.ProjectUserRoleId);
                            }
                            else
                            {
                                activityRow.SetField(5, "User is not mapped to this project");
                                activityRow.SetField(3, sbstatus.Append("User is not mapped to this project"));
                                activityRow.SetField(8, false);
                                activityRow.SetField(12, "Fail");
                            }

                            // checkreportingto existence
                            var checkreporting = userslist.FirstOrDefault(a => a.LoginId == activityRow.ItemArray[2].ToString().ToLower());
                            if (checkreporting?.LoginId != null)
                            {
                                activityRow.SetField(11, checkreporting.ProjectUserRoleId);
                            }
                            else
                            {
                                activityRow.SetField(7, "Reporting To is not mapped to this project");
                                activityRow.SetField(3, sbstatus.Append("Reporting To is not mapped to this project"));
                                activityRow.SetField(8, false);
                                activityRow.SetField(12, "Fail");
                            }

                            // Role existence
                            var role = userslist.FirstOrDefault(a => a.RoleCode == activityRow.ItemArray[1].ToString().ToLower());

                            if (role?.RoleCode == null)
                            {
                                activityRow.SetField(6, "Role doesn't exist");
                                activityRow.SetField(3, sbstatus.Append("Role doesn't exist"));
                                activityRow.SetField(8, false);
                                activityRow.SetField(12, "Fail");
                            }
                            else
                            {
                                //Login users role level check

                                var Prolelevelorder = userslist.FirstOrDefault(a => a.ProjectUserRoleId == Convert.ToInt32(activityRow.ItemArray[10].ToString().ToLower()));

                                // Reportingto users role level check

                                var Qrolelevelorder = userslist.FirstOrDefault(a => a.ProjectUserRoleId == Convert.ToInt32(activityRow.ItemArray[11].ToString().ToLower()));

                                // comparing both users level order
                                if (Prolelevelorder != null && Qrolelevelorder != null)
                                {
                                    if (Prolelevelorder.Order == Qrolelevelorder.Order)
                                    {
                                        activityRow.SetField(5, "Circular reporting is not allowed");
                                        activityRow.SetField(7, "Circular reporting is not allowed");
                                        activityRow.SetField(3, sbstatus.Append("Circular Reporting,Ex:Marker is reporting to marker"));
                                        activityRow.SetField(8, false);
                                        activityRow.SetField(12, "Fail");
                                    }
                                    else if (Prolelevelorder.Order < Qrolelevelorder.Order)
                                    {
                                        activityRow.SetField(7, Prolelevelorder.RoleCode.ToUpper() + " cannot be reporting to " + Qrolelevelorder.RoleCode.ToUpper());
                                        activityRow.SetField(3, sbstatus.Append(Prolelevelorder.RoleCode.ToLower() + " cannot be reporting to " + Qrolelevelorder.RoleCode.ToLower()));
                                        activityRow.SetField(8, false);
                                        activityRow.SetField(12, "Fail");
                                    }
                                }
                                if (role != null && checkusers?.LoginId != null)
                                {
                                    var rolecode = userslist.FirstOrDefault(a => a.LoginId == activityRow.ItemArray[0].ToString().ToLower() && a.RoleCode != activityRow.ItemArray[1].ToString().ToLower());
                                    if (rolecode != null && rolecode.RoleCode != activityRow.ItemArray[1].ToString().ToLower())
                                    {
                                        activityRow.SetField(6, activityRow.ItemArray[0].ToString() + " User is not belongs to this Role");
                                        activityRow.SetField(3, sbstatus.Append("User is not belongs to this Role"));
                                        activityRow.SetField(8, false);
                                        activityRow.SetField(12, "Fail");
                                    }
                                }
                            }

                            // Duplicate rows
                            var duplicatesData = CSVDataTable.AsEnumerable().GroupBy(r => r[0]).Where(gr => gr.Count() > 1).ToList();
                            if (duplicatesData.Count > 0)
                            {
                                foreach (var d in duplicatesData)
                                {
                                    if (activityRow.ItemArray[0].ToString().ToLower() == d.Key.ToString().ToLower() && d.Key.ToString().ToLower() != string.Empty)
                                    {
                                        activityRow.SetField(5, "Entry Exists more than once");
                                        activityRow.SetField(8, false);
                                        activityRow.SetField(12, "Fail");
                                    }
                                }
                            }
                        }
                        else if (activityRow.ItemArray[0].ToString().ToLower() == "" && activityRow.ItemArray[1].ToString().ToLower() == "" && activityRow.ItemArray[2].ToString().ToLower() == "")
                        {
                            activityRow.SetField(5, "Login Name is empty");
                            activityRow.SetField(6, "Role of Login Name is empty");
                            activityRow.SetField(7, "Reporting To is empty");
                            activityRow.SetField(3, sbstatus.Append("Login Name,Role and Reporting To are empty"));
                            activityRow.SetField(8, false);
                            activityRow.SetField(12, "Fail");
                        }
                        else if (activityRow.ItemArray[0].ToString().ToLower() == "" && activityRow.ItemArray[1].ToString().ToLower() == "")
                        {
                            activityRow.SetField(5, "Login Name is empty");
                            activityRow.SetField(6, "Role of Login Name is empty");
                            activityRow.SetField(3, sbstatus.Append("Login Name and Role are empty"));
                            activityRow.SetField(8, false);
                            activityRow.SetField(12, "Fail");
                        }
                        else if (activityRow.ItemArray[0].ToString().ToLower() == "" && activityRow.ItemArray[2].ToString().ToLower() == "")
                        {
                            activityRow.SetField(5, "Login Name is empty");
                            activityRow.SetField(7, "Reporting To is empty");
                            activityRow.SetField(3, sbstatus.Append("Login Name and Reporting To are empty"));
                            activityRow.SetField(8, false);
                            activityRow.SetField(12, "Fail");
                        }
                        else if (activityRow.ItemArray[1].ToString().ToLower() == "" && activityRow.ItemArray[2].ToString().ToLower() == "")
                        {
                            activityRow.SetField(6, "Role of Login Name is empty");
                            activityRow.SetField(7, "Reporting To is empty");
                            activityRow.SetField(3, sbstatus.Append("Role and Reporting To are empty"));
                            activityRow.SetField(8, false);
                            activityRow.SetField(12, "Fail");
                        }
                        else if (activityRow.ItemArray[0].ToString().ToLower() == "")
                        {
                            activityRow.SetField(5, "Login Name is empty");
                            activityRow.SetField(3, sbstatus.Append("Login Name is empty"));
                            activityRow.SetField(8, false);
                            activityRow.SetField(12, "Fail");
                        }
                        else if (activityRow.ItemArray[1].ToString().ToLower() == "")
                        {
                            activityRow.SetField(6, "Role of Login Name is empty");
                            activityRow.SetField(3, sbstatus.Append("Role of Login Name is empty"));
                            activityRow.SetField(8, false);
                            activityRow.SetField(12, "Fail");
                        }
                        else if (activityRow.ItemArray[2].ToString().ToLower() == "")
                        {
                            activityRow.SetField(7, "Reporting To is empty");
                            activityRow.SetField(3, sbstatus.Append("Reporting To is empty"));
                            activityRow.SetField(8, false);
                            activityRow.SetField(12, "Fail");
                        }
                        Table.Rows.Add(activityRow.ItemArray);
                    }
                }
                else
                {
                    index++;
                    activityRow.SetField(4, index);
                    activityRow.SetField(10, 0);
                    activityRow.SetField(11, 0);
                    activityRow.SetField(8, false);
                    activityRow.SetField(12, "Fail");
                    activityRow.SetField(5, "Login Name is empty");
                    activityRow.SetField(6, "Role of Login Name is empty");
                    activityRow.SetField(7, "Reporting To Column is empty");
                    Table.Rows.Add(activityRow.ItemArray);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while Uploading file for specific Qig:Method Name:ValidateImportUsers() and ProjectID: ProjectID=" + ProjectID.ToString());
                throw;
            }
            return Table;
        }

        private static DataTable CreateColumnRemark()
        {
            DataTable ImportData = new();
            ImportData.Columns.Add("LoginName", typeof(string));
            ImportData.Columns.Add("Role", typeof(string));
            ImportData.Columns.Add("ReportingTo", typeof(string));
            ImportData.Columns.Add("Remarks", typeof(string));
            ImportData.Columns.Add("Rownum", typeof(int));
            ImportData.Columns.Add("loginnamemsg", typeof(string));
            ImportData.Columns.Add("roleidmsg", typeof(string));
            ImportData.Columns.Add("reportingtomsg", typeof(string));
            ImportData.Columns.Add("status", typeof(bool));
            ImportData.Columns.Add("dbstatus", typeof(string));
            ImportData.Columns.Add("loginprojuserroleid", typeof(long));
            ImportData.Columns.Add("reportingprojuserroleid", typeof(long));
            ImportData.Columns.Add("returnstatus", typeof(string));
            return ImportData;
        }

        private static DataRow AddRowRemark(DataRow excelrowdata, DataRow excelcolumn)
        {
            excelcolumn["LoginName"] = excelrowdata["LoginName"];
            excelcolumn["Role"] = excelrowdata["Role"];
            excelcolumn["ReportingTo"] = excelrowdata["ReportingTo"];
            excelcolumn["Remarks"] = "";
            excelcolumn["Rownum"] = 0;
            excelcolumn["loginnamemsg"] = "";
            excelcolumn["roleidmsg"] = "";
            excelcolumn["reportingtomsg"] = "";
            excelcolumn["status"] = true;
            excelcolumn["dbstatus"] = "";
            excelcolumn["loginprojuserroleid"] = 0;
            excelcolumn["reportingprojuserroleid"] = 0;
            excelcolumn["returnstatus"] = "Pass";
            return excelcolumn;
        }

        /// <summary>
        /// GetQiguserDataById : This API method is to Get Qig User Details by Id
        /// </summary>
        /// <returns></returns>
        public async Task<QigUsersViewByIdModel> GetQiguserDataById(long ProjectId, long QigId, long ProjectQIGTeamHierarchyID)
        {
            QigUsersViewByIdModel qigUsersViewByIdModel = new();
            try
            {
                qigUsersViewByIdModel = await (from pqt in context.ProjectQigteamHierarchies
                                               join pur in context.ProjectUserRoleinfos
                                               on pqt.ProjectUserRoleId equals pur.ProjectUserRoleId
                                               join ui in context.UserInfos
                                               on pur.UserId equals ui.UserId
                                               join ri in context.Roleinfos
                                               on pur.RoleId equals ri.RoleId
                                               join rl in context.RoleLevels
                                               on ri.RoleLevelId equals rl.RoleLevelId
                                               where pqt.ProjectQigteamHierarchyId == ProjectQIGTeamHierarchyID && !pur.Isdeleted && !pqt.Isdeleted && !ui.IsDeleted && !ri.Isdeleted
                                               && !rl.IsDeleted
                                               select new QigUsersViewByIdModel
                                               {
                                                   UserName = ui.FirstName + ' ' + ui.LastName,
                                                   LoginName = ui.LoginId,
                                                   RoleName = ri.RoleName,
                                                   RoleCode = ri.RoleCode,
                                                   AppointStartDate = pur.AppointStartDate,
                                                   AppointEndDate = pur.AppointEndDate,
                                                   NRIC = pur.Nric,
                                                   Email = ui.EmailId,
                                                   Phone = pur.PhoneNo,
                                                   Order = rl.Order,
                                                   ProjectQIGTeamHierarchyID = pqt.ProjectQigteamHierarchyId,
                                                   ReportingToID = pqt.ReportingTo
                                               }).FirstOrDefaultAsync();

                var schoolid = (from pusm in context.ProjectUserSchoolMappings
                                where pusm.ExemptionSchoolId == qigUsersViewByIdModel.ProjectUserRoleID && !pusm.IsDeleted && pusm.IsSendingSchool
                                select pusm.ExemptionSchoolId).FirstOrDefault();
                if (schoolid != null)
                {
                    qigUsersViewByIdModel.SendingSchoolID = (from si in context.SchoolInfos
                                                             where si.SchoolId == schoolid.Value && !si.IsDeleted && si.ProjectId == ProjectId
                                                             select si.SchoolName).FirstOrDefault();
                }
                if (qigUsersViewByIdModel != null)
                {
                    qigUsersViewByIdModel.ReportingToIds = (from pqt in context.ProjectQigteamHierarchies
                                                            join pur in context.ProjectUserRoleinfos
                                                            on pqt.ProjectUserRoleId equals pur.ProjectUserRoleId
                                                            join ui in context.UserInfos
                                                            on pur.UserId equals ui.UserId
                                                            join ri in context.Roleinfos
                                                            on pur.RoleId equals ri.RoleId
                                                            join rl in context.RoleLevels
                                                            on ri.RoleLevelId equals rl.RoleLevelId
                                                            where pqt.ProjectId == ProjectId && pqt.Qigid == QigId && !pur.Isdeleted && pur.IsActive == true &&
                                                            !pqt.Isdeleted && pqt.IsActive == true && !ui.IsDeleted && !ri.Isdeleted && rl.Order < qigUsersViewByIdModel.Order && (ri.RoleCode != "EO" && ri.RoleCode != "Admin" && ri.RoleCode != "SUPERADMIN" && ri.RoleCode != "SERVICEADMIN" && ri.RoleCode != "EM")
                                                            select new ReportingToDetails()
                                                            {
                                                                ReportingToName = ui.FirstName + ' ' + ui.LastName,
                                                                ProjectUserRoleID = pqt.ProjectUserRoleId != 0 ? pqt.ProjectUserRoleId : 0,
                                                                RoleCode = ri.RoleCode
                                                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while getting Qig user for specific Qig:Method Name:GetQiguserDataById() and QigID: QigID=" + QigId.ToString());
                throw;
            }
            return qigUsersViewByIdModel;
        }

        /// <summary>
        /// UpdateQiguserDataById : This API method is to Update Qig User Details by Id
        /// </summary>
        /// <returns></returns>
        public async Task<string> UpdateQiguserDataById(long ProjectUserRoleID, long ProjectQIGTeamHierarchyID, long ReportingToId)
        {
            string status = "E001";
            ProjectQigteamHierarchy projectQigteamHierarchy;
            try
            {
                projectQigteamHierarchy = await context.ProjectQigteamHierarchies.Where(z => z.ProjectQigteamHierarchyId == ProjectQIGTeamHierarchyID).FirstOrDefaultAsync();
                if (projectQigteamHierarchy != null)
                {
                    projectQigteamHierarchy.ReportingTo = ReportingToId;
                    projectQigteamHierarchy.ModifiedBy = ProjectUserRoleID;
                    projectQigteamHierarchy.ModifiedDate = DateTime.UtcNow;
                    context.ProjectQigteamHierarchies.Update(projectQigteamHierarchy);
                    context.SaveChanges();
                    status = "U001";
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while updating reportingto for specific Qig:Method Name:UpdateQiguserDataById()");
                throw;
            }
            return status;
        }

        /// <summary>
        /// DeleteUsers : This API method is to User Delete.
        /// </summary>
        /// <returns></returns>
        public Task<string> DeleteUsers(long UserRoleId, long QigId, long ProjectID, long ProjectUserRoleID)
        {
            string status = string.Empty;

            try
            {
                logger.LogDebug($"ProjectUsersRepository > DeleteUsers() method started {UserRoleId}");

                using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Marking.UspDeleteQIGProjectUserReportees", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectID;
                        sqlCmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = QigId;
                        sqlCmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = UserRoleId;
                        sqlCmd.Parameters.Add("@UpdatedBy", SqlDbType.BigInt).Value = ProjectUserRoleID;
                        sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
                        sqlCon.Open();
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        status = sqlCmd.Parameters["@Status"].Value.ToString();
                    }
                }

                logger.LogDebug($"ProjectUsersRepository > DeleteUsers() method completed {UserRoleId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $" Error in User Management page > DeleteUsers() method {UserRoleId}");
                throw;
            }
            return Task.FromResult(status);
        }

        public async Task<List<BlankQigIds>> GetBlankQigIds(long ProjectId)
        {
            List<BlankQigIds> blankQigIds = new List<BlankQigIds>();

            try
            {
                logger.LogDebug($"ProjectUsersRepository > GetBlankQigIds() method started {ProjectId}");

                var qigids = context.ProjectQigs.Where(pq => pq.ProjectId == ProjectId && !pq.IsDeleted && pq.IsManualMarkingRequired).Select(pq => pq.ProjectQigid).ToList();

                string[] roleCodes = { "CM", "ACM", "TL", "ATL", "MARKER" };

                blankQigIds = (await (from pqt in context.ProjectQigteamHierarchies
                                      join pq in context.ProjectQigs
                                      on pqt.Qigid equals pq.ProjectQigid
                                      join pur in context.ProjectUserRoleinfos
                                      on pqt.ProjectUserRoleId equals pur.ProjectUserRoleId
                                      join ri in context.Roleinfos
                                      on pur.RoleId equals ri.RoleId
                                      where qigids.Contains(pqt.Qigid) && roleCodes.Contains(ri.RoleCode)
                                      && !pqt.Isdeleted && !pq.IsDeleted && !pur.Isdeleted && !ri.Isdeleted && pq.IsManualMarkingRequired

                                      select new BlankQigIds()
                                      {
                                          QigIds = pqt.Qigid,
                                          QigName = pq.Qigname
                                      }).Distinct().ToListAsync()).ToList();

                logger.LogDebug($"ProjectUsersRepository > GetBlankQigIds() method completed {ProjectId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page > GetBlankQigIds() method {ProjectId}");
                throw;
            }
            return blankQigIds;
        }

        /// <summary>
        /// UsersRoles : This API method is to get Roles List
        /// </summary>
        /// <returns></returns>
        public async Task<List<RolesModel>> UsersRoles(long ProjectUserRoleID)
        {
            List<RolesModel> rolesList = new();
            try
            {
                rolesList = (await (from role in context.Roleinfos
                                    select new RolesModel
                                    {
                                        RoleId = role.RoleId,
                                        RoleName = role.RoleName,
                                        RoleCode = role.RoleCode
                                    }).ToListAsync()).ToList();

                var roleCode = (from pri in context.ProjectUserRoleinfos
                                join ri in context.Roleinfos
                                on pri.RoleId equals ri.RoleId
                                where pri.ProjectUserRoleId == ProjectUserRoleID && !pri.Isdeleted && pri.IsActive == true && !ri.Isdeleted
                                select new QigUsersViewByIdModel
                                {
                                    RoleCode = ri.RoleCode
                                }).FirstOrDefault();

                if (rolesList != null && rolesList.Count > 0 && roleCode?.RoleCode != "EO")
                {
                    rolesList = rolesList.Where(a => a.RoleCode == "CM" || a.RoleCode == "ACM" || a.RoleCode == "TL" || a.RoleCode == "ATL" || a.RoleCode == "MARKER").ToList();
                }
                else
                {
                    rolesList = rolesList.Where(a => a.RoleCode == "AO" || a.RoleCode == "CM" || a.RoleCode == "ACM" || a.RoleCode == "TL" || a.RoleCode == "ATL" || a.RoleCode == "MARKER").ToList();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, " Error in User Management page > UsersRoles() method");
                throw;
            }
            return rolesList;
        }

        /// <summary>
        /// CreateUser : This API method is to create Single User
        /// </summary>
        /// <returns></returns>
        public Task<string> CreateUser(CreateUserModel createUserModel, long ProjectID, long ProjectCurrentUserID, long projectUserRoleID)
        {
            string status = string.Empty;
            try
            {
                logger.LogDebug($"ProjectUsersRepository > CreateUser() method started {createUserModel}");

                var roleCode = (from pri in context.ProjectUserRoleinfos
                                join ri in context.Roleinfos
                                on pri.RoleId equals ri.RoleId
                                where pri.ProjectUserRoleId == projectUserRoleID && !pri.Isdeleted && pri.IsActive == true && !ri.Isdeleted
                                select new QigUsersViewByIdModel
                                {
                                    RoleCode = ri.RoleCode
                                }).FirstOrDefault();

                if (roleCode?.RoleCode != "EO")
                {
                    string[] validroles1 = { "CM", "ACM", "TL", "ATL", "MARKER" };

                    if (!validroles1.Contains(createUserModel.RoleCode.ToUpper()))
                    {
                        throw new InvalidOperationException();
                    }
                }
                else
                {
                    string[] validroles2 = { "AO", "CM", "ACM", "TL", "ATL", "MARKER" };

                    if (!validroles2.Contains(createUserModel.RoleCode.ToUpper()))
                    {
                        throw new InvalidOperationException();
                    }
                }

                using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[Marking].[USPInsertProjectUsers]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectID;
                        sqlCmd.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value = ProjectCurrentUserID;
                        sqlCmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 50).Value = createUserModel.UserName;
                        sqlCmd.Parameters.Add("@LoginName", SqlDbType.NVarChar, 50).Value = createUserModel.LoginName;
                        sqlCmd.Parameters.Add("@SendingSchoolCode", SqlDbType.NVarChar, 50).Value = createUserModel.SendingSchooolName;
                        sqlCmd.Parameters.Add("@Role", SqlDbType.NVarChar, 50).Value = createUserModel.RoleCode;
                        sqlCmd.Parameters.Add("@AppointmentStartDate", SqlDbType.DateTime).Value = createUserModel.Appointmentstartdate;
                        sqlCmd.Parameters.Add("@AppointmentEndDate", SqlDbType.DateTime).Value = createUserModel.Appointmentenddate;
                        sqlCmd.Parameters.Add("@NRIC", SqlDbType.NVarChar, 100).Value = createUserModel.NRIC;
                        sqlCmd.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 50).Value = createUserModel.Phone;
                        sqlCmd.Parameters.Add("@Password", SqlDbType.NVarChar, 50).Value = createUserModel.Password;
                        sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;
                        sqlCon.Open();
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        status = sqlCmd.Parameters["@Status"].Value.ToString();
                    }
                }

                logger.LogDebug($"ProjectUsersRepository > CreateUser() method completed {createUserModel}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page > CreateUser() method {createUserModel}");
                throw;
            }
            return Task.FromResult(status);
        }

        public async Task<string> MoveMarkingTeamToEmptyQig(MoveMarkingTeamToEmptyQig moveMarkingTeamToEmptyQig)
        {
            string status = "ERR001";
            try
            {
                logger.LogDebug($"ProjectUsersRepository > MoveMarkingTeamToEmptyQig() method started {moveMarkingTeamToEmptyQig}");

                var TeamHeirarchy = (await (from p in context.ProjectQigteamHierarchies
                                            join pur in context.ProjectUserRoleinfos
                                             on p.ProjectUserRoleId equals pur.ProjectUserRoleId
                                            join ri in context.Roleinfos
                                            on pur.RoleId equals ri.RoleId
                                            where
                                             !p.Isdeleted && !pur.Isdeleted && !ri.Isdeleted && p.IsActive == true
                                            && p.Qigid == moveMarkingTeamToEmptyQig.FromQigId && p.ProjectId == moveMarkingTeamToEmptyQig.ProjectID

                                            select new
                                            {
                                                p.ProjectId,
                                                p.Qigid,
                                                p.ProjectUserRoleId,
                                                ri.RoleCode,
                                                p.ReportingTo,
                                                p.Isdeleted,
                                                p.IsKp,
                                                p.IsActive
                                            }).ToListAsync()).ToList();

                string[] roleCodes = { "CM", "ACM", "TL", "ATL", "MARKER" };

                var ExistingQigIds = (await (from pqt in context.ProjectQigteamHierarchies
                                             join pq in context.ProjectQigs
                                             on pqt.Qigid equals pq.ProjectQigid
                                             join pur in context.ProjectUserRoleinfos
                                             on pqt.ProjectUserRoleId equals pur.ProjectUserRoleId
                                             join ri in context.Roleinfos
                                             on pur.RoleId equals ri.RoleId
                                             where pqt.Qigid == moveMarkingTeamToEmptyQig.ToQigId && roleCodes.Contains(ri.RoleCode)
                                             && !pqt.Isdeleted && !pq.IsDeleted && !pur.Isdeleted && !ri.Isdeleted

                                             select new BlankQigIds()
                                             {
                                                 QigIds = pqt.Qigid,
                                                 ProjectUserRoleID = pqt.ProjectUserRoleId,
                                             }).Distinct().ToListAsync()).ToList();

                if (TeamHeirarchy.Count > 0 && ExistingQigIds.Count == 0)
                {
                    bool isfalse = false;

                    foreach (var team in TeamHeirarchy)
                    {
                        if (!context.ProjectQigteamHierarchies.Any(pq => pq.ProjectUserRoleId == team.ProjectUserRoleId
                        && pq.ProjectId == moveMarkingTeamToEmptyQig.ProjectID && pq.Qigid == moveMarkingTeamToEmptyQig.ToQigId
                          && !pq.Isdeleted && pq.IsActive == true))
                        {
                            ProjectQigteamHierarchy projectQigteamHierarchy = new ProjectQigteamHierarchy();

                            projectQigteamHierarchy.ProjectId = team.ProjectId;
                            projectQigteamHierarchy.Qigid = moveMarkingTeamToEmptyQig.ToQigId;
                            projectQigteamHierarchy.ProjectUserRoleId = team.ProjectUserRoleId;
                            projectQigteamHierarchy.ReportingTo = team.ReportingTo;
                            projectQigteamHierarchy.Isdeleted = team.Isdeleted;
                            projectQigteamHierarchy.IsKp = (team.RoleCode == "TL" || team.RoleCode == "ATL") ? isfalse : team.IsKp;
                            projectQigteamHierarchy.IsActive = team.IsActive;
                            projectQigteamHierarchy.CreatedDate = DateTime.UtcNow;

                            context.ProjectQigteamHierarchies.Add(projectQigteamHierarchy);
                        }
                    }

                    await context.SaveChangesAsync();

                    status = "SU001";
                }

                logger.LogDebug($"ProjectUsersRepository > MoveMarkingTeamToEmptyQig() method completed {moveMarkingTeamToEmptyQig}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page > MoveMarkingTeamToEmptyQig() method {moveMarkingTeamToEmptyQig}");
                throw;
            }
            return status;
        }

        public async Task<string> CheckS1StartedOrLiveMarkingEnabled(long QigId, long ProjectID)
        {
            string status = String.Empty;
            try
            {
                var s1started = await (from pus in context.ProjectUserScripts
                                       where !pus.Isdeleted && pus.IsRecommended == true && pus.Qigid == QigId && pus.ProjectId == ProjectID
                                       select pus).FirstOrDefaultAsync();

                var livemarkingstarted = await (from WST in context.ProjectWorkflowStatusTrackings
                                                join WS in context.WorkflowStatuses
                                                on WST.WorkflowStatusId equals WS.WorkflowId
                                                where WST.EntityId == QigId && WS.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Live_Marking_Qig) && WST.ProcessStatus == (int)WorkflowProcessStatus.Completed
                                                && WST.EntityType == 2 && !WST.IsDeleted && !WS.IsDeleted
                                                select new { WST, WS }).FirstOrDefaultAsync();
                if (s1started != null)
                {
                    status = "S1Started";
                }
                else if (livemarkingstarted != null)
                {
                    status = "LiveMarkingStarted";
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, " Error in User Management page > CheckS1StartedOrLiveMarkingEnabled() method");
                throw;
            }
            return status;
        }

        public async Task<List<BlankQigIds>> GetEmptyQigIds(long projectID)
        {
            List<BlankQigIds> ltemptyQigIds = new List<BlankQigIds>();

            try
            {
                var qigids = context.ProjectQigs.Where(pq => pq.ProjectId == projectID && !pq.IsDeleted && pq.IsManualMarkingRequired).Select(x =>
                    new BlankQigIds()
                    {
                        QigIds = x.ProjectQigid,
                        QigName = x.Qigname
                    }).ToList();

                string[] roleCodes = { "CM", "ACM", "TL", "ATL", "MARKER" };

                var emptyQigIds = (await (from pqt in context.ProjectQigteamHierarchies
                                          join pq in context.ProjectQigs
                                          on pqt.Qigid equals pq.ProjectQigid
                                          join pur in context.ProjectUserRoleinfos
                                          on pqt.ProjectUserRoleId equals pur.ProjectUserRoleId
                                          join ri in context.Roleinfos
                                          on pur.RoleId equals ri.RoleId
                                          where roleCodes.Contains(ri.RoleCode) && pq.ProjectId == projectID && pq.IsManualMarkingRequired
                                          && !pqt.Isdeleted && !pq.IsDeleted && !pur.Isdeleted && !ri.Isdeleted

                                          select new BlankQigIds()
                                          {
                                              QigIds = pqt.Qigid,
                                              QigName = pq.Qigname
                                          }).Distinct().ToListAsync()).ToList();

                var qids = emptyQigIds.Select(q => q.QigIds).ToList();

                ltemptyQigIds = qigids.Where(q => !qids.Contains(q.QigIds)).ToList();

                logger.LogDebug($"ProjectUsersRepository > GetBlankQigIds() method completed {projectID}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page > GetBlankQigIds() method {projectID}");
                throw;
            }

            return ltemptyQigIds;
        }

        public async Task<string> MoveMarkingTeamToEmptyQigs(MoveMarkingTeamToEmptyQigs moveMarkingTeamToEmptyQig)
        {
            string status = "ERR001";
            try
            {
                logger.LogDebug($"ProjectUsersRepository > MoveMarkingTeamToEmptyQig() method started {moveMarkingTeamToEmptyQig}");

                if (moveMarkingTeamToEmptyQig.ToQigId.Count > 0)
                {
                    string ToQigIds = "";
                    foreach (var qigId in moveMarkingTeamToEmptyQig.ToQigId)
                    {
                        ToQigIds += qigId + ",";
                    }

                    ToQigIds = ToQigIds.Remove(ToQigIds.Length - 1, 1);

                    await using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                    {
                        using (SqlCommand sqlCmd = new SqlCommand("marking.USPMoveQIGHierarchy", sqlCon))
                        {
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            sqlCmd.Parameters.Add("@ProjectID ", SqlDbType.BigInt).Value = moveMarkingTeamToEmptyQig.ProjectID;
                            sqlCmd.Parameters.Add("@FromQIG", SqlDbType.BigInt).Value = moveMarkingTeamToEmptyQig.FromQigId;
                            sqlCmd.Parameters.Add("@ToQIGs", SqlDbType.NVarChar, 1000).Value = ToQigIds;
                            sqlCmd.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value = moveMarkingTeamToEmptyQig.ProjectUserRoleId;
                            sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;
                            sqlCon.Open();
                            sqlCmd.ExecuteNonQuery();
                            sqlCon.Close();
                            status = sqlCmd.Parameters["@Status"].Value.ToString();
                        }
                    }
                }

                logger.LogDebug($"ProjectUsersRepository > MoveMarkingTeamToEmptyQig() method completed {moveMarkingTeamToEmptyQig}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page > MoveMarkingTeamToEmptyQig() method {moveMarkingTeamToEmptyQig}");
                throw;
            }
            return status;
        }

        public async Task<string> UnBlockUsers(long ProjectUserRoleID, long UserId)
        {
            string status = "E001";
            UserInfo objUserInfo;
            try
            {
                objUserInfo = await context.UserInfos.Where(z => z.UserId == UserId && z.IsBlock && !z.IsDeleted).FirstOrDefaultAsync();
                if (objUserInfo != null)
                {
                    objUserInfo.IsBlock = false;
                    objUserInfo.ForgotPasswordCount = 0;
                    objUserInfo.IsActive = true;
                    objUserInfo.IsApprove = true;
                    objUserInfo.ModifiedBy = ProjectUserRoleID;
                    objUserInfo.ModifiedDate = DateTime.UtcNow;
                    context.UserInfos.Update(objUserInfo);
                    context.SaveChanges();
                    status = "U001";
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while Unblocking users:Method Name:UnBlockUsers()");
                throw;
            }
            return status;
        }

        /// <summary>
        /// MappedUsers : The below MappedUsers API is used to Get Project User Details.
        /// </summary>
        /// <param name="searchFilterModel"></param>
        /// <param name="ProjectID"></param>
        /// <param name="currentloginrolecode"></param>
        /// <param name="TimeZone"></param>
        /// <returns></returns>
        public async Task<MappedUsersList> MappedUsers(SearchFilterModel searchFilterModel, long ProjectID, string currentloginrolecode, Domain.ViewModels.UserTimeZone TimeZone)
        {
            MappedUsersList mappedUsersList = new();
            List<GetAllMappedUsersModel> allmappedlist = new();
            List<MappedUserscount> mappedusercount = new();
            List<GetAllMappedUsersModel> result = new();
            List<GetAllMappedUsersModel> currentuserId = new();

            currentuserId = (from puri in context.ProjectUserRoleinfos
                             join ri in context.Roleinfos on puri.RoleId equals ri.RoleId
                             where puri.ProjectId == ProjectID && !puri.Isdeleted && !ri.Isdeleted && puri.IsActive == true

                             select new GetAllMappedUsersModel
                             {
                                 CurrentuserroleID = puri.UserId,
                                 RoleName = ri.RoleName
                             }).ToList();
            var det = currentuserId.Select(x => x.CurrentuserroleID).ToList();

            if (currentuserId == null)

            {
                result = (from ui in context.UserInfos
                          join urm in context.UserToRoleMappings on ui.UserId equals urm.UserId
                          join ri in context.Roleinfos on urm.RoleId equals ri.RoleId
                          where !ui.IsDeleted && !urm.IsDeleted && !ri.Isdeleted && ui.IsActive == true && !ui.IsBlock
                          && ri.RoleCode == "AO"
                          select new GetAllMappedUsersModel
                          {
                              Name = ui.FirstName,
                              LoginName = ui.LoginId,
                              RoleCode = ri.RoleCode,
                              RoleName = ri.RoleName,
                              UserId = ui.UserId
                          }
                              ).ToList();
            }
            else
            {
                result = (from ui in context.UserInfos
                          join urm in context.UserToRoleMappings on ui.UserId equals urm.UserId
                          join ri in context.Roleinfos on urm.RoleId equals ri.RoleId
                          where !ui.IsDeleted && !urm.IsDeleted && !ri.Isdeleted && ui.IsActive == true && !ui.IsBlock
                          && ri.RoleCode == "AO"
                          select new GetAllMappedUsersModel
                          {
                              Name = ui.FirstName,
                              LoginName = ui.LoginId,
                              RoleCode = ri.RoleCode,
                              UserId = ui.UserId
                          }).ToList();

                result = result.Where(x => !det.Contains(x.UserId)).Distinct().ToList();
            }
            try
            {
                await using SqlConnection connection = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCommand = new SqlCommand("[Marking].[USPGetProjectUserDetails]", connection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectID;
                sqlCommand.Parameters.Add("@SearchText", SqlDbType.NVarChar, 250).Value = searchFilterModel.SearchText;
                sqlCommand.Parameters.Add("@PageNo", SqlDbType.Int).Value = searchFilterModel.PageNo;
                sqlCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = searchFilterModel.PageSize;
                sqlCommand.Parameters.Add("@SortOrder", SqlDbType.TinyInt).Value = searchFilterModel.SortOrder;
                sqlCommand.Parameters.Add("@SortField", SqlDbType.NVarChar).Value = searchFilterModel.SortField;
                sqlCommand.Parameters.Add("@RoleCode", SqlDbType.NVarChar, 100).Value = searchFilterModel.RoleCode;
                sqlCommand.Parameters.Add("@UnmappedUser", SqlDbType.Bit).Value = searchFilterModel.UnmappedUserCountbit;

                connection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader != null && sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        mappedusercount.Add(new MappedUserscount()
                        {
                            RoleCounts = sqlDataReader["RoleCounts"] is DBNull ? 0 : (int)sqlDataReader["RoleCounts"],
                            RoleName = sqlDataReader["Role"] is DBNull ? null : (string)sqlDataReader["RoleCode"],
                            RoleCode = sqlDataReader["RoleCode"] is DBNull ? null : (string)sqlDataReader["Role"]
                        });
                    }

                    // this advances to the next resultset
                    sqlDataReader.NextResult();

                    while (sqlDataReader.Read())
                    {
                        allmappedlist.Add(new GetAllMappedUsersModel()
                        {
                            ProjectuserroleID = sqlDataReader["ProjectuserroleID"] is DBNull ? 0 : (long)sqlDataReader["ProjectuserroleID"],
                            Name = sqlDataReader["UserName"] is DBNull ? null : (string)sqlDataReader["UserName"],
                            LoginName = sqlDataReader["LoginName"] is DBNull ? null : (string)sqlDataReader["LoginName"],
                            RoleName = sqlDataReader["Role"] is DBNull ? null : (string)sqlDataReader["RoleCode"],
                            RoleCode = sqlDataReader["RoleCode"] is DBNull ? null : (string)sqlDataReader["Role"],
                            SchooolName = sqlDataReader["SchoolName"] is DBNull ? null : (string)sqlDataReader["SchoolName"],
                            NRIC = sqlDataReader["NRIC"] is DBNull ? null : (string)sqlDataReader["NRIC"],
                            StartDate = sqlDataReader["AppointmentStartDate"] is DBNull ? null : ((DateTime)sqlDataReader["AppointmentStartDate"]).UtcToProfileDateTime(TimeZone),
                            EndDate = sqlDataReader["AppointmentEndDate"] is DBNull ? null : ((DateTime)sqlDataReader["AppointmentEndDate"]).UtcToProfileDateTime(TimeZone),
                            IsActive = sqlDataReader["IsActive"] is DBNull ? false : (Boolean)sqlDataReader["IsActive"],
                            UserId = sqlDataReader["UserID"] is DBNull ? 0 : (long)sqlDataReader["UserID"],
                            IsS1Enabled = sqlDataReader["IsS1Enabled"] is DBNull ? false : (Boolean)sqlDataReader["IsS1Enabled"],
                            IsLiveMarkingEnabled = sqlDataReader["IsLiveMarkingEnabled"] is DBNull ? false : (Boolean)sqlDataReader["IsLiveMarkingEnabled"]
                        });
                    }

                    //this advances to next unmappedusercounts
                    sqlDataReader.NextResult();

                    if (sqlDataReader.Read())
                    {
                        mappedUsersList.UnMappedUserscount = sqlDataReader["UnMappedUserCount"] is DBNull ? 0 : (int)sqlDataReader["UnMappedUserCount"];
                    }

                    //this  advances to next FilterUserCount
                    sqlDataReader.NextResult();

                    if (sqlDataReader.Read())
                    {
                        mappedUsersList.FilterUserCount = sqlDataReader["FilterUserCount"] is DBNull ? 0 : (int)sqlDataReader["FilterUserCount"];
                    }

                    if (!sqlDataReader.IsClosed) { sqlDataReader.Close(); }
                    mappedUsersList.mappedusercount = mappedusercount;
                    mappedUsersList.allmappeduserlist = allmappedlist;
                    mappedUsersList.currentloginrolecode = currentloginrolecode;
                    mappedUsersList.OnlyAOresult = result;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while getting All users list");
                throw;
            }
            return mappedUsersList;
        }

        /// <summary>
        /// UnMappedUsers : The below UnMappedUsers API is used to Get Unmapped User Details in a Project.
        /// </summary>
        /// <param name="searchFilterModel"></param>
        /// <param name="currentloginrolecode"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public async Task<UNMappedUsersList> UnMappedUsers(SearchFilterModel searchFilterModel, string currentloginrolecode, long ProjectID)
        {
            UNMappedUsersList unmappedUsersList = new();
            List<GetAllUnMappedUsersModel> allunmappedlist = new();
            MappedUserscount unmappedusercount = new();
            Boolean AOExist = false;
            try
            {
                await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmmd = new("[Marking].[USPGetUnmappedUserDetails]", sqlCon);
                sqlCmmd.CommandType = CommandType.StoredProcedure;
                sqlCmmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectID;
                sqlCmmd.Parameters.Add("@SearchText", SqlDbType.NVarChar, 250).Value = searchFilterModel.SearchText;
                sqlCmmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = searchFilterModel.PageNo;
                sqlCmmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = searchFilterModel.PageSize;
                sqlCmmd.Parameters.Add("@SortOrder", SqlDbType.TinyInt).Value = searchFilterModel.SortOrder;
                sqlCmmd.Parameters.Add("@SortField", SqlDbType.NVarChar).Value = searchFilterModel.SortField;
                sqlCon.Open();

                SqlDataReader sqlDataReader = sqlCmmd.ExecuteReader();
                if (sqlDataReader != null && sqlDataReader.HasRows)
                {
                    GetUnMappedUserData(sqlDataReader, allunmappedlist);
                    
                    // this advances to the next resultset
                    sqlDataReader.NextResult();

                    while (sqlDataReader.Read())
                    {
                        unmappedusercount.RoleCounts = sqlDataReader["TotalUsers"] is DBNull ? 0 : (int)sqlDataReader["TotalUsers"];
                        unmappedusercount.FilterUsersCount = sqlDataReader["FilterUsersCount"] is DBNull ? 0 : (int)sqlDataReader["FilterUsersCount"];
                        AOExist = sqlDataReader["AOExists"] is DBNull ? false : (Boolean)sqlDataReader["AOExists"];
                    }

                    unmappedUsersList.unmappedusercount = unmappedusercount;
                    unmappedUsersList.allunmappeduserlist = allunmappedlist;
                    unmappedUsersList.currentloginrolecode = currentloginrolecode;
                    unmappedUsersList.AoCount = AOExist;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while Upmapping the user from a project.:Method Name:UnMappedUsers()");
                throw;
            }
            return unmappedUsersList;
        }
        private void GetUnMappedUserData(SqlDataReader sqlDataReader, List<GetAllUnMappedUsersModel> allunmappedlist)
        {
            while (sqlDataReader.Read())
            {
                allunmappedlist.Add(new GetAllUnMappedUsersModel()
                {
                    UserId = sqlDataReader["UserID"] is DBNull ? 0 : (Int64)sqlDataReader["UserID"],
                    Name = sqlDataReader["UserName"] is DBNull ? null : (string)sqlDataReader["UserName"],
                    LoginName = sqlDataReader["LoginName"] is DBNull ? null : (string)sqlDataReader["LoginName"],
                    RoleName = sqlDataReader["Role"] is DBNull ? null : (string)sqlDataReader["Role"],
                    RoleCode = sqlDataReader["RoleCode"] is DBNull ? null : (string)sqlDataReader["RoleCode"],
                    SchooolName = sqlDataReader["SchoolName"].ToString(),
                    NRIC = sqlDataReader["NRIC"] is DBNull ? null : (string)sqlDataReader["NRIC"],
                });
            }
        }

        /// <summary>
        /// GetSelectedMappedUsers : The below API is used to Edit the selected Users details.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<GetAllMappedUsersModel> GetSelectedMappedUsers(long UserId)
        {
            GetAllMappedUsersModel result = null;
            try
            {
                await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmmd = new("[Marking].[USPGetMappedProjectUserDetails]", sqlCon);
                sqlCmmd.CommandType = CommandType.StoredProcedure;
                sqlCmmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = UserId;
                sqlCon.Open();
                SqlDataReader reader = sqlCmmd.ExecuteReader();
                if (reader.HasRows)
                {
                    GetSelectedMapUserData(reader, result);
                }

                ConnectionClose(reader, sqlCon);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while Upmapping the user from a project.:Method Name:UnMappedUsers()");
                throw;
            }
            return result;
        }
        private void GetSelectedMapUserData(SqlDataReader reader, GetAllMappedUsersModel result)
        {
            result = new GetAllMappedUsersModel();
            while (reader.Read())
            {
                GetAllMappedUsersModel objuserinfo = new()
                {
                    Name = reader["FirstName"] is DBNull ? null : Convert.ToString(reader["FirstName"]),
                    LoginName = reader["LoginID"] is DBNull ? null : Convert.ToString(reader["LoginID"]),
                    RoleName = reader["RoleName"] is DBNull ? null : Convert.ToString(reader["RoleName"]),
                    NRIC = reader["NRIC"] is DBNull ? null : Convert.ToString(reader["NRIC"]),
                    SchooolName = reader["SchoolName"] is DBNull ? null : Convert.ToString(reader["SchoolName"]),
                    Phone = reader["PhoneNumber"] is DBNull ? null : Convert.ToString(reader["PhoneNumber"]),
                    StartDate = reader["AppointStartDate"] is DBNull ? null : (DateTime)reader["AppointStartDate"],
                    EndDate = reader["AppointEndDate"] is DBNull ? null : (DateTime)reader["AppointEndDate"]
                };
                result = objuserinfo;
            }
        }

        /// <summary>
        /// SaveUnMappedUsers : The below API is used to Map the Available Users for a particular project.
        /// </summary>
        /// <param name="mappedUsersModel"></param>
        /// <param name="ProjectID"></param>
        /// <param name="currentuserid"></param>
        /// <returns></returns>
        public Task<string> SaveUnMappedUsers(MappedUsersModel mappedUsersModel, long ProjectID, long currentuserid)
        {
            string status = string.Empty;
            try
            {
                DataTable dt = ToDataTable(mappedUsersModel.UserID);

                using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[Marking].[USPInsertProjectUsersDetails]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@UDTInsertUserInfo", SqlDbType.Structured).Value = dt;
                        sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectID;
                        sqlCmd.Parameters.Add("@CreatedByUserID", SqlDbType.BigInt).Value = currentuserid;
                        sqlCmd.Parameters.Add("@RoleCode", SqlDbType.NVarChar).Value = mappedUsersModel.RoleCode;
                        sqlCmd.Parameters.Add("@AppointmentStartDate", SqlDbType.DateTime).Value = mappedUsersModel.AppointmentStartDate;
                        sqlCmd.Parameters.Add("@AppointmentEndDate", SqlDbType.DateTime).Value = mappedUsersModel.AppointmentEndDate;
                        sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;
                        sqlCon.Open();
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        status = sqlCmd.Parameters["@Status"].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while Saving UnMapped Users for a project.:Method Name:SaveUnMappedUsers()");
                throw;
            }
            return Task.FromResult(status);
        }

        /// <summary>
        /// ToDataTable : The below DataTable API is used to store one or more selected users from Available users list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        /// <summary>
        /// SaveUnMappedAO : The below API is used to Map only the AO to a particular from Available Users List.
        /// </summary>
        /// <param name="mappedUsersModel"></param>
        /// <param name="ProjectID"></param>
        /// <param name="currentuserid"></param>
        /// <returns></returns>
        public async Task<string> SaveUnMappedAO(MappedUsersModel mappedUsersModel, long ProjectID, long currentuserid)
        {
            string status = string.Empty;
            try
            {
                DataTable dt = ToDataTable(mappedUsersModel.UserID);
                var IsAOExist = (await (from pur in context.ProjectUserRoleinfos
                                        join pi in context.Roleinfos on pur.RoleId equals pi.RoleId
                                        where pur.ProjectId == ProjectID && !pur.Isdeleted && !pi.Isdeleted && pi.RoleCode == "AO"
                                        select pur).ToListAsync()).ToList();

                if (IsAOExist.Count == 0)
                {
                    using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                    {
                        using (SqlCommand sqlCmd = new SqlCommand("[Marking].[USPInsertProjectUsersDetails]", sqlCon))
                        {
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            sqlCmd.Parameters.Add("@UDTInsertUserInfo", SqlDbType.Structured).Value = dt;
                            sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectID;
                            sqlCmd.Parameters.Add("@RoleCode", SqlDbType.NVarChar).Value = mappedUsersModel.RoleCode;
                            sqlCmd.Parameters.Add("@CreatedByUserID", SqlDbType.BigInt).Value = currentuserid;
                            sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;
                            sqlCon.Open();
                            sqlCmd.ExecuteNonQuery();
                            sqlCon.Close();
                            status = sqlCmd.Parameters["@Status"].Value.ToString();
                        }
                    }
                }
                else
                {
                    status = "AOEXT001";
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while Saving UnMapped Users for a project.:Method Name:SaveUnMappedAO()");
                throw;
            }
            return status;
        }

        /// <summary>
        /// UnMappingParticularUsers : These API is used to UnMap the particular users from the current project.
        /// </summary>
        /// <param name="UnmapAodetail"></param>
        /// <param name="GetCurrentProjectId"></param>
        /// <param name="GetCurrentProjectUserRoleID"></param>
        /// <returns></returns>
        public Task<string> UnMappingParticularUsers(UnMapAodetails UnmapAodetail, long GetCurrentProjectId, long GetCurrentProjectUserRoleID)
        {
            string status = string.Empty;
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[Marking].[USPUnmapProjectUser]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@UnmapProjectUserRoleID", SqlDbType.BigInt).Value = UnmapAodetail.UnmapProjectUserRoleID;
                        sqlCmd.Parameters.Add("@ReplacementUserID", SqlDbType.BigInt).Value = UnmapAodetail.ReplacementUserID;
                        sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = GetCurrentProjectId;
                        sqlCmd.Parameters.Add("@CurrentProjectUSerRoleID", SqlDbType.BigInt).Value = GetCurrentProjectUserRoleID;
                        sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;
                        sqlCon.Open();
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        status = sqlCmd.Parameters["@Status"].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while Unmapping Users from the project.:Method Name:UnMappingParticularUsers()");
                throw;
            }
            return Task.FromResult(status);
        }

        /// <summary>
        /// GetRoles : It's used to get the List of Available Roles.
        /// </summary>
        /// <returns></returns>
        public Task<List<Roleinfo>> GetRoles()
        {
            List<Roleinfo> result = new List<Roleinfo>();
            try
            {
                var mrpl = context.Roleinfos.Where(z => z.RoleCode == "MARKINGPERSONNEL").Select(z => z.RoleId).FirstOrDefault();

                result = (from ri in context.Roleinfos
                          join rl in context.RoleLevels
                          on ri.RoleLevelId equals rl.RoleLevelId
                          where ri.ParentRoleId == mrpl && !ri.Isdeleted && !rl.IsDeleted
                          orderby rl.Order
                          select ri).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while Saving UnMapped Users for a project.:Method Name:SaveUnMappedUsers()");
                throw;
            }
            return Task.FromResult(result);
        }

        public Task<string> BlockorUnblockUserQig(long QigId, long currentprojectuserroleid, long UserRoleId, bool Isactive)
        {
            string status = "E001";
            ProjectQigteamHierarchy projectqigteamHierarchy;
            try
            {
                projectqigteamHierarchy = context.ProjectQigteamHierarchies.Where(z => z.ProjectUserRoleId == UserRoleId && z.Qigid == QigId && !z.Isdeleted).FirstOrDefault();
                if (projectqigteamHierarchy != null)
                {
                    if (Isactive)
                    {
                        projectqigteamHierarchy.IsActive = false;
                        projectqigteamHierarchy.ModifiedDate = DateTime.UtcNow;
                        projectqigteamHierarchy.ModifiedBy = currentprojectuserroleid;

                        context.ProjectQigteamHierarchies.Update(projectqigteamHierarchy);
                        context.SaveChanges();
                        status = "BLK001";
                    }
                    else
                    {
                        projectqigteamHierarchy.IsActive = true;
                        projectqigteamHierarchy.ModifiedDate = DateTime.UtcNow;
                        projectqigteamHierarchy.ModifiedBy = currentprojectuserroleid;

                        context.ProjectQigteamHierarchies.Update(projectqigteamHierarchy);
                        context.SaveChanges();
                        status = "BLK002";
                    }
                }
                else
                {
                    status = "ERR01";
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while Unblocking users:Method Name:UnBlockUsers()");
                throw;
            }
            return Task.FromResult(status);
        }

        /// <summary>
        /// GetWithDrawUsers : This API is used to get the users.
        /// </summary>
        /// <param name="objectSearch"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public Task<TotalUserWithdrawModel> GetWithDrawUsers(long ProjectID, SearchFilterModel objectSearch)
        {
            logger.LogInformation($"UserMangementRepository GetWithDrawUsers() Method started.  ProjectID = {ProjectID}");

            TotalUserWithdrawModel total = new TotalUserWithdrawModel();

            List<UserWithdrawModel> result = null;
            try
            {
                var ProjMoaId = context.ProjectInfos.Where(x => x.ProjectId == ProjectID && !x.IsDeleted).Select(s => s.Moa).FirstOrDefault();
                var MasterMoaId = context.ModeOfAssessments.Where(w => w.Moacode.ToLower().Contains("oral") && !w.IsDeleted).Select(m => m.Moaid).FirstOrDefault();

                total.IseOral = ProjMoaId > 0 && MasterMoaId > 0 && ProjMoaId == MasterMoaId;

                using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmmd = new("[Marking].[USPGetProjectScheduleUserDetails]", sqlCon);
                sqlCmmd.CommandType = CommandType.StoredProcedure;
                sqlCmmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectID;
                sqlCmmd.Parameters.Add("@PageNo", SqlDbType.BigInt).Value = objectSearch.PageNo;
                sqlCmmd.Parameters.Add("@PageSize", SqlDbType.BigInt).Value = objectSearch.PageSize;
                sqlCmmd.Parameters.Add("@SearchText", SqlDbType.NVarChar).Value = objectSearch.SearchText;
                sqlCmmd.Parameters.Add("@Status", SqlDbType.NVarChar).Value = objectSearch.Status;
                sqlCon.Open();
                SqlDataReader reader = sqlCmmd.ExecuteReader();
                if (reader != null && reader.HasRows)
                {
                    result = new List<UserWithdrawModel>();
                    while (reader.Read())
                    {
                        UserWithdrawModel objuserinfo = new UserWithdrawModel();
                        objuserinfo.LoginName = Convert.ToString(reader["LoginName"]);
                        if (!total.IseOral)
                        {
                            objuserinfo.ID = Convert.ToInt64(reader["ScheduleUserId"]);
                        }
                        objuserinfo.ProjectID = Convert.ToInt64(reader["ProjectID"]);
                        objuserinfo.Status = Convert.ToByte(reader["Status"]);
                        objuserinfo.IsWithDrawn = Convert.ToBoolean(reader["IsWithdrawn"]);
                        objuserinfo.RowCount = Convert.ToInt64(reader["Total_Rows"]);
                        total.UserWithdraw.Add(objuserinfo);
                    }
                }

                reader.NextResult();

                while (reader.Read())
                {
                    total.TotalUserCount = (int)reader["TotalUserCount"];
                    total.TotalWithdrawnCount = (int)reader["TotalWithdrawnCount"];
                }

                ConnectionClose(reader, sqlCon);

                logger.LogInformation($"UserMangementRepository GetWithDrawUsers() Method ended.   ProjectID = {ProjectID}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while GetUserWithdraw the user from a project.:Method Name:GetUserWithdraw()");
                throw;
            }
            return Task.FromResult(total);
        }

        /// <summary>
        ///  WithDrawUsers : This POST Api is used to Withdraw the users.
        /// </summary>
        /// <param name="ObjectUserWithdraw"></param>
        /// <param name="WithDrawBy"></param>
        /// <returns></returns>
        public Task<string> WithDrawUsers(List<UserWithdrawModel> ObjectUserWithdraw, long WithDrawBy)
        {
            logger.LogInformation($"UserMangementRepository WithDrawUsers() Method started.  WithDrawBy = {WithDrawBy}");
            string status = "";

            try
            {
                foreach (var user in ObjectUserWithdraw)
                {
                    ProjectCandidateWithdraw WithdrawUsersList = new ProjectCandidateWithdraw();
                    WithdrawUsersList.ProjectId = user.ProjectID;
                    WithdrawUsersList.IndexNumber = user.LoginName;
                    WithdrawUsersList.ScheduleUserId = user.ID;
                    WithdrawUsersList.WithDrawBy = WithDrawBy;
                    WithdrawUsersList.WithDrawDate = DateTime.Now;
                    context.Add(WithdrawUsersList);
                    context.SaveChanges();
                    status = "Success";
                }
                logger.LogInformation($"UserMangementRepository WithDrawUsers() Method ended.   WithDrawBy = {WithDrawBy}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while Withdrawn the user from a project.:Method Name:WithDrawUsers()");
                throw;
            }
            return Task.FromResult(status);
        }

        public Task<Boolean> GetSuspendUserDetails(long currentprojectuserroleid, int SuspendOrUnmap)
        {
            Boolean status = false;
            try
            {
                using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmmd = new("[Marking].[USPvalidateProjectUserScripts]", sqlCon);
                sqlCmmd.CommandType = CommandType.StoredProcedure;
                sqlCmmd.Parameters.Add("@projectUserRoleID", SqlDbType.BigInt).Value = currentprojectuserroleid;
                sqlCmmd.Parameters.Add("@SuspendOrUnmap", SqlDbType.BigInt).Value = SuspendOrUnmap;
                sqlCon.Open();
                SqlDataReader reader = sqlCmmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    status = Convert.ToBoolean(reader["IsScriptExists"]);
                }

                if (!reader.IsClosed)
                {
                    reader.Close();
                }
                if (sqlCon.State != ConnectionState.Closed)
                {
                    sqlCon.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while Saving UnMapped Users for a project.:Method Name:SaveUnMappedUsers()");
                throw;
            }
            return Task.FromResult(status);
        }

        public Task<string> UnTagQigUser(SuspendUserDetails suspendUserDetails, long ProjectUserRoleId)
        {
            string status = string.Empty;
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[Marking].[USPUntagProjectUser]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@UnmapProjectUserID", SqlDbType.BigInt).Value = suspendUserDetails.unmapProjectUserId;
                        sqlCmd.Parameters.Add("@ModifiedByProjectUserID", SqlDbType.BigInt).Value = ProjectUserRoleId;
                        sqlCmd.Parameters.Add("@ReplacementPURID", SqlDbType.BigInt).Value = suspendUserDetails.replacementPURId;
                        sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = suspendUserDetails.ProjectId;
                        sqlCmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = suspendUserDetails.QigId;
                        sqlCmd.Parameters.Add("@ReportingTo", SqlDbType.BigInt).Value = suspendUserDetails.ReportingTo;
                        sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 5).Direction = ParameterDirection.Output;
                        sqlCon.Open();
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        status = sqlCmd.Parameters["@Status"].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while un tagging Qig Users.:Method Name:UnTagQigUser()");
                throw;
            }
            return Task.FromResult(status);
        }

        public Task<List<SuspendUserDetails>> GetUpperHierarchyRole(long RoleId, long QigId, long ProjectId)
        {
            logger.LogInformation($"UserMangementRepository GetUpperHierarchyRole() Method started.  RoleId = {RoleId}");
            List<SuspendUserDetails> result = null;
            try
            {
                using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmmd = new("[Marking].[USPGetRoleAndUserInfo]", sqlCon);
                sqlCmmd.CommandType = CommandType.StoredProcedure;
                sqlCmmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = RoleId;
                sqlCmmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
                sqlCmmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = QigId;
                sqlCon.Open();
                SqlDataReader reader = sqlCmmd.ExecuteReader();
                if (reader.HasRows)
                {
                    GetUpperHierarchyData(reader, result);
                }

                ConnectionClose(reader, sqlCon);

                logger.LogInformation($"UserMangementRepository GetUpperHierarchyRole() Method ended.   RoleId = {RoleId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while GetUpperHierarchyRole the user from a project.:Method Name:GetUpperHierarchyRole()");
                throw;
            }
            return Task.FromResult(result);
        }

        private void GetUpperHierarchyData(SqlDataReader reader, List<SuspendUserDetails> result)
        {
            result = new List<SuspendUserDetails>();
            while (reader.Read())
            {
                SuspendUserDetails objuserinfo = new SuspendUserDetails();

                objuserinfo.roleName = Convert.ToString(reader["RoleName"]);
                objuserinfo.roleCode = Convert.ToString(reader["RoleCode"]);
                objuserinfo.ProjectUserRoleID = Convert.ToInt64(reader["ProjectUserRoleID"]);
                objuserinfo.FirstName = Convert.ToString(reader["FirstName"]);

                result.Add(objuserinfo);
            }
        }

        public Task<string> ReTagQigUser(long RoleId, long QigId, long ReportingTo, string RoleCode, long ProjectId, long ProjectUserRoleId)
        {
            string status = string.Empty;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[Marking].[USPRetagUser]", sqlConnection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReTagProjectUserRoleID", SqlDbType.BigInt).Value = RoleId;
                        cmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
                        cmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = QigId;
                        cmd.Parameters.Add("@ReportingTo", SqlDbType.BigInt).Value = ReportingTo;
                        cmd.Parameters.Add("@ModifiedBy", SqlDbType.BigInt).Value = ProjectUserRoleId;
                        cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 5).Direction = ParameterDirection.Output;
                        sqlConnection.Open();
                        cmd.ExecuteNonQuery();
                        sqlConnection.Close();
                        status = cmd.Parameters["@Status"].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while re tagging Qig Users.:Method Name:ReTagQigUser()");
                throw;
            }
            return Task.FromResult(status);
        }

        public Task<List<SuspendUserDetails>> GetReTagUpperHierarchyRole(long RoleId, long QigId, long ProjectId)
        {
            logger.LogInformation($"UserMangementRepository GetUpperHierarchyRole() Method started.  RoleId = {RoleId}");
            List<SuspendUserDetails> result = null;
            try
            {
                var RoleLeveLID = (from pur in context.ProjectUserRoleinfos
                                   join ri in context.Roleinfos on pur.RoleId equals ri.RoleId
                                   where pur.ProjectUserRoleId == RoleId
                                   select ri.RoleLevelId
                               ).FirstOrDefault();

                result = (from pur in context.ProjectUserRoleinfos
                          join pqt in context.ProjectQigteamHierarchies on pur.ProjectUserRoleId equals pqt.ProjectUserRoleId
                          join ui in context.UserInfos on pur.UserId equals ui.UserId
                          join ri in context.Roleinfos on pur.RoleId equals ri.RoleId
                          join rl in context.RoleLevels on ri.RoleLevelId equals rl.RoleLevelId
                          where pur.ProjectId == ProjectId
                                && pqt.Qigid == QigId
                                && !pqt.Isdeleted
                                && pqt.IsActive == true
                                && !ri.Isdeleted
                                && ri.RoleGroup == 2
                                && rl.RoleLevelId < RoleLeveLID
                                && !ui.IsDeleted
                                && !rl.IsDeleted
                          orderby pur.RoleId ascending
                          select new SuspendUserDetails
                          {
                              ProjectUserRoleID = pur.ProjectUserRoleId,
                              LoginID = ui.LoginId,
                              FirstName = ui.FirstName,
                              roleCode = ri.RoleCode,
                              roleName = ri.RoleName
                          }).ToList();

                logger.LogInformation($"UserMangementRepository GetUpperHierarchyRole() Method ended.   RoleId = {RoleId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while GetUpperHierarchyRole the user from a project.:Method Name:GetUpperHierarchyRole()");
                throw;
            }
            return Task.FromResult(result);
        }

        public Task<List<SuspendUserDetails>> GetReportingToHierarchy(long RoleId, long QigId, long ProjectId)
        {
            logger.LogInformation($"UserMangementRepository GetReportingToHierarchy() Method started.  RoleId = {RoleId}");
            List<SuspendUserDetails> result = null;
            try
            {
                using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmmd = new("[Marking].[USPGetProjectUserReportees]", sqlCon);
                sqlCmmd.CommandType = CommandType.StoredProcedure;
                sqlCmmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = RoleId;
                sqlCmmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
                sqlCmmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = QigId;
                sqlCon.Open();
                SqlDataReader reader = sqlCmmd.ExecuteReader();
                if (reader.HasRows)
                {
                    GetRptHierarchyData(reader, result);
                }

                ConnectionClose(reader, sqlCon);

                logger.LogInformation($"UserMangementRepository GetReportingToHierarchy() Method ended.   RoleId = {RoleId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while GetReportingToHierarchy the user from a project.:Method Name:GetReportingToHierarchy()");
                throw;
            }
            return Task.FromResult(result);
        }

        private void GetRptHierarchyData(SqlDataReader reader, List<SuspendUserDetails> result)
        {
            result = new List<SuspendUserDetails>();
            while (reader.Read())
            {
                SuspendUserDetails objuserinfo = new()
                {
                    roleName = Convert.ToString(reader["RoleName"]),
                    roleCode = Convert.ToString(reader["RoleCode"]),
                    ProjectUserRoleID = Convert.ToInt64(reader["ProjectUserRoleID"]),
                    FirstName = Convert.ToString(reader["FirstName"]),
                    LoginID = Convert.ToString(reader["LoginID"]),
                    CurrentReportingTo = (reader["CurrentReportingTo"] != DBNull.Value) && Convert.ToBoolean(reader["CurrentReportingTo"])
                };
                result.Add(objuserinfo);
            }
        }

        public Task<Boolean> Untaguserhaschildusers(long RoleId, long QigId, long ProjectId)
        {
            logger.LogInformation($"UserMangementRepository Untaguserhaschildusers() Method started.  RoleId = {RoleId}");
            Boolean status = false;
            try
            {
                status = context.ProjectQigteamHierarchies.Any(a => a.ReportingTo == RoleId && a.Qigid == QigId && a.ProjectId == ProjectId && !a.Isdeleted && a.IsActive == true);

                logger.LogInformation($"UserMangementRepository Untaguserhaschildusers() Method ended.   RoleId = {RoleId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while Untaguserhaschildusers the user from a project.:Method Name:Untaguserhaschildusers()");
                throw;
            }
            return Task.FromResult(status);
        }

        public Task<ChangeRoleModel> CheckActivityOfMP(long ProjectUserRoleId, long ProjectId)
        {
            logger.LogInformation($"UserMangementRepository CheckActivityOfMP() Method started.  ProjectUserRoleId = {ProjectUserRoleId}");
            ChangeRoleModel changeRoleModel = new();
            List<SupervisorRoledetails> supervisorRoledetails = new List<SupervisorRoledetails>();
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[Marking].[USPValidateProjectUserActivity]", sqlCon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ProjectUSerRoleID", SqlDbType.BigInt).Value = ProjectUserRoleId;
                        cmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
                        sqlCon.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            reader.Read();
                            changeRoleModel.IsActivityExists = Convert.ToBoolean(reader["IsActivityExists"]);
                        }
                        if (changeRoleModel.IsActivityExists == false)
                        {
                            /// this advances to the next resultset
                            reader.NextResult();

                            while (reader.Read())
                            {
                                changeRoleModel.QIGdetails.Add(new QIGdetails()
                                {
                                    ProjectUserRoleID = (long)reader["ProjectUserRoleID"],
                                    ProjectQIGID = (long)reader["ProjectQIGID"],
                                    QIGCode = reader["QIGCode"] is DBNull ? null : (string)reader["QIGCode"],
                                    ReportingTo = reader["ReportingTo"] is DBNull ? 0 : (long)reader["ReportingTo"]
                                });
                            }

                            /// this advances to the next resultset
                            reader.NextResult();

                            while (reader.Read())
                            {
                                supervisorRoledetails.Add(new SupervisorRoledetails()
                                {
                                    ProjectUserRoleID = (long)reader["ProjectUserRoleID"],
                                    RoleID = (int)reader["RoleID"],
                                    RoleCode = reader["RoleCode"] is DBNull ? null : (string)reader["RoleCode"],
                                    Order = (short)reader["Order"],
                                    FirstName = reader["FirstName"] is DBNull ? null : (string)reader["FirstName"],
                                    LastName = reader["LastName"] is DBNull ? null : (string)reader["LastName"],
                                    QIGID = (long)reader["QIGID"]
                                });
                            }

                            /// this advances to the next resultset
                            reader.NextResult();

                            while (reader.Read())
                            {
                                changeRoleModel.Roledetails.Add(new Roledetails()
                                {
                                    RoleID = (int)reader["RoleID"],
                                    Order = (short)reader["Order"],
                                    RoleLevelID = (Byte)reader["RoleLevelID"],
                                    RoleCode = reader["RoleCode"] is DBNull ? null : (string)reader["RoleCode"]
                                });
                            }
                        }

                        if (supervisorRoledetails.Count > 0 && changeRoleModel.QIGdetails.Count > 0)
                        {
                            foreach (QIGdetails item in changeRoleModel.QIGdetails)
                            {
                                item.SupervisorRoledetails = supervisorRoledetails.Where(a => a.QIGID == item.ProjectQIGID).ToList();
                            }
                        }

                        ConnectionClose(reader, sqlCon);

                    }
                }
                logger.LogInformation($"UserMangementRepository CheckActivityOfMP() Method ended.   ProjectUserRoleId = {ProjectUserRoleId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while checking the Activity of MP.:Method Name:CheckActivityOfMP()");
                throw;
            }
            return Task.FromResult(changeRoleModel);
        }

        ///<summary>
        ///CreateEditProjectUserRoleChange:This SP is used to upgrade/downgrade the ProjectUsers role
        ///</summary>
        ///<returns></returns>
        public async Task<string> CreateEditProjectUserRoleChange(CreateEditProjectUserRoleChange model, long ProjectID, long CurrentUserRoleId)
        {
            string status = string.Empty;
            try
            {
                ///ChangeType = 1;Upgrade, 2= downgrade
                DataTable dt = ToDataTable(model.qigsupervisorroledetails);

                await using SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString);
                await using SqlCommand sqlCmd = new SqlCommand("[Marking].[USPProjectUserRoleChange]", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectID;
                sqlCmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = model.ProjectUserRoleID;
                sqlCmd.Parameters.Add("@RoleCode", SqlDbType.VarChar, 250).Value = model.RoleCode;
                sqlCmd.Parameters.Add("@ChangeType", SqlDbType.Int).Value = model.ChangeType;
                sqlCmd.Parameters.Add("@UDTQIGUserInfo", SqlDbType.Structured).Value = dt;
                sqlCmd.Parameters.Add("@ModifiedByProjectUserID", SqlDbType.BigInt).Value = CurrentUserRoleId;
                sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 5).Direction = ParameterDirection.Output;
                sqlCon.Open();
                sqlCmd.ExecuteNonQuery();
                sqlCon.Close();
                status = sqlCmd.Parameters["@Status"].Value.ToString();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while upgrade/downgrade the ProjectUsers role.:Method Name:CreateEditProjectUserRoleChange()");
                throw;
            }
            return status;
        }

        private void ConnectionClose(SqlDataReader reader, SqlConnection sqlCon)
        {
            if (!reader.IsClosed)
            {
                reader.Close();
            }
            if (sqlCon.State != ConnectionState.Closed)
            {
                sqlCon.Close();
            }
        }
    }
}