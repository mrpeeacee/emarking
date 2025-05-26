import { WorkflowProcessStatus } from "../common-model";

export interface IRecommedQigData {
    RoleCode: string;
    QIGId: number;
    QIGName: string;
    TotalTargetRecomendations?: number;
    TotalRecomended?: number;
    TotalRecomendedByMe?: number;
    IsAoCm: boolean;
    QigScripts: IRecommedQigData[];
    IsKP: boolean;
    IsStdRequired: boolean;
}

export interface IRecommedQigScriptData {
    ProjectID: number;
    ScriptID: number;
    ScriptName: string;
    IsRecommended?: boolean;
    RecommendedBy?: string;
    IsRecommendedByMe: boolean;
    ProcessStatus: WorkflowProcessStatus;
    WorkFlowStatusCode: string;
    CenterID:number;
    CenterName:string;
    CenterCode:string;
    IscenterSelected: boolean;
}
