using Saras.eMarking.Domain.Extensions;

namespace Saras.eMarking.Domain.ViewModels
{   
    public class UserQigModel
    {
        public UserQigModel()
        {
        }
        public long QigId { get; set; }
        [XssTextValidation]
        public string QigName { get; set; }
        public string QigCode { get; set; }
        public bool? IsS1Available { get; set; }
        public bool IsKp { get; set; }
    }
}
