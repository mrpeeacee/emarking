using Saras.eMarking.Domain.Extensions;

namespace Saras.eMarking.Domain.ViewModels.ResponseProcessing
{
    public class ViewAllBlankSummaryModel
    {
        public ViewAllBlankSummaryModel()
        {

        }
        [XssTextValidation]
        public string BlankName { get; set; }
        [XssTextValidation]
        public string QigName { get; set; }
        public long? ResponsesToBeEvaluated { get; set; }
        public long QigId { get; set; }
        public decimal? BlankMarks { get; set; }

        public long? TotalNoofCandidates { get; set; }
        public bool IsManualMarkingRequired { get; set; }
        public bool? IsS1Available { get; set; }
        public string ManualMarkingEnabled { get; set; }
        public string Standardization { get; set; }
        [XssTextValidation]
        public string Remarks { get; set; }
        public byte? NoofResponsestobeevaluted { get; set; }
        public byte? MarkingType { get; set; }

        public int RowSpanIndex { get; set; }
        public int RowSpan { get; set; }
        public int? QuestionOrder { get; set; }
        ////public int? ResponseProcessingType { get; set; }
    }
}
