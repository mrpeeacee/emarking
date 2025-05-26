import { Component, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';
import { first } from 'rxjs';
import {
  GetAllProjectModel,
  LiveMarkingProgressModel,
} from 'src/app/model/admin-tools/admin-tools-model';
import { AdminToolsService } from 'src/app/services/admin-tools/admin-tools.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';

@Component({
  templateUrl: './livemarking-progress-report.component.html',
  styleUrls: ['./livemarking-progress-report.component.css'],
})
export class LiveMarkingProgressReportComponent implements OnInit {
  constructor(
    public Alert: AlertService,
    public translate: TranslateService,
    public commonService: CommonService,
    public adminService: AdminToolsService,
    public datepipe: DatePipe
  ) {}

  getLiveMarkingProgressList!: LiveMarkingProgressModel[];
  selectedProject = '0';
  projectList!: GetAllProjectModel[];
  projectId: any = 0;
  datalist!: any[];
  projectName!: string;
  IsProjectSame!: boolean;
  ProjectSearchValue: string = '';
  ProjectsSearchList!: GetAllProjectModel[];
  selected: any[] = [];
  ProjectActiveCount: any;
  IsProjectActive: boolean = false;

  ngOnInit(): void {
    this.setTitles();
    this.GetProjects();
  }

  AltMessage: any = {
    SelProjAlt: '',
    InProjAlt: '',
    RptName: '',
    MrkProj: '',
    RptDate: '',
    NoRec: '',
  };

  setTitles() {
    this.translate.get('LiveMarking.Title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });

    this.translate
      .get('LiveMarking.Pagedesc')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });

    this.translate
      .get('LiveMarking.SelProjAlt')
      .subscribe((translated: string) => {
        this.AltMessage.SelProjAlt = translated;
      });

    this.translate
      .get('LiveMarking.InProjAlt')
      .subscribe((translated: string) => {
        this.AltMessage.InProjAlt = translated;
      });

    this.translate
      .get('LiveMarking.RptName')
      .subscribe((translated: string) => {
        this.AltMessage.RptName = translated;
      });

    this.translate
      .get('LiveMarking.MarkProj')
      .subscribe((translated: string) => {
        this.AltMessage.MarkProj = translated;
      });

    this.translate
      .get('LiveMarking.RptDate')
      .subscribe((translated: string) => {
        this.AltMessage.RptDate = translated;
      });

    this.translate
      .get('LiveMarking.NoRecAlt')
      .subscribe((translated: string) => {
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
              this.getLiveMarkingProgress();
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

  Ischecked(event: any) {
    if (
      (event.source.value != null || event.source.value != undefined) &&
      event.source.value != 0
    ) {
      this.projectId = event.source.value;
    }
    if (event.source.value == 0) {
      this.projectId = 0;
    }
  }

  getLiveMarkingProgress() {
    if (this.projectId != 0) {
      this.adminService
        .getLiveMarkingProgress(this.projectId)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            if (data != null && data?.length > 0) {
              this.processGetLiveMarkingdata(data);
            } else {
              this.getLiveMarkingProgressList = [];
            }
          },
          error: (a: any) => {
            throw a;
          },
        });
    } else {
      this.getLiveMarkingProgressList = [];
      this.Alert.warning(this.AltMessage.SelProjAlt);
      this.projectId = 0;
    }
  }

  processGetLiveMarkingdata(data: any) {
    if (data[0].status != 'P001') {
      this.getLiveMarkingProgressList = data;
      this.projectName = data.find((x: any) => x.MarkingProject).MarkingProject;
      if (this.getLiveMarkingProgressList.length == 0) {
        this.getLiveMarkingProgressList = [];
        this.Alert.warning(this.AltMessage.SelProjAlt);
      }
    } else {
      this.Alert.warning(this.AltMessage.InProjAlt);
    }
  }

  download() {
    if (
      this.getLiveMarkingProgressList != null &&
      this.getLiveMarkingProgressList.length > 0
    ) {
      this.ProjectActiveCount = this.getLiveMarkingProgressList.filter(zc => zc.TotalManualMarkingScript == 0).length;
      
      if (this.getLiveMarkingProgressList.length == this.ProjectActiveCount) {
        this.Alert.warning(this.AltMessage.NoRecAlt);
      } else {
        const now = new Date();
        let latest_date = this.datepipe
          .transform(now, 'd MMM y H:mm')
          ?.toString();
        let fileName = this.AltMessage.RptName;
        let header: string = '';
        this.translate
          .get('LiveMarking.TblHeader')
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
          ' ,' +
          latest_date +
          ' \r\n\r\n ';
        csv += header + ' \r\n ';
        this.getLiveMarkingProgressList.forEach((element) => {
          this.processGetLiveMarkingDownload(element);
          this.datalist.forEach((c) => {
            csv += [
              c['SlNo'],
              c['QIGName'],
              c['TotalManualMarkingScript'],
              c['DownloadedScripts'],
              c['ActionNeeded'],
              c['TotalPending'],
              c['TotalMarked'],
              c['CompletionRate'],
            ].join(',');
            csv += '\r\n';
          });
        });

        var blob = new Blob([csv], {
          type: 'text/csv;charset=utf-8;',
        });

        var link = document.createElement('a');
        this.getDataList(link, blob, fileName);
      }
    } else {
      this.Alert.warning(this.AltMessage.NoRecAlt);
    }
  }

  processGetLiveMarkingDownload(element: any) {
    let slno: number = 1;
    this.datalist = [
      {
        SlNo: slno++,
        QIGName: element == null ? null : element.QIGName,
        TotalManualMarkingScript:
          element == null ? null : element.TotalManualMarkingScript,
        DownloadedScripts: element == null ? null : element.DownloadedScripts,
        ActionNeeded: element == null ? null : element.ActionNeeded,
        TotalPending: element == null ? null : element.TotalPending,
        TotalMarked: element == null ? null : element.TotalMarked,
        CompletionRate: element == null ? null : element.CompletionRate,
      },
    ];
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
