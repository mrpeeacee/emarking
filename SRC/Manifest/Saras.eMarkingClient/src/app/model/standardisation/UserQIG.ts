import { ScriptCategorizationPoolType } from "../common-model";

export interface IUserQig {
  ProjectId: number;
  QigId: number;
  QigCode: string;
  QigName: string;
  IsS1Available: boolean;
}
export interface ITrialMarkQIG {
  ProjectId: number;
  QigId: number;
  QigCode: string;
  QigName: string;
  NoOfRecommendedScripts: number;
  NoOfTrialMarkedScripts: number;
  NoOfCategorizedScripts: number;
  StandardizationScriptsCount: number;
  AdditionalStdScriptsCount: number;
  BenchmarkScriptsCount: number;
  TrialMarkedScripts: ITrialMarkScripts;
  IsMarkSchemeIDMapped: boolean;
  MarkSchemeLevel: string;
}
export interface ITrialMarkScripts {
  ScriptId: number;
  ScriptName: string;
  NoOfKpsTrialMarked: number;
  CategoryType: ScriptCategorizationPoolType;
  IsTrialMarked: boolean;
  IsCategorized: boolean;
  IsTrailMarkedByMe: boolean;
  BandName: string;
}
export interface IScriptBandInfo {
  BandId: number;
  BandCode: string;
  BandName: string;
  QuestionCode: number;
  QuestionText: number;
}
