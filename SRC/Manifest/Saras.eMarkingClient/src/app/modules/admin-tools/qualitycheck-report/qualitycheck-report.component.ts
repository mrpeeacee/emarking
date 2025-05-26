import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { first } from 'rxjs';
import {
  AdHocReportModel,
  AdminToolsModel,
  GetAllProjectModel,
  RC1ReportModel,
  RC2ReportModel,
} from 'src/app/model/admin-tools/admin-tools-model';
import { AdminToolsService } from 'src/app/services/admin-tools/admin-tools.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';

@Component({
  templateUrl: './qualitycheck-report.component.html',
  styleUrls: ['./qualitycheck-report.component.css'],
})
export class QualityCheckReportComponent implements OnInit {
  constructor(
    public Alert: AlertService,
    public translate: TranslateService,
    public commonService: CommonService,
    public adminService: AdminToolsService,
    public datepipe: DatePipe
  ) {}

  getRC1ReportList!: RC1ReportModel[];
  getRC2ReportList!: RC2ReportModel[];
  getAdHocList!: AdHocReportModel[];
  getQualityCheckList!: AdminToolsModel[];
  projectList!: GetAllProjectModel[];
  selectedProject = '0';
  selectedReport = '0';
  projectId: any = 0;
  reportCode: any = 0;
  datalist!: any[];
  projectName!: string;
  IsProjectSame!: boolean;
  ProjectSearchValue: string = '';
  ProjectsSearchList!: GetAllProjectModel[];
  selected: any[] = [];
  ZeroCountRC1: any;
  ZeroCountRC2: any;
  ZeroCountAdDoc: any;

  ngOnInit(): void {
    this.setTitles();
    this.GetProjects();
  }

  AltMessage: any = {
    SelProjAlt: '',
    SelRptAlt: '',
    InProjAlt: '',
    Rc1Rpt: '',
    Rc2Rpt: '',
    AdHocRpt: '',
    MrkProj: '',
    RptDate: '',
    NoRec: '',
  };

