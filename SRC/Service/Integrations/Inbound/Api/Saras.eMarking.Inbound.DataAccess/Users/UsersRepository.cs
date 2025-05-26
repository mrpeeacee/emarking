using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saras.eMarking.Inbound.Domain.Models;
using Saras.eMarking.Inbound.Interfaces.RepositoryInterfaces;
using Saras.eMarking.Inbound.Services.Model;
using System.Data;
using static Saras.eMarking.Inbound.Domain.Models.eMarkingSyncUserResponse;

namespace Saras.eMarking.Inbound.Infrastructure.Users
{
    public class UsersRepository : IUsersRepository
    {
        public readonly ILogger logger;
        private readonly AppOptions appSettings;

        public UsersRepository(ILogger<UsersRepository> _logger, AppOptions settings)
        {
            appSettings = settings;
            logger = _logger;
        }

        public Task<List<eMarkingSyncUserResponse>> SynceMarkingUser(DataTable filteredDT, string? Status, bool IsUserSyncFromDelivery)
        {
            List<eMarkingSyncUserResponse> eMarkingSyncUserResponses = new List<eMarkingSyncUserResponse>();
            try
            {
                logger.LogInformation("Begin SynceMarkingUser()");
                DataTable tblResponse = new();
                DataTable InsertedtblResponse = new();
                SqlDataReader dataReader;
                DataTable filteredDatatbl = new();

                // Insert MP Info into Marking Tables from T&A tbl
                if (Status != "NoRecords")
                {
                    var filteredDatatable = filteredDT.AsEnumerable()
                                        .GroupBy(r => new
                                        {
                                            ExamLevel = r["ExamLevel"],
                                            ExamSeries = r["ExamSeries"],
                                            ExamYear = r["ExamYear"],
                                            ModeofAssessment = r["ModeofAssessment"],
                                            PaperNumber = r["PaperNumber"],
                                            SubjectCode = r["SubjectCode"],
                                            SubjectName = r["SubjectName"]
                                        })
                                        .ToList();
                    foreach (var tbldata in filteredDatatable)
                    {
                        eMarkingSyncUserResponse objResponse = new eMarkingSyncUserResponse();

                        var filtablesdata = filteredDT.AsEnumerable().Where(row => row.Field<string>("ExamLevel") == tbldata.Key.ExamLevel.ToString() &&
                        row.Field<string>("ExamSeries") == tbldata.Key.ExamSeries.ToString() &&
                        row.Field<string>("ExamYear") == tbldata.Key.ExamYear.ToString() &&
                        row.Field<string>("ModeofAssessment") == tbldata.Key.ModeofAssessment.ToString() &&
                        row.Field<string>("PaperNumber") == tbldata.Key.PaperNumber.ToString() &&
                        row.Field<string>("SubjectCode") == tbldata.Key.SubjectCode.ToString());
                        if (filtablesdata != null && filtablesdata.Any())
                        {
                            filteredDatatbl = filtablesdata.CopyToDataTable();
                        }
                        ProjectInfo projectInfo = new()
                        {
                            ExamLevel = tbldata.Key.ExamLevel.ToString(),
                            ExamSeries = tbldata.Key.ExamSeries.ToString(),
                            ExamYear = tbldata.Key.ExamYear.ToString(),
                            ModeofAssessment = tbldata.Key.ModeofAssessment.ToString(),
                            PaperNumber = tbldata.Key.PaperNumber.ToString(),
                            SubjectCode = tbldata.Key.SubjectCode.ToString()
                        };

                        if (filtablesdata != null && filtablesdata.Any() && filteredDatatbl.Rows.Count > 0)
                        {
                            logger.LogInformation("Begin SynceMarkingUser() to eMarking DB");
                            SqlParameter sqlParameter;
                            try
                            {
                                using (SqlConnection connectionSynceMarkingUser = new SqlConnection(appSettings.ConnectionStrings.EMarkingConnection))
                                {
                                    using (SqlCommand commandSynceMarkingUser = new SqlCommand("[Marking].[USPImportMarkingPersonnelUserInfo]", connectionSynceMarkingUser))
                                    {
                                        commandSynceMarkingUser.CommandType = CommandType.StoredProcedure;
                                        commandSynceMarkingUser.Parameters.Clear();
                                        connectionSynceMarkingUser.Open();
                                        sqlParameter = commandSynceMarkingUser.Parameters.AddWithValue("@UDTUserInfo", filteredDatatbl);
                                        sqlParameter.SqlDbType = SqlDbType.Structured;
                                        sqlParameter.TypeName = "Marking.UDTUserInfo";

                                        // Fill result set to datatable
                                        dataReader = commandSynceMarkingUser.ExecuteReader();
                                        if (dataReader.HasRows)
                                        {
                                            while (dataReader.Read())
                                            {
                                                Status = dataReader.GetValue(0).ToString();
                                            }
                                            if (Status == "S001")
                                            {
                                                dataReader.NextResult();
                                                InsertedtblResponse.Load(dataReader);
                                            }
                                        }
                                        if (!dataReader.IsClosed)
                                        {
                                            dataReader.Close();
                                        }
                                    }
                                    connectionSynceMarkingUser.Close();
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.LogInformation(ex.Message, ex);
                                Status = "InValidRecordsFound";
                            }
                        }
                        logger.LogInformation("End of SynceMarkingUser() to eMarking DB" + " Status: " + InsertedtblResponse);

                        // Update Paper Proctor Info which have been processed in the eMarking DB
                        if (Status == "S001")
                        {
                            logger.LogInformation("Begin to Update Paper Proctor Info which have been processed in the eMarking DB");
                            SqlParameter sqlParameter;
                            SqlConnection userSyncFrom = new SqlConnection(appSettings.ConnectionStrings.InboundConnection);
                            if (!IsUserSyncFromDelivery)
                            {
                                userSyncFrom = new SqlConnection(appSettings.ConnectionStrings.EMarkingConnection);
                            }
                            try
                            {
                                using (SqlConnection connectionSEAB = userSyncFrom)
                                {
                                    connectionSEAB.Open();
                                    using (SqlCommand commandSynceMarkingUser = new SqlCommand("[Marking].[USPUpdatePaperProctorInfo]"))
                                    {
                                        commandSynceMarkingUser.Connection = connectionSEAB;
                                        commandSynceMarkingUser.CommandType = CommandType.StoredProcedure;
                                        commandSynceMarkingUser.Parameters.Clear();
                                        sqlParameter = commandSynceMarkingUser.Parameters.AddWithValue("@UDTProctorProcessedInfo", InsertedtblResponse);
                                        sqlParameter.SqlDbType = SqlDbType.Structured;
                                        sqlParameter.TypeName = "Marking.UDTProctorProcessedInfo";
                                        sqlParameter = commandSynceMarkingUser.Parameters.Add("@Status", SqlDbType.NVarChar, 20);
                                        sqlParameter.Direction = ParameterDirection.Output;
                                        commandSynceMarkingUser.ExecuteNonQuery();
                                        Status = (string)sqlParameter.Value;
                                    }
                                    connectionSEAB.Close();
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.LogInformation(ex.Message, ex);
                                Status = "InValidRecordsFound";
                            }
                        }
                        else if (Status == "EOO1")
                        {
                            Status = "Error while inserting the record";
                        }
                        else if (Status == "E002")
                        {
                            Status = "Invalid Exam Level";
                        }
                        else if (Status == "E003")
                        {
                            Status = "Invalid Exam Series";
                        }
                        else if (Status == "E004")
                        {
                            Status = "Invalid Exam Year";
                        }
                        else if (Status == "E005")
                        {
                            Status = "Invalid MoA";
                        }
                        else if (Status == "E006")
                        {
                            Status = "Invalid Subject Paper Information";
                        }
                        else if (Status == "E007")
                        {
                            Status = "Invalid Subject info";
                        }
                        else if (Status == "E008")
                        {
                            Status = "Invalid Project info";
                        }

                        objResponse.Status = Status;
                        objResponse.ProjectData = JsonConvert.SerializeObject(projectInfo);
                        objResponse.ResponseData = JsonConvert.SerializeObject(InsertedtblResponse);

                        eMarkingSyncUserResponses.Add(objResponse);
                        logger.LogInformation("End of Update Paper Proctor Info which have been processed in the eMarking DB" + " Status: " + Status);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex.Message, ex);
                Status = "InValidRecordsFound";
            }
            return Task.FromResult(eMarkingSyncUserResponses);
        }
    }
}