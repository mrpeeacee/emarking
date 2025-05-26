using System;

namespace Saras.eMarking.Domain.Entities
{
    public partial class PassageHistory
    {
        public long PassageId { get; set; }
        public string PassageCode { get; set; }
        public string PassageLabel { get; set; }
        public string PassageXml { get; set; }
        public bool IsDeleted { get; set; }
        public long? CreatedBy { get; set; }
        public int? PassageTypeId { get; set; }
        public decimal Version { get; set; }
        public bool Alignment { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsEncrypted { get; set; }
        public string QuestionJson { get; set; }
        public bool? IsApproved { get; set; }
    }
}
