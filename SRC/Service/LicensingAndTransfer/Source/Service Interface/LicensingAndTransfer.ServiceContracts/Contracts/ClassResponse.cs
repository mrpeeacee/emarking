using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
   public class ClassResponse
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
                this.StatusField = value;
            }
        }
    }
}
