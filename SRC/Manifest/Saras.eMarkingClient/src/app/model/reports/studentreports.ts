
export class StudentsResultStatistics {
    TotalStudentsCount!: number;
    TotalSchoolCount!: number;
    TotalMarks!: number;
}

export class StudentsResult {
    StudentId!: string;
    SchoolName!: string;
    MaxMark!: number;
    SecuredMark!: number;
    Result!: boolean;
}

export class StudentwiseReportModel {
    QigName!: string;
    QigCode!: string;
    Questions!: StudentwiseQuestions[];
}

export class StudentwiseQuestions {
    QuestionNo!: string;
    QuestionCode!: string;
    MaxMarks!: number;
    SecuredMarks!: number;
}

export class SchoolInfoDetails {
    SchoolCode!: string;
    SchoolName!: string;
    SchoolID!: number;
    IsSchoolSelected!:boolean;
}

export class ParamStdDetails {
    StudentId: string = "";
    SchoolCode: string = "";
    Markfrom: number = 0;
    MarkTo: number = 100;
    pageNumber!: number;
    pageSize!: number;
    Check: boolean = false;
}

export class CourseValidationModel {
    Scheduleid!: string;
    ScheduleName!: string;
    IsExamClosed!: boolean;
    JobStatus!: string;
    ProjectName!: string;
    ProjectCreatedDate!: Date;
    IsMarkPersonImported!: boolean;
    IsScriptsImported!: boolean;
    IsReadyForEmarkingProcess!: boolean;
}


export class StudentReport{
    ProjectID!:number;
    QIGID!:number;
    ScheduleUserID!:string;
    ProjectQuestionID!:number;
    QuestionVersion!:string;
    QuestionType!:string;
    QuestionCode!:string;
    RowNumber!:number;
    QuestionID!:number;
    LoginID!:string;
    QIGName!:string;
    Question_No!:string;
    MaxMarks!:number;
    Awarded_Marks!:number;
    BandName!:string;
    BandFrom!:number;
    BandTo!:number;
    IsChildExists!:boolean;
    scoringComponentMarksModels!:ScoringComponentMarksModel[];
    childfib!:StudentReport[]
}

export class ScoringComponentMarksModel{
    ProjectQuestionID!:number;
    QuestionID!:number;
    ComponentName!:string;
    MaxMarks!:number;
    AwardedMarks!:number;

    BandName!:string;
    BandFrom!:number;
    BandTo!:number;
}


export class StudentResultReportModel{
    Questioncode!:string;
    QIGID!:number;
    PageNo!:number;
    PageSize!:number;
    PageIndex!:number;
    LoginID!:any;
}


export class QuestionCodeModel{
    QuestionCode!:any;
    QigId!:number;
    QuestionID!:number;
}

export class UserResponses{
    ProjectId!:number;
    ProjectQuestionId!:number;
    QuestionType!:number;
    QuestionCode!:number;
    ParentQuestionId!:number;
    IsChildExists!:number;
    QuestionId!:number;
    ResponseText:any;
    ProjectCenterId:any;
    candidateindex:any;
    QuestionText:any;
  UserQuestionResponses: any;
imgBase64Data:any;

}