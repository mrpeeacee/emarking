export class ProjectClosureModel {
  Remarks!: string;
  ReopenRemarks!: string;
  ProjectStatus!: string;
  Rpackexist!: boolean;
  QigModels!: ProjectClosureQigModel[];
  DiscrepancyModels!: CheckDiscrepancyModel[];
}

export class ProjectClosureQigModel {
  QigName!: string;
  TotalScriptCount!: number;
  ManualMarkingCount!: number;
  LivePoolScriptCount!: number;
  SubmittedScriptCount!: number;
  Rc1UnApprovedCount!: number;
  Rc2UnApprovedCount!: number;
  CheckedOutScripts!: number;
  IsClosed!: boolean;
  QuestionsType!: number;
  ToBeSampledForRC2!: number;
  RC2Exists!: number;
  ToBeSampledForRC1!: number;
  TotalSubmitted!: number;
  QigId!: number;
}

export class CheckDiscrepancyModel {
  QigName!: string;
  IsDiscrepancyExist!: boolean;
}
