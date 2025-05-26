using Saras.eMarking.Domain.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration
{
    public class QuestionModel
    {
        public QuestionModel()
        {

        }
        public long ProjectMarkSchemeId { get; set; }
        public long ProjectQuestionId { get; set; }
        public string SchemeCode { get; set; }
        public string SchemeName { get; set; }
        public decimal? Marks { get; set; }
        public long? ProjectId { get; set; }
        public bool IsDeleted { get; set; }
        public string SchemeDescription { get; set; }
        public bool IsTagged { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsMarkscheme { get; set; }
        public int ToleranceLimit { get; set; }
        [Required]
        public bool IsScoreComponentExists { get; set; }
        //[Required]
        //[Range(0, 999.99, ErrorMessage = "Please enter a value bigger than {0}")]
        public decimal? StepValue { get; set; }
        [XssTextValidation]
        [Required]
        public string QuestionText { get; set; }
        public long? ProjectQuestionID { get; set; }

    }
}
