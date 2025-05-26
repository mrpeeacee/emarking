using LicensingAndTransfer.API.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LicensingAndTransfer.API.Libraries;

namespace LicensingAndTransfer.API.DataAccess
{
    public class IntegrationDAL : IDisposable
    {
        /// <summary>
        /// Dispose the objects created in the instance
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Method use to validate the date time object, and then construct the valid string format and return it
        /// </summary>
        /// <param name="objInput">Date Time object</param>
        /// <returns></returns>
        public object SkipInvalidDate(DateTime objInput)
        {
            if (objInput == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                return DBNull.Value;
            else
                return objInput;
        }

        public Int64 savefile(string Code, string location, string name, Int64 filesize, Int64 userid)
        {
            Int64 FileID = 0;
            System.Data.DataTable dtFile = new System.Data.DataTable();
            dtFile.Columns.Add("CODE", typeof(String));
            dtFile.Columns.Add("LOCATION", typeof(String));
            dtFile.Columns.Add("NAME", typeof(String));
            dtFile.Columns.Add("FILESIZE", typeof(Int64));
            dtFile.Columns.Add("USERID", typeof(Int64));
            //dtFile.Columns.Add("ASSETNAME",typeof(String)); dtFile.Columns.Add("ASSETTYPECODE", typeof(String)); dtFile.Columns.Add("TYPE", typeof(String)); dtFile.Columns.Add("REPOSITORYID", typeof(Int64));
            dtFile.Columns.Add("ValidateSpace", typeof(bool));
            dtFile.Columns.Add("FILEID", typeof(Int64));
            dtFile.Columns.Add("Status", typeof(String));


            System.Data.DataRow drFile = dtFile.NewRow();
            drFile["CODE"] = Code;
            drFile["LOCATION"] = location;
            drFile["NAME"] = name;
            drFile["FILESIZE"] = filesize;
            drFile["USERID"] = userid;
            //drFile["ASSETNAME"] = dtAssistant; drFile["ASSETTYPECODE"] = objrequest.admission.AdmittedBy; drFile["TYPE"] = 38;//objrequest.admission.CandidateSignature; drFile["REPOSITORYID"] = 43; //objrequest.admission.CandidatePhoto;
            drFile["ValidateSpace"] = false;

            dtFile.Rows.Add(drFile);

            using (System.Data.SqlClient.SqlConnection objConnection = new System.Data.SqlClient.SqlConnection(Constants.DBConnectionString))
            {
                objConnection.Open();
                using (System.Data.SqlClient.SqlDataAdapter dataAdapter = new System.Data.SqlClient.SqlDataAdapter())
                {
                    dataAdapter.InsertCommand = new System.Data.SqlClient.SqlCommand();
                    dataAdapter.InsertCommand.Connection = objConnection;
                    dataAdapter.InsertCommand.CommandTimeout = Constants.DBConnectionTimeout;
                    dataAdapter.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    dataAdapter.InsertCommand.CommandText = "[dbo].[InsertFileDetails]";
                    dataAdapter.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.OutputParameters;
                    dataAdapter.UpdateBatchSize = 50;
                    dataAdapter.InsertCommand.Parameters.Add("@CODE", System.Data.SqlDbType.NVarChar, int.MaxValue, "CODE");
                    dataAdapter.InsertCommand.Parameters.Add("@FILESIZE", System.Data.SqlDbType.BigInt, int.MaxValue, "FILESIZE");
                    dataAdapter.InsertCommand.Parameters.Add("@LOCATION", System.Data.SqlDbType.NVarChar, int.MaxValue, "LOCATION");//Need to check
                    dataAdapter.InsertCommand.Parameters.Add("@NAME", System.Data.SqlDbType.NVarChar, int.MaxValue, "NAME");
                    dataAdapter.InsertCommand.Parameters.Add("@USERID", System.Data.SqlDbType.BigInt, int.MaxValue, "USERID");
                    // dataAdapter.InsertCommand.Parameters.Add("@TYPE", System.Data.SqlDbType.NVarChar, -1, "TYPE"); dataAdapter.InsertCommand.Parameters.Add("@ASSETTYPECODE", System.Data.SqlDbType.BigInt, int.MaxValue, "ASSETTYPECODE");//Need to check dataAdapter.InsertCommand.Parameters.Add("@ASSETNAME", System.Data.SqlDbType.BigInt, int.MaxValue, "ASSETNAME"); dataAdapter.InsertCommand.Parameters.Add("@REPOSITORYID", System.Data.SqlDbType.DateTime, int.MaxValue, "REPOSITORYID");
                    dataAdapter.InsertCommand.Parameters.Add("@ValidateSpace", System.Data.SqlDbType.Bit, int.MaxValue, "ValidateSpace");
                    dataAdapter.InsertCommand.Parameters.Add("@FILEID", System.Data.SqlDbType.BigInt, int.MaxValue, "FILEID").Direction = System.Data.ParameterDirection.Output;
                    dataAdapter.InsertCommand.Parameters.Add("@Status", System.Data.SqlDbType.NVarChar, 10, "Status").Direction = System.Data.ParameterDirection.Output;

                    dataAdapter.Update(dtFile);
                }
                if (dtFile != null && dtFile.Rows != null && dtFile.Rows.Count > 0)
                    FileID = (dtFile.Rows[0]["FILEID"] == DBNull.Value) ? 0 : Convert.ToInt64(dtFile.Rows[0]["FILEID"].ToString());
                objConnection.Close();
            }
            return FileID;
        }

