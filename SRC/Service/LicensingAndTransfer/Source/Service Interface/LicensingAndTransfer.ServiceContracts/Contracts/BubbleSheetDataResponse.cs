using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts.Contracts
{
    [MessageContract]
    public class BubbleSheetDataResponse
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.BubbleSheetData> ListBubbleSheetDataField;
        public List<DataContracts.BubbleSheetData> ListBubbleSheetData
        {
            get
            {
                return this.ListBubbleSheetDataField;
            }
            set
            {
                this.ListBubbleSheetDataField = value;
            }
        }
    }
}
