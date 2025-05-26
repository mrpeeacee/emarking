using Microsoft.Extensions.Logging;
using Saras.eMarking.Inbound.Services.Model;
using System.Data;
using Microsoft.Data.SqlClient;
using Saras.eMarking.Inbound.Domain.Interfaces.RepositoryInterfaces;
using static Saras.eMarking.Inbound.Domain.Models.MailModel;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Net;

namespace Saras.eMarking.Inbound.Infrastructure.Mail
{
    public class MailRepository : IMailRepository
    {
        public readonly ILogger logger;
        private readonly AppOptions appSettings;
        public MailRepository(ILogger<MailRepository> _logger, AppOptions settings)
        {
            appSettings = settings;
            logger = _logger;
        }

        public Task<List<SendMailResponseModel>> EMarkingSendEmail(List<EmailUsers> UsersList)
        {
            string Status = string.Empty;
            List<SendMailResponseModel> sendMailResponseModels = new();
            DataTable datatbl = new DataTable();
            bool isEmailSent;
            try
            {
                logger.LogInformation("Begin EMarkingSendEmail()");

                if (UsersList.Count > 0)
                {
                    foreach (var user in UsersList)
                    {
                        // Send Email
                        logger.LogInformation("Begin To Send EMail");
                        MailMessage message = new MailMessage();
                        Regex validemail = new Regex(@"^[a-zA-Z0-9][\w\-._]+(\.[a-z0-9]+)*@[a-zA-Z]{3,}([.]{1}[a-zA-Z]{2,}|[.]{1}[a-zA-Z]{2,}[.com]{1}[a-zA-Z]{2,})*$", RegexOptions.IgnoreCase);

                        if (user.EmailID != "" && validemail.IsMatch(user.EmailID))
                        {
                            message = BuildUserRegistrationTemplate(user);
                            isEmailSent = SendEmail(message);
                            sendMailResponseModels.Add(new SendMailResponseModel()
                            {
                                ID = user.ID,
                                IsMailSent = isEmailSent
                            });

                            if (!isEmailSent)
                            {
                                logger.LogInformation("Sending EMail Failed :" + user.ToLoginID);
                            }
                        }
                        else
                        {
                            sendMailResponseModels.Add(new SendMailResponseModel()
                            {
                                ID = user.ID,
                                IsMailSent = false
                            });
                            logger.LogInformation("Sending EMail is Null or Invalid EMail format.");
                        }
                        logger.LogInformation("End of Sending EMail");
                    }
                }

                // Update IsMailSent = 1
                if (sendMailResponseModels.Count > 0)
                {
                    datatbl.Columns.Add("ID");
                    datatbl.Columns.Add("IsMailSent");
                    DataRow row = null;

                    foreach (var rowObj in sendMailResponseModels)
                    {
                        row = datatbl.NewRow();
                        datatbl.Rows.Add(rowObj.ID, rowObj.IsMailSent);
                    }

                    logger.LogInformation("Begin to update IsMailSent");
                    using (SqlConnection connectioneMarking = new SqlConnection(appSettings.ConnectionStrings.EMarkingConnection))
                    {
                        using (SqlCommand command = new SqlCommand("[Marking].[USPUpdateMailUsers]", connectioneMarking))
                        {
                            SqlParameter sqlParameter;
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Clear();
                            connectioneMarking.Open();
                            sqlParameter = command.Parameters.AddWithValue("@UDTMailSentInfo", datatbl);
                            sqlParameter.SqlDbType = SqlDbType.Structured;
                            sqlParameter.TypeName = "Marking.UDTMailSentInfo";
                            command.ExecuteNonQuery();
                        }
                        connectioneMarking.Close();
                        logger.LogInformation("End of update IsMailSent");
                    }
                }
            }
            catch (Exception ex)
            {
                Status = "Error Found";
                logger.LogError(ex.Message, ex+" "+Status);

            }
            return Task.FromResult(sendMailResponseModels);
        }

        private MailMessage BuildUserRegistrationTemplate(EmailUsers user)
        {
            MailMessage message = new MailMessage();
            try
            {
                logger.LogInformation("Begin To Build User Registration Template");

                message.From = new MailAddress(Convert.ToString(appSettings.AppSettings.EmailConfig.From));
                message.To.Add(new MailAddress(user.EmailID));
                message.Subject = "SEAB eExam2 System login credentials";
                message.Body = user.TemplateBody.Replace("[FirstName]", user.FirstName)
                    .Replace("[LoginID]", user.EmailID)
                    .Replace("[Year]", Convert.ToString(user.Year))
                    .Replace("[ECS]", appSettings.AppSettings.EmailTemplate.ECS)
                    .Replace("[eExam2Delivery]", appSettings.AppSettings.EmailTemplate.EExam2Delivery)
                    .Replace("[eExam2Monitoring]", appSettings.AppSettings.EmailTemplate.EExam2Monitoring)
                    .Replace("[eExam2System]", appSettings.AppSettings.EmailTemplate.EExam2System)
                    .Replace("[Defaultpwd]", user.PassPhrase);
                message.IsBodyHtml = true;
                message.Priority = MailPriority.High;

                logger.LogInformation("End of Build User Registration Template");
            }
            catch (Exception ex)
            {
                logger.LogInformation("Error while Build User Registration Template", ex);
            }
            return message;
        }

        private bool SendEmail(MailMessage message)
        {
            bool Status = false;
            try
            {
                // Send Email                

                bool DirectoryPickUpEnabled = Convert.ToString(appSettings.AppSettings.EmailConfig.DirectoryPickup).ToUpper() == "TRUE"; // Read it from Config
                if (!DirectoryPickUpEnabled)
                {
                    logger.LogInformation("Begin To Send EMail");
                    SmtpClient s = new SmtpClient(appSettings.AppSettings.EmailConfig.SMTPServer);
                    s.Host = appSettings.AppSettings.EmailConfig.SMTPServer;
                    if (Convert.ToString(appSettings.AppSettings.EmailConfig.SMTPPort) != string.Empty)
                        s.Port = Convert.ToInt32(appSettings.AppSettings.EmailConfig.SMTPPort);
                    else
                        s.Port = 25;
                    s.Credentials = new NetworkCredential(Convert.ToString(appSettings.AppSettings.EmailConfig.NetworkUserName),
                        Convert.ToString(appSettings.AppSettings.EmailConfig.NetworkPwd));
                    if (Convert.ToString(appSettings.AppSettings.EmailConfig.SMTPSSL) != string.Empty)
                    {
                        s.EnableSsl = Convert.ToBoolean(appSettings.AppSettings.EmailConfig.SMTPSSL);
                    }
                    else
                        s.EnableSsl = false;

                    s.Send(message);
                    Status = true;
                    logger.LogInformation("End of Sending EMail to Users");
                }
                else
                {
                    using (SmtpClient client = new SmtpClient())
                    {
                        logger.LogInformation("Begin To Save EMail to PickupDirectory");
                        client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                        client.Send(message);
                        Status = true;
                        logger.LogInformation("End of Saving EMail to PickupDirectory");
                    }
                }
            }
            catch (SmtpFailedRecipientException ex)
            {
                logger.LogError(ex.Message, ex);
                Status = false;
            }
            catch (Exception ex)
            {
                logger.LogInformation("Error while sending Email", ex);
                Status = false;
            }
            return Status;
        }
    }
}
