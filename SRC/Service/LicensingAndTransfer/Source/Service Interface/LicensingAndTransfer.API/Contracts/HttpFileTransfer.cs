using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace LicensingAndTransfer.API
{
    [MessageContract]
    public class HttpFileTransferRequest
    {
        [MessageBodyMember(Order = 0)]
        private DataContracts.HttpFileTransferRequest RequestField;
        public DataContracts.HttpFileTransferRequest Request
        {
            get
            {
                return this.RequestField;
            }
            set
            {
                this.RequestField = value;
            }
        }
    }

    [MessageContract]
    public class HttpFileTransferResponse
    {
        [MessageBodyMember(Order = 0)]
        public DataContracts.HttpFileTransferResponse Response { get; set; }
    }
}
