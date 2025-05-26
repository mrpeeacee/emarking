using Saras.eMarking.Domain.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels
{
    public class ProjectQigModel
    {
        public ProjectQigModel()
        {
        }

        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long? ProjectQIGID { get; set; }
        [XssTextValidation]
        [Required]
        public string QIGCode { get; set; }
        [XssTextValidation]
        [Required]
        public string QIGName { get; set; }
        [Required]
        public int QuestionsType { get; set; }
        [Required]
        public int NoOfQuestions { get; set; }
        [Required]
        public bool IsAllQuestionMandatory { get; set; }
        [Required]
        public int NOOfMandatoryQuestion { get; set; }
        [Required]
        [Range(1, 999.99, ErrorMessage = "Please enter a value bigger than {1}")]
        public decimal? TotalMarks { get; set; }
        [Required]
        public int ToleranceLimit { get; set; }
        [Required]
        public bool? IsScoreComponentExists { get; set; }
        [Required]
        [Range(1, 999.99, ErrorMessage = "Please enter a value bigger than {1}")]
        public decimal StepValue { get; set; }
        [XssTextValidation]
        [Required]
        public string QuestionText { get; set; }
        public string SchemeName { get; set; }
        public long? ProjectId { get; set; }

    }
}