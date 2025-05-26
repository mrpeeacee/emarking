import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import { first } from 'rxjs';
import { SearchFilterModel } from 'src/app/model/Global/UserManagement/UserManagementModel';
import { FIDIIdDetails, GetAllProjectModel, QuestionValue, SyncMetaDataModel } from 'src/app/model/admin-tools/admin-tools-model';
import { AdminToolsService } from 'src/app/services/admin-tools/admin-tools.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { QigService } from 'src/app/services/project/qig.service';


@Component({
  templateUrl: './FIDI-report.component.html',
  styleUrls: ['./FIDI-report.component.css'],
})
export class FIDIReportComponent implements OnInit {
  fidimaxmarks!: number;
  SyncMetaDatadetails: any;
  IsSyncMetadta: boolean | undefined;
  constructor(
    public Alert: AlertService,
    public translate: TranslateService,
    public commonService: CommonService,
    public adminService: AdminToolsService, public qigservice: QigService
  ) { }

  projectList!: GetAllProjectModel[];
  fidiiddetails!: FIDIIdDetails[];
  selectedProject = '0';
  projectId!: any;
  projectName!: string;
  IsProjectSame!: boolean;
  MaxMarks!: any;
  MinMarks!: any;
  // Pagination
  pageEvent!: PageEvent;
  length = 0;
  pageSize = 10;
  pageIndex = 0;
  pageSizeOptions = [5, 10, 25, 100];
  showPageSizeOptions = true;
  objSearch: SearchFilterModel = new SearchFilterModel();
  @ViewChild('excelTable', { static: false }) excelTable!: ElementRef;
  Isprojectclosed: boolean = true;
  ProjectSearchValue: string = '';
  ProjectsSearchList!: GetAllProjectModel[];
  selected: any[] = [];
  Scorelist: any[] = [];
  dataload: boolean = false;

  ngOnInit(): void {
    this.objSearch.PageNo = 1;
    this.objSearch.PageSize = this.pageSize;
    this.setTitles();
    this.GetProjects();
  }

  AltMessage: any = {
    SelProjAlt: '',
    InProjAlt: '',
    RptName: '',
    MrkProj: '',
    NoRec: '',
  };

