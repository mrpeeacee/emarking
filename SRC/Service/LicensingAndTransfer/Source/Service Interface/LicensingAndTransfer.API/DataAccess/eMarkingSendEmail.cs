using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace LicensingAndTransfer.API
{
    public class eMarkingSendEmail
    {
        private int commandTimeout = 0;

        public eMarkingSendEmail()
        {
            commandTimeout = 2147483647;
        }

        public List<SendMailResponseModel> EMarkingSendEmail(long? QueueId)
        {
            string Status = String.Empty;
            List<EmailUsers> UsersList = new List<EmailUsers>();
            List<SendMailResponseModel> sendMailResponseModels = new List<SendMailResponseModel>();
            EmailUsers test = new EmailUsers();
            DataTable datatbl = new DataTable();
            bool isEmailSent;
            try
            {
                Constants.Log.Info("Begin EMarkingSendEmail()");

                // Get Users
                using (System.Data.SqlClient.SqlConnection connectioneMarking = CommonDAL.GeteMarkingConnection())
                {
                    Constants.Log.Info("Begin To Get EMail Users");
                    connectioneMarking.Open();
                    using (System.Data.SqlClient.SqlCommand commandSynceMarkingUser = new SqlCommand("[Marking].[USPGetMailUsers]"))
                    {
                        commandSynceMarkingUser.CommandTimeout = commandTimeout;
                        commandSynceMarkingUser.Connection = connectioneMarking;
                        commandSynceMarkingUser.CommandType = CommandType.StoredProcedure;
                        if (QueueId != null)
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
                                TemplateName = reader["TemplateName"].ToString(),
                                ToLoginID = reader["LoginID"].ToString(),
                                PassPhrase = reader["PassPharseCode"].ToString(),
                                Year = int.Parse(reader["Year"].ToString())
                            };
                            UsersList.Add(test);
                        }
                        if (!reader.IsClosed) { reader.Close(); }
                    }
                    connectioneMarking.Close();
                    Constants.Log.Info("End of Getting EMail Users");
                }

                if (UsersList.Count > 0)
                {
                    foreach (var user in UsersList)
                    {
                        // Send Email
                        Constants.Log.Info("Begin To Send EMail");
                        MailMessage message = new MailMessage();
                        Regex validemail = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.IgnoreCase);

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
                                Constants.Log.Info("Sending EMail Failed :" + user.ToLoginID);
                            }
                        }
                        else
                        {
                            sendMailResponseModels.Add(new SendMailResponseModel()
                            {
                                ID = user.ID,
                                IsMailSent = false
                            });
                            Constants.Log.Info("Sending EMail is Null or Invalid EMail format.");
                        }
                        Constants.Log.Info("End of Sending EMail");
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

                    Constants.Log.Info("Begin to update IsMailSent");
                    using (System.Data.SqlClient.SqlConnection connectioneMarking = CommonDAL.GeteMarkingConnection())
                    {
                        using (System.Data.SqlClient.SqlCommand command = new SqlCommand("[Marking].[USPUpdateMailUsers]", connectioneMarking))
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
                        Constants.Log.Info("End of update IsMailSent");
                    }
                }
            }
            catch (Exception ex)
            {
                Constants.Log.Error(ex.Message, ex);
                Status = "Error Found";
            }
            return sendMailResponseModels;
        }

        private MailMessage BuildUserRegistrationTemplate(EmailUsers user)
        {
            MailMessage message = new MailMessage();
            try
            {
                Constants.Log.Info("Begin To Build User Registration Template");

                message.From = new MailAddress(ConfigurationManager.AppSettings.Get("From"));
                message.To.Add(new MailAddress(user.EmailID));
                if (user.TemplateName != "WARNINGMAIL")
                {
                    message.Subject = ConfigurationManager.AppSettings.Get("SystemloginSubject");
                }
                else if (user.TemplateName == "WARNINGMAIL")
                {
                    message.Subject = ConfigurationManager.AppSettings.Get("UserloginstatusSubject");
                }
                message.Body = user.TemplateBody.Replace("[FirstName]", user.FirstName)
                    .Replace("[LoginID]", user.EmailID)
                    .Replace("[Year]", Convert.ToString(user.Year))
                    .Replace("[ECS]", ConfigurationManager.AppSettings.Get("ECS"))
                    .Replace("[eExam2Delivery]", ConfigurationManager.AppSettings.Get("eExam2Delivery"))
                    .Replace("[eExam2Monitoring]", ConfigurationManager.AppSettings.Get("eExam2Monitoring"))
                    .Replace("[eExam2System]", ConfigurationManager.AppSettings.Get("eExam2System"))
                    .Replace("[Defaultpwd]", user.PassPhrase);
                message.IsBodyHtml = true;
                message.Priority = MailPriority.High;

                Constants.Log.Info("End of Build User Registration Template");
            }
            catch (Exception ex)
            {
                Constants.Log.Info("Error while Build User Registration Template", ex);
            }
            return message;
        }

        private bool SendEmail(MailMessage message)
        {
            bool Status = false;
            try
            {
                // Send Email

                bool DirectoryPickUpEnabled = Convert.ToString(ConfigurationManager.AppSettings["DirectoryPickup"]).ToUpper() == "TRUE"; // Read it from Config
                if (!DirectoryPickUpEnabled)
                {
                    Constants.Log.Info("Begin To Send EMail");
                    SmtpClient s = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"]);
                    s.Host = ConfigurationManager.AppSettings["SMTPServer"];
                    if (ConfigurationManager.AppSettings.Get("SMPTPort") != string.Empty)
                        s.Port = Convert.ToInt32(ConfigurationManager.AppSettings.Get("SMTPPort"));
                    else
                        s.Port = 25;
                    s.Credentials = new NetworkCredential(ConfigurationManager.AppSettings.Get("NetworkUserName"),
                        ConfigurationManager.AppSettings.Get("NetworkPwd"));
                    if (ConfigurationManager.AppSettings.Get("SMTPSSL") != string.Empty)
                    {
                        s.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("SMTPSSL"));
                    }
                    else
                        s.EnableSsl = false;

                    s.Send(message);
                    Status = true;
                    Constants.Log.Info("End of Sending EMail to Users");
                }
                else
                {
                    using (SmtpClient client = new SmtpClient())
                    {
                        Constants.Log.Info("Begin To Save EMail to PickupDirectory");
                        client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                        client.Send(message);
                        Status = true;
                        Constants.Log.Info("End of Saving EMail to PickupDirectory");
                    }
                }
            }
            catch (SmtpFailedRecipientException ex)
            {
                Constants.Log.Error(ex.Message, ex);
                Status = false;
            }
            catch (Exception ex)
            {
                Constants.Log.Info("Error while sending Email", ex);
                Status = false;
            }
            return Status;
        }
    }

    public class SendEmailtoDeactivate
    {
        private int commandTimeout = 0;

        public SendEmailtoDeactivate()
        {
            commandTimeout = 2147483647;
        }

        public List<MailDeactivateModel> sendEmailtoDeactivate()
        {
            List<UserDeactivateDetails> UsersList = new List<UserDeactivateDetails>();
            List<MailDeactivateModel> sendMailResponseModels = new List<MailDeactivateModel>();
            try
            {
                Constants.Log.Info("Begin SendEmailtoDeactivate()");

                using (System.Data.SqlClient.SqlConnection connectioneMarking = CommonDAL.GeteMarkingConnection())
                {
                    Constants.Log.Info("Begin To Update Users Activity Status.");
                    connectioneMarking.Open();
                    using (System.Data.SqlClient.SqlCommand command = new SqlCommand("[Marking].[USPUpdateUserActivityStatus]", connectioneMarking))

                    {
                        command.CommandTimeout = commandTimeout;
                        command.Connection = connectioneMarking;
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@WarningDuration", SqlDbType.BigInt).Value = Convert.ToInt64(System.Configuration.ConfigurationManager.AppSettings["UserWarningDuration"].ToString());
                        command.Parameters.Add("@DisableDuration", SqlDbType.BigInt).Value = Convert.ToInt64(System.Configuration.ConfigurationManager.AppSettings["UserDisableDuration"].ToString());

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            UserDeactivateDetails test = new UserDeactivateDetails
                            {
                                UserID = reader["UserID"].ToString()
                            };
                            UsersList.Add(test);
                        }
                        if (!reader.IsClosed) { reader.Close(); }
                    }

                    connectioneMarking.Close();
                    Constants.Log.Info("End of Updating Users Activity Status.");
                }
            }
            catch (Exception ex)
            {
                Constants.Log.Error(ex.Message, ex);
                string Status = "Error Found";
            }
            return sendMailResponseModels;
        }
    }
}