export class GetAllApplicationUsersModel {
    getAllUsersModel!: GetAllUsersModel[];
    userscount!: Userscount;
}

export class GetAllUsersModel {
    UserId!: number;
    ApplicationLevel!: number;
    Name!: string;
    LoginName!: string;
    RoleName!: string;
    SchooolName!: string;
    NRIC!: string;
    Phone!: number;
    Status!: boolean;
    isdeleted!: boolean;
    isactive!: boolean;
    isblocked!: boolean;
    RoleCode!: string;
    isDisable!: boolean;
    ROWCOUNT!: number;
    CuurentloggedinUserId!:number;
    selected!: boolean;
}

export class Userscount {
    Totalusers!: number;
    Activeusers!: number;
    InActiveusers!: number;
    Blockedusers!: number;
    ApplicationLevelOfLoginUserID!: number;
}

export class GetCreateEditUserModel {
    UserId!: number;
    Username!: string;
    Loginname!: string;
    Nric!: string;
    PhoneNum!: string;
    RoleCode!: string;
    RoleName!: string;
    SchooolCode!: string;
    SchoolName!: string;
  selectedroleinfo!: string;
  checkdataexist!: boolean;
    roles!: RoleDetails[];
    schools!: SchoolDetails[];
    Examlevels!: ExamLevels[];
}

export class ExamLevels {
    ExamLevelID!: number;
    ExamLevelCode!: string;
    ExamLevelName!: string;
    isselected!: boolean;
}

export class CreateEditUser {
    UserId!: number;
    Username!: string;
    Loginname!: string;
    SchooolCode!: string;
    Nric!: string;
    PhoneNum!: string;
    EmailId!: string;
    IsBlock!: string;
    ForgotCount!: number;
    LoginCount!: number;
    RoleCode!: string;
    RoleName!: string;
    firstName!: string;
    lastName!: string;
    SchoolName!: string;
    ProjectRoleName!:string;
    Examlevels!: ExamLevels[];
}

export class RoleDetails {
    RoleID!: number;
    RoleName!: string;
    RoleCode!: string;
    IsRoleSelected!: boolean;
}

export class SchoolDetails {
    SchoolID!: number;
    SchoolName!: string;
    SchoolCode!: string;
    IsSchoolSelected!: boolean;
}

export class RoleSchooldetails {
    roles!: RoleDetails[];
    schools!: SchoolDetails[];
}

export class ImportUsers {
    FirstName!: string;
    LoginName!: string;
    NRIC!: string;
    Role!: string;
    School!: string;
    Status!: boolean;
}

export class SearchFilterModel {
    SearchText!: string;
    SchoolCode: string = "";
    RoleCode: string = "";
    Status: string = "";
    PageNo!: number;
  PageSize!: number;
  SortField: string = "";
  SortOrder: number = 0;
  navigate!:number
}

export class PassPhrase {
    Id!: number;
    PassPharseCode!: string;
    IsActive!: boolean;
    IsDeleted!: boolean;
    CreatedBy!: number;
    CreatedDate!: string;
    ModifiedBy!: number;
    ModifiedDate!: string;
}

export class UserDetails {
    FirstName!: string;
    LoginName!: string;
    NRIC!: string;
    RoleCode!: string;
    SchoolName!: string;
    SchoolCode!: string;
    Status!: boolean;
    Password!: string;
    Error!: any[];

}

export class UserCreations {
    status!: string
    users!: UserDetails[];
}