  setTitles() {
    this.translate.get('QuaCheck.Title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });

    this.translate.get('QuaCheck.Pagedesc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });

    this.translate
      .get('QuaCheck.SelProjAlt')
      .subscribe((translated: string) => {
        this.AltMessage.SelProjAlt = translated;
      });

    this.translate.get('QuaCheck.SelRptAlt').subscribe((translated: string) => {
      this.AltMessage.SelRptAlt = translated;
    });

    this.translate.get('QuaCheck.InProjAlt').subscribe((translated: string) => {
      this.AltMessage.InProjAlt = translated;
    });

    this.translate.get('QuaCheck.Rc1Rpt').subscribe((translated: string) => {
      this.AltMessage.Rc1Rpt = translated;
    });

    this.translate.get('QuaCheck.Rc2Rpt').subscribe((translated: string) => {
      this.AltMessage.Rc2Rpt = translated;
    });

    this.translate.get('QuaCheck.AdHocRpt').subscribe((translated: string) => {
      this.AltMessage.AdHocRpt = translated;
    });

    this.translate.get('QuaCheck.MarkProj').subscribe((translated: string) => {
      this.AltMessage.MarkProj = translated;
    });

    this.translate.get('QuaCheck.RptDate').subscribe((translated: string) => {
      this.AltMessage.RptDate = translated;
    });

    this.translate.get('QuaCheck.NoRecAlt').subscribe((translated: string) => {
      this.AltMessage.NoRecAlt = translated;
    });
  }

  clearSearch() {
    this.ProjectSearchValue = '';
    this.GetProjects();
  }

  GetProjects() {
    this.adminService
      .getAllProject()
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data != null || data != undefined || data.length > 0) {
            this.projectList = data;
            this.ProjectsSearchList = data;
            if (this.projectList.length == 1) {
              this.IsProjectSame = true;
              this.projectId = data[0].ProjectId;
            } else {
              this.IsProjectSame = false;
            }
          }
        },
        error: (a: any) => {
          throw a;
        },
      });
  }

  SearchProjects() {
    var ProjectSearchValue = this.ProjectSearchValue;
    this.projectList = this.ProjectsSearchList.filter(function (el) {
      return el.ProjectName.toLowerCase().includes(
        ProjectSearchValue.trim().toLowerCase()
      );
    });
    this.projectList = this.projectList.filter(
      (x) => this.selected.includes(x.ProjectId) || this.selected.length == 0
    );
  }

  Isprojchecked(event: any) {
    if (
      (event.source.value != null || event.source.value != undefined) &&
      event.source.value != 0
    ) {
      this.projectId = event.source.value;
    }
    if (event.source.value == 0) {
      this.projectId = this.getAdHocList;
      this.projectId = 0;
    }
    this.ProjectSearchValue = '';
    this.GetProjects();
  }

  Isreptchecked(event: any) {
    if (
      (event.source.value != null || event.source.value != undefined) &&
      event.source.value != 0
    ) {
      this.reportCode = event.source.value;
    }
    if (event.source.value == 0) {
      if (
        this.reportCode == 1 ||
        this.reportCode == 2 ||
        this.reportCode == 3
      ) {
        this.projectId = 0;
      }
    }
  }

  getQualityCheck(repCode: any = 0) {
    this.reportCode = repCode;
    if (this.projectId != 0 && repCode != 0) {
      if (repCode == 1) {
        this.getRC1Details();
      } else if (repCode == 2) {
        this.getRC2Details();
      } else {
        this.getAdHocDetails();
      }
    } else {
      if (this.projectId == 0) {
        this.Alert.warning(this.AltMessage.SelProjAlt);
      } else if (repCode == 0) {
        this.Alert.warning(this.AltMessage.SelRptAlt);
      }
    }
  }

  getRC1Details() {
    this.adminService
      .getQualityCheckDetails(this.projectId, 1)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data != null || data.length > 0) {
            if (data.status != 'P001') {
              this.getQualityCheckList = data;
              this.getRC1ReportList = data.rc1report;
              this.projectName = data.rc1report.find(
                (x: any) => x.MarkingProject
              ).MarkingProject;
              if (this.getQualityCheckList.length == 0) {
                this.Alert.warning(this.AltMessage.SelProjAlt);
              }
            } else {
              this.Alert.warning(this.AltMessage.InProjAlt);
            }
          } else {
            this.getQualityCheckList = [];
          }
        },
        error: (a: any) => {
          throw a;
        },
      });
  }

  getRC2Details() {
    this.adminService
      .getQualityCheckDetails(this.projectId, 2)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data != null || data?.length > 0) {
            if (data.status != 'P001') {
              this.getQualityCheckList = data;
              this.getRC2ReportList = data.rc2report;
              this.projectName = data.rc2report.find(
                (x: any) => x.MarkingProject
              ).MarkingProject;
              if (this.getQualityCheckList.length == 0) {
                this.Alert.warning(this.AltMessage.SelProjAlt);
              }
            } else {
              this.Alert.warning(this.AltMessage.InProjAlt);
            }
          } else {
            this.getQualityCheckList = [];
          }
        },
        error: (a: any) => {
          throw a;
        },
      });
  }

  getAdHocDetails() {
    this.adminService
      .getQualityCheckDetails(this.projectId, 3)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data != null || data?.length > 0) {
            if (data.status != 'P001') {
              this.getQualityCheckList = data;
              this.getAdHocList = data.adhocreport;
              this.projectName = data.adhocreport.find(
                (x: any) => x.MarkingProject
              ).MarkingProject;
              if (this.getQualityCheckList.length == 0) {
                this.Alert.warning(this.AltMessage.SelProjAlt);
              }
            } else {
              this.Alert.warning(this.AltMessage.InProjAlt);
            }
          } else {
            this.getQualityCheckList = [];
          }
        },
        error: (a: any) => {
          throw a;
        },
      });
  }

  download() {
    if (this.reportCode == 1) {
      this.downloadRC1Details();
    } else if (this.reportCode == 2) {
      this.downloadRC2Details();
    } else {
      this.downloadAdHocDetails();
    }
  }

  downloadRC1Details() {
    if (this.getRC1ReportList != null && this.getRC1ReportList.length > 0) {
      this.ZeroCountRC1 = this.getRC1ReportList.filter(zc => zc.TotalScript == 0).length;
      if (this.getRC1ReportList.length == this.ZeroCountRC1) {
        this.Alert.warning(this.AltMessage.NoRecAlt);
      } else {
        const now = new Date().toLocaleString();
        let latest_date = this.datepipe
          .transform(now, 'd MMM y H:mm')
          ?.toString();
        let fileName = this.AltMessage.Rc1Rpt;
        let header: string = '';
        this.translate
          .get('QuaCheck.TblHeader')
          .subscribe((translated: any) => {
            let columnNames = translated;
            header = columnNames.join(',');
          });
        let csv = '';
        csv =
          '\r\n ' +
          this.AltMessage.MarkProj +
          ', ' +
          this.projectName +
          ' \r\n ' +
          this.AltMessage.RptDate +
          ',' +
          latest_date +
          ' \r\n\r\n ';
        csv += header + ' \r\n ';

        this.getRC1ReportList.forEach((element) => {
          this.processRCDetailsData(element);

          this.datalist.forEach((c) => {
            csv += [
              c['SlNo'],
              c['QIGName'],
              c['TotalScript'],
              c['TotalInProgressScript'],
              c['CheckOutScripts'],
              c['TotalCompleted'],
              c['CompletionRateInPercentage'],
            ].join(',');
            csv += '\r\n';
          });
        });

        var blob = new Blob([csv], {
          type: 'text/csv;charset=utf-8;',
        });

        var link = document.createElement('a');
        this.getRCDataList(link, blob, fileName);
      }
    } else {
      this.Alert.warning(this.AltMessage.NoRecAlt);
    }
  }

  downloadRC2Details() {
    if (this.getRC2ReportList != null && this.getRC2ReportList.length > 0) {
      this.ZeroCountRC2 = this.getRC2ReportList.filter(zc => zc.TotalScript == 0).length;
      
      if (this.getRC2ReportList.length == this.ZeroCountRC2) {
        this.Alert.warning(this.AltMessage.NoRecAlt);
      } else {
        const now = new Date().toLocaleString();
        let latest_date = this.datepipe
          .transform(now, 'd MMM y H:mm')
          ?.toString();
        let fileName = this.AltMessage.Rc2Rpt;
        let header: string = '';
        this.translate
          .get('QuaCheck.TblHeader')
          .subscribe((translated: any) => {
            let columnNames = translated;
            header = columnNames.join(',');
          });
        let csv = '';
        csv =
          '\r\n ' +
          this.AltMessage.MarkProj +
          ', ' +
          this.projectName +
          ' \r\n ' +
          this.AltMessage.RptDate +
          ',' +
          latest_date +
          ' \r\n\r\n ';
        csv += header + ' \r\n ';
        this.getRC2ReportList.forEach((element) => {
          this.processRCDetailsData(element);

          this.datalist.forEach((c) => {
            csv += [
              c['SlNo'],
              c['QIGName'],
              c['TotalScript'],
              c['TotalInProgressScript'],
              c['CheckOutScripts'],
              c['TotalCompleted'],
              c['CompletionRateInPercentage'],
            ].join(',');
            csv += '\r\n';
          });
        });

        var blob = new Blob([csv], {
          type: 'text/csv;charset=utf-8;',
        });

        var link = document.createElement('a');
        this.getRCDataList(link, blob, fileName);
      }
    } else {
      this.Alert.warning(this.AltMessage.NoRecAlt);
    }
  }

  processRCDetailsData(element: any) {
    let slno: number = 1;
    this.datalist = [
      {
        SlNo: slno++,
        QIGName: element == null ? null : element.QIGName,
        TotalScript: element == null ? null : element.TotalScript,
        TotalInProgressScript:
          element == null ? null : element.TotalInProgressScript,
        CheckOutScripts: element == null ? null : element.CheckOutScripts,
        TotalCompleted: element == null ? null : element.TotalCompleted,
        CompletionRateInPercentage:
          element == null ? null : element.CompletionRateInPercentage,
      },
    ];
  }

  downloadAdHocDetails() {
    if (this.getAdHocList != null && this.getAdHocList.length > 0) {
      this.ZeroCountAdDoc = this.getAdHocList.filter(zc => zc.TotalScript == 0).length;
      
      if (this.getAdHocList.length == this.ZeroCountAdDoc) {
        this.Alert.warning(this.AltMessage.NoRecAlt);
      } else {
        const now = new Date().toLocaleString();
        let latest_date = this.datepipe
          .transform(now, 'd MMM y H:mm')
          ?.toString();
        let fileName = this.AltMessage.AdHocRpt;
        let header: string = '';
        this.translate
          .get('QuaCheck.TblAdHocHeader')
          .subscribe((translated: any) => {
            let columnNames = translated;
            header = columnNames.join(',');
          });
        let csv = '';
        csv =
          '\r\n ' +
          this.AltMessage.MarkProj +
          ', ' +
          this.projectName +
          ' \r\n ' +
          this.AltMessage.RptDate +
          ',' +
          latest_date +
          ' \r\n\r\n ';
        csv += header + ' \r\n ';
        this.getAdHocList.forEach((element) => {
          this.processAdHocDetailsData(element);

          this.datalist.forEach((c) => {
            csv += [
              c['SlNo'],
              c['QIGName'],
              c['TotalScript'],
              c['CheckOutScripts'],
              c['TotalCompleted'],
              c['CompletionRateInPercentage'],
            ].join(',');
            csv += '\r\n';
          });
        });

        var blob = new Blob([csv], {
          type: 'text/csv;charset=utf-8;',
        });

        var link = document.createElement('a');
        this.getRCDataList(link, blob, fileName);
      }
    } else {
      this.Alert.warning(this.AltMessage.NoRecAlt);
    }
  }

  processAdHocDetailsData(element: any) {
    let slno: number = 1;
    this.datalist = [
      {
        SlNo: slno++,
        QIGName: element == null ? null : element.QIGName,
        TotalScript: element == null ? null : element.TotalScript,
        CheckOutScripts: element == null ? null : element.CheckOutScripts,
        TotalCompleted: element == null ? null : element.TotalCompleted,
        CompletionRateInPercentage:
          element == null ? null : element.CompletionRateInPercentage,
      },
    ];
  }

  getRCDataList(link: any, blob: any, fileName: any) {
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
