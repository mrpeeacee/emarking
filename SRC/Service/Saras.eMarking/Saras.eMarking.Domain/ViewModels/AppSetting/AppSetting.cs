namespace Saras.eMarking.Domain.ViewModels
{



    public enum EnumAppSettingEntityType
    {
        None = 0,
        Project = 1,
        QIG = 2,
        User = 3,
        Role = 4,
        Question = 5
    }

    public enum EnumAppSettingValueType
    {
        String = 1,
        Integer = 2,
        Float = 3,
        XML = 4,
        DateTime = 5,
        Bit = 6,
        Int = 7,
        BigInt = 8,
        None = 0
    }
    public enum EnumAppSettingGroup
    {
        [StringValue("PRJCTSTTNG")]
        ProjectSettings,
        [StringValue("ANNTSN")]
        Annotation,
        [StringValue("RCSTNG")]
        RandomCheckSettings,
        [StringValue("STTNGS1")]
        Standardization1Settings,
        [StringValue("QIGSETTINGS")]
        QIGSettings,

    }
    public enum EnumAppSettingKey
    {
        [StringValue("GRCPRD")]
        GracePeriod,
        [StringValue("DSPLQUSTNTYPE")]
        DisplaycompleteAnswerbooklet,
        [StringValue("ALLQSTNTYPE")]
        AllQuestionTypes,
        [StringValue("ATMTCTYPE")]
        AutomaticType,
        [StringValue("SMATMTCTYPE")]
        SemiAutomatic,
        [StringValue("MNLTYPE")]
        ManualType,
        [StringValue("ANNTNCLR")]
        AnnotationColors,
        [StringValue("RLAO")]
        AssessmentOfficer,
        [StringValue("RLCM")]
        ChiefMarker,
        [StringValue("RLACM")]
        AssistantChiefMarker,
        [StringValue("RLTL")]
        TeamLeader,
        [StringValue("RLATL")]
        AssistantTeamLeader,
        [StringValue("MRKR")]
        Marker,
        [StringValue("MRKTYPE")]
        MarkingType,
        [StringValue("DSCRT")]
        Discrete,
        [StringValue("HOLSTC")]
        Holistic,
        [StringValue("RCT1")]
        RandomCheckTier1,
        [StringValue("SMPLRTT1")]
        SampleRateTier1,
        [StringValue("JBTMT1")]
        JobTimeTier1,
        [StringValue("RCT2")]
        RandomCheckTier2,
        [StringValue("SMPLRTT2")]
        SampleRateTier2,
        [StringValue("JBTMT2")]
        JobTimeTier2,
        [StringValue("RCMNDPOOLCNT")]
        RecomendationPoolCount,
        [StringValue("RCMNDCNTKP")]
        RecomendationPerKP,
        [StringValue("STEPVAL")]
        StepValue,
        [StringValue("QIGGRCEPERIOD")]
        QIGGracePeriod,
        [StringValue("DWNLDBTCHSZE")]
        DownloadBatchSize,
        [StringValue("RCMNDMRKSCHME")]
        RecommendationMarksScheme,
        [StringValue("QIGLVL")]
        QIGLevel,
        [StringValue("QUELVL")]
        QuestionLevel,
        [StringValue("EXDDAILYQUOTALIM")]
        ExceedDailyQuotaLimit,
        [StringValue("ANNTTNMANDTRY")]
        AnnotationsMandatory,
        [StringValue("PRCTCEMANDTRY")]
        PracticeMandatory,
        [StringValue("ALLSCPTS")]
        AllScripts,
        [StringValue("ONEORMORESCPTS")]
        OneorMoreScripts,
        [StringValue("DAILYQUOTA")]
        DailyQuotaLimitValue,
        [StringValue("SHOWQUOTATOMARKERS")]
        DailyQuotaToMarkers,        
        [StringValue("PROJECTRCJOBDURATION")]
        ProjectJobDuration,
        [StringValue("QCSTDSETTINGS")]
        QCStdSetting,
        [StringValue("QCANNOTATIONSETTINGS")]
        QCAnnotationSettings,
        [StringValue("QCLIVEMARKINGSETTINGS")]
        QCLiveMarking,
        [StringValue("QCMARKINGTYPE")]
        QCMarkingType,
        [StringValue("QCRANDOMCHECK")]
        QCRandomCheck,
        [StringValue("QCQUESTIONS")]
        QCQigQuestions
    }
}
