using Saras.eMarking.Domain.Extensions;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Saras.eMarking.Domain.ViewModels.Categorisation
{
    public class CategorisationTrialMarkModel1
    {
        public CategorisationTrialMarkModel1()
        {
            TrailMarkedScripts = new List<CategorisationTrialMarkModel>();
        }
        public bool IsQigPaused { get; set; }
        public bool IsInQfAsses { get; set; }
        [JsonIgnore]
        public long QigId { get; set; }
        public string QigName { get; set; }
        public long ScriptId { get; set; }
        public string ScriptName { get; set; }
        public string Poolcategory { get; set; } 
        public bool IsS1Completed { get; set; }
        public decimal? TotalMark { get; set; }
        public int WorkFlowId { get; set; }
        public int NoKps { get; set; }
        public bool ScoringCompExist { get; set; }
        public ScriptCategorizationPoolType PoolType { get; set; }
        public List<CategorisationTrialMarkModel> TrailMarkedScripts { get; set; }
        public List<CatContentScore> ContentScores { get; set; }
    }

    public class CategorisationTrialMarkModel
    {
        public long? MarkedBy { get; set; }
        [XssTextValidation]
        public string FirstName { get; set; }
        [XssTextValidation]
        public string LastName { get; set; }
        public bool SelectAsDefinitive { get; set; }
        public decimal? TotalMarks { get; set; }
        public decimal? MarkAwarded { get; set; }
        public long? MarkerId { get; set; }
        public string Phase { get; set; }
        public long MarkingRefId { get; set; }
        public List<CatContentScore> ContentScores { get; set; }
        public List<CatQuestionDetails> QuestionDetails { get; set; }
    }

    public class CatContentScore
    {
        public long ScoreComponentId { get; set; }
        public string Name { get; set; }
        public decimal? Marks { get; set; }
        public decimal? MaxMarks { get; set; }
        public int? WorkFlowStatusId { get; set; }
    }

    public class CatQuestionDetails
    {
        public long QuestionId { get; set; }
        public string QuestionCode { get; set; }
        public decimal? Marks { get; set; }
        public decimal? MaxMarks { get; set; }
        public int? Type { get; set; }
        public int? WorkFlowStatusId { get; set; }
        public bool IsScoreComponentExists { get; set; }
        public List<CatContentScore> ContentScores { get; set; }
    }

    public class CategoriseAsModel : IAuditTrail
    {
        public long MarkedBy { get; set; }
        [Required]
        public bool SelectAsDefinitive { get; set; }
        [Required]
        public long QigId { get; set; }
        [Required]
        public long ScriptId { get; set; }
        [Required]
        public ScriptCategorizationPoolType PoolType { get; set; }
        public string Poolcategory { get; set; }
        public string ScriptName { get; set; }

        [Required]
        public long MarkingRefId { get; set; }        
    }   
}
