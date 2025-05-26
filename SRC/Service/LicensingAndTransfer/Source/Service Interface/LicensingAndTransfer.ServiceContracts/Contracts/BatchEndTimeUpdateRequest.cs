using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class BatchEndTimeUpdateRequest
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.Batch> BatchesField;
        /// <summary>
        /// Encrypted Script Data will be sent
        /// </summary>
        public List<DataContracts.Batch> Batches
        {
            get
            {
                return this.BatchesField;
            }
            set
            {
                this.BatchesField = value;
            }
        }        
    }
}