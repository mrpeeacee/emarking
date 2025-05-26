using Saras.eMarking.Domain.ViewModels.AuditTrail;
using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels.AuditModels.Modules.ProjectSetup
{
    public class BasicDetailsAction : IAuditTrail
    {
        public string PaperInfo { get; set; }
    }

    public class QigSettingConfigurationAction
    {
        public long QIGId { get; set; }
        public string DownloadBatchSize { get; set; }
        public Boolean ExceedDailyQuotaLimit { get; set; }
        public string GracePeriod { get; set; }
        public Boolean IsAnnotationsMandatory { get; set; }
        public Boolean IsPauseMarksingProcessEnabled { get; set; }
        public Boolean IsQIGClosureEnabled { get; set; }
        public Boolean IsScriptRecommended { get; set; }
        public Boolean IsScriptTrailMarked { get; set; }
        public string MarkingType { get; set; }
        public string PauseMarkingProcessRemarks { get; set; }
        public string QIGClosureRemarks { get; set; }
        public string RecommendedMarkScheme { get; set; }
        public Int16 StepValue { get; set; }
    }

    public class StandardisationSettingsAction
    {
        public long? QIGID { get; set; }
        public int? StandardizationScript { get; set; }
        public int? BenchmarkScript { get; set; }
        public int? AdditionalStdScript { get; set; }
        public bool? IsS1Available { get; set; }
        public bool? IsS2Available { get; set; }
        public bool? IsS3Available { get; set; }
        public bool IsPracticemandatory { get; set; }
    }

    public class ProjectLevelConfigurationAction
    {
        public long AppSettingKeyId { get; set; }
        public long AppSettingEntityId { get; set; }
        public long SettingGroupID { get; set; }
        public string SettingGroupName { get; set; }
        public string AppsettingKeyName { get; set; }
        public string GracePeriodValue { get; set; }
        public string GracePeriodDefaultValue { get; set; }

        public List<AppSettingModel> AppsettingModels { get; set; }
    }

    public class ProjectScheduleAction
    {
        public long ProjectScheduleID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ExpectedEndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string WorkingDaysConfig { get; set; }
    }

    public class AnnotationSettingAction
    {
        public Boolean IsAnnotationsMandatory { get; set; }
    }

    public class QigModelAction
    {
        public QigModelAction()
        {
        }
        public long QigId { get; set; }
        public string QigName { get; set; }
        public RandomCheckType RcType { get; set; }
        public List<ProjectTeamsIdsModel> TeamIds { get; set; }
        public List<AppSettingModel> RandomCheckSettings { get; set; }
        public List<AppSettingModel> AnnotationSetting { get; set; }
    }
    public class ResolutionCoi
    {
        public string SchoolName { get; set; }
        public string SchoolCode { get; set; }
        public int? ExemptionSchoolID { get; set; }
        public Boolean? IsSendingSchool { get; set; }
        public int? SchoolID { get; set; }
        public Boolean? IsSelectedSchool { get; set; }
    }


}
