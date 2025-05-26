import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonService } from 'src/app/services/common/common.service';
import { TranslateService } from '@ngx-translate/core';
import { AdminToolsService } from 'src/app/services/admin-tools/admin-tools.service';
import { AlertService } from 'src/app/services/common/alert.service';
import {
  GetAllMarkerPerformanceModel,
  GetAllProjectModel,
} from 'src/app/model/admin-tools/admin-tools-model';
import { first } from 'rxjs';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { SearchFilterModel } from 'src/app/model/Global/UserManagement/UserManagementModel';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'emarking-marker-performance-report',
  templateUrl: './marker-performance-report.component.html',
  styleUrls: ['./marker-performance-report.component.css'],
})
export class MarkerPerformanceReportComponent implements OnInit {
  constructor(
    public commonService: CommonService,
    public translate: TranslateService,
    private adminService: AdminToolsService,
    public Alert: AlertService,
    public datepipe: DatePipe
  ) {}

  title!: string;
  markerperformanceDetails!: GetAllMarkerPerformanceModel[];
  markerperformanceDetailsDwnList!: GetAllMarkerPerformanceModel[];
  IsProjectSame!: boolean;
  ProjectSearchValue: string = '';
  projectList!: GetAllProjectModel[];
  ProjectsSearchList!: GetAllProjectModel[];
  selected: any[] = [];
  projectId!: number;
  selectedOption: string = 'Grid1'; // Set the default selected option
  projectName!: string;
  datalist!: any[];

  // Pagination
  pageEvent!: PageEvent;
  length = 0;
  pageSize = 10;
  pageIndex = 0;
  pageSizeOptions = [5, 10, 25, 100];
  showPageSizeOptions = true;
  objSearch: SearchFilterModel = new SearchFilterModel();
  pageindex?: number = 1;
  @ViewChild('paginator') paginator: MatPaginator | undefined;

  ngOnInit(): void {
    this.setPagination(this.pageIndex, this.pageSize);
    this.PageDescTitle();
    this.GetProjects();
    ////this.GetMarkerPerformanceReport();
  }

  AltMessage: any = {
    SelProjAlt: '',
    InProjAlt: '',
    MarkerRptName: '',
    MrkProj: '',
    RptDate: '',
    NoRec: '',
  };

