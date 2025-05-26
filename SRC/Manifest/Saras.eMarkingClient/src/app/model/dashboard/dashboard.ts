import { ProjectUserrole } from "../project/project";


export class ProjectStatistics {
  TotalProjects!: number;
  ProjectsInProgress!: number;
  ProjectsCompleted!: number;
}

export class DashboardProject {
  ProjectID!: number;
  ProjectCode!: string;
  ProjectName!: string;
  ProjectStatus!: number;
  CreationType!: number;
  ProjectEndDate!: string;
  ProjectStartDate!: string;
  ProjectUserRole!: ProjectUserrole;
  RoleID!: number;
  RoleCode!: string;
  RoleName!: string;
  Year!: number;
  ProjectQigs!: DashboardProjectQigModel[];
}

export class DashboardProjectQigModel {
  QigName!: string;
  QigStatus!: string;
}

export interface MarkingOverviewsModel {
  TotalScripts: number;
  Submitted: number;
  Reallocated: number;
  ScriptRcdT1: number;
  ScriptRcToBeT1: number;
  ScriptRcdT2: number;
  ScriptRcToBeT2: number;
  AdhocChecked: number;
  RandomChecked: number;
  IsLiveMarkingStart: boolean;
  InGracePeriod:number;
  TodayOverview: TodayOverviewModel[];
}

export interface TodayOverviewModel {
  Today: Date;
  Downloaded: number;
  Submitted: number;
  PendingSubmission: number;
  Reallocated: number;
  InGracePeriod: number;
  RCDone: number;
  AdhocChecked:number;
}

export interface TodayOverviewModel {
  Today: Date;
  Downloaded: number;
  Submitted: number;
  PendingSubmission: number;
  Reallocated: number;
  InGracePeriod: number;
  RcDone: number;
}

export interface PrivilegesModel {
  ID: number;
  Name: string;
  RoleCode: string;
  ParentID: number;
  Url: string;
  PrivilegeOrder: number;
  PrivilegeCode: string;
}

export interface AllExamYear {
  YearId: number;
  Year: number;
  IsSelected: boolean;
}

export class ProjectStatusDetail{
  StartTime!:Date;
  EndTime!:Date;
  DayType!:number;
  status!:string;
  curDateTime!:Date
}



