export class Project {
    date!: any;
    summary!: any;
    ProjectName!: string;
    ProjectCode!: string;
    ProjectStatus!: number;
    CreationType!: number;
    ProjectEndDate!: string;
    CreateDate!: string;
    ProjectStartDate!: string;
    SubjectID!: number;
    PaperID!: number;
    ExamYear!: number;
    ExamseriesID!: number;
    ExamLevelID!: number;
    MOA!: number;
    ProjectID!: number;
    ProjectInfo!: string;
    ProjectUserrole!: ProjectUserrole;
    RoleID!: number;
    RoleCode!: string;
    RoleName!: string;
  }
  
  export class ProjectUserrole {
    RoleName!: string;
    RoleCode!: string;
    RoleID!: number;
    ProjectUserRoleID!: number;
    ReportingTo!: number;
    IsActive!: boolean;
  }
  
  export class ProjectStatistics {
    TotalProjects!: number;
    ProjectsInProgress!: number;
    ProjectsCompleted!: number;
  }
  export class ProjectInfo {
    ProjectName!: string;
    ProjectCode!: string;
    ProjectStatus!: number;
    CreationType!: number;
    ProjectEndDate!: string;
    CreateDate!: string;
    ProjectStartDate!: string;
    SubjectID!: number;
    PaperID!: number;
    ExamYear!: number;
    ExamseriesID!: number;
    ExamLevelID!: number;
    MOA!: number;
    ProjectInfo!: string;
  }
  export class ScheduleDate {
  
    ProjectScheduleID!: number;
    ProjectCalendarID!: number;
    ProjectID!: number;
    StartDate!: any;
    ExpectedEndDate!: any;
    StartTime!: string;
    EndTime!: string;
    WorkingDaysConfig!: any;
    CalendarDate!: string;
    DayType!: number;
    SelectedDate!: string;
    Remarks!: string;
    WorkingDaysConfigJson!: JSON;
    scheduleTimeModels!: ScheduleTime[];
    Confirmdialogeventvalue!: number;
    IsStartdatedisabled!: boolean;
    IsEnddatedisabled!: boolean;
  }
  export class ScheduleTime {
    ProjectCalendarID!: number;
    ProjectScheduleID!: number;
    CalendarDate!: string;
    DayType!: number;
    StartTime!: string;
    EndTime!: string;
    Remarks!: string;
  }
  export class ProjectTeamInfo {
    ProjectID!: number;
    ReportingTo!: number;
    ProjectTeamID!: number;
    TeamCode!: string;
    TeamName!: string;
    Markers: any = [];
    IsChecked: boolean = false;
  }
  
  export class QIGStandardizationScriptSettings {
    QIGName!: string;
    QIGCode!: string;
    SettingID!: number;
    QIGID!: number;
    StandardizationScript!: number;
    BenchmarkScript!: number;
    AdditionalStdScript!: number;
    QualityAssuranceScript!: string;
    Isdeleted!: boolean;
    CreatedBy!: number;
    CreatedDate!: string;
    ModifiedBy!: number;
    ModifiedDate!: string;
    IsS1Available!: boolean;
    IsS2Available!: boolean;
    IsS3Available!: boolean;
    S1StartDate!: string;
    Status!: any;
    IsPracticemandatory!: boolean;
  }
  
  export class ProjectConfig {
    EntityID!: number;
    EntityType!: number;
    AppSettingKeyID!: number;
    Value!: string;
    DefaultValue!: string;
    ValueType!: number;
    ReferanceID!: number;
    ProjectID!: number;
    OrganizationID!: number;
    Isdeleted!: boolean;
    CreatedBy!: number;
    CreatedDate!: string;
    ModifiedBy!: number;
    ModifiedDate!: string;
  }
  
  export class ProjectSchedule {
    ProjectScheduleID!: number;
    ProjectCalendarID!: number;
    ProjectID!: number;
    StartDate!: string;
    ExpectedEndDate!: string;
    StartTime!: string;
    EndTime!: string;
    WorkingDaysConfig!: string;
  }
  
  