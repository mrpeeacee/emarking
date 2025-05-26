using Saras.eMarking.Domain.Extensions;

namespace Saras.eMarking.Domain.ViewModels.Categorisation
{
    public class CategorisationModel
    {
        public long ScriptId { get; set; }
        [XssTextValidation]
        public string ScriptName { get; set; }
        public int TotalKpMarked { get; set; }
        public ScriptCategorizationPoolType PoolType { get; set; }
        public decimal? FinalizedMarks { get; set; }
        public bool IsInQfAsses { get; set; }
        public bool IsUnRecommandEnable { get; set; }
        public bool IsCategorization { get; set; }
    }
}
