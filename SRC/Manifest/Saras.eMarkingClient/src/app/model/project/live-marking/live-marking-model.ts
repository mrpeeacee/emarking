export interface LiveMarkingModel {
  DownloadLimitCount: number;
  LivescriptCount: number;
  GraceperiodScript: number;
  SubmittedScript: number;
  SubmitScriptDailyCount: number;
  ReallocatedScript: number;
  QigName: string;
  RoleCode: string;
  Livescripts: Livescripts[];
}



export interface Livescripts {
  ScriptId: number;
  ScriptName: string;
  Seconds: number;
  Minute: number;
  ProjectId: number;
  MarkedBy: number;
  WorkflowStatusID: number;
  phase: number;
  GracePeriodEndDateTime: any;
  status: number;
  SubmittedDate: any;
  UserMarkRefID: any;
  Remarks: string;
  TotalAwardedMarks: number | undefined;
  TotalMaxMarks: number | undefined;
}

export class ClsLivescript {
  QigID: number = 0;
  pool: number = 0;
  FromDate: any;
  ToDate!: Date;
}

export class Timer {
  Minute: number = 0;
  Seconds: number = 0;
}
