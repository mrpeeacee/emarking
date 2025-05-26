export class QigManagementModel {
    QuestionCode!: string;
    QuestionMarks!: number;
    ProjectQigId!: number;
    QigName!: string;
    TolerenceLimit!: number;
    IsChildExist!: boolean;
    ParentQuestionId!: number;
    ProjectQuestionId!: number;
    QnsType!: number;
    QigType!: number;
    remarks!: string;
    IsSelected !: boolean;
    QuestionType!: number;
    IsSetupCompleted!: boolean;
    QigFibQuestions!: QigManagementModel[];
    TempQigFibQuestions!: QigManagementModel[];
}
export class ManageQigs {
    projectqigId!: number;
    QigName!: string;
    NoOfQuestions!: number;
    TotalMarks!: number;
    MarkingType!: string;
    QigType!: number;
    MandtoryQuestions!: number;
    IsQigSetupFinalized!: boolean;
    Remarks!: string;
    TotalNoOfQuestions?: number;
    TotalNoOfQIGs?: number;
    //public int TotalNoOfQuestions { get; set; }
    TotalNoOfTaggedQuestions?: number;
    TotalNoOfUnTaggedQuestions?: number;
}

export class ManageQigsCounts {
    TotalNoOfQIGs!: number;
    TotalNoOfQuestions!: number;
    TotalNoOfTaggedQuestions!: number;
    TotalNoOfUnTaggedQuestions!: number;
    IsProjectClosed!:number;
}
export class GetManagedQigListDetails {
    IsResetDisable!: boolean;
    IsResetEnable!:boolean;
    ManageQigsCountsList!: ManageQigsCounts;
    ManageQigsList!: ManageQigs[];
}

export class QigDetails {
    QigName!: string;
    NoOfQuestions!: number;
    TotalMarks!: number;
    MarkingType!: boolean;
    MandatoryQuestion?: number;
    IsQigSetup!: boolean;
    qigQuestions!: QigQuestions[];
}

export class QigQuestions {
    QigQuestionName!: string;
    TotalMarks!: number;
}

export class QigQuestionModel {
    QuestionCode!: string;
    QuestionType!: number;
    MaxMark!: number;
    QuestionText!: string;
    QuestionXML!: string;
}

export class QigQuestionsDetails {
    QuestionId!: number;
    QigQuestionName!: string;
    QigName!: string;
    QigTotalMarks!: number;
    TotalMarks!: number;
    QIGID!: number;
    QuestionType !: number;
    QigIds!: QignameDetails[];
}

export class QignameDetails {
    QIGID!: number;
    QIGName!: string;
    QIGCode!: string;
}

export class Tagqigdetails {
    ProjectQuestionId!: number;
    ProjectQigId!: number;
    MoveQigId!: number;
    QigTotalMarks!: number;
    QnsTotalMarks!: number;
}

export class CreateQigsModel {
    QigId!: number;
    QigName!: string;
    QigMarkingType!: number;
    QigMarkingTypeName!:string;
    ManadatoryQuestions!: number;
    QuestionMarkingType!: number;
    projectQuestions!: ProjectQuestionIds[];

}
export class FinalRemarks {
    Remarks!: string;
    projectqigId!: ProjectQuestionQigIds[];
}
export class ProjectQuestionQigIds {
    projectqigId?:number;
}
export class ProjectQuestionIds {
    ProjectQuestionId!: number;
    ParentQuestionId!: number;
}

export class BulkUserUploadModel {
    FirstName!: string;
    LastName!: string;
    LoginName!: string;
    RoleCode!: string;
    ESSNNumber!: string;
    PhoneNumber!: string;
}

export class LoginCredential {
  UserName!: string;
  Password!: string;
}
