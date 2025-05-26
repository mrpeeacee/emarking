export class ScriptQuestionModel {
  Bands!: Array<BandModel>
  ScriptId!: number;
  ProjectQnsId!: number;
  TotalNoOfQuestions!: number;
  QuestionCode!: string;
  QuestionText?: string;
  QuestionOrder?: number;
  ResponseType?: string;
  ResponseText?: string;

}

export class BandModel {
  BandId?: number;
  MarkSchemeId?: number
  BandFrom?: number;
  BandTo?: number;
  BandDescription?: string;
  BandCode?: string;
  BandName?: string;
  IsSelected?: boolean;
}
