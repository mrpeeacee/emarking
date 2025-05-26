import { WorkflowProcessStatus } from '../common-model';

export class StandardisationQualifyAssessmentModel{
    Qigname!: string;
    IsQigPaused!: boolean;
    Noofscripts!: number;
    Markedscript!: number;
    Result!: number;
    AddResult!: number;
    TotalMarks!: number;
    ProcessStatus!:number;
    Isopened: boolean = false;
    UserScriptMarkingRefID!: number;
    WorkflowId!: number;
    ApprovalStatus!: string;
    Remarks!: string;
    PauseRemarks!:string;
    IsAdditionalDone!: boolean;
    Scripts!: StandardisationScriptClass[];
}

export class StandardisationScriptClass{
    ScriptName!: string;
    IsCompleted!: boolean;
    Result!: number;
    MarkedBy!: number;
    ScriptId!: number;
    WorkflowStatusID!: number;
    UserMarkedBy!: number;
    UserScriptMarkingRefID!: number;
    StdScore!: number;
    MarkerScore!: number;
    Isopened: boolean = false;
    IsOutOfTolerance!: boolean;
}

export class PracticeQuestionDetails{
    QuestionLabel!: string;
    TotalMarks!: number;
    DefenetiveMarks!: number;
    AwardedMarks!: number;
    ToleranceLimit!: number;
    IsOutOfTolerance!: boolean;
}

export class PracticeQualifyingEnable{
    IsPracticeEnable!: boolean;
    IsQualifyingEnable!: boolean;
    IsS1Enable!:boolean;
    IsS2Enable!:boolean;
    IsS3Enable!:boolean;
    IsKp!: boolean;
    S1Completed?: number;
    IsLiveMarkingStart!: boolean;
}

export class WorkflowStatusTrackingModel{
    WorkflowStatusCode!: string;
    ProcessStatus!: WorkflowProcessStatus;
    Remark!: string;
    ProjectStatus!: number;
}
