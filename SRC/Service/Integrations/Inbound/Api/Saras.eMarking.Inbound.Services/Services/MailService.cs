using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Inbound.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Inbound.Domain.Interfaces.RepositoryInterfaces;
using Saras.eMarking.Inbound.Services.Model;
using System.Data;
using static Saras.eMarking.Inbound.Domain.Models.MailModel;

namespace Saras.eMarking.Inbound.Business.Services
{
    public class MailService : IMailService
    {
        public readonly ILogger logger;
        private readonly AppOptions appSettings;
        private readonly IMailRepository _mailRepository;
        private int commandTimeout = 0;

        public MailService(ILogger<MailService> logger, AppOptions appSettings, IMailRepository mailRepository)
        {
            this.logger = logger;
            this.appSettings = appSettings;
            _mailRepository = mailRepository;
            commandTimeout = 2147483647;
        }

        public async Task<List<SendMailResponseModel>> EMarkingSendEmail(long? QueueId)
        {
            logger.LogInformation("Mail Service >> EMarkingSendEmail() started");
            try
            {
                List<EmailUsers> UsersList = new();
                EmailUsers test = new();
                // Get Users
                using (SqlConnection connectioneMarking = new SqlConnection(appSettings.ConnectionStrings.EMarkingConnection))
                {
                    logger.LogInformation("Begin To Get EMail Users");
                    connectioneMarking.Open();
                    using (SqlCommand commandSynceMarkingUser = new SqlCommand("[Marking].[USPGetMailUsers]"))
                    {
                        commandSynceMarkingUser.CommandTimeout = commandTimeout;
                        commandSynceMarkingUser.Connection = connectioneMarking;
                        commandSynceMarkingUser.CommandType = CommandType.StoredProcedure;
                        if (QueueId != null || QueueId != 0)
                        {
                            commandSynceMarkingUser.Parameters.Add("@TemplateUserMappingID", SqlDbType.BigInt).Value = QueueId;
                        }

                        SqlDataReader reader = commandSynceMarkingUser.ExecuteReader();

                        while (reader.Read())
                        {
                            test = new EmailUsers
                            {
                                ID = long.Parse(reader["ID"].ToString()),
                                EmailID = reader["EmailID"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                TemplateBody = reader["TemplateBody"].ToString(),
                                ToLoginID = reader["LoginID"].ToString(),
                                PassPhrase = reader["PassPharseCode"].ToString(),
                                Year = int.Parse(reader["Year"].ToString())
                            };
                            UsersList.Add(test);
                        }
                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                    connectioneMarking.Close();
                    logger.LogInformation("End of Getting EMail Users");
                }
                return await _mailRepository.EMarkingSendEmail(UsersList);
            }
            catch (Exception ex)
            {
                logger.LogError("Error while EMarkingSendEmail:Method Name:EMarkingSendEmail()", ex.Message);
                throw;
            }
        }
    }
}