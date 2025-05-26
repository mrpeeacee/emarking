import { FileModel } from "../../file/file-model";

export interface MarkScheme {
  ProjectMarkSchemeId: number;
  ProjectName: string;
  SchemeCode: string;
  SchemeName: string;
  Marks: number;
  questionID: number;
  QuestionCode: string;
  Bands: Band[];
  Selected: boolean;
  SchemeDescription: string;
  MarkSchemeType: MarkSchemeType;
  MarkScheme: number;
  IsBandExist: boolean;
  IsQuestionTagged: boolean;
  filedetails: FileModel[];
}
export interface Band {
  BandChk: boolean;
  BandName: string;
  BandFrom: number;
  BandTo: number;
  BandDescription: string;
}
export interface Questions {
  QuestionId: number;
  QuestionLabel: string;
  MaxScore: string;
  SchemeName: string;
  IsTagged: boolean;
  TotalRows: number;
}
export interface QuestionTag {
  ProjectQuestionId: number;
  QuestionId: number;
  MarkSchemeId: number;
  MaxMark: string;
  IsTagged: boolean;
  QuestionCode: string;
  PanelOpenState: boolean;
  IsScoringComponentExist: boolean;
  ComponentName: string;
}

export interface QuestionText {
  QuestionText: string;
}

export enum MarkSchemeType {
  QuestionLevel = 1,
  ScoreComponentLevel = 2,
}

export interface Filedetails {
  Id?: number;
  FileName: string;
  FileContent?: File;
}
