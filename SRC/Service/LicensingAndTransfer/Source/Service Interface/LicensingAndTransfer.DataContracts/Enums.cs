using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    public enum ServerTypes
    {
        [EnumMemberAttribute]
        ControllerOfExamination,
        [EnumMemberAttribute]
        DataExchangeServer,
        [EnumMemberAttribute]
        TestCenter,
        [EnumMemberAttribute]
        DataServer,
        [EnumMemberAttribute]
        DataCenterDistributed,
        [EnumMemberAttribute]
        DataCenterCentralized,
        [EnumMemberAttribute]
        EmarkingCenter
        
    }
    public enum Operations
    {
        [EnumMemberAttribute]
        QPackTransfer,
        [EnumMemberAttribute]
        QPackFetch,
        [EnumMemberAttribute]
        RPackTransfer,
        [EnumMemberAttribute]
        RPackFetch
    }

    public enum TransferMedium
    {
        [EnumMemberAttribute]
        FTP,
        [EnumMemberAttribute]
        SharedPath,
        [EnumMemberAttribute]
        HTTP,
        [EnumMemberAttribute]
        SFTP,
        [EnumMemberAttribute]
        S3
    }
}