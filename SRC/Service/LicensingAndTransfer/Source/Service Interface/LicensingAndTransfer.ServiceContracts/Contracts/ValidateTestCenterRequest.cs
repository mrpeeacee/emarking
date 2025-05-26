using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class ValidateTestCenterRequest
    {
        [MessageBodyMember(Order = 0)]
        private string MacIDField;
        public string MacID
        {
            get
            {
                return this.MacIDField;
            }
            set
            {
                this.MacIDField = value;
            }
        }

        [MessageBodyMember(Order = 1)]
        private DataContracts.ServerTypes ServerTypeField;
        public DataContracts.ServerTypes ServerType
        {
            get
            {
                return this.ServerTypeField;
            }
            set
            {
                this.ServerTypeField = value;
            }
        }
    }
}
