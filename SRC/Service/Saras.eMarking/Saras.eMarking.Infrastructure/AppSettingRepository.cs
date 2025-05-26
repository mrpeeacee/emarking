using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Business;
using Saras.eMarking.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure
{
    public static class AppSettingRepository
    {

        /// <summary>
        /// GetAllSettings : Method to GET all app setting key with value to given parameter
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        /// <param name="projectid"></param>
        /// <param name="groupcode"></param>
        /// <param name="entitytype"></param>
        /// <param name="entityid"></param>
        /// <returns>App setting key with value for given patameters</returns>
        public static async Task<IList<AppSettingModel>> GetAllSettings(ApplicationDbContext context, ILogger logger, long projectid, string groupcode, byte? entitytype = 0, long? entityid = 0)
        {
            List<AppSettingModel> result = new();
            int status = 0;

            try
            {
                status = context.ProjectInfos.Where(p=>p.ProjectId == projectid && p.IsDeleted == false).Select(s=>s.ProjectStatus).FirstOrDefault();
                
                if (result != null)
                {
                    using SqlConnection con = new(context.Database.GetDbConnection().ConnectionString);
                    using SqlCommand sqlcmd = new("[Marking].[UspGetAppsettingDetails]", con);
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.Add("@AppSettingGroupCode", SqlDbType.NVarChar).Value = groupcode;
                    sqlcmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectid;
                    sqlcmd.Parameters.Add("@EntityType", SqlDbType.TinyInt).Value = entitytype;
                    sqlcmd.Parameters.Add("@EntityID", SqlDbType.BigInt).Value = entityid;
                    sqlcmd.Parameters.Add("@Status", SqlDbType.VarChar, 10).Direction = ParameterDirection.Output;
                    await con.OpenAsync();
                    using (SqlDataReader reader = await sqlcmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (reader.HasRows)
                            {
                                result.Add(new AppSettingModel
                                {
                                    AppsettingKey = SqlHelper.GetValue<string>(reader["AppsettingKey"]),
                                    AppSettingKeyID = SqlHelper.GetValue<long>(reader["AppSettingKeyID"]),
                                    AppsettingKeyName = SqlHelper.GetValue<string>(reader["AppSettingKeyName"]),
                                    SettingGroupID = SqlHelper.GetValue<long>(reader["AppsettingGroupID"]),
                                    SettingGroupCode = SqlHelper.GetValue<string>(reader["AppSettingGroupCode"]),
                                    SettingGroupName = SqlHelper.GetValue<string>(reader["AppSettingGroupName"]),
                                    ValueType = (EnumAppSettingValueType)SqlHelper.GetValue<byte>(reader["ValueType"]),
                                    Value = SqlHelper.GetValue<string>(reader["Value"]),
                                    DefaultValue = SqlHelper.GetValue<string>(reader["DefaultValue"]),
                                    ParentAppsettingKeyID = (reader["ParentAppsettingKeyID"] != DBNull.Value) ? SqlHelper.GetValue<long>(reader["ParentAppsettingKeyID"]) : null,
                                    EntityID = SqlHelper.GetValue<long>(reader["EntityID"]),
                                    EntityType = (EnumAppSettingEntityType)SqlHelper.GetValue<byte>(reader["EntityType"]),
                                    ProjectID = SqlHelper.GetValue<long>(reader["ProjectID"]),
                                    ProjectStatus = status
                                });

                            }

                        }
                    }
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
            return result;
        }

        /// <summary>
        /// UpdateAllSettings : Method to Update all app setting key with value to given parameter
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        /// <param name="appSettingModels"></param>
        /// <param name="ProjectUserRoleID"></param>
        /// <returns>Return status as sucess for failure of update</returns>
        public static string UpdateAllSettings(ApplicationDbContext context, ILogger logger, List<AppSettingModel> appSettingModels, long ProjectUserRoleID)
        {
            string status = string.Empty;
            try
            {
                SqlParameter tvpParam = new("@UDTAppsetting", BuildTable(appSettingModels, logger))
                {
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "Marking.UDTAppsetting"
                };
                SqlParameter createdby = new()
                {
                    ParameterName = "@CreatedBy",
                    SqlDbType = SqlDbType.BigInt,
                    Value = ProjectUserRoleID
                };
                SqlParameter output = new()
                {
                    ParameterName = "@Status",
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Output,
                    Size = 10
                };

                context.Database.ExecuteSqlRaw("[Marking].[USPInsertAppsettingDetail]  @UDTAppsetting, @CreatedBy, @Status OUTPUT", tvpParam, createdby, output);
                if (output != null)
                    status = Convert.ToString(output.Value);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
            return status;
        }

        /// <summary>
        /// BuildTable : This Method used to Build Table
        /// </summary>
        /// <param name="appSettingModels"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static DataTable BuildTable(List<AppSettingModel> appSettingModels, ILogger logger)
        {
            try
            {
                var Table = CreateColumn(logger);
                appSettingModels.ForEach(e =>
                {
                    if (e.Value != null)
                    {
                        var activityRow = Table.NewRow();
                        activityRow = AddRow(e, activityRow, logger);
                        Table.Rows.Add(activityRow);
                    }
                });
                return Table;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// AddRow : This Method used to Add the rows
        /// </summary>
        /// <param name="appSettingModels"></param>
        /// <param name="row"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static DataRow AddRow(AppSettingModel appSettingModels, DataRow row, ILogger logger)
        {
            try
            {
                row["EntityID"] = appSettingModels.EntityID;
                row["EntityType"] = (byte)appSettingModels.EntityType;
                row["AppSettingKeyID"] = appSettingModels.AppSettingKeyID;
                row["Value"] = appSettingModels.Value;
                row["DefaultValue"] = appSettingModels.DefaultValue;
                row["ValueType"] = (byte)appSettingModels.ValueType;
                row["ReferanceID"] = null;
                row["ProjectID"] = appSettingModels.ProjectID;
                row["OrganizationID"] = 0;
                row["AppsettingGroupID"] = appSettingModels.SettingGroupID;
                return row;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// CreateColumn : This Method used to create the colums
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static DataTable CreateColumn(ILogger logger)
        {
            try
            {
                var AppsettingColumns = new DataTable();
                AppsettingColumns.Columns.Add("EntityID");
                AppsettingColumns.Columns.Add("EntityType");
                AppsettingColumns.Columns.Add("AppSettingKeyID");
                AppsettingColumns.Columns.Add("Value");
                AppsettingColumns.Columns.Add("DefaultValue");
                AppsettingColumns.Columns.Add("ValueType");
                AppsettingColumns.Columns.Add("ReferanceID");
                AppsettingColumns.Columns.Add("ProjectID");
                AppsettingColumns.Columns.Add("OrganizationID");
                AppsettingColumns.Columns.Add("AppsettingGroupID");
                return AppsettingColumns;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// UpdateProjectStatus 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        /// <param name="projectID"></param>
        /// <param name="projectUserRoleID"></param>
        /// <returns>Return status </returns>
        public static string UpdateProjectStatus(ApplicationDbContext context, ILogger logger, long projectID, long projectUserRoleID)
        {
            string status = string.Empty;
            try
            {
                SqlParameter projectId = new()
                {
                    ParameterName = "@ProjectID",
                    SqlDbType = SqlDbType.BigInt,
                    Value = projectID
                };
                SqlParameter projectUserRoleId = new()
                {
                    ParameterName = "@CreatedBy",
                    SqlDbType = SqlDbType.BigInt,
                    Value = projectUserRoleID
                };
                SqlParameter output = new()
                {
                    ParameterName = "@Status",
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Output,
                    Size = 10
                };

                context.Database.ExecuteSqlRaw("[Marking].[USPUpdateProjectProgressStatus]  @ProjectID, @CreatedBy, @Status OUTPUT", projectId, projectUserRoleId, output);
                if (output != null)
                    status = Convert.ToString(output.Value);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
            return status;
        }
    }
}
