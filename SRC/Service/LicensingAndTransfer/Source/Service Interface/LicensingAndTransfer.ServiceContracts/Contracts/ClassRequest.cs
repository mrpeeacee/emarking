using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
  public  class ClassRequest
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.Class> ClassField;
        public List<DataContracts.Class> LstClass
        {
            get
            {
                return this.ClassField;
            }
            set
            {
                this.ClassField = value;
            }
        }
    }
}
