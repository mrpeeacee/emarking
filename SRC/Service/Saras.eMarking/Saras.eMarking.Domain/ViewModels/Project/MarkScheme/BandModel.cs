using Saras.eMarking.Domain.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.MarkScheme
{
    public class BandModel
    {
        public BandModel() { }
        public long BandId { get; set; }
        public long MarkSchemeId { get; set; }
        public decimal BandFrom { get; set; }
        public decimal BandTo { get; set; } 
        [MaxLength(2000)]
        public string BandDescription { get; set; }
        [XssTextValidation]
        public string BandCode { get; set; }
        [XssTextValidation]
        public string BandName { get; set; }
        public bool IsSelected { get; set; } = false;
    }
}
