export class MarkerDetails {
    QigID: number = 0;
    ProjectId: number = 0;
    ExamYear: number = 0;
    MarkerName: string = "";
    SchoolCode: string = "";
}

export class MarkerPerformance{
     MarkerName :string="";
     School :string="";
     TotalScripts: number = 0;
     RC1: number = 0;
     RC2: number = 0;
     Adhoc: number = 0;
     RealLocated: number = 0;
}

export class MarkerPerformanceStatistics{
    TotalMarkerCount: number = 0;
    TotalSchoolCount: number = 0;
}

export class SchoolDetails{
    SchoolId: number = 0;
    SchoolCode:string="";
    SchoolName :string="";
}
