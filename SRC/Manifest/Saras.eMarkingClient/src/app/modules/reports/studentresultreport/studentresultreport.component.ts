import { Component, OnInit, ViewChild } from '@angular/core';
import { EMSReportService } from 'src/app/services/reports/ems-report.service';
import {
  StudentResultReportModel,
  StudentReport,
  QuestionCodeModel,
} from 'src/app/model/reports/studentreports';
import { first } from 'rxjs';
import { QigService } from 'src/app/services/project/qig.service';
import { QigUserModel } from 'src/app/model/project/qig';
import { AlertService } from 'src/app/services/common/alert.service';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';

@Component({
  selector: 'emarking-studentresultreport',
  templateUrl: './studentresultreport.component.html',
  styleUrls: ['./studentresultreport.component.css'],
})
export class StudentresultreportComponent implements OnInit {
  @ViewChild('indexnumber') stdnumber: any;
  selector: string = '.main-panel';
  studentReport: StudentReport[] = [];
  downloadreport!: StudentReport[];
  datalist!: any[];
  Ltqigs!: QigUserModel[];
  ltquestioncode!: QuestionCodeModel[];
  questioncodes!: QuestionCodeModel[];
  pagenumber: number = 1;
  title!: string;
  selectedvalue!: any;
  IsPageLoading: boolean = false;
  panelOpenState = false;
  config = {
    suppressScrollX: true,
  };
  constructor(
    private emsService: EMSReportService,
    public qigservice: QigService,
    public Alert: AlertService,
    public translate: TranslateService,
    public commonservice: CommonService
  ) {}

  ngOnInit(): void {
    this.studentReport = [];
    this.Internalization();
    this.GetQigs();
    this.GetStudentResultReport();
    this.GetQuestionsCodes(0);
  }
  Internalization(): any {
    this.translate
      .get('StdResultReport.title')
      .subscribe((translated: string) => {
        this.commonservice.setHeaderName(translated);
        this.title = translated;
      });
    this.translate.get('PageDescription').subscribe((translated: string) => {
      this.commonservice.setPageDescription(translated);
    });
  }

  GetStudentResultReport() {
    let stdreslt = new StudentResultReportModel();

    stdreslt.Questioncode = '';
    stdreslt.PageSize = 10;
    stdreslt.PageNo = this.pagenumber;

    this.GetStudentDetails(stdreslt);
  }

