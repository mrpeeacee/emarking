export class AdminToolsModel {
  rc1report!: RC1ReportModel[];
  rc2report!: RC2ReportModel[];
  adhocreport!: AdHocReportModel[];
}

export class LiveMarkingProgressModel {
  SlNo!: number;
  MarkingProject!: string;
  QIGName!: string;
  DownloadedDateTime!: string;
  TotalManualMarkingScript!: number;
  DownloadedScripts!: number;
  ActionNeeded!: number;
  TotalPending!: number;
  TotalMarked!: number;
  CompletionRate!: number;
  IsProjectSame!: boolean;
}

export class RC1ReportModel {
  MarkingProject!: string;
  QIGName!: string;
  TotalScript!: number;
  TotalInProgressScript!: number;
  CheckOutScripts!: number;
  TotalCompleted!: number;
  CompletionRateInPercentage!: number;
}

export class RC2ReportModel {
  MarkingProject!: string;
  QIGName!: string;
  TotalScript!: number;
  TotalInProgressScript!: number;
  CheckOutScripts!: number;
  TotalCompleted!: number;
  CompletionRateInPercentage!: number;
}

export class AdHocReportModel {
  MarkingProject!: string;
  QIGName!: string;
  TotalScript!: number;
  CheckOutScripts!: number;
  TotalCompleted!: number;
  CompletionRateInPercentage!: number;
}

export class CandidateScriptModel {
  LoginName!: string;
  QIGName!: string;
  ScriptName!: string;
}

export class FrequencyDistributionModel {
  MarkingProject!: string;
  QuestionCode!: number;
  Blank!: string;
  ResponseText!: string;
  NoOfCandidatesAnswered!: number;
  Responses!: string;
  PercentageDistribution!: number;
  MarksAwarded!: number;
  MarkingType!: string;
  Remarks!: string;
  RowNumbers!: number;
  TotalRows!: number;
}

export class FIDIIdDetails
{
  ProductCode!: string;
  TNAQuestionCode!: string;
  ProjectQuestionID!:number;
  ScoreComponentID!:number;
  QIGID!:number;
  FI!:any;
  DI!:any;
  QuestionType!:any;
  QuestionCode!:any;
  SectionName!:string;
  ComponentName!: any;
  IsQIGLevel!: any;
  TotalNoOfCandidates!:any;
  Mean!:any;
  SD!:any;
  Marks!:any;
  Subpercentage!:any
  NullresponseMarks!:any;
  Nullresponsepercentage!:any;
  ZeroMarks!:any;
  Zeromarkspercentage!:any
  ItemMeanMark!:any;
  subjectMarksItemScoresModels!: SubjectMarksItemScoresModel[];
  ExtraScorelist!: any[];
  QuestionMarks!:any;
  ComponentMarks!:any;
  PercentTotScoredFullMark!:any;
  MaxMarks!:number;
  MinMarks!:number;
}
export class SyncMetaDataModel
{
  examLevel!: string;
  examSeries!: string;
  examYear!: string;
  subject!: string;
  paperNumber!: string;
  questionValues!: QuestionValue[];

}
export class QuestionValue{
  questionCode!: string;
  fi!: number;
  di!: number;
}
export class SubjectMarksItemScoresModel
{
  SubMarksitemsscores!:any;
  Marks!:any;
  Subpercentage!:any
}

export class GetAllProjectModel {
  ProjectId!: number;
  ProjectCode!: string;
  ProjectName!: string;
}

export class AllAnswerKeysModel
{ 
  ParentQuestionCode!:string;
  QuestionCode! :string;
  ChoiceText! : string;
  QuestionType!: number;
  QuestionOrder! :number;
  QuestionName!: string;
  TotalCount!:number;
  QuestionMarks!:number;
  OptionText!: string;
}

export class ClsMailSent
{
  ProjectUserRoleID!: number;
  Role!: string;
  School!: string;
  SearchText!: string;
  Status!: string;
  PageSize!: number;
  PageNo!: number;
  SortOrder!: number;
  SortField!: string;
  IsEnabled!: number;
}

export class Mailsentdetails
{
  UserName!: string;
  LoginName!: string;
  Role!: string;
  RoleCode!: string;
  NRIC!: string;
  IsActive!: boolean;
  IsMailSent!: boolean;
  MailSentDate!: any;
  TotalCount!: number;
}

export class ClsFilter
{
  Id!: number;
  Text!: string;
  Selected!: boolean;
  ischecked!: boolean;
}
export class GetAllMarkerPerformanceModel {
  ID!: number;
  ProjectId!: number;
  ProjectCode!: string;
  ProjectName!: string;
  RoleCode!: string;
  ReMarkedScripts!: number;
  AverageTime!: number;
  TotalTimeTaken!: number;
  NoOfScripts!: number;
  StartDate!: string;
  EndDate!: string;
  ProjectUserRoleID!: number;
  MarkerName!: string;
}
