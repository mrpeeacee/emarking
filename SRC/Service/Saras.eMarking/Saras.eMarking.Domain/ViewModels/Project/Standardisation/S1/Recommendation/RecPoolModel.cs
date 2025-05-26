using Saras.eMarking.Domain.Extensions;

namespace Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Recommendation
{
    public class RecPoolModel
    {
        public RecPoolModel()
        {
        }
        public long QIGId { get; set; }
        [XssTextValidation]
        public string QIGCode { get; set; }
        [XssTextValidation]
        public string QIGName { get; set; }
        public long? TotalTargetRecomendations { get; set; }
        public long? TotalRecomended { get; set; }
        public long? TotalRecomendedByMe { get; set; }
        public bool? IsAoCm { get; set; }
        public bool? IsKp { get; set; } = false;
        public long? TotalTargetRecomendationsPerKP { get; set; }
        public bool IsStdRequired { get; set; }
    }

}
