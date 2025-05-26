using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "AssessmentDetailsResponse")]
    public  class AssessmentDetailsResponse
    {
       private int NoOfQuestionsField;
       [DataMember(IsRequired = true, Order = 0, Name = "NoOfQuestions")]

       public int NoOfQuestions
       {
           get
           {
               return NoOfQuestionsField;
           }
           set
           {
               NoOfQuestionsField = value;
           }
       }

       private int MaxChoicesField;
       [DataMember(IsRequired = true, Order = 1, Name = "MaxChoices")]
       public int MaxChoices
       {
           get
           {
               return MaxChoicesField;
           }
           set
           {
               MaxChoicesField = value;
           }
       }
    }
}