  PageDescTitle() {
    this.translate
      .get('MarkerPerformanceReport.pagedesc')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });

    this.translate
      .get('MarkerPerformanceReport.pagetitle')
      .subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
        this.title = translated;
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
      .get('FrequencyDist.NoRecAlt')
      .subscribe((translated: string) => {
        this.AltMessage.NoRecAlt = translated;
      });

    this.translate
      .get('MarkerPerformanceReport.MarkerRptName')
      .subscribe((translated: string) => {
        this.AltMessage.MarkerRptName = translated;
      });

    this.translate
      .get('FrequencyDist.MarkProj')
      .subscribe((translated: string) => {
        this.AltMessage.MarkProj = translated;
      });

    this.translate
      .get('FrequencyDist.RptDate')
      .subscribe((translated: string) => {
        this.AltMessage.RptDate = translated;
      });
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
              this.getMarkerPerformanceReport();
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
      this.paginator?.firstPage();
      this.length = 0;
      this.markerperformanceDetailsDwnList =
        [] as GetAllMarkerPerformanceModel[];
    }
    if (event.source.value == 0) {
      this.projectId = 0;
    }
  }

  handlePageEvent(e: PageEvent) {
    this.pageEvent = e;
    this.length = e.length;
    this.setPagination(e.pageIndex, e.pageSize);
    this.pageindex = e.pageIndex * e.pageSize + 1;
    this.getMarkerPerformanceReport();
  }

  getMarkerPerformanceReport() {
    //// if (this.IsProjectSame) {
    ////   this.GetProjects();
    //// }
    if (this.projectId != 0 && this.projectId != undefined) {
      this.adminService
        .GetMarkerPerformanceReport(this.projectId, this.objSearch)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            if (data != null && data?.length > 0) {
              this.processPerfomanceData(data);
            } else {
              this.markerperformanceDetails = [];
              this.Alert.warning(this.AltMessage.NoRecAlt);
            }
          },
          error: (a: any) => {
            throw a;
          },
        });
    } else {
      this.markerperformanceDetails = [];
      this.Alert.warning(this.AltMessage.SelProjAlt);
      this.projectId = 0;
    }
  }

  processPerfomanceData(data: any) {
    if (data[0].status != 'P001') {
      this.markerperformanceDetails = data;
      this.projectName = data.find((x: any) => x.ProjectCode).ProjectCode;
      this.length = data.find((x: any) => x.ProjectCode).RowCounts;
      if (this.markerperformanceDetails.length == 0) {
        this.markerperformanceDetails = [];
        this.Alert.warning(this.AltMessage.SelProjAlt);
      }
    } else {
      this.Alert.warning(this.AltMessage.InProjAlt);
    }
  }

  setPagination(pageNum: number, pageSize: number) {
    this.objSearch.PageNo = pageNum + 1;
    this.objSearch.PageSize = pageSize;
  }

  clearSearch() {
    this.ProjectSearchValue = '';
    this.GetProjects();
  }

  download() {
    if (
      this.markerperformanceDetails != null &&
      this.markerperformanceDetails.length > 0
    ) {
      const now = new Date().toLocaleString();
      let latest_date = this.datepipe
        .transform(now, 'd MMM y H:mm')
        ?.toString();

      let fileName = this.AltMessage.MarkerRptName;
      let header: string = '';
      let slno: number = 1;
      this.translate
        .get('MarkerPerformanceReport.MarkerTblHeader')
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
        ', ' +
        latest_date +
        ' \r\n\r\n ';
      csv += header + ' \r\n ';

      this.adminService
        .GetMarkerPerformanceReport(this.projectId, this.objSearch, true)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            this.markerperformanceDetailsDwnList = data;
            this.markerperformanceDetailsDwnList.forEach((element) => {
              this.datalist = [
                {
                  SlNo: slno++,
                  MarkingPersonnelName: element == null ? null : element.MarkerName,
                  Role: element == null ? null : element.RoleCode,
                  MarkingProject: element == null ? null : element.ProjectCode,
                  StartDate:
                    element == null
                      ? null
                      : element.StartDate
                      ? this.datepipe.transform(element.StartDate, 'd MMM y')
                      : 'NA',
                  EndDate:
                    element == null
                      ? null
                      : element.EndDate
                      ? this.datepipe.transform(element.EndDate, 'd MMM y')
                      : 'NA',
                  ScriptEvaluted: element == null ? null : element.NoOfScripts,
                  TimeSpent: element == null ? null : element.TotalTimeTaken,
                  AverageTime: element == null ? null : element.AverageTime,
                  RemarkedScripts: element == null ? null : element.ReMarkedScripts,
                },
              ];
          
              this.datalist.forEach((c) => {
                csv += [
                  c['SlNo'],
                  c['MarkingPersonnelName'],
                  c['Role'],
                  c['MarkingProject'],
                  c['StartDate'],
                  c['EndDate'],
                  c['ScriptEvaluted'],
                  c['TimeSpent'],
                  c['AverageTime'],
                  c['RemarkedScripts'],
                ].join(',');
                csv += '\r\n';
              });
            });

            var blob = new Blob([csv], {
              type: 'text/csv;charset=utf-8;',
            });

            var link = document.createElement('a');
            this.getDataList(link, blob, fileName);
          },
          error: (a: any) => {
            throw a;
          },
        });
    } else {
      this.Alert.warning(this.AltMessage.NoRecAlt);
    }
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

  removeHtmlTags(input: string): string {
    const withoutHtml = input.replace(/<[^>]*>/g, '');

    return withoutHtml.replace(/,/g, ' ');

  }

  IscheckedQue(event: any) {
    if (
      (event.source.value != null || event.source.value != undefined) &&
      event.source.value != 0
    ) {
      this.paginator?.firstPage();
      this.length = 0;
      this.markerperformanceDetailsDwnList =
        [] as GetAllMarkerPerformanceModel[];
    }
    if (event.source.value == 0) {
      ////this.questionType = 0;
    }
  }
}
