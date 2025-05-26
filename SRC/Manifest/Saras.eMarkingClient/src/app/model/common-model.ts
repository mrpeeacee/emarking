export enum ScriptCategorizationPoolType {
    StandardizationScript = 1,
    AdditionalStandardizationScript = 2,
    BenchMarkScript = 3,
    None = 0
}

export enum WorkflowStatus {
    Recomended = "RCMMED",
    TrailMarking = "TRMARKG",
    Categorization = "CTGRTN",
    Standardization_1 = "STDZTN-1",
    Pause = "PAUSE",
    Closure = "CLSURE",
    ProjectClosure='PRCLSR'
}

export enum WorkflowType {
    None = 0,
    Script = 1,
    Project = 2,
    Categorization = 3,
    Qig = 4
}


export enum WorkflowProcessStatus {
    None = 0,
    Started = 1,
    InProgress = 2,
    Completed = 3,
    OnHold = 4,
    Closure = 5,
    ProjectClosure = 3
}

export class QigScriptModule{
    ScriptId!:number;
    QigId!:number;
}
