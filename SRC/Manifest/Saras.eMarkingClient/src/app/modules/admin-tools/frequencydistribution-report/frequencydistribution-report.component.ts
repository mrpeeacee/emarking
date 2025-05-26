import { DatePipe } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import { first } from 'rxjs';
import { SearchFilterModel } from 'src/app/model/Global/UserManagement/UserManagementModel';
import {
  FrequencyDistributionModel,
  GetAllProjectModel,
} from 'src/app/model/admin-tools/admin-tools-model';
import { AdminToolsService } from 'src/app/services/admin-tools/admin-tools.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';

@Component({
  templateUrl: './frequencydistribution-report.component.html',
  styleUrls: ['./frequencydistribution-report.component.css'],
})
export class FrequencyDistributionReportComponent implements OnInit {
  constructor(
    public Alert: AlertService,
    public translate: TranslateService,
    public commonService: CommonService,
    public adminService: AdminToolsService,
    public datepipe: DatePipe
  ) {}

  projectList!: GetAllProjectModel[];
  getFrequencyDistributionList!: FrequencyDistributionModel[];
  getFrequencyDistributionDwnList!: FrequencyDistributionModel[];
  selectedProject = '0';
  projectId: any = 0;
  datalist!: any[];
  pageindex?: number = 1;
  projectName!: string;
  IsProjectSame!: boolean;
  questionType: number = 0;
  ProjectSearchValue: string = '';
  ProjectsSearchList!: GetAllProjectModel[];
  selected: any[] = [];
  serialnumber!: number;
  firstpage!: boolean;
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
    SelQueAlt: '',
    InProjAlt: '',
    RptName: '',
    MrkProj: '',
    RptDate: '',
    NoRec: '',
  };

  setTitles() {
    this.translate
      .get('FrequencyDist.Title')
      .subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
      });

    this.translate
      .get('FrequencyDist.Pagedesc')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });

    this.translate
      .get('FrequencyDist.SelProjAlt')
      .subscribe((translated: string) => {
        this.AltMessage.SelProjAlt = translated;
      });

    this.translate
      .get('FrequencyDist.SelQueAlt')
      .subscribe((translated: string) => {
        this.AltMessage.SelQueAlt = translated;
      });

    this.translate
      .get('FrequencyDist.InProjAlt')
      .subscribe((translated: string) => {
        this.AltMessage.InProjAlt = translated;
      });

    this.translate
      .get('FrequencyDist.RptName')
      .subscribe((translated: string) => {
        this.AltMessage.RptName = translated;
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

    this.translate
      .get('FrequencyDist.NoRecAlt')
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
      this.getFrequencyDistributionDwnList = [] as FrequencyDistributionModel[];
    }
    if (event.source.value == 0) {
      this.projectId = 0;
    }
  }

  IscheckedQue(event: any) {
    if (
      (event.source.value != null || event.source.value != undefined) &&
      event.source.value != 0
    ) {
      this.questionType = event.source.value;
      this.firstpage=true;   
      this.length = 0;
      this.getFrequencyDistributionDwnList = [] as FrequencyDistributionModel[];    
    }
    if (event.source.value == 0) {
      this.questionType = 0;
    }
  }

  handlePageEvent(e: PageEvent) {  
    this.pageEvent = e;
    this.length = e.length;
    this.setPagination(e.pageIndex, e.pageSize);
    this.pageindex = (e.pageIndex * e.pageSize) + 1;
    this.getFrequencyDistribution();
  }

  getFrequencyDistribution() {
    if(this.firstpage)
    {
        this.paginator?.firstPage(); 
        this.firstpage=false;
    } 
    if (this.projectId != 0 && this.questionType != 0) {
      this.adminService
        .getFrequencyDistributionReport(
          this.projectId,
          this.objSearch,
          this.questionType
        )
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            if (data != null && data.length > 0) {
              this.getAndBindFrequencyDistribution(data);
            } else {
              this.getFrequencyDistributionList = [];
              this.Alert.warning(this.AltMessage.NoRecAlt);
            }
          },
          error: (a: any) => {
            throw a;
          },
        });
    } else {
      this.getFrequencyDistributionList = [];
      this.projectValidation();
    }
  }

  getAndBindFrequencyDistribution(data: any){
    if (data[0].status != 'P001') {
      this.getFrequencyDistributionList = data;
      this.projectName = data.find(
        (x: any) => x.MarkingProject
      ).MarkingProject;

      this.length = data.find((x: any) => x.MarkingProject).TotalRows;
      if (this.getFrequencyDistributionList.length == 0) {
        this.Alert.warning(this.AltMessage.SelProjAlt);
      }
    } else {
      this.Alert.warning(this.AltMessage.InProjAlt);
    }
  }

  projectValidation(){
    if (this.projectId == 0) {
      this.Alert.warning(this.AltMessage.SelProjAlt);
    } else if (this.questionType == 0) {
      this.Alert.warning(this.AltMessage.SelQueAlt);
    } else {
      this.projectId = 0;
      this.questionType = 0;
    }
  }

  download() {
    this.serialnumber=1
    if (
      this.getFrequencyDistributionList != null &&
      this.getFrequencyDistributionList.length > 0
    ) {
      const now = new Date().toLocaleString();
      let latest_date = this.datepipe
        .transform(now, 'd MMM y H:mm')
        ?.toString();
      let fileName = this.AltMessage.RptName;
      
      let header: string = '';
      this.translate
        .get('FrequencyDist.TblHeader')
        .subscribe((translated: any) => {
          let columnNames = translated;
          header = columnNames.join(',');
        });
      let csv = '';
      csv = ''
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
        .getFrequencyDistributionReport(
          this.projectId,
          this.objSearch,
          this.questionType,
          true
        )
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            this.getFrequencyDistributionDwnList = data;
            this.getFrequencyDistributionDwnList.forEach((element) => {
              this.processFrequencyDistributionData(element);          
              this.datalist.forEach((c) => {
                csv += [
                  c['SlNo'],
                  c['QuestionCode'],
                  c['Blank'],
                  c['ResponseText'],
                  c['NoOfCandidatesAnswered'],
                  c['PercentageDistribution'],
                  c['MarksAwarded'],
                  c['MarkingType'],
                  c['Remarks'],
                ].join(',');
                csv += '\r\n';
              });
            });

            var blob = new Blob([new Uint8Array([0xEF, 0xBB, 0xBF]), csv], {
              type: 'text/csv;charset=utf-8;',
            });

            var link = document.createElement('a');
            this.getDataList(link, blob, fileName);
          },
          error: (a: any) => {
            throw a;
          }
        });
    } else {
      this.Alert.warning(this.AltMessage.NoRecAlt);
    }
  }

  processFrequencyDistributionData(element: any){
    
    let slno: number = 1;

    this.datalist = [
      {
        SlNo: this.serialnumber++,
        QuestionCode: element == null ? null : element.QuestionCode,
        Blank: element == null ? null : element.Blank,
        ResponseText:
          element == null
            ? null
            : this.removeHtmlTags(element.ResponseText),
        NoOfCandidatesAnswered:
          element == null ? null : element.NoOfCandidatesAnswered,
        PercentageDistribution:
          element == null ? null : element.PercentageDistribution,
        MarksAwarded: element == null ? null : element.MarksAwarded,
        MarkingType: element == null ? null : element.MarkingType,
        Remarks:
          element?.Remarks == ''
            ? 'NA'
            : this.removeHtmlTags(element?.Remarks),
      },
    ];
  }

  getDataList(link: any, blob: any, fileName: any){
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

  setPagination(pageNum: number, pageSize: number) {
    this.objSearch.PageNo = pageNum + 1;
    this.objSearch.PageSize = pageSize;
  }
}
