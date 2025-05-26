using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class InstructorResponse
    {
        [MessageBodyMember(Order = 0)]
        private string StatusField;
        public string Status
        {
            get
            {
                return StatusField;
            }
            set
            {
                StatusField = value;
            }
        }
    }
}
