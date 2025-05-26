using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts.Contracts
{
    [MessageContract]
    public class TcVerificationRequest
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
                MacIDField = value;
            }
        }
        [MessageBodyMember(Order = 1)]
        private string ScheduleDateField;
        public string ScheduleDate
        {
            get
            {
                return this.ScheduleDateField;
            }
            set
            {
                ScheduleDateField = value;
            }
        }
        [MessageBodyMember(Order = 2)]
        private string BatchTimeField;
        public string BatchTime
        {
            get
            {
                return this.BatchTimeField;
            }
            set
            {
                BatchTimeField = value;
            }
        }
    }
}
