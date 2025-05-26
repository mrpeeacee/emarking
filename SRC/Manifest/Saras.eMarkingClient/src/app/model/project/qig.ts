import { AppSettingModel } from "../appsetting/app-setting-model";

export enum RandomCheckType {
  OneTier = 1,
  TwoTier = 2,
  None = 0
}

export class QigModel {
  QigId!: number;
  TotalLivePoolScriptCount!: number;
  LivePoolCount!: number;
  SubmittedCount!: number;
  RC1SubmittedCount!: number;
  RC1SelectedCount!: number;
  RC2SelectedCount!: number;
  FinalisedScriptCountLiveMarking!:number;
  FinalisedScriptCountRC2!:number;

  QigName!: string;
  ShowTeam!: boolean;
  ShowAnnotation!: boolean;
  ShowRc!: boolean;
  ShowQns!: boolean;
  IsQiGClosureEnabled!:boolean;
  TeamIds!: ProjectTeamsIdsModel[];
  RandomCheckSettings!: AppSettingModel[];
  RcType!: RandomCheckType;
  AnnotationSetting: AppSettingModel[] = []
}
export class ProjectTeamsIdsModel {
  QigId!: number;
  TeamQigId!: number;
  TeamId!: number;
  IsChecked!: boolean;
}
export class RandomCheckSettingsModel {
  QigId!: number;
  RcType!: RandomCheckType;
  SampleRate!: number;
  JobScheduleInMinutes!: number;
  RcDetailId!: number;
}

export class QigQuestionModel {
  QuestionCode!: string;
  QigId!:number;
  QuestionType!: number;
  SchemeName !: string;
  QuestionText!: string;
  ProjectQuestionID!: number;
  ProjectMarkSchemeId!: number;
  ToleranceLimit!: number;
  StepValue!: number;
  IsScoreComponentExists!: boolean;
  QuestionXML !: string;
  PassageId !: number;
  Scorecomponentdetails!: Scorecomponentdetails[];
  optionAreas!: OptionArea[];
  PassageText!: string;
  QText!: string;
  status!: string;
  IsQuestionXMLExist!: boolean;
  QigType!: number;
  MaxMark!:number;
  IsDiscrepancyExist!:boolean;
  MarkingType!:boolean;
  QuestionId!:number;
  BlankText!:string;
  noOfQuestions!:number;
  noOfMandatoryQuestion!:number;
  Isqigreset!:boolean;
  DisableMaxmark!:boolean;
  DiscrepancyStatus!:number;

}
export class QigUserModel {
  QigId!: number;
  QigCode!: string;
  QigName!: string;
  IsKp!: boolean;
  IsS1Available!: boolean;
}
export class Scorecomponentdetails {
  ScoreComponentId!: number;
  MaxMark!: number;
  ComponentCode!: string;
  ComponentName!: string;
  CompMarkSchemeId!: number;
  SchemeName!: string;
  ProjectMarkSchemeId!: number;
  ProjectQuestionId!: number;
  IsAutoCreated!:boolean;
}
export class OptionArea {
  OptionAreaName!: string;

}
export enum EnumQigConfigDetails {
  QigQuesstions = "QCQUESTIONS",
  MarkingType = "QCMARKINGTYPE",
  StdSetting = "QCSTDSETTINGS",
  AnnotationSetting = "QCANNOTATIONSETTINGS",
  LiveMarkingSetting = "QCLIVEMARKINGSETTINGS",
  RandomCheck = "QCRANDOMCHECK",
  Others = "QCOTHERS",
  QigSummary = "QCQIGSUMMARY",
  QigConfigDetailsGroupCode="QIGSETTINGS"
}
export class QigconfigDetails {
  AppsettingKey!: string;
  AppsettingKeyName!: string;
  Value!: boolean;
  DefaultValue!: boolean;
  ValueType!: number;
  SettingGroupCode!: string;
  SettingGroupName!: string;
  EntityID!: number;

}


