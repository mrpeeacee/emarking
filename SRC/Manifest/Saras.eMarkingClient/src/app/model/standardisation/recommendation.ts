export interface IRecommedData {
    ProjectId: number;
    ScriptId: number;
    ScriptName: string;
    QigId: number;
    QigName: string;
    IsViewMode: boolean | undefined;
    Markedby?: any;
    Workflowid?: number; 
    UserScriptMarkingRefId?: number;
    Status:number;
    RcType:number;
    PhaseStatusTrackingId:number;

}

export interface IBandingScriptResponse {
    element: {};
    ScriptId: number;
    ProjectQnsId: number;
    TotalNoOfQuestions: number;
    QuestionCode: string;
    QuestionText: string;
    QuestionOrder: number;
    ResponseText: string;
    RecommendedBand?: IBanding;
    Bands: IBanding[];
    IsActive: boolean;
    IsQigLevel: boolean;
    IsMarkSchemeTagged: boolean;
}

export interface IBanding {
    BandId: number;
    BandName: string;
    BandCode: string;
    IsSelected: boolean;
    BandFrom: number;
    BandTo: number;
}