  GetStudentDetails(stdreslt: StudentResultReportModel) {
    this.IsPageLoading = true;
    this.emsService
      .StudentResultReport(stdreslt)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          for (let item of data) {
            if(item.QuestionType==152)
              {
                for(let childfib of item.childfib)
                  {
                    item.Awarded_Marks += childfib.Awarded_Marks;
                  }
              }
            this.studentReport.push(item);
          }
        },
        complete: () => {
          this.IsPageLoading = false;
        },
        error: (er: any) => {
          throw er;
        },
      });
  }

  GetQigs() {
    this.qigservice.getQigs().subscribe({
      next: (data: QigUserModel[]) => {
        this.Ltqigs = data;
      },
      error: (a: any) => {
        throw a;
      },
    });
  }

  GetQuestionsCodes(qigidval: number) {
    this.emsService.GetQuestionsCodes(qigidval).subscribe({
      next: (data: QuestionCodeModel[]) => {
        this.questioncodes = data;
        this.ltquestioncode = this.questioncodes.sort(
          (a, b) => a.QuestionCode - b.QuestionCode
        );
      },
      error: (a: any) => {
        throw a;
      },
    });
  }

  SelectQig(qigid: any) {
    this.selectedvalue = '0';
    this.GetQuestionsCodes(qigid);
  }

  GetStudentRecords(qigid: number, qc: any) {
    this.pagenumber = 1;
    this.studentReport = [];
    qc = qc == '0' ? '' : qc;
    let stdreslt = new StudentResultReportModel();

    stdreslt.Questioncode = qc;
    stdreslt.QIGID = qigid;
    stdreslt.LoginID = this.stdnumber.nativeElement.value.trim();
    stdreslt.PageSize = 10;
    stdreslt.PageNo = this.pagenumber;
    this.GetStudentDetails(stdreslt);
  }

  onScrollDown(qigid: number, qc: any) {
    this.pagenumber = this.pagenumber + 1;
    qc = qc == '0' ? '' : qc;
    let stdreslt = new StudentResultReportModel();
    stdreslt.Questioncode = qc;
    stdreslt.QIGID = qigid;
    stdreslt.LoginID = this.stdnumber.nativeElement.value.trim();
    stdreslt.PageSize = 10;
    stdreslt.PageNo = this.pagenumber;

    this.GetStudentDetails(stdreslt);
  }

  DownloadReport() {
    let stdreslt = new StudentResultReportModel();

    stdreslt.Questioncode = '';

    var scomp = '';
    this.emsService
      .StudentResultReport(stdreslt)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.downloadreport = data;
          if (
            this.downloadreport != null &&
            this.downloadreport.length > 0 &&
            this.downloadreport != undefined
          ) {
            let fileName = 'StudentResultReport';
            let columnNames = [
              'Student Index Number',
              'QIG Name',
              'Question No.',
              'Max. Marks',
              'Awarded Marks',
              'Band Name',
              'Scoring Component',
            ];
            let header = columnNames.join(',');

            let csv = header;
            csv += '\r\n';

            this.downloadreport.forEach((element) => {
              scomp = '';
              element.scoringComponentMarksModels.forEach((el) => {
                if (el.BandName == null || el.BandName == '') {
                  scomp += el.ComponentName + ':' + el.AwardedMarks + ' ';
                } else {
                  scomp +=
                    el.ComponentName +
                    ':' +
                    el.AwardedMarks +
                    ':' +
                    el.BandName +
                    ' ';
                }
              });

              this.datalist = [
                {
                  LoginID: element == null ? null : element.LoginID,
                  QIGName: element == null ? null : element.QIGName,
                  QuestionCode: element == null ? null : element.QuestionCode,
                  MaxMarks: element == null ? null : element.MaxMarks,
                  Awarded_Marks: element == null ? null : element.Awarded_Marks,
                  BandName: element == null ? null : element.BandName,
          
                  ComponentName: scomp,
                },
              ];
          
              this.datalist.forEach((c) => {
                csv += [
                  c['LoginID'],
                  c['QIGName'],
                  c['QuestionCode'],
                  c['MaxMarks'],
                  c['Awarded_Marks'],
                  c['BandName'],
                  c['ComponentName'],
                ].join(',');
                csv += '\r\n';
              });

              if (element.IsChildExists) {
                element.childfib.forEach((cf) => {

                  this.datalist = [
                    {
                      LoginID: cf == null ? null : cf.LoginID,
                      QIGName: cf == null ? null : cf.QIGName,
                      QuestionCode: cf == null ? null : cf.QuestionCode,
                      MaxMarks: cf == null ? null : cf.MaxMarks,
                      Awarded_Marks: cf == null ? null : cf.Awarded_Marks,
                      BandName: cf == null ? null : cf.BandName,
              
                      ComponentName: scomp,
                    },
                  ];
              
                  this.datalist.forEach((c) => {
                    csv += [
                      c['LoginID'],
                      c['QIGName'],
                      c['QuestionCode'],
                      c['MaxMarks'],
                      c['Awarded_Marks'],
                      c['BandName'],
                      c['ComponentName'],
                    ].join(',');
                    csv += '\r\n';
                  });

                });
              }
            });

            var blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });

            var link = document.createElement('a');
            this.getDataList(link, blob, fileName);
          } else {
            this.Alert.warning('No Data Found');
          }
        },
        error: (er: any) => {
          throw er;
        },
      });
  }

  getDataList(link: any, blob: any, fileName: any) {
    if (link.download !== undefined) {
      var url = URL.createObjectURL(blob);
      link.setAttribute('href', url);
      link.setAttribute('download', fileName);
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
  }
}
