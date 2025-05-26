export class EMS1ReportModel {
  ExamYear!: number;
  ExamLevelCode!: string;
  ExamSeriesCode!: string;
  SubjectCode!: string;
  PaperNumber!: number;
  MOACode!: string;
  IndexNumber!: string;
  Attendance!: number;
  QuestionID!: number;
  QuestionCode!: string;
  ContentMark!: number;
  LanguageOrganisationMark!: number;
  TotalMark!: number;
  Results!: any;
}

export class EMS2ReportModel {
  ExamYear!: number;
  ExamLevelCode!: string;
  ExamSeriesCode!: string;
  SubjectCode!: number;
  PaperNumber!: number;
  MOACode!: string;
  IndexNumber!: string;
  Attendance!: number;
  MarkerGroup!: string;
  QuestionID!: number;
  QuestionCode!: string;
  Mark!: number;
  SupervisorIndicator!: number;
  Results!: any;
}

export class OmsReportModel {
  ExamYear!: number;
  ExamLevel!: string;
  ExamSeries!: string;
  SubjectCode!: number;
  PaperCode!: number;
  MOACode!: string;
  IndexNumber!: string;
  Attendance!: string;
  Mark!: number;
  Results!: string;
}

export class GetModeOfAssessmentModel {
  MOACode!: string;
}

export class ReportsOutboundLogsModel {
  CorrelationId!: string;
  ReportType!: number;
  RequestDate!: Date;
  PageIndex!: number;
  FileName!: string;
  ProcessedOn!: Date;
  Status!: number;
  IsProcessed!: boolean;
  Remark!: string;
  RequestedBy!: string;
}

export class GetOralProjectClosureDetailsModel {
  IsReadyForSync!: boolean;
  scheduleDetails!: OralScheduleDetailsModel[];
}

export class OralScheduleDetailsModel {
  ScheduleCode!: string;
  ScheduleName!: string;
  ScheduleID!: number;
}

export class DownloadOutBoundLogModel {
  Count!: number;
  Results!: any;
}
