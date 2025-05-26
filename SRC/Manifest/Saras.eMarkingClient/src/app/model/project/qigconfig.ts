export interface IQigConfig
{
  GracePeriod :string,
  DownloadBatchSize :string,
  MarkingType :string,
  StepValue :string,
  RecommendMarkScheme :string,
  ExceedDailyQuotaLimit :string,
  IsQiGClosureEnabled :string,
  QiGClosureRemarks :string,
  IsPauseMarkingProcessEnabled :string,
  PauseMarkingProcessRemarks :string,
  IsAnnotationsMandatory :string
}
export interface IQigstandardizationsettings{
  QIGName: string;
  QIGCode: string;
  SettingID: number;
  QIGID: number;
  StandardizationScrip: number;
  BenchmarkScript: number;
  AdditionalStdScript: number;
  QualityAssuranceScript: string;
  Isdeleted: boolean;
  CreatedBy: number;
  CreatedDate: string;
  ModifiedBy: number;
  ModifiedDate: string;
  IsS1Available: boolean;
  IsS2Available: boolean;
  IsS3Available: boolean;
  S1StartDate: string;

}
export class AnnotationSettings{
  IsAnnotationsMandatory!:boolean;
}
export class ProjectQigModel{
  NoOfQuestions!:number;
  TotalMarks!:any;
  QuestionsType!:number;
}

