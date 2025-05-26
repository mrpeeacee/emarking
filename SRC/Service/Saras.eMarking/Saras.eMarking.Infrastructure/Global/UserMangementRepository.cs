using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.GlobalRepositoryInterfaces;
using Saras.eMarking.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Global
{
    public class UserMangementRepository : BaseRepository<UserMangementRepository>, IUserManagementRepository
    {
        private readonly ApplicationDbContext context;
        private static string AesDecryptkey { get; set; } = "K+iCU4H+AtV4uy0+Skmo8w==";
        public AppOptions AppOptions { get; set; }

        public UserMangementRepository(ApplicationDbContext context, ILogger<UserMangementRepository> _logger, AppOptions appOptions) : base(_logger)
        {
            this.context = context;
            AppOptions = appOptions;
        }

        /// <summary>
        /// GetAllUsers:this api is to Get All users at global level
        /// </summary>
        /// <returns></returns>
        public async Task<GetAllApplicationUsersModel> GetAllUsers(SearchFilterModel searchFilterModel, long CuurentUserId)
        {
            GetAllApplicationUsersModel Alluserslst = new();

            try
            {
                if (searchFilterModel.Status == "")
                {
                    searchFilterModel.Status = "0";
                }

                await using SqlConnection connection = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCommand = new SqlCommand("[Marking].[USPGetUserDetails]", connection);
                sqlCommand.Parameters.Add("@SearchText", SqlDbType.NVarChar, 250).Value = searchFilterModel.SearchText;
                sqlCommand.Parameters.Add("@Role", SqlDbType.NVarChar, 250).Value = searchFilterModel.RoleCode;
                sqlCommand.Parameters.Add("@School", SqlDbType.NVarChar, 250).Value = searchFilterModel.SchoolCode;

                sqlCommand.Parameters.Add("@Status", SqlDbType.NVarChar).Value = searchFilterModel.Status;
                sqlCommand.Parameters.Add("@SortOrder ", SqlDbType.TinyInt).Value = searchFilterModel.SortOrder;
                sqlCommand.Parameters.Add("@SortField ", SqlDbType.NVarChar).Value = searchFilterModel.SortField;

                sqlCommand.Parameters.Add("@LoginUserID", SqlDbType.BigInt).Value = CuurentUserId;
                sqlCommand.Parameters.Add("@PageNo", SqlDbType.Int).Value = searchFilterModel.PageNo;
                sqlCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = searchFilterModel.PageSize;
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader != null && sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        Alluserslst.userscount.Totalusers = (int)sqlDataReader["TotalUsers"];
                        Alluserslst.userscount.Activeusers = (int)sqlDataReader["ActiveUsers"];
                        Alluserslst.userscount.InActiveusers = (int)sqlDataReader["InactiveUsers"];
                        Alluserslst.userscount.Blockedusers = (int)sqlDataReader["BlockedUsers"];
                        Alluserslst.userscount.ApplicationLevelOfLoginUserID = (int)sqlDataReader["ApplicationLevelOfLoginUserID"];
                    }

                    // this advances to the next resultset
                    sqlDataReader.NextResult();

                    while (sqlDataReader.Read())
                    {
                        Alluserslst.getAllUsersModel.Add(new GetAllUsersModel()
                        {
                            UserId = (long)sqlDataReader["UserID"],
                            ApplicationLevel = (int)sqlDataReader["ApplicationLevel"],
                            Name = sqlDataReader["UserName"] is DBNull ? null : (string)sqlDataReader["UserName"],
                            LoginName = sqlDataReader["LoginName"] is DBNull ? null : (string)sqlDataReader["LoginName"],
                            RoleName = sqlDataReader["Role"] is DBNull ? null : (string)sqlDataReader["Role"],
                            RoleCode = sqlDataReader["RoleCode"] is DBNull ? null : (string)sqlDataReader["RoleCode"],
                            SchooolName = sqlDataReader["SchoolName"] is DBNull ? null : (string)sqlDataReader["SchoolName"],
                            NRIC = sqlDataReader["NRIC"] is DBNull ? null : (string)sqlDataReader["NRIC"],
                            Phone = sqlDataReader["PhoneNumber"] is DBNull ? null : (string)sqlDataReader["PhoneNumber"],
                            isactive = (bool)sqlDataReader["IsActive"],
                            isblocked = (bool)sqlDataReader["IsBlock"],
                            isDisable = (bool)sqlDataReader["IsDisable"],
                            ROWCOUNT = (int)sqlDataReader["ROWCOUNTS"],
                            CuurentloggedinUserId = CuurentUserId
                        });
                    }
                }
                if (sqlDataReader != null && !sqlDataReader.IsClosed) { sqlDataReader.Close(); }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while getting All users list");
                throw;
            }
            return Alluserslst;
        }

        /// <summary>
        /// GetCreateEditUserdetails:this api is to Get Role School and user details
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<GetCreateEditUserModel> GetCreateEditUserdetails(long UserId, long CuurentUserId)
        {
            GetCreateEditUserModel roleSchooldetails = new();

            try
            {
                roleSchooldetails.Examlevels = (await (from exam in context.ExamLevels
                                                       where !exam.IsDeleted
                                                       select new ExamLevels
                                                       {
                                                           ExamLevelID = exam.ExamLevelId,
                                                           ExamLevelName = exam.ExamLevelName,
                                                           ExamLevelCode = exam.ExamLevelCode
                                                       }).ToListAsync()).ToList();

                var selecteduserroleinfo = await (from usertorole in context.UserToRoleMappings
                                                  join roleinfo in context.Roleinfos
                                                  on usertorole.RoleId equals roleinfo.RoleId
                                                  where usertorole.UserId == UserId && !usertorole.IsDeleted && !roleinfo.Isdeleted
                                                  select roleinfo.RoleCode
                                                 ).FirstOrDefaultAsync();

                var appltnrolelvl = await (from user in context.UserInfos
                                           join usertorole in context.UserToRoleMappings
                                           on user.UserId equals usertorole.UserId
                                           join role in context.Roleinfos
                                           on usertorole.RoleId equals role.RoleId
                                           where !user.IsDeleted && !usertorole.IsDeleted && !role.Isdeleted && user.UserId == (UserId == 0 ? CuurentUserId : UserId)
                                           select role.ApplicationLevel).FirstOrDefaultAsync();

                if (UserId == 0)
                {
                    roleSchooldetails = new GetCreateEditUserModel();
                    {
                        roleSchooldetails.roles = (await (from role in context.Roleinfos
                                                          where !role.Isdeleted && role.ParentRoleId == null && (role.ApplicationLevel > appltnrolelvl || (role.ApplicationLevel == appltnrolelvl && role.RoleCode == "SERVICEADMIN"))
                                                          orderby role.ApplicationLevel
                                                          select new RoleDetails
                                                          {
                                                              RoleID = role.RoleId,
                                                              RoleName = role.RoleName,
                                                              RoleCode = role.RoleCode
                                                          }).ToListAsync()).ToList();
                    }

                    roleSchooldetails.schools = (await (from school in context.SchoolInfos
                                                        where school.ProjectId == null && !school.IsDeleted
                                                        select new SchoolDetails
                                                        {
                                                            SchoolID = school.SchoolId,
                                                            SchoolName = school.SchoolName,
                                                            SchoolCode = school.SchoolCode
                                                        }).ToListAsync()).ToList();

                    roleSchooldetails.Examlevels = (await (from exam in context.ExamLevels
                                                           where !exam.IsDeleted
                                                           select new ExamLevels
                                                           {
                                                               ExamLevelID = exam.ExamLevelId,
                                                               ExamLevelName = exam.ExamLevelName,
                                                               ExamLevelCode = exam.ExamLevelCode
                                                           }).ToListAsync()).ToList();
                }
                else
                {
                    // To check weather the project is closed.
                    var result = (from puri in context.ProjectUserRoleinfos
                                  join pri in context.ProjectInfos on puri.ProjectId equals pri.ProjectId
                                  where puri.UserId == UserId && pri.ProjectStatus != 3 && puri.IsActive == true && !puri.Isdeleted && !pri.IsDeleted
                                  select new
                                  {
                                      puri,
                                      pri
                                  }).ToList();
                    bool checkdataexist = result.Any();

                    roleSchooldetails.roles = (await (from role in context.Roleinfos
                                                      where !role.Isdeleted && role.ParentRoleId == null && (role.ApplicationLevel == appltnrolelvl || role.ApplicationLevel == appltnrolelvl - 1)
                                                      orderby role.ApplicationLevel
                                                      select new RoleDetails
                                                      {
                                                          RoleID = role.RoleId,
                                                          RoleName = role.RoleName,
                                                          RoleCode = role.RoleCode,
                                                          ApplicationLevel = role.ApplicationLevel
                                                      }).OrderByDescending(z => z.ApplicationLevel).ToListAsync()).ToList();

                    roleSchooldetails.Examlevels = (await (from exam in context.ExamLevels
                                                           where !exam.IsDeleted
                                                           select new ExamLevels
                                                           {
                                                               ExamLevelID = exam.ExamLevelId,
                                                               ExamLevelName = exam.ExamLevelName,
                                                               ExamLevelCode = exam.ExamLevelCode
                                                           }).ToListAsync()).ToList();

                    await using SqlConnection connection = new(context.Database.GetDbConnection().ConnectionString);
                    using SqlCommand sqlCommand = new SqlCommand("[Marking].[USPGetApplicationUserDetails]", connection);
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserId;
                    connection.Open();
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    if (sqlDataReader != null && sqlDataReader.HasRows)
                    {
                        while (sqlDataReader.Read())
                        {
                            roleSchooldetails.UserId = UserId;
                            roleSchooldetails.Username = sqlDataReader["FirstName"] is DBNull ? null : (string)sqlDataReader["FirstName"];
                            roleSchooldetails.Loginname = sqlDataReader["LoginID"] is DBNull ? null : (string)sqlDataReader["LoginID"];
                            roleSchooldetails.RoleName = sqlDataReader["RoleName"] is DBNull ? null : (string)sqlDataReader["RoleName"];
                            roleSchooldetails.RoleCode = sqlDataReader["RoleCode"] is DBNull ? null : (string)sqlDataReader["RoleCode"];
                            roleSchooldetails.SchoolName = sqlDataReader["SchoolName"] is DBNull ? null : (string)sqlDataReader["SchoolName"];
                            roleSchooldetails.SchooolCode = sqlDataReader["SchoolCode"] is DBNull ? null : (string)sqlDataReader["SchoolCode"];
                            roleSchooldetails.Nric = sqlDataReader["NRIC"] is DBNull ? null : (string)sqlDataReader["NRIC"];
                            roleSchooldetails.PhoneNum = sqlDataReader["PhoneNumber"] is DBNull ? null : (string)sqlDataReader["PhoneNumber"];
                        }

                        // this advances to the next resultset
                        sqlDataReader.NextResult();

                        while (sqlDataReader.Read())
                        {
                            var examlevel = roleSchooldetails.Examlevels.FirstOrDefault(a => a.ExamLevelCode == Convert.ToString(sqlDataReader["ExamLevelCode"]));
                            if (examlevel != null)
                            {
                                examlevel.isselected = true;
                            }
                        }

                        // this advances to the next resultset
                        sqlDataReader.NextResult();

                        while (sqlDataReader.Read())
                        {
                            roleSchooldetails.schools.Add(new SchoolDetails()
                            {
                                SchoolCode = sqlDataReader["SchoolCode"] is DBNull ? null : (string)sqlDataReader["SchoolCode"],
                                SchoolName = sqlDataReader["SchoolName"] is DBNull ? null : (string)sqlDataReader["SchoolName"]
                            });
                        }

                        roleSchooldetails.checkdataexist = checkdataexist;
                        roleSchooldetails.selectedroleinfo = selecteduserroleinfo;
                    }
                    if (sqlDataReader != null && !sqlDataReader.IsClosed) { sqlDataReader.Close(); }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while getting All Roles & School Details:Method Name:GetCreateEditUserdetails()");
                throw;
            }
            return roleSchooldetails;
        }

        ///<summary>
        ///CreateEditUser:this api is to Create and Edit a user
        ///</summary>
        ///<returns></returns>
        public async Task<string> CreateEditUser(CreateEditUser model, long CurrentUserRoleId)
        {
            string status = string.Empty;
            try
            {
                var Defaultpwd = await context.PassPharses.Where(a => a.IsActive == true).Select(a => a.PassPharseCode).FirstOrDefaultAsync();
                Defaultpwd = TokenLibrary.EncryptDecrypt.Hmac.Hashing.GetHash(AppOptions.AppSettings.ChangePasswords.EncryptionKey, string.Concat(Defaultpwd.Trim(), model.Nric.AsSpan(model.Nric.Length - 4)));

                DataTable dt = ToDataTable(model.Examlevels);

                await using SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString);
                await using SqlCommand sqlCmd = new SqlCommand("[Marking].[USPInsertUpdateUsers]", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = model.UserId;
                sqlCmd.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value = CurrentUserRoleId;
                sqlCmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = model.Username;
                sqlCmd.Parameters.Add("@LoginName", SqlDbType.NVarChar, 320).Value = model.Loginname;
                sqlCmd.Parameters.Add("@SendingSchoolCode", SqlDbType.NVarChar, 50).Value = model.SchooolCode;
                sqlCmd.Parameters.Add("@Role", SqlDbType.NVarChar, 100).Value = model.RoleCode;
                sqlCmd.Parameters.Add("@NRIC", SqlDbType.NVarChar, 100).Value = model.Nric;
                sqlCmd.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 50).Value = model.PhoneNum;
                sqlCmd.Parameters.Add("@Password", SqlDbType.NVarChar, 50).Value = Defaultpwd;
                sqlCmd.Parameters.Add("@UDTExamLevelInfo", SqlDbType.Structured).Value = dt;
                sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;
                sqlCon.Open();
                sqlCmd.ExecuteNonQuery();
                sqlCon.Close();
                status = sqlCmd.Parameters["@Status"].Value.ToString();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while Edit user:Method Name:CreateEditUser()");
                throw;
            }
            return status;
        }

        ///<summary>
        ///Resetpwd:this api is to Reset user password
        ///</summary>
        ///<returns></returns>
        public UserContext Resetpwd(long UserId, long CurrentUserRoleId)
        {
            UserContext userContext = null;
            UserInfo objUserInfo;
            UserPwdDetail objUserpswddtls;
            try
            {
                objUserInfo = context.UserInfos.Where(a => a.UserId == UserId && !a.IsDeleted && a.IsActive == true && !a.IsBlock).FirstOrDefault();

                objUserpswddtls = context.UserPwdDetails.Where(a => a.UserId == UserId && a.IsActive == true).FirstOrDefault();

                var Defaultpwd = context.PassPharses.Where(a => a.IsActive == true).Select(a => a.PassPharseCode).FirstOrDefault();
                Defaultpwd = TokenLibrary.EncryptDecrypt.Hmac.Hashing.GetHash(AppOptions.AppSettings.ChangePasswords.EncryptionKey, string.Concat(Defaultpwd.Trim(), objUserInfo.Nric.AsSpan(objUserInfo.Nric.Length - 4)));

                var templateupdate = context.Templates.Where(a => a.TemplateName == "RESETPASSWORD" && a.IsDeleted == false).FirstOrDefault();

                if (objUserInfo != null)
                {
                    objUserInfo.Password = Defaultpwd;
                    objUserInfo.ModifiedDate = DateTime.UtcNow;
                    objUserInfo.ModifiedBy = CurrentUserRoleId;
                    objUserInfo.IsFirstTimeLoggedIn = true;
                    context.UserInfos.Update(objUserInfo);
                    context.SaveChanges();

                    objUserpswddtls.ActivationEnddate = DateTime.UtcNow;
                    objUserpswddtls.IsActive = false;
                    context.UserPwdDetails.Update(objUserpswddtls);
                    context.SaveChanges();

                    UserPwdDetail userPwdDetail = new()
                    {
                        UserId = UserId,
                        Password = Defaultpwd,
                        CreatedDate = DateTime.UtcNow,
                        ActivationStartdate = DateTime.UtcNow,
                        IsActive = true
                    };
                    context.UserPwdDetails.Add(userPwdDetail);

                    context.SaveChanges();

                    TemplateUserMapping templateUsermapping = new TemplateUserMapping()
                    {
                        UserId = UserId,
                        TryOut = templateupdate.NoOfTryOut,
                        TemplateId = templateupdate.TemplateId
                    };
                    context.TemplateUserMappings.Add(templateUsermapping);

                    context.SaveChanges();

                    userContext = new UserContext
                    {
                        MailQueueId = templateUsermapping.Id,
                        Status = "SUCC001"
                    };
                }
                else
                {
                    userContext = new UserContext
                    {
                        Status = "ERR01"
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while Resetting User Pwd:Method Name:Resetpwd()");
                throw;
            }
            return userContext;
        }

        ///<summary>
        ///UserCreations:this api is to Create Bulk Users
        ///</summary>
        ///<returns></returns>
        public async Task<UserCreations> UserCreation(UserCreations userCreations, long ProjectUserRoleId, int type)
        {
            UserCreations uc = new UserCreations();
            try
            {
                var Defaultpwd = await context.PassPharses.Where(a => a.IsActive == true).Select(a => a.PassPharseCode).FirstOrDefaultAsync();
                userCreations.users.ForEach(a =>
                {
                    a.Password = TokenLibrary.EncryptDecrypt.Hmac.Hashing.GetHash(AppOptions.AppSettings.ChangePasswords.EncryptionKey, string.Concat(Defaultpwd.Trim(), a.NRIC.AsSpan(a.NRIC.Length - 4)));
                });

                DataTable dt = ToGetDataTable(userCreations.users);

                using (SqlConnection con = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[Marking].[USPValidateInsertAppUsersDetails]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UDTInsertUserInfo", SqlDbType.Structured).Value = dt;
                        cmd.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value = ProjectUserRoleId;
                        cmd.Parameters.Add("@ValidateOrInsert", SqlDbType.TinyInt).Value = type;

                        if (type == 2)
                        {
                            cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            uc.status = cmd.Parameters["@Status"].Value.ToString();
                        }
                        else if (type == 1)
                        {
                            con.Open();
                            SqlDataReader sqlDataReader = cmd.ExecuteReader();
                            if (sqlDataReader != null && sqlDataReader.HasRows)
                            {
                                List<UserDetails> ltuserDetails = new List<UserDetails>();
                                while (sqlDataReader.Read())
                                {
                                    UserDetails userDetails = new UserDetails();
                                    userDetails.FirstName = sqlDataReader["FirstName"] is DBNull ? null : (string)sqlDataReader["FirstName"];
                                    userDetails.LoginName = sqlDataReader["LoginID"] is DBNull ? null : (string)sqlDataReader["LoginID"];
                                    userDetails.RoleCode = sqlDataReader["Role"] is DBNull ? null : (string)sqlDataReader["Role"];
                                    userDetails.SchoolName = sqlDataReader["SchoolName"] is DBNull ? null : (string)sqlDataReader["SchoolName"];
                                    userDetails.NRIC = sqlDataReader["NRIC"] is DBNull ? null : (string)sqlDataReader["NRIC"];
                                    userDetails.Error = new List<string>();

                                    if (Convert.ToBoolean(sqlDataReader["LoginIDExists"]))
                                        userDetails.Error.Add("Login name already exists");
                                    if (Convert.ToBoolean(sqlDataReader["NRICExists"]))
                                        userDetails.Error.Add("NRIC already Exists");
                                    if (!Convert.ToBoolean(sqlDataReader["IsRoleValid"]))
                                        userDetails.Error.Add("Role does not exist");
                                    if (!Convert.ToBoolean(sqlDataReader["IsSchoolValid"]))
                                        userDetails.Error.Add("School does not exist");

                                    userDetails.Error.RemoveAll(s => string.IsNullOrEmpty(s));
                                    userDetails.Status = !userDetails.Error.Any();
                                    ltuserDetails.Add(userDetails);
                                }

                                uc.status = ltuserDetails.Any(u => !u.Status) ? "FAILED" : "S001";
                                uc.users = ltuserDetails;
                            }
                            if (sqlDataReader != null && !sqlDataReader.IsClosed) { sqlDataReader.Close(); }
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while creating bulk users:Method Name:UserCreation()");
                throw;
            }

            return uc;
        }

        private static DataTable ToGetDataTable(List<UserDetails> userCreations)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("@Name", typeof(string));
            dt.Columns.Add("@LoginName", typeof(string));
            dt.Columns.Add("@Password", typeof(string));
            dt.Columns.Add("@NRIC", typeof(string));
            dt.Columns.Add("Role", typeof(string));
            dt.Columns.Add("SchoolCode", typeof(string));
            dt.Columns.Add("SchoolName", typeof(string));
            dt.Columns.Add("PhoneNumber", typeof(string));
            if (userCreations.Count > 0)
            {
                userCreations.ForEach(uc =>
                {
                    dt.Rows.Add(uc.FirstName, uc.LoginName, uc.Password, uc.NRIC, uc.RoleCode, uc.SchoolCode, uc.SchoolName, "");
                });
            }

            return dt;
        }

        private static DataTable ToDataTable(List<ExamLevels> examLevels)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("@ExamLevelCode", typeof(string));
            if (examLevels.Count > 0)
            {
                examLevels.ForEach(el =>
                {
                    dt.Rows.Add(el.ExamLevelCode);
                });
            }

            return dt;
        }

        /// <summary>
        /// GetBlockedUsers:this api is to get used to GetBlockedUsers
        /// </summary>
        /// <returns></returns>
        public async Task<List<GetAllUsersModel>> GetBlockedUsers()
        {
            logger.LogInformation($"UserMangementRepository GetBlockedUsers() Method started. ");

            List<GetAllUsersModel> Alluserslst = new();
            try
            {
                Alluserslst = (await (from UI in context.UserInfos
                                      join UTR in context.UserToRoleMappings
                                      on UI.UserId equals UTR.UserId
                                      join RI in context.Roleinfos
                                      on UTR.RoleId equals RI.RoleId
                                      join school in context.SchoolInfos on UI.SchoolId equals school.SchoolId into school
                                      from SI in school.DefaultIfEmpty()
                                      where !UI.IsDeleted && UI.IsBlock && !UTR.IsDeleted && !RI.Isdeleted
                                      select new GetAllUsersModel
                                      {
                                          UserId = UI.UserId,
                                          Name = UI.FirstName + "" + UI.LastName,
                                          LoginName = UI.LoginId,
                                          RoleCode = RI.RoleCode,
                                          SchooolName = SI.SchoolName,
                                          NRIC = UI.Nric,
                                          Phone = UI.PhoneNumber
                                      }).Distinct().ToListAsync()).ToList();

                logger.LogInformation($"UserMangementRepository GetBlockedUsers() Method ended. ");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while getting all Blocked users list");
                throw;
            }
            return Alluserslst;
        }

        /// <summary>
        /// unblockUser:this api is to unblock  Users
        /// <param name="objunblockUsers"></param>
        /// </summary>
        /// <returns></returns>
        public async Task<string> unblockUser(List<GetAllUsersModel> objunblockUsers)
        {
            logger.LogInformation($"UserMangementRepository unblockUser() Method started. ");

            string status = "";
            try
            {
                foreach (var blockedUser in objunblockUsers)
                {
                    var userToUnblock = await context.UserInfos.Where(u => u.UserId == blockedUser.UserId).FirstOrDefaultAsync();
                    if (userToUnblock != null)
                    {
                        userToUnblock.IsBlock = false;
                        userToUnblock.IsActive = true;
                        userToUnblock.IsApprove = true;
                        userToUnblock.ForgotPasswordCount = 0;
                        status = "S002";
                        context.SaveChanges();
                    }
                    else
                    {
                        status = "S003";
                    }
                }

                logger.LogInformation($"UserMangementRepository unblockUser() Method ended. ");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while unblock the users");
                throw;
            }

            return status;
        }

        /// <summary>
        /// GetPassPhrase: this get Api used to get a passphrase
        /// <param name="ModifiedId"></param>
        /// <returns></returns>
        public async Task<List<PassPharseModel>> GetPassPhrase(long ModifiedId)
        {
            logger.LogInformation($"UserMangementRepository GetPassPhrase() Method started.  ModifiedId = {ModifiedId}");

            List<PassPharseModel> Alluserslst = new();
            try
            {
                Alluserslst = (await context.PassPharses.Where(a => !a.IsDeleted)
                    .Select(a => new PassPharseModel
                    {
                        PassPharseCode = a.PassPharseCode,
                        IsActive = a.IsActive,
                        ModifiedLoginId = a.ModifiedBy != null ? context.UserInfos.Where(u => u.UserId == a.ModifiedBy).FirstOrDefault().LoginId : context.UserInfos.Where(u => u.UserId == a.CreatedBy).FirstOrDefault().LoginId,
                        ModifiedDate = a.ModifiedBy != null ? a.ModifiedDate : a.CreatedDate,
                    }).OrderByDescending(a => a.IsActive).ThenByDescending(a => a.ModifiedDate).ToListAsync());

                logger.LogInformation($"UserMangementRepository GetPassPhrase() Method ended.   ModifiedId = {ModifiedId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while getting All PassPhrase list");
                throw;
            }
            return Alluserslst;
        }

        /// <summary>
        /// AddPassPhrase:this api is used to add and update a passphrase
        /// <param name="passPhraseObject"></param>
        /// </summary>
        /// <returns></returns>
        public async Task<string> AddPassPhrase(PassPharseModel passPhraseObject, long ModifiedId)
        {
            logger.LogInformation($"UserMangementRepository AddPassPhrase() Method started.  ModifiedId = {ModifiedId}");
            string status = "";
            try
            {
                if (passPhraseObject.PassPharseCode != null)
                {
                    var Results = await context.PassPharses.Where(a => a.IsActive == true && !a.IsDeleted).FirstOrDefaultAsync();
                    if (Results == null)
                    {
                        PassPharse passPharseList = new()
                        {
                            PassPharseCode = passPhraseObject.PassPharseCode,
                            CreatedBy = ModifiedId,
                            CreatedDate = DateTime.Now,
                            IsActive = true
                        };
                        context.Add(passPharseList);
                        context.SaveChanges();
                        status = "success";
                    }
                    else
                    {
                        Results.IsActive = false;
                        Results.ModifiedBy = ModifiedId;
                        Results.ModifiedDate = DateTime.Now;
                        context.Update(Results);
                        context.SaveChanges();

                        PassPharse passPharseList = new()
                        {
                            PassPharseCode = passPhraseObject.PassPharseCode,
                            CreatedBy = ModifiedId,
                            CreatedDate = (DateTime)Results.ModifiedDate,
                            IsActive = true
                        };
                        context.Add(passPharseList);
                        context.SaveChanges();
                        status = "success";
                    }
                }
                else
                {
                    status = "failed";
                }
                logger.LogInformation($"UserMangementRepository AddPassPhrase() Method ended.   ModifiedId = {ModifiedId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while adding PassPhrase");
                throw;
            }
            return status;
        }

        /// <summary>
        /// ValidateRemoveUser:this api is to validate the user is mapped with any  project
        /// <param name="UserId"></param>
        /// </summary>
        /// <returns></returns>
        public async Task<string> ValidateRemoveUser(long UserId)
        {
            logger.LogInformation($"UserMangementRepository ValidateRemoveUser() Method started.  UserId = {UserId}");
            string status = "";
            try
            {
                var usermapped = await context.ProjectUserRoleinfos.Where(u => u.UserId == UserId && !u.Isdeleted && u.IsActive == true).FirstOrDefaultAsync();
                if (usermapped != null)
                {
                    status = "MAP001";
                }
                else
                {
                    status = "UNM001";
                }
                logger.LogInformation($"UserMangementRepository ValidateRemoveUser() Method ended.  UserId = {UserId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while getting Remove Button for unmapped users");
                throw;
            }
            return status;
        }

        /// <summary>
        /// ScriptExists:this api is to validate for a particular users as a script
        /// <param name="StatusId"></param>
        /// <param name="UserId"></param>
        /// </summary>
        /// <returns></returns>
        public async Task<string> ScriptExists(int StatusId, long UserId)
        {
            logger.LogInformation($"UserMangementRepository ScriptExists() Method started.  StatusId = {StatusId},UserId = {UserId}");
            string status = "";
            bool statusScript = false;
            try
            {
                if (StatusId == 2 || StatusId == 3)
                {
                    await using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                    {
                        await using (SqlCommand sqlCmd = new SqlCommand("[Marking].[USPGetUserStatus]", sqlCon))
                        {
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            sqlCmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserId;
                            sqlCon.Open();
                            SqlDataReader sqlDataReader = sqlCmd.ExecuteReader();
                            if (sqlDataReader.Read())
                            {
                                statusScript = Convert.ToBoolean(sqlDataReader["IsScriptExists"]);
                                if (statusScript)
                                {
                                    status = "SCRIPTEXIST";
                                    return status;
                                }
                            }
                            if (!sqlDataReader.IsClosed) { sqlDataReader.Close(); }
                            sqlCon.Close();
                        }
                    }
                }

                logger.LogInformation($"UserMangementRepository ScriptExists() Method ended.  StatusId = {StatusId},UserId = {UserId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while  Script is exists ");
                throw;
            }
            return status;
        }

        /// /// <summary>
        /// ChangeStatusUsers:this api is to changestatus of particular user
        /// <param name="StatusId"></param>
        /// <param name="UserId"></param>
        /// </summary>
        /// <returns></returns>
        public async Task<UserContext> ChangeStatusUsers(int StatusId, long UserId, long currentUserId)
        {
            UserContext userContext = null;
            logger.LogInformation($"UserMangementRepository ChangeStatusUsers() Method started.  StatusId = {StatusId},UserId = {UserId},currentUserId = {currentUserId}");
            string status = "";
            string LivePoolStatus = "";

            try
            {
                //if status is inactive and disable move to a livepool
                if (StatusId == 2 || StatusId == 3)
                {
                    using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                    {
                        using (SqlCommand sqlCmd = new SqlCommand("Marking.USPMoveApplicationUserScriptsToLivePool", sqlCon))
                        {
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            sqlCmd.Parameters.Add("@UnmapUserID", SqlDbType.BigInt).Value = UserId;
                            sqlCmd.Parameters.Add("@ModifiedByUserID", SqlDbType.BigInt).Value = currentUserId;
                            sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
                            sqlCon.Open();
                            sqlCmd.ExecuteNonQuery();
                            sqlCon.Close();
                            LivePoolStatus = sqlCmd.Parameters["@Status"].Value.ToString();
                        }
                    }
                }

                var UserChangeStatusId = await context.UserInfos.Where(u => u.UserId == UserId && !u.IsDeleted).FirstOrDefaultAsync();
                if (UserChangeStatusId != null)
                {
                    //To activate a user
                    if (StatusId == 1)
                    {
                        UserChangeStatusId.IsActive = true;
                        context.SaveChanges();

                        userContext = new UserContext
                        {
                            Status = "A001"
                        };

                        InsertUserStatusTracking(UserId, (int)UserStatusTrackingType.Active, (int)EnumUserTrackingStatusLevel.ApplicationLevel, currentUserId);
                    }
                    //To inactive a user
                    else if (StatusId == 2)
                    {
                        if (LivePoolStatus == "S001")
                        {
                            UserChangeStatusId.IsActive = false;
                            context.SaveChanges();

                            userContext = new UserContext
                            {
                                Status = "IA001"
                            };
                            InsertUserStatusTracking(UserId, (int)UserStatusTrackingType.InActive, (int)EnumUserTrackingStatusLevel.ApplicationLevel, currentUserId);

                            var userLoginToken = await context.UserLoginTokens.Where(u => u.UserId == UserId && u.IsExpired == false && u.IsActive == true).FirstOrDefaultAsync();
                            if (userLoginToken != null)
                            {
                                userLoginToken.IsExpired = true;
                                userLoginToken.IsActive = false;
                                context.SaveChanges();
                            }
                        }
                        else if (LivePoolStatus == "E001")
                        {
                            userContext = new UserContext
                            {
                                Status = "E001"
                            }; //User Does not Exist in the Project
                        }
                        else if (LivePoolStatus == "E002")
                        {
                            userContext = new UserContext
                            {
                                Status = "EM002" //Modified By Does not Exist in the Project
                            };
                        }
                        else
                        {
                            userContext = new UserContext
                            {
                                Status = "ERR01"
                            };
                        }
                    }
                    //To disable a user
                    else if (StatusId == 3)
                    {
                        if (LivePoolStatus == "S001")
                        {
                            UserChangeStatusId.IsDisable = true;
                            context.SaveChanges();

                            userContext = new UserContext
                            {
                                Status = "D001"
                            };

                            InsertUserStatusTracking(UserId, (int)UserStatusTrackingType.Disable, (int)EnumUserTrackingStatusLevel.ApplicationLevel, currentUserId);

                            var userLoginToken = await context.UserLoginTokens.Where(u => u.UserId == UserId && u.IsExpired == false && u.IsActive == true).FirstOrDefaultAsync();
                            if (userLoginToken != null)
                            {
                                userLoginToken.IsExpired = true;
                                userLoginToken.IsActive = false;
                                context.SaveChanges();
                            }
                        }
                        else if (LivePoolStatus == "E001")
                        {
                            userContext = new UserContext
                            {
                                Status = "E001"
                            }; //User Does not Exist in the Project
                        }
                        else if (LivePoolStatus == "E002")
                        {
                            userContext = new UserContext
                            {
                                Status = "EM002"
                            }; //Modified By Does not Exist in the Project
                        }
                        else
                        {
                            userContext = new UserContext
                            {
                                Status = "ERR01"
                            };
                        }
                    }
                    //To Enable a users
                    else if (StatusId == 4)
                    {
                        UserChangeStatusId.IsDisable = false;
                        context.SaveChanges();

                        InsertUserStatusTracking(UserId, (int)UserStatusTrackingType.Enable, (int)EnumUserTrackingStatusLevel.ApplicationLevel, currentUserId);

                        var templateupdate = context.Templates.Where(a => a.TemplateName == "ENABLEUSER" && a.IsDeleted == false).FirstOrDefault();

                        TemplateUserMapping templateUsermapping = new TemplateUserMapping()
                        {
                            UserId = UserId,
                            TryOut = templateupdate.NoOfTryOut,
                            TemplateId = templateupdate.TemplateId
                        };

                        context.TemplateUserMappings.Add(templateUsermapping);
                        context.SaveChanges();
                        userContext = new UserContext
                        {
                            MailQueueId = templateUsermapping.Id,
                            Status = "EN001"
                        };
                    }
                    //To remove a user
                    else if (StatusId == 5)
                    {
                        using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                        {
                            using (SqlCommand sqlCmd = new SqlCommand("Marking.USPDeletetUser", sqlCon))
                            {
                                sqlCmd.CommandType = CommandType.StoredProcedure;
                                sqlCmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserId;
                                sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
                                sqlCon.Open();
                                sqlCmd.ExecuteNonQuery();
                                sqlCon.Close();
                                status = sqlCmd.Parameters["@Status"].Value.ToString();
                                if (status == "S001")
                                {
                                    userContext = new UserContext
                                    {
                                        Status = "S001"
                                    };
                                    var userLoginToken = await context.UserLoginTokens.Where(u => u.UserId == UserId && u.IsExpired == false && u.IsActive == true).FirstOrDefaultAsync();
                                    if (userLoginToken != null)
                                    {
                                        userLoginToken.IsExpired = true;
                                        userLoginToken.IsActive = false;
                                        context.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    //To activate and enable a user
                    else if (StatusId == 6)
                    {
                        UserChangeStatusId.IsDisable = false;
                        UserChangeStatusId.IsActive = true;
                        context.SaveChanges();
                        userContext = new UserContext
                        {
                            Status = "EA001"
                        };
                        InsertUserStatusTracking(UserId, (int)UserStatusTrackingType.EnableActive, (int)EnumUserTrackingStatusLevel.ApplicationLevel, currentUserId);
                    }
                    else
                    {
                        userContext = new UserContext
                        {
                            Status = "ERR01"
                        };
                    }
                }
                else
                {
                    userContext = new UserContext
                    {
                        Status = "ERR01"
                    };
                }

                logger.LogInformation($"UserMangementRepository ChangeStatusUsers() Method ended.  StatusId = {StatusId},UserId = {UserId},currentUserId = {currentUserId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while changing the status");
                throw;
            }
            return userContext;
        }

        public string InsertUserStatusTracking(long UserId, int UserStatus, int TrackingStatus, long currentUserId)
        {
            logger.LogInformation($"UserMangementRepository InsertUserStatusTracking() Method started.  UserStatus = {UserStatus},UserId = {UserId},TrackingStatus = {TrackingStatus},currentUserId = {currentUserId}");
            string status = "";
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new SqlCommand("INSERT INTO marking.UserStatusTracking(UserID, StatusLevel,Status, ActionByUserID, ActionDate,Remarks) VALUES(@UserID, @StatusLevel, @Status, @ActionByUserID,@ActionDate,@Remarks)", sqlCon))
                    {
                        sqlCmd.Parameters.AddWithValue("@UserID", UserId);
                        sqlCmd.Parameters.AddWithValue("@StatusLevel", TrackingStatus);
                        sqlCmd.Parameters.AddWithValue("@Status", UserStatus);
                        sqlCmd.Parameters.AddWithValue("@ActionByUserID", currentUserId);
                        sqlCmd.Parameters.AddWithValue("@ActionDate", DateTime.UtcNow);
                        sqlCmd.Parameters.AddWithValue("@Remarks", "");
                        sqlCon.Open();
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                    }
                }
                logger.LogInformation($"UserMangementRepository InsertUserStatusTracking() Method ended.  UserStatus = {UserStatus},UserId = {UserId},TrackingStatus = {TrackingStatus},currentUserId = {currentUserId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while inserting UserStatus Tracking");
                throw;
            }
            return status;
        }

        /// <summary>
        /// GetMyProfileDetails:this api is to Get user for headers
        /// </summary>
        /// <returns></returns>
        public async Task<CreateEditUser> GetMyProfileDetails(long UserId, long? ProjectuserroleId = null)
        {
            logger.LogInformation($"Error in AuthRepository-> GetMyProfileDetails() Method started.  UserId = {UserId}");

            CreateEditUser createEditUser = new();
            try
            {
                if (ProjectuserroleId > 0)
                {
                    createEditUser = (from pur in context.ProjectUserRoleinfos
                                      join ri in context.Roleinfos on pur.RoleId equals ri.RoleId
                                      join ui in context.UserInfos on pur.UserId equals ui.UserId
                                      join si in context.SchoolInfos.Where(s => !s.IsDeleted) on pur.SendingSchoolId equals si.SchoolId into schools
                                      from si in schools.DefaultIfEmpty()
                                      where pur.ProjectUserRoleId == ProjectuserroleId && !pur.Isdeleted && pur.IsActive == true && !ri.Isdeleted && !ui.IsDeleted
                                      select new CreateEditUser
                                      {
                                          UserId = ui.UserId,
                                          firstName = ui.FirstName,
                                          RoleCode = ri.RoleCode,
                                          ProjectRoleName = ri.RoleName,
                                          Nric = ui.Nric,
                                          SchoolName = si == null ? null : si.SchoolName,
                                          lastName = ui.LastName,
                                          EmailId = ui.EmailId,
                                          PhoneNum = ui.PhoneNumber,
                                          Loginname = ui.LoginId,
                                      }).FirstOrDefault();
                    if (createEditUser == null)
                    {
                        createEditUser = (from pur in context.ProjectUserRoleinfoArchives
                                          join ri in context.Roleinfos on pur.RoleId equals ri.RoleId
                                          join ui in context.UserInfos on pur.UserId equals ui.UserId
                                          join si in context.SchoolInfos.Where(s => !s.IsDeleted) on pur.SendingSchoolId equals si.SchoolId into schools
                                          from si in schools.DefaultIfEmpty()
                                          where pur.ProjectUserRoleId == ProjectuserroleId && !pur.Isdeleted && pur.IsActive == true && !ri.Isdeleted && !ui.IsDeleted
                                          select new CreateEditUser
                                          {
                                              UserId = ui.UserId,
                                              firstName = ui.FirstName,
                                              RoleCode = ri.RoleCode,
                                              ProjectRoleName = ri.RoleName,
                                              Nric = ui.Nric,
                                              SchoolName = si == null ? null : si.SchoolName,
                                              lastName = ui.LastName,
                                              EmailId = ui.EmailId,
                                              PhoneNum = ui.PhoneNumber,
                                              Loginname = ui.LoginId,
                                          }).FirstOrDefault();
                    }

                    var role = (from ui in context.UserInfos
                                join urm in context.UserToRoleMappings on ui.UserId equals urm.UserId
                                join ri in context.Roleinfos on urm.RoleId equals ri.RoleId
                                join si in context.SchoolInfos on ui.SchoolId equals si.SchoolId into siGroup
                                from si in siGroup.DefaultIfEmpty()
                                where !ui.IsDeleted && !urm.IsDeleted && !ri.Isdeleted && !ui.IsBlock && ui.UserId == UserId
                                select new
                                {
                                    ri.RoleCode,
                                    ri.RoleName,
                                }).FirstOrDefault();
                    if (role != null)
                    {
                        createEditUser.RoleName = role.RoleName;
                    }
                }
                else
                {
                    var result = (await (from ui in context.UserInfos
                                         join urm in context.UserToRoleMappings on ui.UserId equals urm.UserId
                                         join ri in context.Roleinfos on urm.RoleId equals ri.RoleId
                                         join si in context.SchoolInfos on ui.SchoolId equals si.SchoolId into siGroup
                                         from si in siGroup.DefaultIfEmpty()
                                         where !ui.IsDeleted && !urm.IsDeleted && !ri.Isdeleted && !ui.IsBlock && ui.UserId == UserId
                                         select new
                                         {
                                             ui.UserId,
                                             ui.FirstName,
                                             ui.LoginId,
                                             ri.RoleCode,
                                             ri.RoleName,
                                             ui.Nric,
                                             ui.SchoolId,
                                             SchoolName = si == null ? null : si.SchoolName,
                                             ui.LastName,
                                             ui.EmailId,
                                             ui.PhoneNumber,
                                         }).ToListAsync()).FirstOrDefault();
                    if (result != null)
                    {
                        createEditUser.Username = result.FirstName + " " + result.LastName;
                        createEditUser.Nric = result.Nric;
                        createEditUser.PhoneNum = result.PhoneNumber;
                        createEditUser.EmailId = result.EmailId;
                        createEditUser.SchoolName = result.SchoolName;
                        createEditUser.RoleName = result.RoleName;
                        createEditUser.RoleCode = result.RoleCode;
                        createEditUser.Loginname = result.LoginId;
                        createEditUser.firstName = result.FirstName;
                        createEditUser.lastName = result.LastName;
                    }
                }

                logger.LogInformation($"Error in User Management-> GetMyProfileDetails() Method ended.   UserId = {UserId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while getting My Profile Details");
                throw;
            }
            return createEditUser;
        }

        /// <summary>
        /// GetApplicationLevelUserRoles:this api is to Get Role School details
        /// </summary>
        /// <returns></returns>
        public async Task<RoleSchooldetails> GetApplicationLevelUserRoles()
        {
            RoleSchooldetails rolescholDetails = new RoleSchooldetails();

            try
            {
                rolescholDetails.roles = (await (from role in context.Roleinfos
                                                 where !role.Isdeleted && role.ParentRoleId == null && role.ApplicationLevel != null
                                                 orderby role.ApplicationLevel
                                                 select new RoleDetails
                                                 {
                                                     RoleID = role.RoleId,
                                                     RoleName = role.RoleName,
                                                     RoleCode = role.RoleCode
                                                 }).ToListAsync()).ToList();

                rolescholDetails.schools = (await (from school in context.SchoolInfos
                                                   where school.ProjectId == null && !school.IsDeleted
                                                   select new SchoolDetails
                                                   {
                                                       SchoolID = school.SchoolId,
                                                       SchoolName = school.SchoolName,
                                                       SchoolCode = school.SchoolCode
                                                   }).ToListAsync()).ToList();

                return rolescholDetails;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while getting All Roles :Method Name:GetApplicationLevelUserRoles()");
                throw;
            }
        }
    }
}