using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LicensingAndTransfer.API.BusinessEntities
{
    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class Exam
    {
        public Int64? UniqueID { get; set; }
        public string PEName { get; set; }
        public string PECode { get; set; }
        public Int64? PETID { get; set; }
        public string PEAID { get; set; }
        public string PERJ { get; set; }
        public string PER { get; set; }
        public string PES { get; set; }
        public DateTime? RID { get; set; }
        public DateTime? ESD { get; set; }
        public string VS { get; set; }

    }
}