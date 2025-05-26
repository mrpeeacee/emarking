using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "LogoutputRequest")]
    public class LogoutputRequest
    {
        private System.Int64 TestCenterLogDetailIDField;
        [DataMember(IsRequired = true, Name = "TestCenterLogDetailID", Order = 0)]
        public System.Int64 TestCenterLogDetailID
        {
            get
            {
                return this.TestCenterLogDetailIDField;
            }
            set
            {
                this.TestCenterLogDetailIDField = value;
            }
        }
        private System.String LogFilePathField;
        [DataMember(IsRequired = true, Name = "LogFilePath", Order=1)]
        public System.String LogFilePath
        {
            get
            {
                return this.LogFilePathField;
            }
            set
            {
                LogFilePathField = value;
            }
        }
        
        
    }
}
