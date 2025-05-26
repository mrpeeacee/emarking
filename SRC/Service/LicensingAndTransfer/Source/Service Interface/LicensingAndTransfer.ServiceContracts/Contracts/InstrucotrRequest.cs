using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
  public  class InstrucotrRequest
    {
        [MessageBodyMember(Order = 0)]
        List<DataContracts.Instructor> LstInsturctorField;
        public List<DataContracts.Instructor> LstInstructor
        {
            get
            {
                return LstInsturctorField;
            }
            set
            {
                LstInsturctorField = value;
            }
        }
    }
}
