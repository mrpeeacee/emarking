using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "GetListOfLogRequest")]
  public  class GetListOfLogRequest
    {
        private System.String MacIDField;
        [DataMember(IsRequired = true, Name = "MacID", Order = 0)]
        public System.String MacID
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
    }
}
