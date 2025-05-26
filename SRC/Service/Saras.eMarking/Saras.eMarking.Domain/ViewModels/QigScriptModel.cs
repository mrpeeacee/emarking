using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels
{
    public class QigScriptModel
    {
        [Required(ErrorMessage = "QigId is required")]
        public long QigId { get; set; }
        [Required(ErrorMessage = "ScriptId is required")]
        public long ScriptId { get; set; }
    }
}
