import { MarkScheme, MarkSchemeType } from './mark-scheme/mark-scheme-model';
export class ProjectQuestionModel {

    ScriptId!: number;
    ProjectQnsId!: number;
    TotalNoOfQuestions!: number;
    QuestionCode!: string;
    QuestionText?: string;
    QuestionOrder?: number;
    ResponseType?: string;
    ResponseText?: string;
}

export class UserScriptResponseModel {
    ProjectQuestionId?: number;
    ScriptId?: number;
    ScriptName?: string;
    TotalNoOfQuestions?: number;
    TotalNoOfResponses?: number;
    QIGName?: string;
    QuestionMarks!: number;
    userscriptID?: number;
    ProjectUserQuestionResponseID?: number;
    Annotation?: string;
    ImageBase64?: string;
    Comments?: string;
    BandID?: number;
    Marks?: number;
    Lastvisited?: boolean;
    MarkingStatus?: number;
    Remarks?: string;
    ScoreComponentMarkingDetail?: QuestionScoreComponentMarkingDetail[];
    ResponseText?:string;
    MarkedType?:number;
    Workflowstatusid?:number;
    TotalMarks?:number;
    NoofMandatoryQuestion?:number
    awardedmarks?:number;
    IsAutoSave?:boolean;
    IsViewMode?: boolean;
   
}

export class MarkingScriptTimeTracking {

    Qigid!:number;
    ProjectQuestionId!:number;
    UserScriptMarkingRefId!:number;
    WorkFlowStatusId!:number; 
    Mode?:number;
    Action?:number;
    TimeTaken?:string ;
   
}

export class QuestionUserResponseMarkingDetailsmodel {
    ScriptID!: number;
    ProjectQuestionResponseID?: number;
    CandidateID?: number;
    ScheduleUserID?: number;
    Annotation?: string;
    ImageBase64?: string;
    Comments?: string;
    BandID?: number;
    Marks?: number | null;
    IsActive?: boolean;
    IsDeleted!: boolean;
    MarkedBy?: any;
    Markeddate?: Date;
    MarkingStatus?: number;
    WorkflowstatusID?: any;
    LastVisited!: boolean;
    Remarks?: string;
    Markedtype?:number;
    TotalMarks?: number;
    Timer?: number;
    ScoreComponentMarkingDetail?: QuestionScoreComponentMarkingDetail[];
}

export interface ITrialMarkingData {
    ProjectId: number;
    ScriptId: number;
    ScriptName: string;
    QigId: number;
    QigName: string;
    IsViewMode: boolean;
    Markedby?: number;
}

export interface Banddet {
    BandId: number;
    MarkSchemeId: number;
    BandFrom: number;
    BandTo: number;
    BandDescription: string;
    BandCode: string;
    BandName: string;
    IsSelected: boolean;
    ScoreComponentId: number;
    ComponentCode: string;
    ComponentName: string;
    MaxMarks: number;

}

export interface Band {
    BandId: number;
    BandFrom: number;
    BandTo: number;
     BandName: string;
    IsSelected: boolean;
}

export interface Scorecompnentdetails {
    ScoreComponentId: number;
    ComponentCode: string;
    ComponentName: string;
    MaxMarks: number;
    band: Band[];
}

export class UserScriptMarking {
    ScriptID!: number;
    ProjectId!: number;
    CandidateId!: number;
    ScheduleUserId!: number;
    TotalNoOfQuestions!: number;
    MarkedQuestions!: number;
    ScriptMarkingStatus!: number;
    WorkFlowStatusID!: number;
    MarkedBy?: any;
    scriptstatus?: boolean;
    IsViewMode?: boolean;
    UserScriptMarkingRefId?: number;
}

export interface QuestionScoreComponentMarkingDetail {
    UserScriptMarkingRefId?: number;
    QuestionUserResponseMarkingRefId?: number;
    ScoreComponentId: number;
    MaxMarks: number;
    AwardedMarks?: number | null;
    BandId?: number | null;
    MarkingStatus: number;
    WorkflowStatusId: number;
    IsActive: boolean;
    IsDeleted: boolean;
    MarkedBy: number;
}

export interface ResponseDetails {
    ScriptId: number;
    ProjectQnsId: number;
    TotalNoOfQuestions: number;
    QuestionCode: string;
    QuestionText: string;
    QuestionOrder: object;
    ResponseType: number;
    ResponseText: string;
    RecommendedBand: object;
    Bands: Banddet[];
    RecommendedBandId: number;
    QuestionMarks: object;
    ProjectUserQuestionResponseID: object;
    IsQigLevel: boolean;
    IsMarkSchemeTagged: boolean;
    IsScoreComponentExists: boolean;
    StepValue: object;
}

export class DownloadMarkschemeModel {
    MarkschemeType!: MarkSchemeType;
    MarkSchemes!: MarkScheme[];
}

export class DownloadMarkschemeDetails {
    SchemeName?: string;
    Marks?: number;
    SchemeDescription?: number;
    MarkingType?: number;
    BandName?: string;
    Bandfrom?: number;
    Bandto?: number;
    Banddescription?: string;
}

export class ViewScriptModel { 
    Type!:number;
    LoginName!:string;
    ScriptID!:number; 
    ScriptName!:string;
    QIGName!:string;
    WorkFlowStatusID!:number; 
    MarkedBy!:number ; 
    MarkerName!:string ;
    MarksAwarded!: number;
    MarkedDate!:Date ;
    ScriptPhaseTrackingID!:number ;
    UserScriptMarkingRefID!:number;
    Phase!:number;
    RoleCode!:string;
    SelectAsDefinitive!: boolean;
} 

