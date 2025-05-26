using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace LicensingAndTransfer.DataContracts
{
   [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "AssessmentDetailsRequest")]
   
    public  class AssessmentDetailsRequest
    {
       [DataMember(IsRequired = true, Name = "ScheduleUserID", Order = 0)]
       private Int64 ScheduleUserIDField;
       public Int64 ScheduleUserID
       {
           get
           {
               return ScheduleUserIDField;
           }
           set
           {
               ScheduleUserIDField = value;
           }
       }
    }
}
