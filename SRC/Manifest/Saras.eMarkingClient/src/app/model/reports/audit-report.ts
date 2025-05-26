export class AuditReportModel {
    EntityId?: number;
    EventId?: number;
    ModuleId?: number;
    UserId?: number;
    LoginId?: string;
    LoginName?: string;
    FirstName?: string;
    Remarks?: string;
    Status?: string;
    EventDateTime?: string;
    EntityCode?: string;
    ModuleCode?:string;
    EntityDescription?: string;
    EventCode?: string;
    EventDescription?: string;
    IpAddress?: string;
    StartDate?:string;
    EndDate?: string;
    FromDate: any;
    ToDate!: Date;
    LoginTime?:string;
    LogoutTime?:string;
    SessionId?:string;
    TotalRows!:number;
}
export class AuditReportRequestModel{
    LoginId?: string;
    StartDate?:string;
    EndDate?:string;
    ModuleCodes?:string;
    PageSize?:number;
    PageNo?:number;


}

export class ClsDates {
    FromDate: any;
    ToDate!: Date;
}

export class Timer {
    Minute: number = 0;
    Seconds: number = 0;
}
