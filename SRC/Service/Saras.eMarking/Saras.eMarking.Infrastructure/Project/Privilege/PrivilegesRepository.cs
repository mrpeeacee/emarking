using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Privilege;
using Saras.eMarking.Domain.ViewModels.Project.Privilege;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Privilege
{
    public class PrivilegesRepository : BaseRepository<PrivilegesRepository>, IPrivilegeRepository
    {
        private readonly ApplicationDbContext context;
        public PrivilegesRepository(ApplicationDbContext context, ILogger<PrivilegesRepository> _logger) : base(_logger)
        {
            this.context = context;
        }

        public async Task<IList<UserPrivilegesModel>> GetUserPrivileges(int Type, long ProjectUserRoleID, long UserId)
        {
            List<UserPrivilegesModel> result = null;
            try
            {
                logger.LogDebug($"PrivilegeRepository  GetUserPrivileges() Method started.  Project ProjectUserRoleID {ProjectUserRoleID} and UserId {UserId}");
                await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmd = new("[Marking].[UspGetUserPrivileges]", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = ProjectUserRoleID;
                sqlCmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value = UserId;
                sqlCmd.Parameters.Add("@PrivilegeType", SqlDbType.BigInt).Value = Type;
                sqlCon.Open();
                SqlDataReader reader = sqlCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    result = new List<UserPrivilegesModel>();
                    while (reader.Read())
                    {
                        UserPrivilegesModel objuserprivileges = new()
                        {
                            ID = Convert.ToInt64(reader["PrivilegeID"]),
                            Name = Convert.ToString(reader["PrivilegeName"]),
                            RoleCode = Convert.ToString(reader["RoleCode"]),
                            ParentID = reader["ParentPrivilegeID"] != DBNull.Value ? Convert.ToInt64(reader["ParentPrivilegeID"]) : 0,
                            Url = Convert.ToString(reader["PrivilegeURL"]),
                            PrivilegeOrder = Convert.ToInt16(reader["PrivilegeOrder"]),
                            PrivilegeCode = Convert.ToString(reader["PrivilegeCode"])
                        };
                        result.Add(objuserprivileges);
                    }
                    if (result.Count > 0)
                    {
                        result = result.OrderBy(a => a.ParentID).ThenBy(a => a.PrivilegeOrder).ToList();
                    }
                }
                if (!reader.IsClosed)
                {
                    reader.Close();
                }
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCon.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error while getting PrivilegeRepository : Method Name : GetUserPrivileges(): ProjectUserRoleID=" + Convert.ToString(ProjectUserRoleID), "Error while getting UserPrivileges: Method Name: GetUserPrivileges(): UserId=" + UserId.ToString());
                throw;
            }
            return result;
        }
    }
}
