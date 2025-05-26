using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Inbound.Domain.Models;
using Saras.eMarking.Inbound.Interfaces.BusinessInterface;
using Saras.eMarking.Inbound.Interfaces.RepositoryInterfaces;
using Saras.eMarking.Inbound.Services.Model;
using System.Data;
using System.Text;

namespace Saras.eMarking.Inbound.Services.Services
{
    public class QRLPackService : IQRLPackService
    {
        public readonly ILogger logger;
        private readonly AppOptions appSettings;
        readonly IQRLPackRepository _qrlRepository;
        int commandTimeout = 0;

        public QRLPackService(ILogger<QRLPackService> logger, AppOptions appSettings, IQRLPackRepository qrlRepository)
        {
            this.logger = logger;
            this.appSettings = appSettings;
            _qrlRepository = qrlRepository;
            commandTimeout = 2147483647;
        }
        public async Task<List<eMarkingSyncUserResponse>> eMarkingQRLpackStatics()
        {
            logger.LogInformation("User Service >> eMarkingQRLpackStatics() started");
            try
            {
                DataTable scheduledetails = new DataTable();
                StringBuilder sbstatus = new StringBuilder();                

                // Insert ScheduleDetails To CourseMovement table(eMarking table)
                using (SqlConnection connectioneMarking = new SqlConnection(appSettings.ConnectionStrings.EMarkingConnection))
                {
                    logger.LogInformation("Begin to Insert ScheduleDetails To CourseMovement table");
                    sbstatus.Append("Begin to Insert ScheduleDetails To CourseMovement table, ");
                    connectioneMarking.Open();
                    using (SqlCommand commandSynceMarkingUser = new SqlCommand("[Marking].[USPInsertScheduleDetailsToCourseMovement]"))
                    {
                        commandSynceMarkingUser.CommandTimeout = commandTimeout;
                        commandSynceMarkingUser.Connection = connectioneMarking;
                        commandSynceMarkingUser.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter adap = new SqlDataAdapter(commandSynceMarkingUser);
                        adap.Fill(scheduledetails);
                    }
                    connectioneMarking.Close();
                }

                logger.LogInformation("End of Insert ScheduleDetails To CourseMovement table in the eMarking DB" + " Response: " + scheduledetails);
                sbstatus.Append("End of Insert ScheduleDetails To CourseMovement table in the eMarking DB, ");

                return await _qrlRepository.eMarkingQRLpackStatics(scheduledetails, sbstatus);
            }
            catch (Exception ex)
            {
                logger.LogError("Error while eMarkingQRLpackStatics:Method Name:eMarkingQRLpackStatics()", ex.Message);
                throw;
            }
        }
    }
}
