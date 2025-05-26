export class AutomaticQuestionsModel {
  QuestionCode!: string;
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
  PassageText!: string;
  QText!: string;
  QuestionId!: number;
  QuestionVersion!: number;
  ChoiceList: ChoiceList[] = [];
  NotResponsded!: NotRespondedChoice;
  status!: string;
  IsQuestionXMLExist!:boolean;
  optionAreas!: OptionArea[];
  guidoptionAreas!:GuidOptionArea[];
  Isqigreset!:boolean;
  QuestionGUID!:string;
  GlobalMarkingType!:boolean;
}
export class ChoiceList
{
  Choices: Choice[] = [];
}
export class Choice {
  OptionsText!: string;
  CandidatesAnswer!: string;
  NoOfCandidatesAnswered!: number;
  NoOfCandidates!: number;
  Responses!: number;
  PerDistribution!: number;
  ProjectQuestionId!: number;
  IsCorrectAnswer!: boolean;
  ChoiceIdentifier!: string;
  markingType!: number;
  Remarks!: string;
}

export class ModeratescoreModel {
  IsCorrectAnswer!: boolean;
  QuestionType!: number;
  Remarks!: string;
  QuestionCode!:string;
  ChoiceText!: number;
  MarkingType!: number;
  ResponseText!: string;
  ProjectQuestionId!: number;
}

export class NotRespondedChoice {
  NoOfCandidatesNotAnswered!: number;
  NoresponsePerDistribution!: number;
  NoOptionText!: string;
  NoRespChoiceIdentifier!: string;
}
export class OptionArea {
  OptionAreaName!: string;
  OptionsText!: string;
  CandidatesAnswer!: string;
  NoOfCandidatesAnswered!: number;
  NoOfCandidates!: number;
  Responses!: number;
  PerDistribution!: number;
  ProjectQuestionId!: number;
  IsCorrectAnswer!: boolean;
  ChoiceIdentifier!: string;
  markingType!: number;
  Remarks!: string;
  QuestionGUID!:string;
}
export class GuidOptionArea {
  OptionAreaName!: string;
  QuestionGUID!:string;
}
