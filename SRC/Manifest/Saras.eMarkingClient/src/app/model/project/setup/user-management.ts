export class Projectuserscount {
  Totuserscount!: number;
  Aocount!: number;
  Cmcount!: number;
  Acmcount!: number;
  Tlcount!: number;
  Atlcount!: number;
  Markercount!: number;
}

export class ProjectUsersModel {
  UserName!: string;
  LoginName!: string;
  SendingSchoolID!: string;
  RoleID!: string;
  AppointStartDate!: any;
  AppointEndDate!: string;
  ProjectUserRoleID!: number;
  TotalUsercount!: number;
  isBlock!: boolean;
  userId!: number;
}

export class Qiguserdataviewmodel {
  UserName!: string;
  UserId!: number;
  LoginName!: string;
  LoginID!:string;
  ReportingToLoginName!:string;
  ReportingTo!: number;
  ReportingTousernamename!: string;
  RoleID!: string;
  RoleName!: string;
  AppointStartDate!: any;
  AppointEndDate!: string;
  ProjectUserRoleID!: number;
  TotalUsercount!: number;
  IsKPVal!: string;
  S2S3Clear!: string;
  RC1Count!: string;
  RC2Count!: string;
  Isactive!: boolean;
  Adhoc!: string;
  ProjectQIGTeamHierarchyID!: number;
  QIGId!:number;
  QIGName!:string;
}

export class UserModel {
  UserName!: string;
  LoginName!: string;
  SendingSchoolID!: string;
  RoleID!: string;
  AppointStartDate!: string;
  AppointEndDate!: string;
  NRIC!: string;
  EmailID!: string;
  PhoneNo!: string;
  Remarks!: string;
  Validuser!: string;
  Count!: number;
}

export class ProjectQigUserModel {
  LoginName!: string;
  Role!: string;
  ReportingTo!: string;
  Remarks!: string;
  Rownum!: number;
  returnstatus!: string;
}

export class QigUsersViewByIdModel {
  UserName!: string;
  LoginName!: string;
  SendingSchoolID!: string;
  RoleName!: string;
  RoleCode!: string;
  ProjectUserRoleID!: number;
  AppointStartDate!: string;
  AppointEndDate!: string;
  NRIC!: string;
  Email!: string;
  Phone!: string;
  ReportingToIds!: ReportingToDetails[];
  Order!: number;
  ProjectQIGTeamHierarchyID!: number;
  ReportingToID!: number;
}

export class ReportingToDetails {
  ProjectUserRoleID!: number;
  ReportingToName!: string;
  RoleCode!: string;
}

export class BlankQigIds {
  QigIds!: number;
  QigName!: string;
}
export class UserCreationModel {
  UserName !: string;
  LoginName !: string;
  SchoolListId!: SchoolListDetails[];
  SendingSchooolName!: string;
  RoleCode !: RoleDetails[];
  Appointmentstartdate !: any;
  Appointmentenddate !: any;
  NRIC !: number;
  Phone?: number;
  Password !: string;
}

export class SchoolListDetails {
  SchoolName !: string;
  SchooldetailsId !: number;
}
export class RoleDetails {
  RoleName !: string;
  RoleNameId !: number;
  RoleCode !: string;
}
export class MoveMarkingTeamToEmptyQig {
  FromQigId!: number;
  ToQigId!: number;
  ProjectID!: number;
  ProjectUserRoleId!: number;
}

export class CopyMarkingTeamCls {
  QigId!: number;
}

export class UserBulkUploadModel {
  FirstName!: string;
  LastName!: string;
  LoginName!: string;
  RoleCode!: string;
  ESSNNumber!: string;
  PhoneNumber!: string;
}
export class MoveMarkingTeamToEmptyQigs {
  FromQigId!: number;
  ToQigId!: number[];
  ProjectID!: number;
  ProjectUserRoleId!: number;
}
export class GetAllMappedUsersModel {
  UserId!: number;
  ProjectuserroleID!: number;
  Name!: string;
  LoginName!: string;
  RoleName!: string;
  RoleCode!: string;
  Phone!: string;
  NRIC!: number;
  SchooolName!: string;
  StartDate !: any;
  IsActive!: boolean;
  EndDate !: any;
  IsS1Enabled!: boolean;
  IsLiveMarkingEnabled!: boolean;
}
export class MappedUsersCount {
  RoleCounts!: number;
  RoleCode!: string;
  FilterUsersCount!: number;
  RoleName!: string;
}

