using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using LicensingAndTransfer.ServiceContracts;
using LicensingAndTransfer.DataContracts;
using System.DirectoryServices;
using System.ServiceModel.Activation;

namespace LicensingAndTransfer.ServiceImplementation
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Name = "LicensingAndTransfer", Namespace = "http://LicensingAndTransfer.ServiceContracts/2010/01")]
    public class LicensingAndTransfer : ILicensingAndTransfer, IAttendance, IApplicationLicensing
    {
        public static Logger Log = new Logger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region ILicensingAndTransfer Members

        public ServiceContracts.TestCenterRegistrationResponse TestCenterRegistration(ServiceContracts.TestCenterRegistrationRequest request)
        {
            Log.LogInfo("------------------------------------ START -------------------------------------");
            Log.LogInfo("Begin TestCenterRegistration()");
            TestCenterRegistrationResponse objResponse = new TestCenterRegistrationResponse();
            try
            {
                (new TestCenterFactory()).UpdateTestCenters(request.Request);
                objResponse.Response = request.Request;
            }
            catch (Exception ex)
            {
                Log.LogError("Error TestCenterRegistration()", ex);
            }
            Log.LogInfo("End TestCenterRegistration()");
            Log.LogInfo("------------------------------------- END --------------------------------------");
            return objResponse;
        }

        public ServiceContracts.TestCenterTagResponse TagUserToTestCenter(ServiceContracts.TestCenterTagRequest request)
        {
            Log.LogInfo("------------------------------------ START -------------------------------------");
            Log.LogInfo("Begin TagUserToTestCenter()");
            TestCenterTagResponse objResponse = new TestCenterTagResponse();
            try
            {
                (new TestCenterFactory()).UpdateTagUserToTestCenter(request.Request);
                objResponse.Response = request.Request;
            }
            catch (Exception ex)
            {
                Log.LogError("Error TestCenterRegistration()", ex);
            }
            Log.LogInfo("End TestCenterRegistration()");
            Log.LogInfo("------------------------------------- END --------------------------------------");
            return objResponse;
        }

        public ServiceContracts.CreateFTPSessionResponse CreateFTPSession(ServiceContracts.CreateFTPSessionRequest request)
        {
            Log.LogInfo("------------------------------------ START -------------------------------------");
            Log.LogInfo("Begin CreateFTPSession()");
            ServiceContracts.CreateFTPSessionResponse objResponse = new CreateFTPSessionResponse();

            //  If the requesting server is valid, then transfer of files can happen.
            Log.LogInfo("Begin ValidateMacID()");
            objResponse.Status = (new GenericDAL()).ValidateMacID(request.MacID, request.ServerType);
            Log.LogInfo("End ValidateMacID() - Status: " + objResponse.Status.ToString());

            if (objResponse.Status == "S000" || objResponse.Status == "S004")
            {
                Log.LogInfo("End CreateFTPSession()");
                Log.LogInfo("------------------------------------- END --------------------------------------");
                return objResponse;
            }

            try
            {
                Log.LogInfo("Operation Type: " + request.Operation);
                Log.LogInfo("Server Type : " + request.ServerType);

                string PackageType = (request.Operation == Operations.QPackFetch || request.Operation == Operations.QPackTransfer) ? "QPack" : "RPack";
                request.GuidFTPName = PackageType;

                string PackageTransferMedium = request.TransferMedium.ToString();
                Log.LogInfo("Package Transfer Medium: " + PackageTransferMedium);

                string temporaryFolderPath = System.Configuration.ConfigurationManager.AppSettings["TemporaryFolder"].ToString();
                Log.LogInfo("Temporary Packages Path: " + temporaryFolderPath);
                string packageFolder = System.Configuration.ConfigurationManager.AppSettings["PackageFolder"].ToString();
                Log.LogInfo("PackageFolder: " + packageFolder);
                Log.LogInfo("GuidFTPName: " + request.GuidFTPName);

                if (request.Operation == Operations.QPackFetch || request.Operation == Operations.RPackFetch)
                {
                    #region Handling QPack Fetch and RPack Fetch Operation
                    Log.LogInfo("Begin SearchPackages()");
                    objResponse.ListPackageStatistics = (new PackageStatisticsFactory()).SearchPackages(request.LoadPackagesFromDCDToDCC, request.MacID, request.ServerType.ToString(), PackageType);
                    Log.LogInfo("End SearchPackages() - Searched Packages Count : " + ((objResponse.ListPackageStatistics == null) ? "null" : objResponse.ListPackageStatistics.Count.ToString()));

                    if (objResponse.ListPackageStatistics != null && objResponse.ListPackageStatistics.Count > 0)
                    {
                        objResponse.Status = "Success";

                        #region Setting up the status for each package
                        for (int i = 0; i < objResponse.ListPackageStatistics.Count; i++)
                        {
                            Log.LogInfo("Package Path : " + objResponse.ListPackageStatistics[i].PackagePath);
                            Log.LogInfo("Package Name : " + objResponse.ListPackageStatistics[i].PackageName);
                            if (request.ServerType == ServerTypes.DataServer && objResponse.ListPackageStatistics[i].IsCentralizedPackage == true)
                            {
                                objResponse.ListPackageStatistics[i].TransferredToDataCenterCentralized = true;
                                objResponse.ListPackageStatistics[i].TransferredToDataCenterCentralizedOn = DateTime.UtcNow;
                            }
                            else if (request.ServerType == ServerTypes.DataServer && objResponse.ListPackageStatistics[i].IsCentralizedPackage == false && request.LoadPackagesFromDCDToDCC == true)
                            {
                                objResponse.ListPackageStatistics[i].TransferredToDataCenterCentralized = true;
                                objResponse.ListPackageStatistics[i].TransferredToDataCenterCentralizedOn = DateTime.UtcNow;
                            }
                            else if (request.ServerType == ServerTypes.DataServer && objResponse.ListPackageStatistics[i].IsCentralizedPackage == false && request.LoadPackagesFromDCDToDCC == false)
                            {
                                objResponse.ListPackageStatistics[i].TransferredToDataCenterDistributed = true;
                                objResponse.ListPackageStatistics[i].TransferredToDataCenterDistributedOn = DateTime.UtcNow;
                            }
                            else if (request.ServerType == ServerTypes.TestCenter)
                            {
                                objResponse.ListPackageStatistics[i].TransferredToTestCenter = true;
                                objResponse.ListPackageStatistics[i].TransferredToTestCenterOn = DateTime.UtcNow;
                            }
                        }
                        #endregion

                        Log.LogInfo("Begin UpdatePackageStatistics()");
                        (new PackageStatisticsFactory()).UpdatePackageStatistics(objResponse.ListPackageStatistics);
                        Log.LogInfo("End UpdatePackageStatistics()");

                        Log.LogInfo("Calling MoveResources()");
                        (new Resource()).MoveResources(packageFolder, temporaryFolderPath, request.MacID, request.GuidFTPName, request.Operation, objResponse.ListPackageStatistics);
                        Log.LogInfo("End of Call MoveResources()");
                    }
                    #endregion
                }
                else
                // QPack and RPack Transfer/Upload - create session scenario
                {
                    #region Handling QPack Transfer and RPack Transfer
                    Log.LogInfo("Begin QPack and RPack Transfer/Upload");
                    if (request != null && request.ListPackageStatistics != null && request.ListPackageStatistics.Count > 0)
                    {
                        Log.LogInfo("Package Transfer Medium: " + PackageTransferMedium);
                        if (!(PackageTransferMedium != null && PackageTransferMedium.Length > 0))
                        {
                            Log.LogInfo("ERROR: Invalid Transfer Medium");
                            throw new ArgumentException("Invalid Transfer Medium");
                        }
                        objResponse.Status = "Success";

                        if (objResponse.Status == "Success" || objResponse.Status == "S001")
                        {
                            Log.LogInfo("Begin - Set the requested packages to the response object");
                            objResponse.ListPackageStatistics = request.ListPackageStatistics;
                            Log.LogInfo("End - Set the requested packages to the response object");

                            #region Setting up the status for each package
                            if (objResponse.ListPackageStatistics != null && objResponse.ListPackageStatistics.Count > 0)
                                for (int i = 0; i < objResponse.ListPackageStatistics.Count; i++)
                                {
                                    objResponse.ListPackageStatistics[i].TransferredToDataExchangeServer = true;
                                    objResponse.ListPackageStatistics[i].TransferredToDataExchangeServerOn = DateTime.UtcNow;
                                }
                            #endregion

                            Log.LogInfo("Begin UpdatePackageStatistics()");
                            (new PackageStatisticsFactory()).UpdatePackageStatistics(objResponse.ListPackageStatistics);
                            Log.LogInfo("End UpdatePackageStatistics()");
                        }
                    }
                    Log.LogInfo("End QPack and RPack Transfer/Upload");
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Error CreateFTPSession()", ex);
            }
            Log.LogInfo("End CreateFTPSession()");
            Log.LogInfo("------------------------------------- END --------------------------------------");
            return objResponse;
        }

        public ServiceContracts.CompleteFTPSessionResponse CompleteFTPSession(ServiceContracts.CompleteFTPSessionRequest request)
        {
            Log.LogInfo("------------------------------------ START -------------------------------------");
            Log.LogInfo("Begin CompleteFTPSession()");
            ServiceContracts.CompleteFTPSessionResponse objResponse = new CompleteFTPSessionResponse();
            try
            {
                string PackageTransferMedium = request.TransferMedium.ToString();
                Log.LogInfo("Package Transfer Medium: " + PackageTransferMedium);

                string TemporaryFolder = System.Configuration.ConfigurationManager.AppSettings["TemporaryFolder"].ToString();
                Log.LogInfo("Temporary Folder Path: " + TemporaryFolder);
                string packageFolder = System.Configuration.ConfigurationManager.AppSettings["PackageFolder"].ToString();
                Log.LogInfo("PackageFolder: " + packageFolder);

                request.GuidFTPName = request.Operation.ToString().IndexOf("QPack", StringComparison.CurrentCultureIgnoreCase) >= 0 ? "QPack" : "RPack";

                if (!(PackageTransferMedium != null && PackageTransferMedium.Length > 0))
                {
                    Log.LogInfo("ERROR: Invalid Transfer Medium");
                    throw new ArgumentException("Invalid Transfer Medium");
                }

                if (request.ListPackageStatistics != null && request.ListPackageStatistics.Count > 0)
                {
                    if (request.Operation == Operations.QPackTransfer || request.Operation == Operations.RPackTransfer)
                    {
                        Log.LogInfo("Begin MoveResources()");
                        (new Resource()).MoveResources(packageFolder, TemporaryFolder, request.MacID, request.GuidFTPName, request.Operation, request.ListPackageStatistics);
                        Log.LogInfo("End MoveResources()");
                    }

                    #region Deleting all files from the temp folder which are listed inside package; which is mandatory
                    string deleteFilePath = string.Empty;
                    Log.LogInfo("GuidFTPName: " + request.GuidFTPName);
                    Log.LogInfo("Operation: " + request.Operation);
                    Log.LogInfo("Package Count : " + ((request.ListPackageStatistics == null) ? "0" : request.ListPackageStatistics.Count.ToString()));

                    //Deleting all files from the temp folder which are listed inside package
                    if (request.ListPackageStatistics != null)
                        foreach (PackageStatistics objPackageStatistics in request.ListPackageStatistics)
                        {
                            try
                            {
                                if (request.Operation == Operations.QPackTransfer || request.Operation == Operations.RPackTransfer)
                                {
                                    System.IO.File.Delete(TemporaryFolder + request.GuidFTPName + "\\" + objPackageStatistics.PackageName);
                                    Log.LogInfo("File deleted with path: " + TemporaryFolder + request.GuidFTPName + "\\" + objPackageStatistics.PackageName);
                                }
                                else
                                {
                                    deleteFilePath = System.IO.Path.Combine(TemporaryFolder, request.GuidFTPName, request.MacID.Replace(":", "-"));
                                    System.IO.File.Delete(System.IO.Path.Combine(deleteFilePath + "\\" + objPackageStatistics.PackageName));
                                    if (System.IO.Directory.GetFiles(deleteFilePath, "*.*").Length <= 0)
                                        System.IO.Directory.Delete(deleteFilePath, true);//if no files are present then delete the MACID folder
                                    Log.LogInfo("File deleted with path: " + deleteFilePath + "\\" + objPackageStatistics.PackageName);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.LogError("File couldn't be deleted with path: " + TemporaryFolder + request.GuidFTPName + "\\" + objPackageStatistics.PackageName, ex);
                            }
                        }
                    #endregion

                    Log.LogInfo("Begin - Set the requested packages to the response object");
                    objResponse.ListPackageStatistics = request.ListPackageStatistics;
                    Log.LogInfo("End - Set the requested packages to the response object");

                    Log.LogInfo("Begin UpdatePackageStatistics()");
                    (new PackageStatisticsFactory()).UpdatePackageStatistics(objResponse.ListPackageStatistics);
                    Log.LogInfo("End UpdatePackageStatistics()");
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Error CompleteFTPSession()", ex);
            }
            Log.LogInfo("End CompleteFTPSession()");
            Log.LogInfo("------------------------------------- END --------------------------------------");
            return objResponse;
        }

        public void PackageReTransferOrReGenerate(ServiceContracts.PackageReTransferReGenerateRequest request)
        {
            Log.LogInfo("------------------------------------ START -------------------------------------");
            Log.LogInfo("Begin PackageReTransferOrReGenerate()");
            (new PackageStatisticsFactory()).PackageReTransferOrReGenerate(request.ListPackageDetails);
            Log.LogInfo("End PackageReTransferOrReGenerate()");
            Log.LogInfo("------------------------------------- END --------------------------------------");
        }

        public void UpdatePackageStatistics(ServiceContracts.UpdatePackageStatisticsRequest request)
        {
            Log.LogInfo("------------------------------------ START -------------------------------------");
            Log.LogInfo("Begin UpdatePackageStatistics() - Service Call");
            (new PackageStatisticsFactory()).UpdatePackageStatistics(request.ListPackageStatistics);
            Log.LogInfo("End UpdatePackageStatistics() - Service Call");
            Log.LogInfo("------------------------------------- END --------------------------------------");
        }

        public void UpdateAssessmentStatistics(ServiceContracts.UpdateAssessmentStatisticsRequest request)
        {
            Log.LogInfo("------------------------------------ START -------------------------------------");
            Log.LogInfo("Begin UpdateAssessmentStatistics() - Service Call");
            (new AssessmentStatisticsFactory()).UpdateAssessmentStatistics(request.ListAssessmentStatistics);
            Log.LogInfo("End UpdateAssessmentStatistics() - Service Call");
            Log.LogInfo("------------------------------------- END --------------------------------------");
        }

        public void UpdateTestCenterAssessmentPacks(ServiceContracts.UpdateTestCenterAssessmentPacksRequest request)
        {
            Log.LogInfo("------------------------------------ START -------------------------------------");
            Log.LogInfo("Begin UpdateTestCenterAssessmentPacks() - Service Call");
            (new AssessmentStatisticsFactory()).UpdateTestCenterAssessmentPacks(request.ListTestCenterAssessmentPacks);
            Log.LogInfo("End UpdateTestCenterAssessmentPacks() - Service Call");
            Log.LogInfo("------------------------------------- END --------------------------------------");
        }

        public ServiceContracts.SearchAssessmentStatisticsResponse SearchAssessmentStatistics(ServiceContracts.SearchAssessmentStatisticsRequest request)
        {
            Log.LogInfo("------------------------------------ START -------------------------------------");
            Log.LogInfo("Begin SearchAssessmentStatistics() - Service Call");
            ServiceContracts.SearchAssessmentStatisticsResponse objResponse = new SearchAssessmentStatisticsResponse();
            objResponse.AssessmentStatistics = (new AssessmentStatisticsFactory()).SearchAssessmentStatistics(request.MacID);
            Log.LogInfo("End SearchAssessmentStatistics() - Service Call");
            Log.LogInfo("------------------------------------- END --------------------------------------");
            return objResponse;
        }

        public void UpdatePackageLoadedDate(ServiceContracts.UpdatePackageLoadedDateRequest request)
        {
            Log.LogInfo("------------------------------------ START -------------------------------------");
            Log.LogInfo("Begin UpdatePackageLoadedDate() - Service Call");
            (new PackageStatisticsFactory()).UpdatePackageLoadedDate(request);
            Log.LogInfo("End UpdatePackageLoadedDate() - Service Call");
            Log.LogInfo("------------------------------------- END --------------------------------------");
        }

        //public void PersistDailyScheduleSummary(ServiceContracts.PersistDailyScheduleSummaryRequest request)
        //{
        //    Log.LogInfo("------------------------------------ START -------------------------------------");
        //    Log.LogInfo("Begin PersistDailyScheduleSummary() - Service Call");
        //    (new CommonDAL()).PersistDailyScheduleSummary(request.ListPersistDailyScheduleSummary);
        //    Log.LogInfo("End PersistDailyScheduleSummary() - Service Call");
        //    Log.LogInfo("------------------------------------- END --------------------------------------");
        //}

        public ServiceContracts.ValidateTestCenterResponse ValidateTestCenter(ServiceContracts.ValidateTestCenterRequest request)
        {
            ServiceContracts.ValidateTestCenterResponse objResponse = new ValidateTestCenterResponse();
            objResponse.StatusCode = (new GenericDAL()).ValidateMacID(request.MacID, request.ServerType);
            if (objResponse.StatusCode.Equals("S000"))
            {
                objResponse.Status = "Failure";
                objResponse.Message = "Invalid Server Type";
            }
            else if (objResponse.StatusCode.Equals("S001"))
            {
                objResponse.Status = "Success";
                objResponse.Message = "Valid Test Center";
            }
            else if (objResponse.StatusCode.Equals("S004"))
            {
                objResponse.Status = "Failure";
                objResponse.Message = "Invalid Test Center";
            }
            return objResponse;
        }

        public ServiceContracts.ValidateMSIInstallationResponse ValidateMSIInstallation(ServiceContracts.ValidateMSIInstallationRequest request)
        {
            ServiceContracts.ValidateMSIInstallationResponse objResponse = new ValidateMSIInstallationResponse();
            objResponse.StatusCode = (new GenericDAL()).ValidateMSIInstallation(request.MacID);
            if (objResponse.StatusCode.Equals("E001"))
            {
                objResponse.Status = "Failure";
                objResponse.Message = "Not Valid MACID";
            }
            else if (objResponse.StatusCode.Equals("S001"))
            {
                objResponse.Status = "Success";
                objResponse.Message = "Valid MSI Installation";
            }
            else if (objResponse.StatusCode.Equals("E002"))
            {
                objResponse.Status = "Failure";
                objResponse.Message = "MSI Already Installed";
            }
            return objResponse;
        }



        public ServiceContracts.DXUTCDateTimeResponse GetDXUTCDateTime()
        {
            ServiceContracts.DXUTCDateTimeResponse objResponse = new DXUTCDateTimeResponse();
            objResponse.UTCDateTime = DateTime.UtcNow;
            return objResponse;
        }

        public ServiceContracts.ScriptResponse SynchronizeDBData(ServiceContracts.ScriptRequestType request)
        {
            Log.LogInfo("------------------------------------ START -------------------------------------");
            Log.LogInfo("Begin PersistDailyScheduleSummary() - Service Call");
            ServiceContracts.ScriptResponse objResponse = new ScriptResponse();
            try
            {
                // objResponse.Script = System.Configuration.ConfigurationManager.AppSettings["SynchronizeScript"].ToString();
                (new CommonDAL()).SaveScript(request.Request);

            }
            catch (Exception ex)
            {
                Log.LogError("Error PersistDailyScheduleSummary() - Service Call", ex);
            }
            Log.LogInfo("End PersistDailyScheduleSummary() - Service Call");
            Log.LogInfo("------------------------------------- END --------------------------------------");
            return objResponse;
        }

        public void UpdateBatchEndTime(ServiceContracts.BatchEndTimeUpdateRequest request)
        {
            if (request != null && request.Batches != null && request.Batches.Count > 0)
            {
                (new CommonDAL()).UpdateBatchEndTime(request.Batches);
            }
        }

        public void ManageUser(ServiceContracts.UserRequest request)
        {
            if (request != null && request.UserProfile != null && request.UserProfile.Count > 0)
            {
                (new UserFactory()).UpsertUser(request.UserProfile);
            }
        }

        public void ManageRole(ServiceContracts.RoleRequest request)
        {
            if (request != null && request.UserRole != null && request.UserRole.Count > 0)
            {
                (new RoleFactory()).DeleteRolePrivilages(request.UserRole[0].RoleID);
                (new RoleFactory()).UpsertRole(request.UserRole);
            }
        }

        public void ManageOrganization(ServiceContracts.OrganizationRequest request)
        {
            try
            {

                if (request != null && request.Organization != null && request.Organization.Count > 0)
                {
                    (new OrganizationFactory()).Load(request.Organization);
                }
            }
            catch (System.Exception exe)
            {
                Log.LogError("Error while updating organizatin information:" + exe.Message, exe);
            }
        }

        public ServiceContracts.ScheduleResponse SynchronizeCandidates(ServiceContracts.ScheduleRequest request)
        {
            ServiceContracts.ScheduleResponse objResponse = new ScheduleResponse();
            Log.LogInfo("------------------------------------ START -------------------------------------");
            Log.LogInfo("Begin SynchronizeCandidates()");
            try
            {
                objResponse.ListScheduledDetails = (new CommonDAL()).SynchronizeCandidates(request.ListScheduledDetails);


            }
            catch (Exception ex)
            {
                Log.LogError("Error SynchronizeCandidates()", ex);
            }
            Log.LogInfo("End SynchronizeCandidates()");
            Log.LogInfo("------------------------------------- END --------------------------------------");
            return objResponse;
        }

        public void UpsertCandidateDetails(ServiceContracts.ScheduleRequest request)
        {
            Log.LogInfo("------------------------------------ START -------------------------------------");
            Log.LogInfo("Begin UpsertCandidateDetails()");
            try
            {
                (new CommonDAL()).UpsertCandidateDetails(request.ListScheduledDetails);
            }
            catch (Exception ex)
            {
                Log.LogError("Error UpsertCandidateDetails()", ex);
            }
            Log.LogInfo("End UpsertCandidateDetails()");
            Log.LogInfo("------------------------------------- END --------------------------------------");
        }

        public void ResetQPackTransferForTestCenter(string MacID, List<long> ScheduleIDs)
        {
            Log.LogInfo("------------------------------------ START -------------------------------------");
            Log.LogInfo("Begin ResetQPackTransferForTestCenter() MACID : " + MacID);
            try
            {
                (new TestCenterFactory()).ResetQPackTransfer(MacID, ScheduleIDs);
            }
            catch (Exception ex)
            {
                Log.LogError("Error ResetQPackTransferForTestCenter()", ex);
            }
            Log.LogInfo("End ResetQPackTransferForTestCenter() MACID : " + MacID);
            Log.LogInfo("------------------------------------- END --------------------------------------");
        }

        public GetNonExecutedScriptsResponseType GetScripts(GetNonExecutedScriptsRequestType request)
        {
            GetNonExecutedScriptsResponseType response = new GetNonExecutedScriptsResponseType();
            try
            {

                response = (new CommonDAL()).GetNonExecutedScript(request);
            }
            catch (Exception ex)
            {
                Log.LogError("Error ResetQPackTransferForTestCenter()", ex);
            }
            return response;
        }

        public void UpdateScripts(UpdateScriptsRequestType request)
        {
            try
            {
                (new CommonDAL()).UpdateScriptsData(request.Request);
            }
            catch (Exception ex)
            {
                Log.LogError("Error ResetQPackTransferForTestCenter()", ex);
            }
        }

        public bool CheckQPackInFTP(string FileName)
        {
            string packageFolder = System.Configuration.ConfigurationManager.AppSettings["PackageFolder"].ToString();
            FileName = System.IO.Path.Combine(packageFolder, FileName);
            return System.IO.File.Exists(FileName);
        }

        public ServiceContracts.HttpFileTransferResponse FileTransfer(ServiceContracts.HttpFileTransferRequest request)
        {
            ServiceContracts.HttpFileTransferResponse response = null;
            FileTransfer action = null;
            try
            {
                if(request.Request.path.EndsWith("QPack.zip") || request.Request.path.EndsWith("LPack.zip") || request.Request.path.EndsWith("RPack.zip"))
                {
                    action = new FileTransfer();
                    response = new ServiceContracts.HttpFileTransferResponse();
                    response.Response = action.DownloadFile(request.Request);
                }
            }
            catch (Exception ex)
            {
                Log.LogError("\nModule Name : ServiceImplementation;\nClass Name : InventoryManager;\nMethod Name : FileTransfer;\nError Message : " + ex.Message + "\n", ex);
            }
            finally
            {
                request = null;
            }
            return response;
        }

        public void UploadFile(ServiceContracts.HttpFileTransferRequest request)
        {
            FileTransfer action = null;
            try
            {
                if (request.Request.path.EndsWith(".zip"))
                {
                    action = new FileTransfer();
                    action.UploadFile(request.Request);
                }
            }
            catch (Exception ex)
            {
                Log.LogError("\nModule Name : ServiceImplementation;\nClass Name : InventoryManager;\nMethod Name : FileTransfer;\nError Message : " + ex.Message + "\n", ex);
            }
            finally
            {
                request = null;
            }
        }

        /// <summary>
        /// This method is for installing excelsoft softwares, just to check valid license key in "ApplicationLicesne" table
        /// When software is getting installed it will ask License Key, user will enter license key, and call service to validate
        /// if License key Matchs the keys in "ApplicationLicesnse" table and IsUsed=0 then its valid key, if no result comes then invalid key, 
        /// if IsUsed is already 1 then the license key is already used
        /// </summary>
        /// <param name="LicenseKey">License Key</param>
        /// <returns>Status - Valid - Invalid - Used</returns>
        public string CheckApplicationLicense(string LicenseKey)
        {
            string result = "FAILED";
            try
            {
                System.Data.SqlClient.SqlParameter[] param = new System.Data.SqlClient.SqlParameter[2];
                param[0] = (new System.Data.SqlClient.SqlParameter() { SqlDbType = System.Data.SqlDbType.NVarChar, ParameterName = "LicenseKey", Value = LicenseKey, Direction = System.Data.ParameterDirection.Input, Size = -1 });
                param[1] = (new System.Data.SqlClient.SqlParameter() { SqlDbType = System.Data.SqlDbType.NVarChar, ParameterName = "Status", Value = LicenseKey, Direction = System.Data.ParameterDirection.Output, Size = 10 });
                using (System.Data.SqlClient.SqlConnection con = CommonDAL.GetConnection())
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("GetApplciationLicense", con))
                {
                    con.Open();
                    cmd.Parameters.AddRange(param);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                switch (param[1].Value.ToString().ToUpper())
                {
                    case "L001":
                        result = "SUCCESS";
                        break;
                    case "L002":
                        result = "FAILED";
                        break;
                    case "L003":
                        result = "USED";
                        break;
                    default:
                        result = "FAILED";
                        break;
                }
                param = null;
            }
            catch (Exception ex)
            {
                Log.LogError("Error in CheckApplicationLicense() method", ex);
            }
            return result;
        }

        /// <summary>
        /// UpdateTestCenterSiteAppraisal - this method will update each workstations and test center hardware and 
        /// software details to Data exchange server for details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public TestCenterSiteAppraisalResponse UpdateTestCenterSiteAppraisal(TestCenterSiteAppraisalRequest request)
        {
            Log.LogInfo("Begin UpdateTestCenterSiteAppraisal()");
            TestCenterSiteAppraisalResponse objResponse = null;
            try
            {
                objResponse = (new CommonDAL()).UpdateTestCenterWorkStationDetails(request);
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, ex);
                objResponse = new TestCenterSiteAppraisalResponse() { Status = "S002", Message = "Error occured : " + ex.Message };
            }
            Log.LogInfo("End UpdateTestCenterSiteAppraisal()");
            return objResponse;
        }

        /// <summary>
        /// This method returns Organization details of particular mac address matching test center 
        /// if mac address doesnt match then all organizations will be returned for user
        /// to selcted organization to register new test center
        /// </summary>
        /// <param name="MacAddress"></param>
        /// <returns></returns>
        public OrganizationsForTestCenterAppraisalResponse GetOrganizationsForTestCenterAppraisal(String MacAddress)
        {
            Log.LogInfo("Begin GetOrganizationsForTestCenterAppraisal()");
            OrganizationsForTestCenterAppraisalResponse objResponse = null;
            try
            {
                objResponse = (new CommonDAL()).OrganizationsForTestCenterAppraisal(MacAddress);
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, ex);
            }
            Log.LogInfo("End GetOrganizationsForTestCenterAppraisal()");
            return objResponse;
        }

        public void UpdatePostInstallationStatus(ServiceContracts.Contracts.PostInstallationRequest request)
        {
            try
            {
                if (request != null && request.PostInstallationComponents != null && request.PostInstallationComponents.Count > 0)
                {
                    (new CommonDAL()).UpdatePostInstallationStatus(request.PostInstallationComponents);


                }
            }
            catch (System.Exception exe)
            {
                Log.LogInfo("Error in UpdatePostInstallationStatus" + exe.Message);
            }
        }

        public ServiceContracts.Contracts.SearchActivatedStatusForBatchResponse SearchActivatedStatusForBatch(ServiceContracts.Contracts.SearchActivatedStatusForBatchRequest request)
        {
            ServiceContracts.Contracts.SearchActivatedStatusForBatchResponse objResponse = new ServiceContracts.Contracts.SearchActivatedStatusForBatchResponse();
            Log.LogInfo("----------------------------START------------------------");
            Log.LogInfo("Begin SearchActivatedStatusForBatch()");
            try
            {
                objResponse.ListPackageStatistics = (new PackageStatisticsFactory()).SearchActivatedStatusForBatch(request.ListPackageStatistics);
            }
            catch (Exception ex)
            {
                Log.LogError("Error  SearchActivatedStatusForBatch()", ex);

            }
            Log.LogInfo("End SearchActivatedStatusForBatch()");
            Log.LogInfo("------------------------------------- END --------------------------------------");
            return objResponse;
        }

        public ServiceContracts.Contracts.ScannedResponse PersistScannedResponse(ServiceContracts.Contracts.ScannedResponseRequest request)
        {
            // List<ServiceContracts.Contracts.Contracts.ScannedResponse> ObjectScannedResponse = new List<ServiceContracts.Contracts.Contracts.ScannedResponse>();

            ServiceContracts.Contracts.ScannedResponse ObjectScannedResponse = new ServiceContracts.Contracts.ScannedResponse();
            try
            {
                if (request != null && request.ListScannedResponse != null && request.ListScannedResponse.Count > 0)
                {

                    ObjectScannedResponse.ListScannedResponse = (new CommonDAL()).PersistScannedResponse(request.ListScannedResponse);

                    //  ObjectScannedResponse = (new CommonDAL()).pers
                }
            }
            catch (System.Exception exe)
            {
                Log.LogError(exe.Message, exe);
                throw exe;
            }
            return ObjectScannedResponse;
        }

        public ServiceContracts.Contracts.TcBatchesResponse GetTestCenterBatch(ServiceContracts.Contracts.TcBatchesRequest request)
        {
            Log.LogInfo("---------------START--------------");
            Log.LogInfo("Begin QpackRpackDetailsForTcVerification()");

            ServiceContracts.Contracts.TcBatchesResponse objResponse = new ServiceContracts.Contracts.TcBatchesResponse();
            try
            {
                objResponse.ListTcBatches = (new GenericDAL()).GetTestCenterBatch(request);
                return objResponse;
            }
            catch (Exception ex)
            {
                Log.LogInfo("Error GetTestCenterBatch()", ex);
                Log.LogInfo(ex.StackTrace);
                return objResponse;
            }
            finally
            {
                Log.LogInfo("End GetTestCenterBatch()");
                Log.LogInfo("----------------END---------------");
            }
        }

        public ServiceContracts.Contracts.TcVerificationResponse QpackRpackDetailsForTcVerification(ServiceContracts.Contracts.TcVerificationRequest request)
        {
            Log.LogInfo("---------------START--------------");
            Log.LogInfo("Begin QpackRpackDetailsForTcVerification()");

            ServiceContracts.Contracts.TcVerificationResponse objResponse = new ServiceContracts.Contracts.TcVerificationResponse();
            try
            {
                objResponse.ListTcVerification = (new GenericDAL()).FetchQpackRpackDetailsForTcVerification(request);
                return objResponse;
            }
            catch (Exception ex)
            {
                Log.LogError("Error QpackRpackDetailsForTcVerification()", ex);

                return objResponse;
            }
            finally
            {
                Log.LogInfo("End QpackRpackDetailsForTcVerification()");
                Log.LogInfo("----------------END---------------");
            }
        }

        public ServiceContracts.Contracts.ResetQpackReceivedStatusResponse ResetTcQpackReceivedStatus(ServiceContracts.Contracts.ResetQpackReceivedStatusRequest request)
        {
            Log.LogInfo("---------------START--------------");
            Log.LogInfo("Begin ResetQpackReceivedStatus()");

            ServiceContracts.Contracts.ResetQpackReceivedStatusResponse objResponse = new ServiceContracts.Contracts.ResetQpackReceivedStatusResponse();
            try
            {
                objResponse.UpdatedStatus = (new GenericDAL()).ResetTcQpackReceivedStatus(request.MacID, request.Sid);

                return objResponse;
            }
            catch (Exception ex)
            {
                Log.LogError("Error ResetQpackReceivedStatus()", ex);
                Log.LogInfo(ex.StackTrace);
                objResponse.UpdatedStatus = -1;
                return objResponse;
            }
            finally
            {
                Log.LogInfo("End ResetQpackReceivedStatus()");
                Log.LogInfo("----------------END---------------");
            }
        }

        public ServiceContracts.Contracts.BubbleSheetDataResponse ExtractBubbleSheetData(ServiceContracts.Contracts.BubbleSheetDataRequest request)
        {
            Log.LogInfo("------START---------");
            Log.LogInfo("Begin Extract Data For Bubble Sheet ");
            ServiceContracts.Contracts.BubbleSheetDataResponse ObjectBubbleSheetResponse = new ServiceContracts.Contracts.BubbleSheetDataResponse();
            if (request != null && request.ScheduleUserID > 0)
            {
                try
                {

                    Log.LogInfo(request.ScheduleUserID.ToString());
                    ObjectBubbleSheetResponse = (new CommonDAL()).ExtractBubbleSheetData(request);
                }
                catch (System.ExecutionEngineException exe)
                {
                    Log.LogError(exe.Message);
                }
            }
            Log.LogInfo("End Extract Data For Bubble Sheet ");
            return ObjectBubbleSheetResponse;


        }

        public ServiceContracts.Contracts.OMRScannedResponse PersistOfflineScannedResponse(ServiceContracts.Contracts.ScannedResponseRequest request)
        {
            //List<ServiceContracts.Contracts.OMRScannedResponse> OMRScannedResponse = new List<ServiceContracts.Contracts.OMRScannedResponse>();
            ServiceContracts.Contracts.OMRScannedResponse objOMRScannedResponse = new ServiceContracts.Contracts.OMRScannedResponse();
            try
            {
                ServiceContracts.Contracts.ScannedResponse ObjectScannedResponse = new ServiceContracts.Contracts.ScannedResponse();
                Log.LogInfo("Scanned Response count :" + request.ListScannedResponse.Count);
                if (request != null && request.ListScannedResponse != null && request.ListScannedResponse.Count > 0)
                {

                    objOMRScannedResponse.ListOMRScannedResponseOutPut = (new CommonDAL()).PersistOfflineScannedResponse(request.ListScannedResponse);

                }
            }
            catch (System.Exception exe)
            {
                Log.LogError("Error:", exe);
                throw exe;

            }
            return objOMRScannedResponse;

        }

        public ServiceContracts.Contracts.ExtractAssessmentDetailsResponse ExtractAssessmentDetails(ServiceContracts.Contracts.ExtractAssessmentDetailsRequest request)
        {
            ServiceContracts.Contracts.ExtractAssessmentDetailsResponse objExtractAssessmentDetailsResponse = null;
            try
            {
                if (request != null && request.AssessmentField != null)


                    objExtractAssessmentDetailsResponse = (new CommonDAL()).ExtractAssessmentDetails(request);
            }
            catch
            {

            }
            return objExtractAssessmentDetailsResponse;
        }

        public ServiceContracts.Contracts.ExtractSummaryResponse ExtraxtSummaryDetails(ServiceContracts.Contracts.ExtractSummaryDetailsRequest request)
        {
            ServiceContracts.Contracts.ExtractSummaryResponse objResponse = new ServiceContracts.Contracts.ExtractSummaryResponse();

            if (request != null && request.SummaryRequest.StartDate.ToString() != string.Empty && request.SummaryRequest.EndDate.ToString() != string.Empty)
            {
                objResponse.lstSummaryDetailsResponse = (new CommonDAL().ExtractSummaryDetails(request));

            }
            return objResponse;
        }

        public void PersistOfflineScannedResponseForMobile(string response) //input partmeter will be string format
        {
            try
            {
                Log.LogInfo("Service call start");
                ServiceContracts.Contracts.OMRScannedResponse objOMRScannedResponse = new ServiceContracts.Contracts.OMRScannedResponse();
                List<ScannedResponse> lstScannedResponse = ExtractScannedResponse(response);
                objOMRScannedResponse.ListOMRScannedResponseOutPut = (new CommonDAL()).PersistOfflineScannedResponse(lstScannedResponse);
            }
            catch (System.Exception exe)
            {
                Log.LogError("Error:", exe);
                throw exe;
            }
        }

        public static List<ScannedResponse> ExtractScannedResponse(string response)
        {

            List<ScannedResponse> lstScannedResponse = new List<ScannedResponse>();


            int i, pos;


            {

                ScannedResponse objScannedResponse = new ScannedResponse();

                pos = response.IndexOf("#");
                i = response.IndexOf("$");
                //objScannedResponse.LoginName = line.Substring(pos + 1, i-(pos+1));
                objScannedResponse.LoginName = response.Substring(0, i);
                response = response.Replace("#", ",");
                objScannedResponse.ResponseString = response.Substring(i + 1).Trim();   //"0,2,3,1,1,3,1,3,3,2,1,3,2,3,1,3,2,3,4,2,3,2,3,1,2,2,4,1,2,2";
                objScannedResponse.SubjectCode = "";
                objScannedResponse.ScannedFileName = "Nofile";

                objScannedResponse.SheetID = objScannedResponse.LoginName; // + System.DateTime.Now.ToLongDateString() + DateTime.Now.Millisecond.ToString();
                lstScannedResponse.Add(objScannedResponse);

            }

            return lstScannedResponse;

        }

        public int VerifyEmailID(string EmailID)
        {
            int result = 0;
            try
            {
                Log.LogInfo("Verification of emailid:" + EmailID + " Starts");
                if (EmailID != null || EmailID != string.Empty)
                {

                    result = new CommonDAL().VerifyMailID(EmailID);
                }
                Log.LogInfo("Verification of emailid:" + EmailID + " Ends");

            }
            catch (System.Exception exe)
            {
                result = 0;
                Log.LogInfo("Error:" + exe.Message);
            }
            return result;

        }

        public int VerifyUser(string LoginName, string Password, bool IsEncrypted = true)
        {
            int Status = 0;

            try
            {
                Log.LogInfo("Verification of user :" + LoginName + " Starts");
                Status = new CommonDAL().VerifyUser(LoginName, Password, IsEncrypted);
                Log.LogInfo("Verification of user :" + LoginName + " Ends");
            }

            catch (System.Exception exe)
            {
                Log.LogInfo("Error:" + exe.Message);
                Status = 0;
            }
            return Status;
        }

        public void PersistOMRResponse(ServiceContracts.Contracts.ScannedResponseRequest Request)
        {
            try
            {
                if (Request != null)
                {

                    (new CommonDAL()).PersistOMRResponse(Request.ListScannedResponse);
                }
            }
            catch (System.Exception exe)
            {
                Log.LogError(exe.Message, exe);
                throw exe;
            }
        }

        public void DoOMRResponseProcessing()
        {

            try
            {
                (new CommonDAL()).DoOMRResponseProcessing();
            }

            catch (System.Exception exe)
            {
                Log.LogError(exe.Message, exe);
                throw exe;
            }
        }

        public void PersistPaperBasedResponseForMIIClient(ServiceContracts.Contracts.ScannedResponseRequest Request)
        {
            try
            {
                Log.LogInfo("PersistPaperBasedResponseForMIIClient starts()");
                if (Request != null)
                {

                    (new CommonDAL()).PersistOMRResponseForMIIClient(Request.ListScannedResponse);
                }
                Log.LogInfo("PersistPaperBasedResponseForMIIClient ends()");
            }
            catch (System.Exception exe)
            {
                Log.LogError(exe.Message, exe);
                throw exe;
            }
        }

        public void DoOMRResponseProcessingForMIIClient()
        {

            try
            {
                (new CommonDAL()).DoOMRResponseProcessingForMIIClient();
            }

            catch (System.Exception exe)
            {
                Log.LogError(exe.Message, exe);
            }
        }

        public void UpsertAttachmentTransferredState(ServiceContracts.Contracts.AttachmentDetailsRequest Request)
        {
            if (Request != null)
                try
                {
                    Log.LogInfo("Update of attachment transferred starts");
                    (new CommonDAL()).UpsertAttachmentTransferredState(Request.ListAttachmentDetails);
                    Log.LogInfo("Update of attachment transferred ends");
                }
                catch (System.Exception exe)
                {
                    Log.LogError(exe.Message, exe);

                    throw exe;
                }

        }

        public void UpdateAttachmentProcessedDetails(ServiceContracts.Contracts.AttachmentProccedDetailsRequest Request)
        {
            if (Request != null)
                try
                {
                    Log.LogInfo("Update of attachment details starts");
                    (new CommonDAL()).UpsertAttachmentProcessedDetails(Request.ListAttachmetnProcessDetails);
                    Log.LogInfo("Update of attachment details ends");
                }
                catch (System.Exception exe)
                {
                    Log.LogInfo(exe.Message, exe);
                    throw exe;
                }
        }

        public void UpdateMobileBasedOMRPath(System.Data.DataTable tblOMR)
        {
            try
            {
                Log.LogInfo("Update of omr image path starts");
                if (tblOMR != null && tblOMR.Rows.Count > 0)
                {
                    (new CommonDAL()).UpdateMobileBasedOMRPath(tblOMR);
                }
                Log.LogInfo("Update of omr image path ends");
            }
            catch (System.Exception exe)
            {
                Log.LogError(exe.Message, exe);
                throw exe;
            }
        }

        public ServiceContracts.CourseResponse UpsertCourse(ServiceContracts.CourseRequest request)
        {

            Log.LogInfo("---Roster Request starts");
            ServiceContracts.CourseResponse response = new CourseResponse();
            if (request != null && request.LstCourse.Count > 0)
            {

                response = (new CommonDAL()).UpsertCourse(request.LstCourse);
            }

            return response;
        }

        public ServiceContracts.CourseBlockResponse UpsertCourseBlock(ServiceContracts.CourseBlockRequest request)
        {
            Log.LogInfo("---CourseBlock Request starts");
            ServiceContracts.CourseBlockResponse response = new CourseBlockResponse();
            if (request != null && request.LstCourseBlock.Count > 0)
            {

                response = (new CommonDAL()).UpsertCourseBlock(request.LstCourseBlock);
            }

            return response;
        }




        public ServiceContracts.ClassResponse UpsertClass(ServiceContracts.ClassRequest request)
        {
            Log.LogInfo("---UpsertClass Request starts--");
            ServiceContracts.ClassResponse response = new ClassResponse();
            if (request != null && request.LstClass.Count > 0)
            {

                response = (new CommonDAL()).UpsertClass(request.LstClass);
            }

            return response;
        }

        public ServiceContracts.CustomUserResponse UpsertCustomUser(ServiceContracts.CustomUserRequest request)
        {
            Log.LogInfo("---UpsertCustomUser Request starts--");
            ServiceContracts.CustomUserResponse response = new CustomUserResponse();
            if (request != null && request.LstCustomUser.Count > 0)
            {

                response = (new CommonDAL()).UpsertCustomUser(request.LstCustomUser);
            }

            return response;
        }



        public ServiceContracts.InstructorResponse UpsertInstructor(ServiceContracts.InstrucotrRequest request)
        {
            Log.LogInfo("---UpsertCustomUser Request starts--");
            ServiceContracts.InstructorResponse response = new InstructorResponse();
            if (request != null && request.LstInstructor.Count > 0)
            {

                response = (new CommonDAL()).UpsertInstructor(request.LstInstructor);
            }

            return response;
        }

        public ServiceContracts.RosterResponse UpsertRoster(ServiceContracts.RosterRequest request)
        {
            Log.LogInfo("---UpsertRoster Request starts--");
            ServiceContracts.RosterResponse response = new RosterResponse();
            if (request != null && request.LstRoster.Count > 0)
            {

                response = (new CommonDAL()).UpsertRoster(request.LstRoster);
            }

            return response;
        }

        public ServiceContracts.Contracts.OMRScannedResponse PersistOfflineScannedResponseForNYCS(ServiceContracts.Contracts.ScannedResponseRequest request)
        {
            //List<ServiceContracts.Contracts.OMRScannedResponse> OMRScannedResponse = new List<ServiceContracts.Contracts.OMRScannedResponse>();
            ServiceContracts.Contracts.OMRScannedResponse objOMRScannedResponse = new ServiceContracts.Contracts.OMRScannedResponse();
            try
            {
                ServiceContracts.Contracts.ScannedResponse ObjectScannedResponse = new ServiceContracts.Contracts.ScannedResponse();
                Log.LogInfo("Scanned Response count :" + request.ListScannedResponse.Count);
                if (request != null && request.ListScannedResponse != null && request.ListScannedResponse.Count > 0)
                {

                    objOMRScannedResponse.ListOMRScannedResponseOutPut = (new CommonDAL()).PersistOfflineScannedResponseForNYCS(request.ListScannedResponse);

                }
            }
            catch (System.Exception exe)
            {
                Log.LogError("Error:", exe);
                throw exe;

            }
            return objOMRScannedResponse;

        }
        public ServiceContracts.GetLogResponseType GetLogRequest(ServiceContracts.GetLogRequestType requestType)
        {
            ServiceContracts.GetLogResponseType response = new GetLogResponseType();
            Log.LogInfo("Get LogRequest starts:");
            try
            { 
                if (requestType != null && requestType.request.MacID != string.Empty)
                {
                    Log.LogInfo("Requested TC macid:" + requestType.request.MacID);
                    response = (new CommonDAL()).FetchLogRequest(requestType.request.MacID);
                }
            }
            catch (System.Exception exe)
            {
                Log.LogError( exe.Message,exe);
            }
            Log.LogInfo("Get LogRequest ends:");

            return response;

        }

        public void UpdateLogResponse(ServiceContracts.LogoutputRequestType requestType)
        {
            string response;
            Log.LogInfo("UpdateLogRequest starts:");
            try
            {
                response = new CommonDAL().UpsertLogResposne(requestType);
            }
            catch (System.Exception exe)
            {
                Log.LogError(exe.Message, exe);
                response = "F001";
            }
            Log.LogInfo("UpdateLogRequest ends:");
           // return response;
        }

        #endregion

        #region IAttendance Members

        public void PersistDailyScheduleSummary(PersistDailyScheduleSummaryRequest request)
        {
            Log.LogInfo("------------------------------------ START -------------------------------------");
            Log.LogInfo("Begin PersistDailyScheduleSummary() - Service Call");
            (new CommonDAL()).PersistDailyScheduleSummary(request.ListPersistDailyScheduleSummary);
            Log.LogInfo("End PersistDailyScheduleSummary() - Service Call");
            Log.LogInfo("------------------------------------- END --------------------------------------");
        }

        #endregion

        #region IApplicationLicensing Members

        public ServiceContracts.ApplicationLicensingResponse GetApplicationLicensingDetailsForTestCenter(ServiceContracts.ApplicationLicensingRequest request)
        {
            Log.LogInfo("------------------------------------ START -------------------------------------");
            Log.LogInfo("Begin GetApplicationLicensingDetailsForTestCenter()");
            ApplicationLicensingResponse objResponse = new ApplicationLicensingResponse();
            try
            {
                objResponse.LstApplicationLicensingStatus = (new ApplicationLicensingFactory()).GetApplicationLicensingForTestCenter(request.MacAddress);
            }
            catch (Exception ex)
            {
                Log.LogError("Error GetApplicationLicensingDetailsForTestCenter()", ex);
            }
            Log.LogInfo("End GetApplicationLicensingDetailsForTestCenter()");
            Log.LogInfo("------------------------------------- END --------------------------------------");
            return objResponse;
        }

        #endregion
        #region ErrorLogs
        public void ETLAppErrorLogs(ServiceContracts.Contracts.ETLAppErrorLogsRequest request)
        {
            try
            {
                (new CommonDAL()).ETLAppErrorLogs(request);
            }
            catch (Exception ex)
            {
                Log.LogError("Error ETLAppErrorLogs()", ex);
            }
        }
        #endregion
        #region MSIInstallationResult
        public void MSIInstallationResult(ServiceContracts.Contracts.MSIInstallationResultRequest request)
        {
            try
            {
                (new CommonDAL()).MSIInstallationResult(request);
            }
            catch (Exception ex)
            {
                Log.LogError("Error MSIInstallationResult()", ex);
            }
        }
        #endregion
        #region SaveMediaPackageDetails
        public void PersistMediaLoadedDetails(ServiceContracts.Contracts.MediaPackageInfoRequest request)
        {
            try
            {
                (new CommonDAL()).SaveMediaPackageLoadedDetails(request);
            }
            catch (Exception ex)
            {
                Log.LogError("Error SaveMediaPackageDetails()", ex);
            }
        }
        #endregion
        public List<ServiceContracts.LoadedMediaPackageDetailsResponse> FetchMediaLoadedDetails(string MACID)
        {
            List<ServiceContracts.LoadedMediaPackageDetailsResponse> MediaDetails = new List<ServiceContracts.LoadedMediaPackageDetailsResponse>();
            MediaDetails = (new GenericDAL()).FetchLoadedMediaDetails(MACID);
            return MediaDetails;
        }

        public ServiceContracts.ValidateTestCenterTypeResponse ValidateTestCenterType(ServiceContracts.ValidateTestCenterTypeRequest request)
        {
            ServiceContracts.ValidateTestCenterTypeResponse objResponse = new ValidateTestCenterTypeResponse();
            objResponse = new GenericDAL().ValidateTestCenterType(request.MacID, request.ServerType);
            if (objResponse.StatusCode.Equals("S000"))
            {
                objResponse.Status = "Failure";
                objResponse.Message = "Invalid Server Type";
            }
            else if (objResponse.StatusCode.Equals("S001"))
            {
                objResponse.Status = "Success";
                objResponse.Message = "Valid Test Center";
            }
            else if (objResponse.StatusCode.Equals("S004"))
            {
                objResponse.Status = "Failure";
                objResponse.Message = "Invalid Test Center";
            }
            return objResponse;
        }
    }
}
