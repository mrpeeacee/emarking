
export class ViewFrequencyDistributionModel {
  TotalMarks!: number;
  NoOfBlanks!: number;
  NoOfCandidates!: number;
  QuestionsText!: string;
  BlankOption!: BlankOption[];
  status!: string;
  IsCaseSensitive!: boolean;
}
export class BlankOption {
  CorrectAnswer!: string;
  BlankMarks!: number;
  ProjectQuestionId!: number;
  CandidateAnswer!: CandidatesAnswerModel[];
  disablemanualmark!: boolean;
  QIGCode!: string;
  QIGName!: string;
  QIGId!: number;
  disablemoderate!: boolean;
  MarkingType!: number;
  isenabledmanualmarking!: boolean;
  IsManuallyMarkEnabled!: boolean;
  IsCaseSensitive!: boolean;
  ParentQuestionId!: number;
  ResponseProcessingType!: number;
}
export class CandidatesAnswerModel {
  CandidatesAnswer!: string;
  Responses!: number;
  IsDiscrepancyExist!: boolean;
  PerDistribution!: number;
  MarkingType!: number;
  MarksAwarded!: number;
  QigId?: number;
  Id!: number;
  DiscrepancyStatus!: number;
}
export class EnableManualMarkigModel {
  Blank!: number;
  Score!: number;
  CorrectAnswer!: string;
  NoOfResponsedtobeEvaluated!: number;
  NoOfAnswerKeywords!: number;
  QigName!: string;
  StandardizationRequired!: boolean;
  Remarks!: string;
  QigId?: number;
  Id!: number;
  ProjectQuestionId!: number;
  ParentQuestionId!: number;

}

export class ViewAllBlankSummaryModel {
  BlankName!: string;
  QigName!: string;
  Responses!: number;
  ManualMarkingEnabled!: string;
  IsManualMarkingRequired!: boolean;
  Standardization!: string;
  IsS1Available!: boolean;
  QigId?: number;
  Remarks!: string;
  ResponsesToBeEvaluated!: number;
  BlankMarks!: number;
  ResponseProcessingType!: number;
}
export class ModerateMarks {
  Id!: number;
  MarkingType!: number;
  MarksAwarded!: number;
  CandidatesAnswer!: string;
  ProjectQuestionId!: number;
  ParentQuestionId!: number;
}
export class FibDiscrepencyReportModel {

  TotalNoOfScripts!: number;
  NoOfUnMarkedScripts!: number;
  NoOfMarkedScripts !: number;
  DistinctMarks!: number;
  ResponseText!: string;
  NormalisedScore!: number;
  QuestionMarks!: number
  FibDiscrepencies!: FibDiscrepancy[];
  FibMarkerDetails!: MarkerDetails[];
  DiscrepancyStatus!: number;
  MarkingType!: number;
}

export class FibDiscrepancy {
  SlNo !: number;
  MarksAwarded !: number;
  NoOfMarkers !: number;

}

export class MarkerDetails {
  UserName !: string;
  MarksAwarded !: number;
  LoginID !: string;
  MarkedDate !: string;
  ScriptName!: string;
  ScriptID!:number;
  Phase!: number;

}
export class DiscrepencyNormalizeScoreModel {
  ResponseText !: string;
  ProjectQuestionID !: number;
  MarksAwarded !: number;
  questionMarks!: number;
  QigId!: number;
  Id!: number;
  ScriptIds!:number[];

}