        public Int64 writeToRepository(string CandidatePhoto, string fileRelativePath, string AppointmentID, string name)
        {
            Repository _objrepository = new Repository();
            CandidatePhoto = LicensingAndTransfer.API.Libraries.TestPlayer.Cryptography.DecryptString(CandidatePhoto);
            fileRelativePath = @"ContentPackages\FileUDDResource\" + AppointmentID + "\\" + name + ".jpg";
            _objrepository.ConvertFromBase64(CandidatePhoto, fileRelativePath);
            return savefile(System.Guid.NewGuid().ToString(), fileRelativePath, name + ".jpg", CandidatePhoto.Length, 1);
        }

        /// <summary>
        /// Save the information to database
        /// </summary>
        /// <param name="request">Request Object</param>
        /// <param name="Module">Module Name</param>
        /// <param name="MethodType">Method Type - Insert / Update / Delete / Select</param>
        /// <param name="StartTime">Start Time of the method call</param>
        /// <param name="EndTime">Method execution completed time</param>
        /// <param name="ErrorMsg">Error details captured during method execution</param>
        /// <param name="status">Final status of method execution</param>
        /// <param name="response">Response Object</param>
        /// <param name="logID">Log Identity</param>
        /// <returns></returns>
        public Int64 SaveLog(string request, string Module, string MethodType, DateTime StartTime, DateTime EndTime, string ErrorMsg, string status, string response, Int64 logID)
        {
            Int64 WebServicelogID = -1;

            System.Data.DataTable dtLog = new System.Data.DataTable();
            dtLog.Columns.Add("RequestObject", typeof(String)); dtLog.Columns.Add("Module", typeof(String)); dtLog.Columns.Add("MethodType", typeof(String));
            dtLog.Columns.Add("StartDate", typeof(DateTime)); dtLog.Columns.Add("EndDate", typeof(DateTime)); dtLog.Columns.Add("ErrorDetails", typeof(String));
            dtLog.Columns.Add("Status", typeof(String)); dtLog.Columns.Add("WebServicelogID", typeof(Int64)); dtLog.Columns.Add("ResponseObject", typeof(String));

            System.Data.DataRow drLog = dtLog.NewRow();
            drLog["RequestObject"] = request; drLog["Module"] = Module; drLog["MethodType"] = MethodType; drLog["StartDate"] = StartTime;
            drLog["EndDate"] = EndTime; drLog["ErrorDetails"] = ErrorMsg; drLog["Status"] = status; drLog["WebServicelogID"] = logID;
            drLog["ResponseObject"] = response;
            dtLog.Rows.Add(drLog);

            using (System.Data.SqlClient.SqlConnection objConnection = new System.Data.SqlClient.SqlConnection(Constants.DBConnectionString))
            {
                objConnection.Open();
                using (System.Data.SqlClient.SqlDataAdapter dataAdapter = new System.Data.SqlClient.SqlDataAdapter())
                {
                    dataAdapter.InsertCommand = new System.Data.SqlClient.SqlCommand();
                    dataAdapter.InsertCommand.Connection = objConnection;
                    dataAdapter.InsertCommand.CommandTimeout = Constants.DBConnectionTimeout;
                    dataAdapter.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    dataAdapter.InsertCommand.CommandText = "[dbo].[UspUpsertWebServiceLog]";
                    dataAdapter.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.OutputParameters;
                    dataAdapter.UpdateBatchSize = 50;
                    dataAdapter.InsertCommand.Parameters.Add("@RequestObject", System.Data.SqlDbType.NVarChar, int.MaxValue, "RequestObject");
                    dataAdapter.InsertCommand.Parameters.Add("@Module", System.Data.SqlDbType.NVarChar, int.MaxValue, "Module");
                    dataAdapter.InsertCommand.Parameters.Add("@MethodType", System.Data.SqlDbType.NVarChar, int.MaxValue, "MethodType");
                    dataAdapter.InsertCommand.Parameters.Add("@StartDate", System.Data.SqlDbType.DateTime, int.MaxValue, "StartDate");
                    dataAdapter.InsertCommand.Parameters.Add("@EndDate", System.Data.SqlDbType.DateTime, int.MaxValue, "EndDate");
                    dataAdapter.InsertCommand.Parameters.Add("@ErrorDetails", System.Data.SqlDbType.NVarChar, int.MaxValue, "ErrorDetails");
                    dataAdapter.InsertCommand.Parameters.Add("@Status", System.Data.SqlDbType.NVarChar, int.MaxValue, "Status");
                    dataAdapter.InsertCommand.Parameters.Add("@WebServicelogID", System.Data.SqlDbType.BigInt, int.MaxValue, "WebServicelogID").Direction = System.Data.ParameterDirection.InputOutput;
                    dataAdapter.InsertCommand.Parameters.Add("@ResponseObject", System.Data.SqlDbType.NVarChar, int.MaxValue, "ResponseObject");
                    dataAdapter.Update(dtLog);
                }

                if (dtLog != null && dtLog.Rows != null && dtLog.Rows.Count > 0)
                    WebServicelogID = (dtLog.Rows[0]["WebServicelogID"] == DBNull.Value) ? 0 : Convert.ToInt64(dtLog.Rows[0]["WebServicelogID"].ToString());
                objConnection.Close();
            }
            return WebServicelogID;
        }
    }
}