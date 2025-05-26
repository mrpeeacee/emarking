using System;

namespace Saras.eMarking.Domain.Entities
{
    public partial class ScriptCategorizationPoolHistory
    {
        public long Id { get; set; }
        public long ScriptCategorizationPoolId { get; set; }
        public long ProjectId { get; set; }
        public decimal? CategorizationVersion { get; set; }
        public long ScriptId { get; set; }
        public byte PoolType { get; set; }
        public long? Qigid { get; set; }
        public decimal? MaxMarks { get; set; }
        public long? UserScriptMarkingRefId { get; set; }
        public decimal? FinalizedMarks { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? ModifiedBy { get; set; }
    }
}
