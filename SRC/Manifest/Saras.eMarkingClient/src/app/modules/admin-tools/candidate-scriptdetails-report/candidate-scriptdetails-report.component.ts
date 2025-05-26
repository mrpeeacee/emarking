import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import { first } from 'rxjs';
import { SearchFilterModel } from 'src/app/model/Global/UserManagement/UserManagementModel';
import {
  CandidateScriptModel,
  GetAllProjectModel,
} from 'src/app/model/admin-tools/admin-tools-model';
import { AdminToolsService } from 'src/app/services/admin-tools/admin-tools.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';

@Component({
  templateUrl: './candidate-scriptdetails-report.component.html',
  styleUrls: ['./candidate-scriptdetails-report.component.css'],
})
export class CandidateScriptDetailsComponent implements OnInit {
  constructor(
    public Alert: AlertService,
    public translate: TranslateService,
    public commonService: CommonService,
    public adminService: AdminToolsService
  ) {}

  projectList!: GetAllProjectModel[];
  getCandidateScriptDetailsList!: CandidateScriptModel[];
  getCandidateScriptDetailsDwnList!: CandidateScriptModel[];
  selectedProject = '0';
  projectId: any = 0;
  datalist!: any[];
  IsProjectSame!: boolean;
  ScriptSearchValue: string = '';
  FilterQigScriptsData!: CandidateScriptModel[];
  selected: any[] = [];
  ProjectSearchValue: string = '';
  ProjectsSearchList!: GetAllProjectModel[];
  IsFilter: boolean = false;
  ScriptNameList: any;
  LoginNameList: any;
  pageindex?: number = 1;
  // Pagination
  pageEvent!: PageEvent;
  length = 0;
  pageSize = 10;
  pageIndex = 0;
  pageSizeOptions = [5, 10, 25, 100];
  showPageSizeOptions = true;
  objSearch: SearchFilterModel = new SearchFilterModel();

  @ViewChild('paginator') paginator: MatPaginator | undefined;

  ngOnInit(): void {
    this.setPagination(this.pageIndex, this.pageSize);
    this.setTitles();
    this.GetProjects();
  }

  AltMessage: any = {
    SelProjAlt: '',
    InProjAlt: '',
    RptName: '',
    NoRec: '',
  };

  setTitles() {
    this.translate.get('CandidateScp.Title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });

    this.translate
      .get('CandidateScp.Pagedesc')
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
          if (data != null && data.length > 0) {
            this.projectList = data;
            this.ProjectsSearchList = data;
            if (this.projectList.length == 1) {
              this.IsProjectSame = true;
              this.projectId = data[0].ProjectId;
              this.getCandidateScriptDetails();
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

  handlePageEvent(e: PageEvent) {
    this.pageEvent = e;
    this.length = e.length;
    this.setPagination(e.pageIndex, e.pageSize);
    this.pageindex = (e.pageIndex * e.pageSize) + 1;
    this.getCandidateScriptDetails();
  }

  Ischecked(event: any) {
    if (
      (event.source.value != null || event.source.value != undefined) &&
      event.source.value != 0
    ) {
      this.projectId = event.source.value;
      this.paginator?.firstPage();
      this.length = 0;
      this.getCandidateScriptDetailsList = [] as CandidateScriptModel[];
    }
    if (event.source.value == 0) {
      this.projectId = 0;
    }
  }

  getCandidateScriptDetails() {
    this.IsFilter = false;
    if (this.projectId != 0) {
      this.adminService
        .getCandidateScriptDetails(this.projectId, this.objSearch)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            if (data != null && data?.length > 0) {
              if (data[0].status != 'P001') {
                this.getCandidateScriptDetailsList = data;
                this.length = data.find((x: any) => x.LoginName).TotalRows;
                if (this.getCandidateScriptDetailsList.length == 0) {
                  this.Alert.warning(this.AltMessage.SelProjAlt); 
                }
              } else {
                this.Alert.warning(this.AltMessage.InProjAlt);
              }
            } else {
              this.getCandidateScriptDetailsList = [];
              this.Alert.warning(this.AltMessage.NoRecAlt); 
            }
          },
          error: (a: any) => {
            throw a;
          },        
        });
    } else {
      this.getCandidateScriptDetailsList = [];
      this.Alert.warning(this.AltMessage.SelProjAlt);
      this.projectId = 0;
    }
  }

  download() {
    if (
      this.getCandidateScriptDetailsList != null &&
      this.getCandidateScriptDetailsList.length > 0
    ) {
      let fileName = this.AltMessage.RptName;
      let slno: number = 1;
      let header: string = '';
      this.translate
        .get('CandidateScp.TblHeader')
        .subscribe((translated: any) => {
          let columnNames = translated;
          header = columnNames.join(',');
        });
      let csv = header;
      csv += '\r\n';

      this.adminService
        .getCandidateScriptDetails(this.projectId, this.objSearch, true)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            this.getCandidateScriptDetailsDwnList = data;
            this.getCandidateScriptDetailsDwnList.forEach((element) => {
              this.datalist = [
                {
                  SlNo: slno++,
                  LoginName: element == null ? null : element.LoginName,
                  QIGName: element == null ? null : element.QIGName,
                  ScriptName: element == null ? null : element.ScriptName,
                },
              ];

              this.datalist.forEach((c) => {
                csv += [
                  c['SlNo'],
                  c['LoginName'],
                  c['QIGName'],
                  c['ScriptName'],
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
          },
          error: (a: any) => {
            throw a;
          },         
        });
    } else {
      this.Alert.warning(this.AltMessage.NoRecAlt);
    }
  }

  SearchScript() {
    this.IsFilter = true;
    var ScriptSearchValue = this.ScriptSearchValue;
    if (ScriptSearchValue != '' && ScriptSearchValue != undefined) {
      this.adminService
        .getCandidateScriptDetails(this.projectId, this.objSearch, true)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            this.length = data.find((x: any) => x.LoginName).TotalRows;
            this.ScriptNameList = data.filter(function (el: {
              ScriptName: string;
            }) {
              return el.ScriptName.toLowerCase().includes(
                ScriptSearchValue.trim().toLowerCase()
              );
            });
            this.LoginNameList = data.filter(function (el: {
              LoginName: string;
            }) {
              return el.LoginName.toLowerCase().includes(
                ScriptSearchValue.trim().toLowerCase()
              );
            });

            this.FilterQigScriptsData = this.ScriptNameList.concat(
              this.LoginNameList
            );

            this.FilterQigScriptsData = this.FilterQigScriptsData.filter(
              (x) =>
                this.selected.includes(x.ScriptName) ||
                this.selected.length == 0
            );
            if (
              this.FilterQigScriptsData == null ||
              this.FilterQigScriptsData?.length == 0
            ) {
              this.getCandidateScriptDetailsList.length = 0;
            }
          },
          error: (a: any) => {
            throw a;
          },         
        });
    } else {
      this.FilterQigScriptsData.length = 0;
      this.getCandidateScriptDetails();
    }
  }

  setPagination(pageNum: number, pageSize: number) {
    this.objSearch.PageNo = pageNum + 1;
    this.objSearch.PageSize = pageSize;
  }

}
