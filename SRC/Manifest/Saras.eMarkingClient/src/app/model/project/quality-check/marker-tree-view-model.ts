export interface MarkerTreeView {
  ProjectUserName: string;
  ProjectUserRoleID: number;
  RoleCode: string;
  ReportingTo: number;
  IsKp: boolean;

}

export interface QualityCheckSummary {
  TotalScripts?: number;
  Submitted?: number;
  Reallocated?: number;
  ScriptRcdT1?: number;
  ScriptRcToBeT1?: number;
  ScriptRcdT2?: number;
  ScriptRcToBeT2?: number;
  RandomChecked?: number;
  AdhocChecked?: number;
  RcLevel: number;
  Downloaded: number;
  Resubmitted?: number;
  UserStatus:string;
  Returntomarker?:number;
}

export interface QualityCheckScriptDetails {
  ScriptId: number;
  ScriptName: string;
  SampledRc1: number;
  RC1Done: number;
  SampledRc2: number;
  RC2Done: number;
  AdhocCheck: number;
  RowCount: number;
  IsScriptCheckedOut: boolean;
  IsFinalised: boolean;
  RoleName: string;
  ACTIONDATE: any;
  IsChecked: boolean;
  IsLivePoolEnable: boolean;
  PhaseStatusTrackingID: number;
  Scriptstatus: number | null;
  Phase: number | null;
}
export class QualityCheckInitialScriptModel {
  ScriptId: number = 0;
  CheckedOutByMe: boolean = false;
  CheckoutEnabled: boolean = false;
  AdhocEnabled: boolean = false;
  WorkflowStatusID: number = 0;
  RcLevel: number = 0;
  Status: number = 0;
  Checkstatus: number = 0;
  IsInGracePeriod: boolean = false;
  IsRc2Adhoc: boolean = false;
  RettomarEnable: boolean = false;
  ScriptChildModel: ScriptChildModel[] = [];
}

export class ScriptChildModel {
  PhaseStatusTrackingId: number = 0;
  Phase: number = 0;
  Status: number = 0;
  MarkedBy: string = "";
  MarksAwarded: number = 0;
  IsActive: boolean = false;
  UserScriptMarkingRefId: number = 0;
  Remarks: string = "";
  Submitted!: Date;
  IsRCJobRun!: boolean;
  IsScriptFinalised!: boolean;
  RcLevel!: number;
  RoleCode!: string;
}



export class LivemarkingApproveModel {
  PhaseStatusTrackingId: number = 0;
  QigID: number = 0;
  ScriptID: number = 0;
  Remark: string = "";
  Status: number = 0;
  IsCheckout: boolean = false;
  WorkflowstatusId: number = 0;
}

export class TrialmarkingScriptDetails {
  QigID: number = 0;
  ScriptID: number = 0;
  WorkflowstatusId: number = 0;
}

export class Qualitycheckedbyusers {
  ScriptId: number = 0;
  ScriptName: string = "";
  ProjectUserRoleID: number = 0;
  UserName: string = "";
  UserRole: string = "";
  IsChecked: boolean = false;
}

export class Livepoolscript {
  QigId: number = 0;
  scriptsids!: Scriptsoflivepool[];
}

export class Scriptsoflivepool {
  ScriptId: number = 0;
}
