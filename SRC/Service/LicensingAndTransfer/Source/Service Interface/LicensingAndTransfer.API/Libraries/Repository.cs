using System;

namespace LicensingAndTransfer.API.Libraries
{
    /// <summary>
    /// Manage Repository
    /// </summary>
    public class Repository
    {
        /// <summary>
        /// Convert repository file to base64 format
        /// </summary>
        /// <param name="fileRelativePath"></param>
        /// <returns></returns>
        public String ConvertToBase64(String fileRelativePath)
        {
            String filePath = Constants.RepositoryPath + fileRelativePath;
            byte[] bytes;
            if (System.IO.File.Exists(filePath))
            {
                using (System.IO.FileStream stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
                {
                    using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();
                    }
                }
                return Convert.ToBase64String(bytes);
            }
            else return null;
        }

        /// <summary>
        /// Convert file content and save it to repository
        /// </summary>
        /// <param name="base64FileData"></param>
        /// <param name="fileRelativePath"></param>
        //  [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
        public void ConvertFromBase64(String base64FileData, String fileRelativePath)
        {
            String filePath = Constants.RepositoryPath + fileRelativePath;
            if(!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(filePath)))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
            }

            //Save the Byte Array as Image File.
            System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(base64FileData));   //Convert Base64 Encoded string to Byte Array.
        }
    }
}