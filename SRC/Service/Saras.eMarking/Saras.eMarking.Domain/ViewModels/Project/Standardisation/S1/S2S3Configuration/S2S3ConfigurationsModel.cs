using Saras.eMarking.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.S2S3Configuraion
{
    
    public class QualifyingAssessmentCreationModel
    {
        public long ProjectId { get; set; }
        public long QigId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int? TotalNoOfScripts { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int? NoOfScriptSelected { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int ToleranceCount { get; set; }
        [Range(1, 2, ErrorMessage = "Please enter a value bigger than {1}")]
        public byte? ScriptPresentationType { get; set; }
        [Required]
        [Range(1, 2, ErrorMessage = "Please enter a value bigger than {1}")]
        public byte ApprovalType { get; set; }
        [Required]
        public bool IsTagged { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long? RefQualifyingAssessmentId { get; set; }
        [Required]
        [Range(0, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long QualifyingAssessmentId { get; set; }
        public string QigName { get; set; } 
        public string ScriptPresentationTypeName { get; set; }
        public string ApprovalTypeName { get; set; }
        public int? StandardizationScriptPoolCount { get; set; }
        public int? BenchMarkScriptPoolCount { get; set; }
        public int? AdditionalStandizationScriptPoolCount { get; set; }
        public int? NoOfStandardizationScriptCategorized { get; set; }
        public int? NoOfBenchMarkScriptPoolCategorized { get; set; }
        public int? NoOfAdditionalStandizationScriptPoolCategorized { get; set; }
        public List<QualifyingAssessmentScriptDetailsCreationModel> Qscriptdetails { get; set; }
    }

    public class QualifyingAssessmentScriptDetailsCreationModel
    {
        public QualifyingAssessmentScriptDetailsCreationModel()
        {
        }
        [Required]
        [Range(0, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long QassessmentScriptId { get; set; }
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long ScriptID { get; set; }
        [Required]
        public string ScriptName { get; set; }
        [Range(1, 999.99, ErrorMessage = "Please enter a value bigger than {1}")]
        public decimal? MaxMarks { get; set; }
        public decimal? FinalizedMarks { get; set; }
        [Required]
        public bool IsSelected { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int? OrderIndex { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long? MarkedBy { get; set; }
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long ScriptCategorizationPoolID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int? IndexNo { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long? UserScriptMarkingRefID { get; set; }

    }


    public class QualifyingAssessmentScriptCreationModel
    {
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long ScriptId { get; set; }
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long QualifyingAssessmentId { get; set; }
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long QassessmentScriptId { get; set; }
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long ScriptCategorizationPoolId { get; set; }
        [Required]
        public bool? IsSelected { get; set; }
        [Required]
        public bool IsTagged { get; set; }

    }

    public class S1Complted
    {

        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long EntityID { get; set; }
        [Required]
        [Range(2, 2, ErrorMessage = "Please enter a value {2}")]
        public byte EntityType { get; set; }
        [MaxLength(2000)]
        public string Remarks { get; set; }
        public string WorkflowStatusCode { get; set; }
        [Required]
        public byte ProcessStatus { get; set; }
        [Required]
        public int WorkflowStatusId { get; set; }
        [Required]
        public long ProjectWorkflowTrackingID { get; set; }
        public Boolean Buttonremarks { get; set; }
        public List<QualifyingAssessmentScriptDetailsCreationModel> ScriptCategorizedList { get; set; }
    }

    public class StandardisationQualifyAssessmentCreationModel
    {

        public StandardisationQualifyAssessmentCreationModel()
        {
            Scripts = new List<StandardisationScriptCreationClass>();
        }
        [XssTextValidation]
        public string Qigname { get; set; }
        public int Noofscripts { get; set; }
        public int Markedscript { get; set; }
        /// <summary>
        /// 1. Not Started
        /// 2. In Progress
        /// 3. Pass
        /// 4. Fail
        /// 5. Pending
        /// </summary>
        public int Result { get; set; }
        public decimal TotalMarks { get; set; }

        public int ProcessStatus { get; set; }

        public List<StandardisationScriptCreationClass> Scripts { get; set; }

    }

    public class StandardisationScriptCreationClass
    {
        [XssTextValidation]
        public string ScriptName { get; set; }
        public bool? IsCompleted { get; set; }
        public bool? Result { get; set; }
        public long MarkedBy { get; set; }
        public long ScriptId { get; set; }
        public int WorkflowStatusID { get; set; }
        public long? UserMarkedBy { get; set; }
        public bool IsRecommended { get; set; }
        public long? RecommendedBy { get; set; }

    }


    public class ScriptCategorizationPoolCreationModel
    {
        public ScriptCategorizationPoolCreationModel()
        {
        }
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long ScriptCategorizationPoolID { get; set; }
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long ScriptID { get; set; }
        [XssTextValidation]
        public string ScriptName { get; set; }
        [Range(1, 999.99, ErrorMessage = "Please enter a value bigger than {1}")]
        public decimal? MaxMarks { get; set; }
        [Range(1, 999.99, ErrorMessage = "Please enter a value bigger than {1}")]
        public decimal? FinalizedMarks { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long? UserScriptMarkingRefID { get; set; }
        [Required]
        public bool IsSelected { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int? OrderIndex { get; set; }
        public bool IsDeleted { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long? MarkedBy { get; set; }

    }

}



