import { ScriptCategorizationPoolType } from 'src/app/model/common-model';

export class CategorisationStasticsModel {
  TrialMarkedScript!: number;
  CategorisedScript!: number;
  StandardisedScript!: number;
  AdlStandardisedScript!: number;
  BenchMarkScript!: number;

  QigStandardisedScript!: number;
  QigAdlStandardisedScript!: number;
  QigBenchMarkScript!: number;
  RecommendationPoolCount!: number;
  RecommendedCount!: number;
}

export class CategorisationModel {
  ScriptId!: number;
  ScriptName!: number;
  TotalKpMarked!: number;
  PoolType!: ScriptCategorizationPoolType;
  FinalizedMarks!: number;
  PanelOpenState: boolean = false;
  CategorisationTrialMarks!: CategorisationTrialMarkModel[];
  IsInQfAsses: boolean = false;
  IsUnRecommandEnable: boolean = false;
  IsCategorization: boolean = false;
}

export class CategorisationTrialMarkModel1 {
  IsQigPaused!: boolean;
  QigId!: number;
  QigName!: string;
  ScriptId!: number;
  ScriptName!: string;
  IsS1Completed!: boolean;
  TotalMark!: number;
  NoKps!: number;
  WorkFlowId!: number;
  TrailMarkedScripts!: CategorisationTrialMarkModel[];
  ContentScores!: CatContentScore[];
  QuestionDetails!: CatQuestionDetails[];
  TotalKpMarked!: number;
  PoolType!: ScriptCategorizationPoolType;
  IsInQfAsses: boolean = false;
  ScoringCompExist: boolean = false;
}

export class CategorisationTrialMarkModel {
  MarkedBy!: number;
  FirstName!: string;
  LastName!: string;
  SelectAsDefinitive!: boolean;
  TotalMarks!: number;
  MarkAwarded!: number;
  MarkerId!: number;
  Phase!: string;
  QuestionDetails!: CatQuestionDetails[];
  LanguageScore!: number;
  MarkingRefId!: number;
  Poolcategory!:string;
  ScriptName!:string;
}

export class CatContentScore {
  Name!: string;
  Marks!: number;
  Id!: number;
  WorkFlowStatusId!:number;
}

export class CatQuestionDetails {
  QuestionId!: number;
  QuestionCode!: string;
  Marks!: number;
  Type!:number;
  IsScoreComponentExists!:boolean;
  WorkFlowStatusId!:number;
  ContentScores!: CatContentScore[];
}

export class CategoriseAsModel {
  MarkedBy!: number;
  SelectAsDefinitive!: boolean;
  ScriptId!: number;
  QigId!: number;
  PoolType!: ScriptCategorizationPoolType;
  Poolcategory!:string;
  ScriptName!:string;
  MarkingRefId!:number;
}
