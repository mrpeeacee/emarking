using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels.Project.Setup.QigManagement
{
    public class QigManagementModel
    {
        public string QuestionCode { get; set; }
        public Decimal QuestionMarks { get; set; }
        public long ProjectQigId { get; set; }
        public string QigName { get; set; }
        public string remarks { get; set; }
        public int TolerenceLimit { get; set; }
        public Boolean IsChildExist { get; set; }
        public long? ParentQuestionId { get; set; }
        public long ProjectQuestionId { get; set; }
        public long QuestionType { get; set; }
        public Boolean IsSelected { get; set; }
        public Boolean IsSetupCompleted { get; set; }
        public Boolean IsComposite { get; set; }
        public int? QuestionOrder { get; set; }
        public List<QigManagementModel> QigFibQuestions { get; set; }

    }

    public class QigDetails
    {
        public long QigId { get; set; }
        public long ProjectId { get; set; }
        public string QigName { get; set; }
        public int NoOfQuestions { get; set; }
        public int TotalMarks { get; set; }
        public int MandatoryQuestion { get; set; }
        public string MarkingType { get; set; }
        public Boolean IsQigSetup { get; set; }
        public List<QigQuestions> qigQuestions { get; set; }
        public int? QigType { get; set; }

    }


    public class QigQuestions
    {
        public long QuestionId { get; set; }
        public string QigQuestionName { get; set; }
        public int TotalMarks { get; set; }
        public long ProjectQuestionId { get; set; }
        public int QuestionType { get; set; }
        public long ParentQuestionID { get; set; }
        public int? questionOrder { get; set; }
        public bool? IsComposite { get; set;}

    }
    public class QigQuestionsDetails
    {
        public long QuestionId { get; set; }
        public long QIGID { get; set; }
        public string QigQuestionName { get; set; }
        public string QigName { get; set; }
        public int? QuestionOrder { get; set; }

        public decimal? TotalMarks { get; set; }
        public decimal? QigTotalMarks { get; set; }
        public int QuestionType { get; set; }
        public List<QignameDetails> QigIds { get; set; }
    }

    public class QignameDetails
    {
        public long QIGID { get; set; }
        public string QIGName { get; set; }
        public string QIGCode { get; set; }
    }
    public class Tagqigdetails
    {
        public long ProjectQuestionId { get; set; }
        public long ProjectQigId { get; set; }
        public long MoveQigId { get; set; }
        public decimal? QnsTotalMarks { get; set; }
        public decimal? QigTotalMarks { get; set; }
    }

    public class GetManagedQigListDetails
    {
        public GetManagedQigListDetails()
        {
            ManageQigsCountsList = new ManageQigsCounts();
            ManageQigsList = new List<ManageQigs>();

        }
        public bool IsResetDisable { get; set; }
        public bool IsResetEnable { get; set; }
        public ManageQigsCounts ManageQigsCountsList { get; set; }
        public List<ManageQigs> ManageQigsList { get; set; }
    }

    public class ManageQigs
    {
        public long projectqigId { get; set; }
        public string QigName { get; set; }
        public int NoOfQuestions { get; set; }
        public Decimal TotalMarks { get; set; }
        public string MarkingType { get; set; }
        public int MandtoryQuestions { get; set; }
        public Boolean IsQigSetupFinalized { get; set; }

        public string Remarks { get; set; }
        public int? QigType { get; set; }
    }
    public class ManageQigsCounts
    {
        public int TotalNoOfQIGs { get; set; }
        public int TotalNoOfQuestions { get; set; }
        public int TotalNoOfTaggedQuestions { get; set; }
        public int TotalNoOfUnTaggedQuestions { get; set; }
        public int IsProjectClosed { get; set; }
    }

    public class CreateQigsModel
    {
        public string QigName { get; set; }
        public int MarkingType { get; set; }
        public byte? Qigtype { get; set; }
    }

    public class BlankQuestionDetails
    {
        public string QigName { get; set; }
        public long ProjectQuestionId { get; set; }
        public decimal? MaxScore { get; set; }
        public string QuestionCode { get; set; }

    }

    public class SaveQigQuestions
    {
        public long QigId { get; set; }
        public string QigName { get; set; }
        public int ManadatoryQuestions { get; set; }
        public int QigMarkingType { get; set; }
        public string QigMarkingTypeName { get; set; }
        public int MarkingType { get; set; }
        public List<ProjectQuestions> projectQuestions { get; set; }
    }

    public class ProjectQuestions
    {
        public long ProjectQuestionId { get; set; }
        public long ParentQuestionId { get; set; }
    }
    public class ProjectQuestionQigIds
    {
        public long ProjectQigId { get; set; }
    }
    public class FinalRemarks
    {
        public string remarks { get; set; }
        public List<ProjectQuestionQigIds> ProjectQigId { get; set; }

    }

    public class QigResetDetails
    {
        public string RoleCode { get; set; }
        public long ProjectUserRoleId { get; set; }
        public string Remarks { get; set; } 
        public long projectId { get; set; }

    }
}
