using System;

namespace Saras.eMarking.Domain.ViewModels.Project.Dashboards
{
    public class MarkingOverviewsModel
    {
        public long? TotalScripts { get; set; }
        public long? Submitted { get; set; }
        public long? Reallocated { get; set; }
        public long? InGracePeriod { get; set; }

        public long? ScriptRcdT1 { get; set; }
        public long? ScriptRcToBeT1 { get; set; }
        public long? ScriptRcdT2 { get; set; }
        public long? ScriptRcToBeT2 { get; set; }

        public long? RandomChecked { get; set; }
        public long? AdhocChecked { get; set; }
        public TodayOverviewModel TodayOverview { get; set; }
        public bool IsLiveMarkingStart { get; set; }
    }

    public class TodayOverviewModel
    {
        public DateTime? Today { get; set; }
        public long Downloaded { get; set; }
        public long Submitted { get; set; }
        public long PendingSubmission { get; set; }
        public long Reallocated { get; set; }
        public long InGracePeriod { get; set; }
        public long RCDone { get; set; }
    }

    public class QuickLinksModel
    {
        public long LinkId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
    public class StandardisationOverviewModel
    {
        public long TotalScripts { get; set; }
        public long CategorisedScripts { get; set; }
        public long StandardisedScripts { get; set; }
        public long AddStandardisedScripts { get; set; }
        public long BenchmarkedScripts { get; set; }
    }

    public class StandardisationApprovalCountsModel
    {
        public long S3Cleared { get; set; }
        public long S2Cleared { get; set; }
        public long S3ApprovalsPending { get; set; }
        public long S2ApprovalsPending { get; set; }
        public bool IsS2available { get; set; }
    }

    public class LiveMarkingOverviewsModel
    {
        public long LivePool { get; set; }
        public long Downloaded { get; set; }
        public long Approved { get; set; }
        public long RcDone { get; set; }
        public long ToBeRcd { get; set; }
        public long? Reallocated { get; set; }
        public long Submitted { get; set; }
        public long Adhoc { get; set; }
        public long InGracePeriod { get; set; }
        public long NoResponseCount { get; set; }
        public long AutoModerated { get; set; }
        public int QuestionType { get; set; }
    }
}
