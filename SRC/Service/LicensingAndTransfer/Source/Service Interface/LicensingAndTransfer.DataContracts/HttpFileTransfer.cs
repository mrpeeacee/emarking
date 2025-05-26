using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2012/01", Name = "HttpFileTransferRequest")]
    public class HttpFileTransferRequest
    {
        [DataMember]
        public byte[] ReadData { get; set; }

        [DataMember]
        public int ReadCount { get; set; }

        [DataMember]
        public string path { get; set; }

        [DataMember]
        public long existingFileSize { get; set; }
    }

    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2012/02", Name = "HttpFileTransferResponse")]
    public class HttpFileTransferResponse
    {
        byte[] ReadDataField;
        [DataMember]
        public byte[] ReadData
        {
            get { return ReadDataField; }
            set { ReadDataField = value; }
        }
        [DataMember]
        public int ReadCount { get; set; }

        [DataMember]
        public long FileSizeCount { get; set; }
        
        [DataMember]
        public long contentLength { get; set; }

    }
}