export class MappedUsersList {
  mappedusercount!: MappedUsersCount[];
  allmappeduserlist!: GetAllMappedUsersModel[];
  OnlyAOresult!: GetAllMappedUsersModel[];
  UnMappedUserscount!: number;
  currentloginrolecode!: string;
}



export class UnMappedUsersList {
  unmappedusercount!: MappedUsersCount;
  allunmappeduserlist!: UnMappedUsersModel[];
  currentloginrolecode!: string;
  AoCount !: boolean;
}
export class UnMappedUsersModel {
  UserId!: number;
  Name!: string;
  LoginName!: string;
  RoleName!: string;
  RoleCode!: string;
  NRIC!: number;
  SchooolName!: string;
  ProjectuserroleID!: number;
  Isselected!: boolean;
}
export class RolesModel {
  ROleId!: number;
  Rolecode!: string;
  Order!: number;
}
export class SearchFilterModel {
  SearchText!: string;
  SchoolCode!: string;
  RoleCode!: string;
  Status!: string;
  PageNo!: number;
  PageSize!: number;
  SortField: string = "";
  SortOrder: number = 0;
  UnmappedUserCountbit!:boolean;
}
export class SaveMappedUsersModel {
  UserID!: any[];
  AOUserid!: number;
  Rolecode!: string;
  IsSelected!: boolean;
  Appointmentstartdate !: any;
  Appointmentenddate !: any;
}
export class UserWithdrawModel {
  LoginName!: string;
  Status !: number;
  ProjectId!: number;
  ID!: number;
  IsWithDrawn!: boolean;
  selected!:boolean;
  RowCount!:number;
  
}
export class TotalUserWithdrawModel {
  UserWithdraw!:UserWithdrawModel[];
  TotalUserCount!:number;
  TotalWithdrawnCount!:number;
  IseOral!: boolean;
}

export class SuspendUserDetails {
  ProjectUserRoleID!: number;
  QigId!: number;
  Remarks!: string;
  Status!: boolean;
  roleCode!:string;
  replacementPURId!:number;
  unmapProjectUserId!:number;
  FirstName!:string;
  roleName!: string;
  ReportingTo!: number;
  CurrentReportingTo!: boolean;
}
export class UnMapAodetails {
  UnmapProjectUserRoleID!: number;
  ReplacementUserID!: number;
 // selectedusername!:string;
}

export class ChangeRoleModel {
  QIGdetails?: QIGdetails[];
  Roledetails?: Roledetails[];
  IsActivityExists?: boolean;
  currentuserrolecode?: string;
  currentusername?: string;
}

export class QIGdetails {
  ProjectUserRoleID!: number;
  ProjectQIGID!: number;
  ReportingTo!:number;
  QIGCode!: string;
  SupervisorRoledetails!: SupervisorRoledetails[];
}

export class SupervisorRoledetails {
  ProjectUserRoleID!: number;
  RoleID!: number;
  RoleCode!: string;
  FirstName!: string;
  LastName!: string;
  Order!: any;
  QIGID!:number;
}

export class Roledetails {
  RoleLevelID!: any;
  RoleID!: number;
  RoleCode!: string;
  Order!: any;
}

export class Qigsupervisorroledetails {
  ProjectUserRoleID!: number; // user clicked
  ProjectQIGID!: number;  
  ReportingTo!: number;// selected
}

export class CreateEditProjectUserRoleChange {
  ProjectUserRoleID!: number;
  RoleCode!: string;
  ChangeType!: number;
  qigsupervisorroledetails!: Qigsupervisorroledetails[];
}


