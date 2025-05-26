using HmacHashing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saras.eMarking.Inbound.Domain.Models;
using Saras.eMarking.Inbound.Interfaces.BusinessInterface;
using Saras.eMarking.Inbound.Interfaces.RepositoryInterfaces;
using Saras.eMarking.Inbound.Services.Model;
using System.Data;

namespace Saras.eMarking.Inbound.Services.Services
{
    public class UsersService : IUsersService
    {
        public readonly ILogger logger;
        private readonly AppOptions appSettings;
        readonly IUsersRepository _usersRepository;
        int commandTimeout = 0;
        public UsersService(ILogger<UsersService> logger, AppOptions appSettings, IUsersRepository usersRepository)
        {
            this.logger = logger;
            this.appSettings = appSettings;
            _usersRepository = usersRepository;
            commandTimeout = 2147483647;
        }

        public async Task<List<eMarkingSyncUserResponse>> SynceMarkingUser()
        {
            logger.LogInformation("User Service >> SynceMarkingUser() started");
            try
            {
                string? Status;
                DataTable filteredDT = new();
                logger.LogInformation("Begin SynceMarkingUser()");
                DataTable tblResponse = new();

                // Get Paper Proctor info for importing into eMarking DB
                SqlConnection userSyncFrom = new SqlConnection(appSettings.ConnectionStrings.InboundConnection);
                string isSyncFromDel = Convert.ToString(appSettings.AppSettings.IsUserSyncFromDelivery);
                bool IsUserSyncFromDelivery = true;
                if (!string.IsNullOrEmpty(isSyncFromDel))
                {
                    IsUserSyncFromDelivery = Convert.ToBoolean(isSyncFromDel);
                }
                if (!IsUserSyncFromDelivery)
                {
                    userSyncFrom = new SqlConnection(appSettings.ConnectionStrings.EMarkingConnection);
                }

                using (SqlConnection connectionSEAB = userSyncFrom)
                {
                    logger.LogInformation("Begin to Get eMarking users from PaperProctorInfo table");
                    connectionSEAB.Open();
                    using (SqlCommand commandSynceMarkingUser = new SqlCommand("[Marking].[UspGetPaperProctorInfoForEmarking]"))
                    {
                        commandSynceMarkingUser.CommandTimeout = commandTimeout;
                        commandSynceMarkingUser.Connection = connectionSEAB;
                        commandSynceMarkingUser.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter adap = new SqlDataAdapter(commandSynceMarkingUser);
                        adap.Fill(tblResponse);

                        var query = from r in tblResponse.AsEnumerable().ToList()
                                    where r.Field<string>("ExamLevel") != null && r.Field<string>("ExamSeries") != null &&
                                    r.Field<string>("ExamYear") != null && r.Field<string>("ModeofAssessment") != null &&
                                    r.Field<string>("PaperNumber") != null && r.Field<string>("SubjectCode") != null &&
                                    (r.Field<string>("MarkerLevel") == "L0" || r.Field<string>("MarkerLevel") == "L1" ||
                                    r.Field<string>("MarkerLevel") == "L2" || r.Field<string>("MarkerLevel") == "L3" || r.Field<string>("MarkerLevel") == "L4" || r.Field<string>("MarkerLevel") == "L5")
                                    select r;

                        if (query != null && query.Count() > 0)
                        {
                            filteredDT = query.Distinct(DataRowComparer.Default).CopyToDataTable();
                            //Password encryption
                            foreach (DataRow row in filteredDT.Rows)
                            {
                                if (!IsUserSyncFromDelivery)
                                {
                                    row["ModeofAssessment"] = AlterModeOdAssessement(row["ModeofAssessment"]);
                                }
                                var nricstr = row["NRIC/FIN"].ToString();
                                row["Password"] = HmacHash.Encrypt(appSettings.AppSettings.encryptionKey_SSO.ToString(), "P@ssword" + nricstr.Substring(nricstr.Length - 4));
                            }
                            Status = JsonConvert.SerializeObject(filteredDT);
                        }
                        else
                        {
                            Status = "NoRecords";
                        }
                    }
                    connectionSEAB.Close();
                }
                logger.LogInformation("End of Getting eMarking users from PaperProctorInfo table" + " Data: " + (tblResponse == null ? " " : Status));

                return await _usersRepository.SynceMarkingUser(filteredDT, Status, IsUserSyncFromDelivery);
            }
            catch (Exception ex)
            {
                logger.LogError("Error while SynceMarkingUser:Method Name:SynceMarkingUser()", ex.Message);
                throw;
            }
        }

        private object AlterModeOdAssessement(object v)
        {
            string? Moa = Convert.ToString(v);
            bool MoaReplaceRequired = Convert.ToBoolean(appSettings.AppSettings.MoaReplaceRequired);
            List<string> MoaToReplace = appSettings.AppSettings.MOAsToReplace.ToList();

            if (MoaReplaceRequired)
            {
                foreach (var item in MoaToReplace)
                {
                    if (item.Contains(Moa.ToUpper()))
                    {
                        Moa = "eWritten";
                    }
                }
            }
            return Moa;
        }

    }
}
