import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import {
  EMS1ReportModel,
  EMS2ReportModel,
  GetModeOfAssessmentModel,
  GetOralProjectClosureDetailsModel,
  OmsReportModel,
  ReportsOutboundLogsModel,
} from 'src/app/model/reports/ems-report';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { QigService } from 'src/app/services/project/qig.service';
import { EMSReportService } from 'src/app/services/reports/ems-report.service';
import { ReportOutboundLogsComponent } from '../report-outbound-logs/report-outbound-logs.component';

@Component({
  templateUrl: './ems-report.component.html',
  styleUrls: ['./ems-report.component.css'],
})
export class EmsReportComponent implements OnInit {
  Archive!: boolean;
  constructor(
    private emsService: EMSReportService,
    public translate: TranslateService,
    public commonService: CommonService,
    public Alert: AlertService,
    public qigservice: QigService,
    private dialog: MatDialog
  ) {}

  esm1ReportDetails!: EMS1ReportModel[];
  esm2ReportDetails!: EMS2ReportModel[];
  datalist!: any[];
  Dwnldloading: boolean = false;
  syncdata!: string;
  isClosed: any;
  MoaCode!: string;
  IsText: number = 1;
  omsReportDetails!: OmsReportModel[];
  ApiLogs!: ReportsOutboundLogsModel[];
  OralProjDtls!: GetOralProjectClosureDetailsModel;

  ngOnInit(): void {

    this.CheckArhive();
    this.GetModeOfAssessment();
    this.getApiLogs(0);
    this.setTitles();
  }

  intMessage: any = {
    nodata1: '',
    nodata2: '',
    nodataoms: '',
    prjnotcls: '',
    prjcls: '',
    prjomssyc: '',
    process: '',
    nodata: '',
    wrong: '',
    norecal: '',
    norecdeloms: '',
    norecdelems1: '',
    norecdelems2: '',
    uploadcancel: '',
  };

