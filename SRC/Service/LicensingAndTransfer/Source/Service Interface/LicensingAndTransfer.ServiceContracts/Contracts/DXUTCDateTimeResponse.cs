using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class DXUTCDateTimeResponse
    {
        [MessageBodyMember(Order = 0)]
        private System.DateTime UTCDateTimeField;
        /// <summary>
        /// UTC Date Time of the Data Exchange Server
        /// </summary>
        public System.DateTime UTCDateTime
        {
            get
            {
                return this.UTCDateTimeField;
            }
            set
            {
                this.UTCDateTimeField = value;
            }
        }        
    }
}