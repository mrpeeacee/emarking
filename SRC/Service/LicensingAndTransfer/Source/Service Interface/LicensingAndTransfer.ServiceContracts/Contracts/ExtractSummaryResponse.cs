using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
namespace LicensingAndTransfer.ServiceContracts.Contracts
{
   [MessageContract]
    public class ExtractSummaryResponse
    {
       [MessageBodyMember(Order = 0)]
       private List<DataContracts.SummaryDetailsResponse> lstSummaryDetailsResponseField;
       public List<DataContracts.SummaryDetailsResponse> lstSummaryDetailsResponse
       {
           get
           {
               return lstSummaryDetailsResponseField;
           }
           set
           {
               lstSummaryDetailsResponseField = value;
           }
       }
    }
}