  setTitles() {
    this.translate.get('EmsReport.title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate.get('EmsReport.nodata1').subscribe((translated: string) => {
      this.intMessage.nodata1 = translated;
    });
    this.translate.get('EmsReport.nodata2').subscribe((translated: string) => {
      this.intMessage.nodata2 = translated;
    });
    this.translate
      .get('EmsReport.prjnotcls')
      .subscribe((translated: string) => {
        this.intMessage.prjnotcls = translated;
      });
    this.translate.get('EmsReport.prjcls').subscribe((translated: string) => {
      this.intMessage.prjcls = translated;
    });
    this.translate
      .get('EmsReport.prjomssyc')
      .subscribe((translated: string) => {
        this.intMessage.prjomssyc = translated;
      });
    this.translate.get('EmsReport.process').subscribe((translated: string) => {
      this.intMessage.process = translated;
    });
    this.translate.get('EmsReport.uploadcancel').subscribe((translated: string) => {
      this.intMessage.uploadcancel = translated;
    });
    this.translate.get('EmsReport.nodata').subscribe((translated: string) => {
      this.intMessage.nodata = translated;
    });
    this.translate.get('EmsReport.wrong').subscribe((translated: string) => {
      this.intMessage.wrong = translated;
    });
    this.translate
      .get('EmsReport.nodataoms')
      .subscribe((translated: string) => {
        this.intMessage.nodataoms = translated;
      });
    this.translate.get('EmsReport.norecal').subscribe((translated: string) => {
      this.intMessage.norecal = translated;
    });
    this.translate
      .get('EmsReport.norecdeloms')
      .subscribe((translated: string) => {
        this.intMessage.norecdeloms = translated;
      });
    this.translate
      .get('EmsReport.norecdelems1')
      .subscribe((translated: string) => {
        this.intMessage.norecdelems1 = translated;
      });
    this.translate
      .get('EmsReport.norecdelems2')
      .subscribe((translated: string) => {
        this.intMessage.norecdelems2 = translated;
      });
  }

  Getqigworkflowtracking() {
    this.qigservice
      .Getqigworkflowtracking(0, AppSettingEntityType.QIG)
      .subscribe((data) => {
        if (data != null && data != undefined) {
    
          let WorkFlowStatusTracking = data;
          this.isClosed = WorkFlowStatusTracking.filter(
            (x: any) => x.ProjectStatus == 3
          );
          if (this.isClosed <= 0) {
            this.translate
              .get(this.intMessage.prjnotcls)
              .subscribe((translated: string) => {
                this.Alert.warning(translated);
              });
          } else {
            this.translate
              .get(this.intMessage.prjcls)
              .subscribe((translated: string) => {
                this.Alert.warning(translated);
              });
          }
        }
      });
  }

  OnlyDelta: number = 0;
  selSchedule(event: any) {
    const value = event.target.value;
    this.OnlyDelta = value;
  }

  downloadEMSReport(sel: any, format: any) {
    if (sel == 1) {
      this.Dwnldloading = true;
      if (format == 'txt') {
        this.IsText = 2;
      } else {
        this.IsText = 1;
      }
      this.emsService.GetEms1Report(this.IsText, this.OnlyDelta).subscribe({
        next: (data: any) => {
          if (
            data != null &&
            data != undefined &&
            data.Items != null &&
            data.Items != undefined &&
            data.Items.length > 0
          ) {
            if (data.Status != 'ND001') {
              var fileName = data.FileName;
              this.esm1ReportDetails = data.Items;
              if (
                this.esm1ReportDetails != null &&
                this.esm1ReportDetails.length > 0 &&
                this.esm1ReportDetails != undefined
              ) {
                if (format == 'csv') {
                  fileName = sel == 0 ? 'no' : 'ems1_report';
                  let header: string = '';
                  this.translate
                    .get('EmsReport.ems1headers')
                    .subscribe((translated: any) => {
                      let columnNames = translated;
                      header = columnNames.join(',');
                    });
                  let csv = header;
                  csv += '\r\n';

                  if (sel == 0) {
                    this.datalist = [
                      {
                        ExamYear: null,
                        ExamLevel: null,
                        ExamSeries: null,
                        SubjectCode: null,
                        PaperCode: null,
                        MOACode: null,
                        IndexNumber: null,
                        Attendance: null,
                        QuestionCode: null,
                        ContentMark: null,
                        LanguageOrganisationMark: null,
                        TotalMark: null,
                      },
                    ];

                    this.datalist.forEach((c) => {
                      csv += [
                        c['ExamYear'],
                        c['ExamLevel'],
                        c['ExamSeries'],
                        c['SubjectCode'],
                        c['PaperCode'],
                        c['MOACode'],
                        c['IndexNumber'],
                        c['Attendance'],
                        c['QuestionCode'],
                        c['ContentMark'],
                        c['LanguageOrganisationMark'],
                        c['TotalMark'],
                      ].join(',');
                      csv += '\r\n';
                    });
                  } else {
                    this.esm1ReportDetails.forEach((element) => {
                      this.datalist = [
                        {
                          ExamYear: element == null ? null : element.ExamYear,
                          ExamLevel:
                            element == null ? null : element.ExamLevelCode,
                          ExamSeries:
                            element == null ? null : element.ExamSeriesCode,
                          SubjectCode:
                            element == null ? null : element.SubjectCode,
                          PaperCode:
                            element == null ? null : element.PaperNumber,
                          MOACode: element == null ? null : element.MOACode,
                          IndexNumber:
                            element == null ? null : element.IndexNumber,
                          Attendance:
                            element == null ? null : element.Attendance,
                          QuestionCode:
                            element == null ? null : element.QuestionCode,
                          ContentMark:
                            element == null ? null : element.ContentMark,
                          LanguageOrganisationMark:
                            element == null
                              ? null
                              : element.LanguageOrganisationMark,
                          TotalMark: element == null ? null : element.TotalMark,
                        },
                      ];

                      this.datalist.forEach((c) => {
                        csv += [
                          c['ExamYear'],
                          c['ExamLevel'],
                          c['ExamSeries'],
                          c['SubjectCode'],
                          c['PaperCode'],
                          c['MOACode'],
                          c['IndexNumber'],
                          c['Attendance'],
                          c['QuestionCode'],
                          c['ContentMark'],
                          c['LanguageOrganisationMark'],
                          c['TotalMark'],
                        ].join(',');
                        csv += '\r\n';
                      });
                    });
                  }
                  var blob = new Blob([csv], {
                    type: 'text/csv;charset=utf-8;',
                  });

                  var link = document.createElement('a');
                  if (link.download !== undefined) {
                    var url = URL.createObjectURL(blob);
                    link.setAttribute('href', url);
                    link.setAttribute('download', fileName);
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                  }
                } else if (format == 'txt') {
                  let columnNames = [''];
                  let header = columnNames.join(',');

                  let csv = header;
                  this.datalist = [];
                  this.esm1ReportDetails.forEach((element) => {
                    this.datalist.push(
                      element == null ? null : element.Results
                    );
                  });
                  this.datalist.forEach((c) => {
                    csv += c;
                    csv += '\r\n';
                  });

                  blob = new Blob([csv], {
                    type: 'text/json;charset=utf-8;',
                  });

                  link = document.createElement('a');
                  if (link.download !== undefined) {
                    url = URL.createObjectURL(blob);
                    link.setAttribute('href', url);
                    link.setAttribute('download', fileName + '.txt');
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                  }
                }
              } else {
                if (this.OnlyDelta == 0) {
                  this.Alert.warning(this.intMessage.nodata1);
                } else if (this.OnlyDelta == 1) {
                  this.Alert.warning(this.intMessage.norecdelems1);
                }
              }
            } else {
              if (this.OnlyDelta == 0) {
                this.Alert.warning(this.intMessage.nodata1);
              } else if (this.OnlyDelta == 1) {
                this.Alert.warning(this.intMessage.norecdelems1);
              }
            }
          } else {
            if (this.OnlyDelta == 0) {
              this.Alert.warning(this.intMessage.nodata1);
            } else if (this.OnlyDelta == 1) {
              this.Alert.warning(this.intMessage.norecdelems1);
            }
          }
        },
        error: (ad: any) => {
          throw ad;
        },
        complete: () => {
          this.Dwnldloading = false;
        },
      });
    } else if (sel == 2) {
      if (format == 'txt') {
        this.IsText = 2;
      } else {
        this.IsText = 1;
      }
      this.Dwnldloading = true;
      this.emsService.GetEms2Report(this.IsText, this.OnlyDelta).subscribe({
        next: (data: any) => {
          if (
            data != null &&
            data != undefined &&
            data.Items != null &&
            data.Items != undefined &&
            data.Items.length > 0
          ) {
            if (data.Status != 'ND001') {
              var fileName = data.FileName;
              this.esm2ReportDetails = data.Items;
              if (
                this.esm2ReportDetails != null &&
                this.esm2ReportDetails.length > 0 &&
                this.esm2ReportDetails != undefined
              ) {
                if (format == 'csv') {
                  let header: string = '';
                  let fileName = sel == 0 ? 'no' : 'ems2_report';
                  this.translate
                    .get('EmsReport.ems2headers')
                    .subscribe((translated: any) => {
                      let columnNames = translated;
                      header = columnNames.join(',');
                    });

                  let csv = header;
                  csv += '\r\n';

                  if (sel == 0) {
                    this.datalist = [
                      {
                        ExamYear: null,
                        ExamLevel: null,
                        ExamSeries: null,
                        SubjectCode: null,
                        PaperCode: null,
                        MOACode: null,
                        IndexNumber: null,
                        Attendance: null,
                        MarkerGroup: null,
                        QuestionCode: null,
                        Mark: null,
                        SupervisorIndicator: null,
                      },
                    ];

                    this.datalist.forEach((c) => {
                      csv += [
                        c['ExamYear'],
                        c['ExamLevel'],
                        c['ExamSeries'],
                        c['SubjectCode'],
                        c['PaperCode'],
                        c['MOACode'],
                        c['IndexNumber'],
                        c['Attendance'],
                        c['MarkerGroup'],
                        c['QuestionCode'],
                        c['Mark'],
                        c['SupervisorIndicator'],
                      ].join(',');
                      csv += '\r\n';
                    });
                  } else {
                    this.esm2ReportDetails.forEach((element) => {
                      this.datalist = [
                        {
                          ExamYear: element == null ? null : element.ExamYear,
                          ExamLevel:
                            element == null ? null : element.ExamLevelCode,
                          ExamSeries:
                            element == null ? null : element.ExamSeriesCode,
                          SubjectCode:
                            element == null ? null : element.SubjectCode,
                          PaperCode:
                            element == null ? null : element.PaperNumber,
                          MOACode: element == null ? null : element.MOACode,
                          IndexNumber:
                            element == null ? null : element.IndexNumber,
                          Attendance:
                            element == null ? null : element.Attendance,
                          MarkerGroup:
                            element == null ? null : element.MarkerGroup,
                          QuestionCode:
                            element == null ? null : element.QuestionCode,
                          Mark: element == null ? null : element.Mark,
                          SupervisorIndicator:
                            element == null
                              ? null
                              : element.SupervisorIndicator,
                        },
                      ];

                      this.datalist.forEach((c) => {
                        csv += [
                          c['ExamYear'],
                          c['ExamLevel'],
                          c['ExamSeries'],
                          c['SubjectCode'],
                          c['PaperCode'],
                          c['MOACode'],
                          c['IndexNumber'],
                          c['Attendance'],
                          c['MarkerGroup'],
                          c['QuestionCode'],
                          c['Mark'],
                          c['SupervisorIndicator'],
                        ].join(',');
                        csv += '\r\n';
                      });
                    });
                    var blob = new Blob([csv], {
                      type: 'text/csv;charset=utf-8;',
                    });

                    var link = document.createElement('a');
                    if (link.download !== undefined) {
                      var url = URL.createObjectURL(blob);
                      link.setAttribute('href', url);
                      link.setAttribute('download', fileName);
                      document.body.appendChild(link);
                      link.click();
                      document.body.removeChild(link);
                    }
                  }
                } else if (format == 'txt') {
                  let columnNames = [''];
                  let header = columnNames.join(',');

                  let csv = header;
                  this.datalist = [];
                  this.esm2ReportDetails.forEach((element) => {
                    this.datalist.push(
                      element == null ? null : element.Results
                    );
                  });
                  this.datalist.forEach((c) => {
                    csv += c;
                    csv += '\r\n';
                  });

                  var blob = new Blob([csv], {
                    type: 'text/json;charset=utf-8;',
                  });

                  var link = document.createElement('a');
                  if (link.download !== undefined) {
                    var url = URL.createObjectURL(blob);
                    link.setAttribute('href', url);
                    link.setAttribute('download', fileName + '.txt');
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                  }
                }
              } else {
                if (this.OnlyDelta == 0) {
                  this.Alert.warning(this.intMessage.nodata2);
                } else if (this.OnlyDelta == 1) {
                  this.Alert.warning(this.intMessage.norecdelems2);
                }
              }
            } else {
              if (this.OnlyDelta == 0) {
                this.Alert.warning(this.intMessage.nodata2);
              } else if (this.OnlyDelta == 1) {
                this.Alert.warning(this.intMessage.norecdelems2);
              }
            }
          } else {
            if (this.OnlyDelta == 0) {
              this.Alert.warning(this.intMessage.nodata2);
            } else if (this.OnlyDelta == 1) {
              this.Alert.warning(this.intMessage.norecdelems2);
            }
          }
        },
        error: (ad: any) => {
          throw ad;
        },
        complete: () => {
          this.Dwnldloading = false;
        },
      });
    } else if (sel == 3) {
      this.Dwnldloading = true;
      if (format == 'txt') {
        this.IsText = 2;
      } else {
        this.IsText = 1;
      }
      if (this.OnlyDelta == undefined && this.OnlyDelta == null) {
        this.OnlyDelta = 0;
      }
      this.emsService.GetOmsReport(this.IsText, this.OnlyDelta).subscribe({
        next: (data: any) => {
          if (
            data != null &&
            data != undefined &&
            data.Items != null &&
            data.Items != undefined &&
            data.Items.length > 0
          ) {
            if (data.Status != 'ND001') {
              var fileName = data.FileName;
              this.omsReportDetails = data.Items;
              if (
                this.omsReportDetails != null &&
                this.omsReportDetails.length > 0 &&
                this.omsReportDetails != undefined
              ) {
                if (format == 'csv') {
                  let fileName = sel == 0 ? 'no' : 'oms_report';
                  let header: string = '';
                  this.translate
                    .get('EmsReport.omsheader')
                    .subscribe((translated: any) => {
                      let columnNames = translated;
                      header = columnNames.join(',');
                    });

                  let csv = header;
                  csv += '\r\n';

                  if (sel == 0) {
                    this.datalist = [
                      {
                        ExamYear: null,
                        ExamLevel: null,
                        ExamSeries: null,
                        SubjectCode: null,
                        PaperCode: null,
                        MOACode: null,
                        IndexNumber: null,
                        Attendance: null,
                        Mark: null,
                        Results: null,
                      },
                    ];

                    this.datalist.forEach((c) => {
                      csv += [
                        c['ExamYear'],
                        c['ExamLevel'],
                        c['ExamSeries'],
                        c['SubjectCode'],
                        c['PaperCode'],
                        c['MOACode'],
                        c['IndexNumber'],
                        c['Attendance'],
                        c['Mark'],
                      ].join(',');
                      csv += '\r\n';
                    });
                  } else {
                    this.omsReportDetails.forEach((element) => {
                      this.datalist = [
                        {
                          ExamYear: element == null ? null : element.ExamYear,
                          ExamLevel: element == null ? null : element.ExamLevel,
                          ExamSeries:
                            element == null ? null : element.ExamSeries,
                          SubjectCode:
                            element == null ? null : element.SubjectCode,
                          PaperCode: element == null ? null : element.PaperCode,
                          MOACode: element == null ? null : element.MOACode,
                          IndexNumber:
                            element == null ? null : element.IndexNumber,
                          Attendance:
                            element == null ? null : element.Attendance,
                          Mark: element == null ? null : element.Mark,
                          Results: element == null ? null : element.Results,
                        },
                      ];

                      this.datalist.forEach((c) => {
                        csv += [
                          c['ExamYear'],
                          c['ExamLevel'],
                          c['ExamSeries'],
                          c['SubjectCode'],
                          c['PaperCode'],
                          c['MOACode'],
                          c['IndexNumber'],
                          c['Attendance'],
                          c['Mark'],
                        ].join(',');
                        csv += '\r\n';
                      });
                    });
                  }
                  var blob = new Blob([csv], {
                    type: 'text/csv;charset=utf-8;',
                  });

                  var link = document.createElement('a');
                  if (link.download !== undefined) {
                    var url = URL.createObjectURL(blob);
                    link.setAttribute('href', url);
                    link.setAttribute('download', fileName);
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                  }
                } else if (format == 'txt') {
                  let columnNames = [''];
                  let header = columnNames.join(',');

                  let csv = header;
                  this.datalist = [];
                  this.omsReportDetails.forEach((element) => {
                    this.datalist.push(
                      element == null ? null : element.Results
                    );
                  });
                  this.datalist.forEach((c) => {
                    csv += c;
                    csv += '\r\n';
                  });

                  var blob = new Blob([csv], {
                    type: 'text/json;charset=utf-8;',
                  });

                  var link = document.createElement('a');
                  if (link.download !== undefined) {
                    var url = URL.createObjectURL(blob);
                    link.setAttribute('href', url);
                    link.setAttribute('download', fileName + '.txt');
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                  }
                }
              } else {
                if (this.OnlyDelta == 0) {
                  this.Alert.warning(this.intMessage.nodataoms);
                } else if (this.OnlyDelta == 1) {
                  this.Alert.warning(this.intMessage.norecdeloms);
                }
              }
            } else {
              if (this.OnlyDelta == 0) {
                this.Alert.warning(this.intMessage.nodataoms);
              } else if (this.OnlyDelta == 1) {
                this.Alert.warning(this.intMessage.norecdeloms);
              }
            }
          } else {
            if (this.OnlyDelta == 0) {
              this.Alert.warning(this.intMessage.nodataoms);
            } else if (this.OnlyDelta == 1) {
              this.Alert.warning(this.intMessage.norecdeloms);
            }
          }
        },
        error: (ad: any) => {
          throw ad;
        },
        complete: () => {
          this.Dwnldloading = false;
        },
      });
    }
  }

  syncEmsReport(isType: number) {
    this.emsService.syncEmsReport(isType, this.OnlyDelta).subscribe({
      next: (res: any) => {
        debugger
        this.syncdata = res;
        if (
          this.syncdata != null ||
          this.syncdata != '' ||
          this.syncdata != undefined
        ) {
          if (this.syncdata == 'S001') {
            this.Alert.success(this.intMessage.process);
            this.getApiLogs(1);
          } 
          else if (this.syncdata == 'E002') {
            this.Alert.error(this.intMessage.uploadcancel);
            this.getApiLogs(1);
          } 
          else {
            if(isType == 1){
              if (this.OnlyDelta == 0) {
                this.Alert.warning(this.intMessage.nodata1);
              } else if (this.OnlyDelta == 1) {
                this.Alert.warning(this.intMessage.norecdelems1);
              }
            } else if(isType == 2){
              if (this.OnlyDelta == 0) {
                this.Alert.warning(this.intMessage.nodata2);
              } else if (this.OnlyDelta == 1) {
                this.Alert.warning(this.intMessage.norecdelems2);
              }
            } else if(isType == 3){
              if (this.OnlyDelta == 0) {
                this.Alert.warning(this.intMessage.nodataoms);
              } else if (this.OnlyDelta == 1) {
                this.Alert.warning(this.intMessage.norecdeloms);
              }
            }
          }
        } else {
          this.Alert.warning(this.intMessage.wrong);
        }
      },
      error: (ad: any) => {
        throw ad;
      },
      complete: () => {
        this.Dwnldloading = false;
      },
    });
  }

  GetModeOfAssessment() {
    this.emsService.GetModeOfAssessment().subscribe({
      next: (data: GetModeOfAssessmentModel) => {
        if (data != null || data != '' || data != undefined) {
          this.MoaCode = data.MOACode;
          if (this.MoaCode != 'eOral') {
            this.translate
              .get('EmsReport.pgedesc')
              .subscribe((translated: string) => {
                this.commonService.setPageDescription(translated);
              });
            this.Getqigworkflowtracking();
          } else {
            this.translate
              .get('EmsReport.omsdesc')
              .subscribe((translated: string) => {
                this.commonService.setPageDescription(translated);
              });
            this.GetOralProjectClosureDetails();
          }
        }
      },
      error: (ad: any) => {
        throw ad;
      },
    });
  }

  viewApiLogs(reportType: number) {
    this.getApiLogs(0);
    const editorDialog = this.dialog.open(ReportOutboundLogsComponent, {
      data: {
        type: reportType,
        ApiLogs: this.ApiLogs,
      },
      panelClass: 'fullviewpop',
    });
    editorDialog.afterClosed().subscribe();
  }

  IsEms1Processing: boolean = false;
  IsEms2Processing: boolean = false;
  IsOmsProcessing: boolean = false;

  getApiLogs(callType: number) {
    debugger
    this.Dwnldloading = true;
    this.IsEms1Processing = false;
    this.IsEms2Processing = false;
    this.IsOmsProcessing = false;

    this.emsService.GetReportApiLogs().subscribe({
      next: (data: ReportsOutboundLogsModel[]) => {
        this.ApiLogs = data;
        if (
          this.ApiLogs != null &&
          this.ApiLogs != undefined &&
          this.ApiLogs.length > 0
        ) {
          if (
            this.ApiLogs.findIndex(
              (a) => !a.IsProcessed && a.ReportType == 1
            ) >= 0
          ) {
            this.IsEms1Processing = true;
          }
          if (
            this.ApiLogs.findIndex(
              (a) => !a.IsProcessed && a.ReportType == 2
            ) >= 0
          ) {
            this.IsEms2Processing = true;
          }
          if (
            this.ApiLogs.findIndex(
              (a) => !a.IsProcessed && a.ReportType == 3
            ) >= 0
          ) {
            this.IsOmsProcessing = true;
          }
        }
      },
      error: (ad: any) => {
        throw ad;
      },
      complete: () => {
        this.Dwnldloading = false;
      },
    });
  }

  GetOralProjectClosureDetails() {
    this.emsService.GetOralProjectClosureDetails().subscribe({
      next: (data: GetOralProjectClosureDetailsModel) => {
        if (data != null || data != '' || data != undefined) {
          this.OralProjDtls = data;
          if (this.OralProjDtls.IsReadyForSync == false) {
            this.Alert.warning(this.intMessage.prjomssyc);
          }
        }
      },
      error: (ad: any) => {
        throw ad;
      },
    });
  }
  CheckArhive()
  {
    this.emsService.CheckArhive().subscribe({
      next: (data: any) => {
       
        this.Archive=data}
  })
}
}
