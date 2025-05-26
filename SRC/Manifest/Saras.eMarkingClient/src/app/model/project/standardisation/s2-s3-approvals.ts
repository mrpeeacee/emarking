export class S2S3Approvals {
  ScriptCount!: number;
  ToleranceCount!: number;
  ApprovalType!: number;
}

export class MarkingPersonal {
  UserRoleId?: number;
  MPName?: string;
  Role?: string;
  Supervisor?: string;
  OutOfTolerance?: number;
  S2S3AddScript?: number;
  Result?: number;
  ApprovalStatus?: string;
  ApprovalBy?: string;
  ToleranceCount?: number;
  Remarks?: string;
  ApprovalType?: string;
  IsQigPaused!: boolean;
  
}

export class AssignAdditionalStdScriptsModel {
  QIGID?: number;
  ProjectUserRoleID?: number;
  ScriptIDs!: AdditionalStdScriptsModel[];
}

export class ApprovalModel {
  projeectUserRoleId?: number;
  Remark!: string;
  markingPersonal!:string;
}

export class AdditionalStdScriptsModel {
  ScriptId?: number;
  ScriptName!: string;
  FinalizedMarks?: number;
  IsSelected!: boolean;
  UserScriptMarkingRefId!: number;
  UserMarkedBy!: number;
  IsCompleted!: boolean;
  Version!: number;
  IsQigpause!: boolean;
}
