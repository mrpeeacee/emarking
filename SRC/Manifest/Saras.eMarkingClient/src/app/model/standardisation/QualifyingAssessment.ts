export class CategorizationPoolModel {
    ProjectID!: number;
    ScriptCategorizationPoolID !: number;
    ScriptID !: number;
    ScriptName!: string;
    QIGID !: number;
    PoolType !: number;
    UserScriptMarkingRefID !: number;
    MaxMarks  !: number;
    FinalizedMarks  !: number;
    IsScriptSelected!: boolean;
    IsSelected !: boolean;
    OrderIndex !: number;
    IsDeleted !: boolean;
    IndexNo !: number;
}
export class QulAssessmentModel {

    ProjectID !: number;
    QIGID !: number;
    QigName!:string;
    QualifyingAssessmentId !: number;
    TotalNoOfScripts !: number;
    NoOfScriptSelected !: number;
    ToleranceCount !: number;
    ScriptPresentationType !: number;
    ScriptPresentationTypeName!:string;
    ApprovalType !: number;
    ApprovalTypeName!:string;
    IsTagged !: boolean;
    IsActive !: boolean;
    RefQualifyingAssessmentId  !: number;
    IsDeleted !: boolean;
    QScriptDetails!: CategorizationPoolModel[]
}
export class ScriptdetailsModel {
    QualifyingAssessmentId !: number;
    ScriptCategorizationPoolId !: number;
    IsSelected !: boolean;
    OrderIndex !: number;
    IsDeleted !: boolean;
}

export class S1CompletedModel {
    ProjectWorkflowTrackingID!: number;
    EntityID !: number;
    EntityType !: number;
    Remarks !: string;
    ProcessStatus !: number;
    Buttonremarks!: boolean;
    ScriptCategorizedList!: CategorizationPoolModel[];
}   