  setTitles() {
    this.translate
      .get('fidireport.Title')
      .subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
      });
    this.translate
      .get('fidireport.pagedesc')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription('This functionality provides the details of the Facility Index (FI) and Discrimination Index (DI) for a question paper.');
      });
    this.translate
      .get('fidireport.SelProjAlt')
      .subscribe((translated: string) => {
        this.AltMessage.SelProjAlt = translated;
      });
    this.translate
      .get('fidireport.InProjAlt')
      .subscribe((translated: string) => {
        this.AltMessage.InProjAlt = translated;
      });
    this.translate
      .get('fidireport.RptName')
      .subscribe((translated: string) => {
        this.AltMessage.RptName = translated;
      });
    this.translate
      .get('fidireport.MarkProj')
      .subscribe((translated: string) => {
        this.AltMessage.MarkProj = translated;
      });
    this.translate
      .get('fidireport.NoRecAlt')
      .subscribe((translated: string) => {
        this.AltMessage.NoRecAlt = translated;
      });
  }

  GetProjects() {
    this.dataload = true;
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
              this.getFIDIDetails();
            } else {
              this.IsProjectSame = false;
            }
          }
        },
        error: (a: any) => {
          this.dataload = false;
          throw a;
        },
        complete: () => {
          this.dataload = false;
        },
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
      (x) => this.selected.includes(x.ProjectId) || this.selected.length == 0);
    this.projectId = this.projectList;
  }

  handlePageEvent(e: PageEvent) {
    this.pageEvent = e;
    this.length = e.length;
    this.objSearch.PageNo = e.pageIndex + 1;
    this.objSearch.PageSize = e.pageSize;
    this.getFIDIDetails();
  }

  CheckProjecttatus() {
    this.Isprojectclosed = true;
    if (this.projectId == undefined) {
      this.projectId = 0;
    }
    this.Alert.clear();
    this.adminService
      .getProjectStatus(this.projectId).subscribe((data) => {
        if (data != 3) {
          this.Isprojectclosed = false;
          this.Alert.warning("This project is not “Closed”.");
        }
      });
  }

  getFIDIDetails() {
    this.dataload = true;
    this.fidiiddetails = [];
    this.Scorelist = [];  
    if (this.projectId != 0 && this.projectId != undefined) {
      this.CheckProjecttatus();
      if (this.Isprojectclosed) {
        this.IsSyncMetadta=false
        this.adminService
          .getFIDIReportDetails(
            this.projectId,
            this.objSearch, this.IsSyncMetadta)
          .pipe(first())
          .subscribe({
            next: (data: any) => {
              if (data.fIDIDetails != null && data.fIDIDetails.length > 0) {
                this.processGetLiveMarkingdata(data);
              } else {
                this.dataload = false;
                this.fidiiddetails = [];
              }
            },
            error: (a: any) => {
              this.dataload = false;
              throw a;
            },
            complete: () => {
              this.dataload = false;
            },
          });
      }
      else {
        this.fidiiddetails = [];
      }
    }
    else {
      this.fidiiddetails = [];
      this.Alert.warning(this.AltMessage.SelProjAlt);
    }
  }

  processGetLiveMarkingdata(data: any){
    this.SyncMetaDatadetails=data.SyncMetaData
    this.fidiiddetails = data.fIDIIdDetails;
    this.MaxMarks = data.MaxMarks;
    this.MinMarks = data.MinMarks;

    for (var i = this.MinMarks + 1; i <= this.MaxMarks; i++) {
      this.Scorelist.push(i);
    }
    
    this.fidimaxmarks=this.Scorelist.length+2
    this.fidiiddetails.forEach(element => {
      if (element.subjectMarksItemScoresModels.length < this.MaxMarks) {
        element.ExtraScorelist = [];
        for (var j = element.subjectMarksItemScoresModels.length + 1; j <= this.MaxMarks; j++) {
          element.ExtraScorelist.push("-");
        }
      }
    });

    this.projectName = data.find(
      (x: any) => x.MarkingProject
    ).MarkingProject;
    this.length = data.find((x: any) => x.MarkingProject).TotalRows;
    if (this.fidiiddetails.length == 0) {
      this.dataload = false;
      this.Alert.warning(this.AltMessage.SelProjAlt);
    }
    else {
      this.dataload = false;
      this.Alert.warning(this.AltMessage.InProjAlt);
    }
  }

  clearSearch() {
    this.ProjectSearchValue = '';
    this.GetProjects();
  }

  exportAsExcel() {
    debugger
    this.Alert.clear();
    if (this.fidiiddetails?.length > 0 && this.fidiiddetails != undefined && this.excelTable != undefined) {
      const uri = 'data:application/vnd.ms-excel;base64,';
      const template = `<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>`;
      const base64 = function (s: string | number | boolean) { return window.btoa(unescape(encodeURIComponent(s))) };
      const format = function (s: string, c: { [x: string]: any; worksheet?: string; table?: any; }) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) };
      if (this.projectId != 0 || this.excelTable != undefined) {
        const table = this.excelTable.nativeElement;
        const ctx = { worksheet: 'Worksheet', table: table.innerHTML };
        const link = document.createElement('a');
        const date = new Date();
        const month = date.toLocaleString('default', { month: 'short' });
        var year = date.getFullYear();
        var day = date.getDate();
        var fileName = "FIDIReport " + day + "-" + month + "-" + year;

        this.Alert.clear();
        if (link.download !== undefined) {
          link.setAttribute('download', fileName);
          link.href = uri + base64(format(template, ctx));
          link.click();
          this.Alert.success("Report Downloaded Successfully.");
          return;
        }
      }
      else {
        this.Alert.warning(this.AltMessage.SelProjAlt);
      }
    }
    else {
      this.Alert.warning("No Records found.");
    }

  }
  exportFiDi() {
    // Define the header for the CSV file
    const header = 'ProductCode,QuestionCode,FI,DI,\n';
  
    // Iterate through fidiiddetails array and construct CSV rows ProductCode
    let csvData = '';
    for (const item of this.fidiiddetails) {
      csvData += `${item.ProductCode},${item.TNAQuestionCode},${item.FI},${item.DI}\n`;
    }
  
    // Combine header and CSV data
    const csv = header + csvData;
  
    // Create a Blob object containing the CSV data
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
  
    // Create a temporary anchor element
    const link = document.createElement('a');
    link.setAttribute('href', URL.createObjectURL(blob));
    link.setAttribute('download', 'exported_data.csv');
    link.style.visibility = 'hidden';
  
    // Append the anchor element to the document body and trigger the click event
    document.body.appendChild(link);
    link.click();
  
    // Clean up by removing the anchor element
    document.body.removeChild(link);
  }
 
  SyncMetaData()
  {
    this.IsSyncMetadta=true
    this.adminService
    .getFIDIReportDetails(
      this.projectId,
      this.objSearch, this.IsSyncMetadta)
    .pipe(first())
    .subscribe({
      next: (data: any) => {
        if (data.fIDIDetails != null && data.fIDIDetails.length > 0) {
          this.SyncMetaDatadetails=data.SyncMetaData
          this.fidiiddetails = data.fIDIIdDetails;
          if(this.SyncMetaDatadetails!=null)
            {
                const mappedData: SyncMetaDataModel[] = this.mapmetadata(this.SyncMetaDatadetails);
            
                this.adminService.SyncMetaData( mappedData).pipe(first()) .subscribe({
                        next: (data: any) => {
                          debugger;
                          if(data.StatusCode=="S001")
                          {
                            this.Alert.success(data.StatusMessage)
                          }
                          else{ 
                            this.Alert.warning(data.StatusMessage)
                          }
                        },error: (a: any) => {
                          this.dataload = false;
                          throw a;
                        },
                        complete: () => {
                          this.dataload = false;
                        },
                      })
              }
              else{
                this.Alert.warning("MetaData cannot be empty")
              }
              this.getFIDIDetails()
        }
      }});

}
  mapmetadata(data: any): SyncMetaDataModel[] {
    // Ensure data is an array
    if (!Array.isArray(data)) {
        console.error('Invalid data format: Expected an array');
        return [];
    }

    return data.map((item: any) => {
        const syncMetaData = new SyncMetaDataModel();
        syncMetaData.examLevel = item.ExamLevel || null;
        syncMetaData.examSeries = item.ExamSeries || null;
        syncMetaData.examYear = item.ExamYear || null;
        syncMetaData.subject = item.Subject || null;
        syncMetaData.paperNumber = item.PaperNumber || null;

        // Process QuestionValues
        if (this.fidiiddetails!=null) {
            syncMetaData.questionValues = this.fidiiddetails.map((code, index) => {
                const questionValue = new QuestionValue();
                questionValue.questionCode =this.fidiiddetails[index].TNAQuestionCode; // Assign from Tnaquestioncode array
                questionValue.fi = this.fidiiddetails[index]?.FI // Use the corresponding FI
                questionValue.di = this.fidiiddetails[index]?.DI // Use the corresponding DI
                return questionValue;
            });
        } else {
            syncMetaData.questionValues = [];
        }

        return syncMetaData;
    });
}
}
