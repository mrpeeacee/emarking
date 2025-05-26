export class StandardisationOverviewModel {
    TotalScripts!: number;
    CategorisedScripts!: number;
    StandardisedScripts!: number;
    AddStandardisedScripts!: number;
    BenchmarkedScripts!: number;
}

export class StandardisationApprovalCountsModel {
    S3Cleared!: number;
    S2Cleared!: number;
    S3ApprovalsPending!: number;
    S2ApprovalsPending!: number;
    IsS2available!: boolean;
}

export class LiveMarkingOverviewsModel {
    LivePool!: number;
    Downloaded!: number;
    Approved!: number;
    RcDone!: number;
    ToBeRcd!: number;
    Reallocated!: number;
    Submitted!: number;
    Adhoc!: number;
    InGracePeriod!: number;
    NoResponseCount!: number;
    AutoModerated!: number;
    QuestionType!: number;
}