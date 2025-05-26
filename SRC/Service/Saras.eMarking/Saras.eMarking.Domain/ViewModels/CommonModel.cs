using System;

namespace Saras.eMarking.Domain.ViewModels
{
    public enum EnumUserRole
    {
        //Admin = 7,
        EO = 7,
        AO = 1,
        CM = 2,
        ACM = 3,
        TL = 4,
        ATL = 5,
        MARKER = 6,
        All = 10
    }
    public enum RandomCheckType
    {
        OneTier = 1,
        TwoTier = 2,
        None = 0
    }

    public enum MarkingType
    {
        Discrete = 1,
        Holistic = 2,
        None = 0
    }

    public enum EnumQigQuestionType
    {
        FIB = 20
    }
    public enum QuestionResponseType : byte
    {
        Text = 1,
        Audio = 2, Video = 3,
        Documents = 4,
        None = 0
    }

    public enum EnumWorkflowType
    {
        None = 0,
        Script = 1,
        Project = 2,
        PoolType = 3,
        Qig = 4
    }

    [Serializable]
    public enum EnumWorkflowStatus
    {
        [StringValue("RCMMED")]
        Recomended,
        [StringValue("TRMARKG")]
        TrailMarking,
        [StringValue("CTGRTN")]
        Categorization,
        [StringValue("LVMRKG")]
        LiveMarking,
        [StringValue("RC1")]
        RandomCheck_1,
        [StringValue("RC2")]
        RandomCheck_2,
        [StringValue("ADCHK")]
        AdhocChecked,
        [StringValue("INISTUP")]
        InitialSetup,
        [StringValue("STDZTN")]
        Standardization,
        [StringValue("PRLVMRKNG")]
        ProjectLiveMarking,
        [StringValue("PRCLSR")]
        ProjectClosure,
        [StringValue("STD")]
        StandardizationScripts,
        [StringValue("ADDSTD")]
        AdditionalStandardizationScripts,
        [StringValue("BMRK")]
        BenchmarkScripts,
        [StringValue("STDZTN-1")]
        Standardization_1,
        [StringValue("STDZTN-2")]
        Standardization_2,
        [StringValue("STDZTN-3")]
        Standardization_3,
        [StringValue("QIGLVMARKNG")]
        Live_Marking_Qig,
        [StringValue("CLSURE")]
        Closure,
        [StringValue("PRACTICE")]
        Practice,
        [StringValue("QASSESSMENT")]
        QualifyingAssessment,
        [StringValue("ADDSTDZN")]
        AdditionalStandardization,
        [StringValue("PROCEED")]
        Proceed,
        [StringValue("PAUSE")]
        Pause,
        [StringValue("DWNDFORLVMRKNG")]
        DownloadLiveMarking,
        [StringValue("ESCALATE")]
        Escalate,
        [StringValue("QIGSETUP")]
        QigSetup,
        [StringValue("ADCHK")]
        AdhocCheck,
        [StringValue("QIGCREATION")]
        QIGCreation,
        [StringValue("PRJTREOPEN")]
        ProjectReOpen
    }

    public enum ScriptMarkingStatus
    {
        None = 0,
        In_Progress = 1,
        Completed = 2
    }

    public enum WorkflowProcessStatus
    {
        None = 0,
        Started = 1,
        InProgress = 2,
        Completed = 3,
        OnHold = 4,
        Closure = 5
    }

    public enum ScriptCategorizationPoolType
    {
        StandardizationScript = 1,
        AdditionalStandardizationScript = 2,
        BenchMarkScript = 3,
        None = 0
    }

    public enum AssessmentApprovalType
    {
        Manual = 1,
        Automatic = 2,
        None = 0
    }

    public enum AssessmentApprovalStatus
    {
        Waitingforsubmission = 0,
        Pending = 1,
        Rejected = 2,
        AdditionalstandardizationScriptsGiven = 3,
        Approved = 4,
        Suspended = 5
    }

    public enum MarkSchemeType
    {
        QuestionLevel = 1,
        ScoreComponentLevel = 2
    }


    public enum MarkingScriptStauts
    {
        Downloaded = 1,
        InProgress = 2,
        Submitted = 3,
        InRCPool = 4,
        Approved = 5,
        ReMarking = 6,
        RESubmitted = 7,
        Escalate = 8,
        InvalidateAndReMark = 9,
        InvalidateandLivePool = 10,
        ReturnToLivePool = 11
    }

    public enum MarkingScriptPhase
    {
        LiveMarking = 1,
        RC1 = 2,
        RC2 = 3,
        Adhoc = 4,
        Escalate = 5

    }

    public enum ScriptMarkedType
    {
        Auto = 1,
        Moderated = 2,
        Manual = 3
    }

    public enum UserStatusTrackingType
    {
        Disable = 1,
        Map = 2,
        UnMap = 3,
        Block = 4,
        Unblock = 5,
        Active = 6,
        InActive = 7,
        Remove = 8,
        Promotion = 9,
        Suspended = 10,
        Resume = 11,
        Enable = 12,
        EnableActive = 13,
    }

    public enum EnumUserTrackingStatusLevel
    {
        ApplicationLevel = 1,
        ProjectLevel = 2
    }
    
    public enum EnumFilesEntityType
    {
        TempFile = 0,
        MarkScheme = 1
    }
}
