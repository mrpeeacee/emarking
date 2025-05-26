using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.MarkScheme
{
    public class ProjectTaggedQuestionModel
    {
        [Required]
        public long ProjectQuestionId { get; set; }
        public long QuestionId { get; set; }
        [Required]
        public long? MarkSchemeId { get; set; }
        public string SchemeName { get; set; }
        public string QuestionCode { get; set; }
        public decimal? MaxMark { get; set; }
        public bool IsTagged { get; set; }
        public bool PanelOpenState { get; set; }
        public int TotalRows { get; set; }
        public bool IsScoringComponentExist { get; set; }
        public string ComponentName { get; set; }
    }
}
