using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;

namespace LicensingAndTransfer.API
{
    public class FTP
    {
        #region Code to Impersonate
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_PROVIDER_DEFAULT = 0;

        System.Security.Principal.WindowsImpersonationContext impersonationContext;

        [System.Runtime.InteropServices.DllImport("advapi32.dll")]
        public static extern int LogonUserA(String lpszUserName,
            String lpszDomain,
            String lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);
        [System.Runtime.InteropServices.DllImport("advapi32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        public static extern int DuplicateToken(IntPtr hToken,
            int impersonationLevel,
            ref IntPtr hNewToken);

        [System.Runtime.InteropServices.DllImport("advapi32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        public static extern bool RevertToSelf();

        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);

        public bool impersonateValidUser(String userName, String domain, String password)
        {
            System.Security.Principal.WindowsIdentity tempWindowsIdentity;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;

            if (RevertToSelf())
            {
                if (LogonUserA(userName, domain, password, LOGON32_LOGON_INTERACTIVE,
                    LOGON32_PROVIDER_DEFAULT, ref token) != 0)
                {
                    if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                    {
                        tempWindowsIdentity = new System.Security.Principal.WindowsIdentity(tokenDuplicate);
                        impersonationContext = tempWindowsIdentity.Impersonate();
                        if (impersonationContext != null)
                        {
                            CloseHandle(token);
                            CloseHandle(tokenDuplicate);
                            return true;
                        }
                    }
                }
            }
            if (token != IntPtr.Zero)
                CloseHandle(token);
            if (tokenDuplicate != IntPtr.Zero)
                CloseHandle(tokenDuplicate);
            return false;
        }
        #endregion
    }

    public class Resource
    {
        public void MoveResources(string packageFolder, string temporaryFolder, string MacID, string QRFolder, DataContracts.Operations Operation, List<DataContracts.PackageStatistics> ListPackageStatistics)
        {
            Constants.Log.Info("Begin MoveResources() - QRFolder: " + QRFolder);
            Constants.Log.Info("Begin MoveResources() - Operation: " + Operation.ToString());
            Constants.Log.Info("Begin MoveResources() - PackageStatistic(s): " + ((ListPackageStatistics == null) ? "0" : ListPackageStatistics.Count.ToString()));

            MacID = MacID.Replace(":", "-");//required because : is not supported while creating folders

            try
            {
                Constants.Log.Info("Begin Move Packages to Local Folder()");

                if (!System.IO.Directory.Exists(packageFolder + QRFolder))
                {
                    Constants.Log.Info("Begin Create Local Folder - " + packageFolder + QRFolder);
                    System.IO.Directory.CreateDirectory(packageFolder + QRFolder);
                    Constants.Log.Info("End Create Local Folder - " + packageFolder + QRFolder);
                }

                if (Operation == DataContracts.Operations.QPackTransfer || Operation == DataContracts.Operations.RPackTransfer)
                {
                    if (ListPackageStatistics != null && ListPackageStatistics.Count > 0)
                    {
                        Constants.Log.Info("Begin CopyPackage()");
                        #region Copying the required packages to the specific folder
                        foreach (DataContracts.PackageStatistics entryPackage in ListPackageStatistics)
                        {
                            Constants.Log.Info("Source Package Path - " + temporaryFolder + QRFolder + @"\" + entryPackage.PackageName);
                            Constants.Log.Info("Destination Package Path - " + packageFolder + QRFolder + @"\" + entryPackage.PackageName);

                            if (System.IO.File.Exists(temporaryFolder + QRFolder + @"\" + entryPackage.PackageName))
                            {
                                Constants.Log.Info("Package Path: " + QRFolder + @"\" + entryPackage.PackageName);
                                System.IO.File.Copy(temporaryFolder + QRFolder + @"\" + entryPackage.PackageName, packageFolder + QRFolder + "/" + entryPackage.PackageName, true);
                            }

                            if (System.IO.File.Exists(packageFolder + QRFolder + @"\" + entryPackage.PackageName))
                                entryPackage.PackagePath = QRFolder + @"\" + entryPackage.PackageName;
                        }
                        #endregion
                        Constants.Log.Info("End CopyPackage()");
                    }
                    Constants.Log.Info("End Move Packages to Local Folder()");
                }
                else if (Operation == DataContracts.Operations.QPackFetch || Operation == DataContracts.Operations.RPackFetch)
                {
                    if (ListPackageStatistics != null && ListPackageStatistics.Count > 0)
                    {
                        Constants.Log.Info("Begin CopyPackage()");
                        string fileName = "";
                        string UniqueFolder = System.IO.Path.Combine(temporaryFolder, QRFolder, MacID);

                        if (!System.IO.Directory.Exists(UniqueFolder))
                        {
                            Constants.Log.Info("Begin Create UniqueFolder - " + UniqueFolder);
                            System.IO.Directory.CreateDirectory(UniqueFolder);
                            Constants.Log.Info("End Create UniqueFolder - " + UniqueFolder);
                        }

                        #region Copying the required packages to the specific folder
                        foreach (DataContracts.PackageStatistics entryPackage in ListPackageStatistics)
                        {
                            fileName = System.IO.Path.GetFileName(entryPackage.PackagePath);
                            try
                            {
                                Constants.Log.Info("File Source - " + packageFolder + entryPackage.PackagePath);
                                Constants.Log.Info("File Destination - " + UniqueFolder + @"\" + "\\" + fileName);

                                //  Condition to check whether the file already exist in the destination folder
                                if (System.IO.File.Exists(packageFolder + entryPackage.PackagePath) && !System.IO.File.Exists(UniqueFolder + @"\" + fileName))
                                {
                                    Constants.Log.Info("Begin File Copy");
                                    System.IO.File.Copy(packageFolder + entryPackage.PackagePath, UniqueFolder + @"\" + fileName, false);
                                    Constants.Log.Info("End File Copy");
                                }
                                else if (System.IO.File.Exists(packageFolder + entryPackage.PackagePath) && System.IO.File.Exists(UniqueFolder + @"\" + fileName))
                                {
                                    Constants.Log.Info("File already exist in the destination location, " + UniqueFolder + @"\" + "\\" + fileName);
                                }
                                else
                                    Constants.Log.Info("File does not exist with Source Path: " + packageFolder + entryPackage.PackagePath);
                            }
                            catch (Exception ex)
                            {
                                Constants.Log.Error("Error File Copy/Overwrite", ex);
                            }
                        }
                        #endregion
                        Constants.Log.Info("End CopyPackage()");
                    }
                    Constants.Log.Info("End Move Packages to Temporary Folder()");
                }
            }
            catch (Exception ex)
            {
                Constants.Log.Error("Error MoveResources()", ex);
                throw new Exception(ex.Message, ex);
            }
            Constants.Log.Info("End MoveResources()");
        }
    }
}
