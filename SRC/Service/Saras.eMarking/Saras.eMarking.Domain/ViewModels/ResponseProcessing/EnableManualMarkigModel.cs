using Saras.eMarking.Domain.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.ResponseProcessing
{
    public class EnableManualMarkigModel
    {
        public EnableManualMarkigModel()
        {

        }
        public long Blank { get; set; }
        public long Score { get; set; }
        [XssTextValidation]
        public string CorrectAnswer { get; set; }
        public long NoOfResponsedtobeEvaluated { get; set; }
        public long NoOfAnswerKeywords { get; set; }
        public long QigId { get; set; }
        [XssTextValidation]
        public string QigName { get; set; }
        public bool? StandardizationRequired { get; set; }
        [XssTextValidation]
        [Required]
        public string Remarks { get; set; }
        public Int64 Id { get; set; }
        public long ProjectQuestionId { get; set; }
        public long ParentQuestionId { get; set; }

    }
}
