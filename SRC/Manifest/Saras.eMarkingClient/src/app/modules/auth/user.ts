import { Userrole } from "./userrole";

export class User {
  _id!: string;
  name!: string;
  email!: string;
  password!: string;
  role!: Userrole;
}


export class UserLogin {
  loginname!: string;
  password!: string;
  IsFirstTimeLoggedIn !: boolean;
  EmailId!: string;
  UserId !: number;
  oldpassword!: string;
  newpassword!: string;
  Status!: string;
  IsArchive!:boolean;
  LoginCount!:number;
  LastLoginDate:any;
  IscaptchaRequired!:boolean;
  CaptchaText!:string;
  GUID!:string;
  Timeleft:any;
  
}
export class PasswordChange {
  Oldpassword!: string;
  Newpassword!: string;
  Cnfnewpassword !: string;
  status!: string;
  NRIC !: string;
  LoginID !: string;
  CaptchaText !: string;
  GUID !: string;
  Timeleft:any;
}
export class CaptchaModel {
  CaptchaText!: string;
  GUID!: string;
  CaptchaImage !: string;
}



