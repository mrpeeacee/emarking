import { Component, OnInit } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import { first } from 'rxjs';
import { SearchFilterModel } from 'src/app/model/Global/UserManagement/UserManagementModel';
import { GetAllProjectModel } from 'src/app/model/admin-tools/admin-tools-model';
import { AdminToolsService } from 'src/app/services/admin-tools/admin-tools.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { AllAnswerKeysModel } from '../../../model/admin-tools/admin-tools-model';

@Component({
  ////selector: 'emarking-answer-keys-report',
  templateUrl: './answer-keys-report.component.html',
  styleUrls: ['./answer-keys-report.component.css'],
})
export class AnswerKeysReportComponent implements OnInit {
  constructor(
    public Alert: AlertService,
    public translate: TranslateService,
    public commonService: CommonService,
    public adminService: AdminToolsService
  ) {}

  projectList!: GetAllProjectModel[];
  FilterQigScriptsData!: AllAnswerKeysModel[];
  selectedProject = '0';
  projectId!: any;
  datalist!: any[];
  IsProjectSame!: boolean;
  ScriptSearchValue: string = '';
  answerlst!: AllAnswerKeysModel[];
  selected: any[] = [];
  ProjectSearchValue: string = '';
  ProjectsSearchList!: GetAllProjectModel[];

  // Pagination
  pageEvent!: PageEvent;
  length = 0;
  pageSize = 10;
  pageIndex = 0;
  pageSizeOptions = [5, 10, 25, 100];
  showPageSizeOptions = true;
  objSearch: SearchFilterModel = new SearchFilterModel();

  ngOnInit(): void {
    this.objSearch.PageNo = 1;
    this.objSearch.PageSize = this.pageSize;
    this.setTitles();
    this.GetProjects();
    this.ScriptSearchValue = '';
  }

  AltMessage: any = {
    SelProjAlt: '',
    InProjAlt: '',
    RptName: '',
    NoRec: '',
  };

  setTitles() {
    this.translate
      .get('AllAnswerKeys.pagetitle')
      .subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
      });
    this.translate
      .get('AllAnswerKeys.pagedesc')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });

    this.translate
      .get('CandidateScp.SelProjAlt')
      .subscribe((translated: string) => {
        this.AltMessage.SelProjAlt = translated;
      });
    this.translate
      .get('CandidateScp.InProjAlt')
      .subscribe((translated: string) => {
        this.AltMessage.InProjAlt = translated;
      });
    this.translate
      .get('CandidateScp.RptName')
      .subscribe((translated: string) => {
        this.AltMessage.RptName = translated;
      });
    this.translate
      .get('CandidateScp.NoRecAlt')
      .subscribe((translated: string) => {
        this.AltMessage.NoRecAlt = translated;
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
              this.GetAllAnswerKeysReport();
            } else {
              this.IsProjectSame = false;
            }
          }
        },
        error: (a: any) => {
          throw a;
        },
        // complete: () => {},
      });
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
    this.ProjectSearchValue = '';
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
    this.projectId = this.projectList;
  }

  handlePageEvent(e: PageEvent) {
    this.pageEvent = e;
    this.length = e.length;
    this.objSearch.PageNo = e.pageIndex + 1;
    this.objSearch.PageSize = e.pageSize;
    this.GetAllAnswerKeysReport();
  }

  GetAllAnswerKeysReport() {
    if (this.projectId != undefined) {
      this.adminService
        .getAllAnswerKeysReport(this.projectId, this.objSearch)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            if (data != null || data != undefined || data.length > 0) {
              this.answerlst = data;
              this.length = data.find((x: any) => x.QuestionCode).TotalRows;
            }
          },
          error: (a: any) => {
            throw a;
          },
          // complete: () => {},
        });
    } else {
      this.Alert.warning(this.AltMessage.SelProjAlt);
    }
  }

  IsFilter: boolean = false;

  SearchScript() {
    this.IsFilter = true;
    var ScriptSearchValue = this.ScriptSearchValue;
    if (ScriptSearchValue != '' && ScriptSearchValue != undefined) {
      this.adminService
        .getAllAnswerKeysReport(this.projectId, this.objSearch)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            this.FilterQigScriptsData = data;

            this.answerlst = this.FilterQigScriptsData.filter(function (el: {
              ParentQuestionCode: string;
            }) {
              return el.ParentQuestionCode.toLowerCase().includes(
                ScriptSearchValue.trim().toLowerCase()
              );
            });

            this.answerlst = this.answerlst.filter(
              (x) =>
                this.selected.includes(x.ParentQuestionCode) ||
                this.selected.length == 0
            );
            if (this.answerlst == null || this.answerlst?.length == 0) {
              this.answerlst.length = 0;
            }
          },
          error: (a: any) => {
            throw a;
          },
          // complete: () => {},
        });
    } else {
      this.FilterQigScriptsData.length = 0;
      this.GetAllAnswerKeysReport();
    }
  }

  userslstloading: boolean = false;

  clearSearch() {
    this.ProjectSearchValue = '';
    this.GetProjects();
  }

  ExcelReport() {
    const date = new Date();
    const month = date.toLocaleString('default', { month: 'short' });
    var year = date.getFullYear();
    var day = date.getDay();
    if (this.projectId != null) {
      this.adminService
        .GetAnswerKeyCompelteReport(this.projectId)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            this.Alert.clear();
            var fileName =
              'AllAnswerKeyReport' + day + '-' + month + '-' + year;
            const blob = new Blob([data], {
              type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
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
          },
          error: (err: any) => {
            throw err;
          },
          complete: () => {
            this.userslstloading = false;
          },
        });
    } else {
      this.Alert.warning('Please Select a Project.');
    }
  }
}
