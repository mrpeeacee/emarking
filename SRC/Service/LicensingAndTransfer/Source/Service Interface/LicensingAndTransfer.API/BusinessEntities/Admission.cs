using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LicensingAndTransfer.API.BusinessEntities
{
    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class AdmissionDetail
    {
        public String AppointmentID;
        public BusinessEntities.Admission admission { get; set; }
        public List<BusinessEntities.AccommodationAssistant> assistants { get; set; }
    }
}