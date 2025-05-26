using Saras.eMarking.Domain.Extensions;

namespace Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Recommendation
{
    public class RecBandModel
    {
        public RecBandModel() { }
        public long BandId { get; set; }
        public long MarkSchemeId { get; set; }
        public decimal BandFrom { get; set; }
        public decimal BandTo { get; set; } 
        public string BandDescription { get; set; }
        [XssTextValidation]
        public string BandCode { get; set; }
        [XssTextValidation]
        public string BandName { get; set; }
        public bool IsSelected { get; set; } = false;

        public long ScoreComponentId { get; set; }
        public string ComponentCode { get; set; }
        public string ComponentName { get; set; }
        
        public decimal MaxMarks { get; set; }
    }
}
