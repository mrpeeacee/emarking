import { Component, OnInit, ViewChild } from '@angular/core';
import { AuditReportService } from '../../../services/reports/audit-report.service';
import { AuditReportRequestModel } from '../../../model/reports/audit-report';
import {
  DateAdapter,
  MAT_DATE_LOCALE,
  MAT_DATE_FORMATS
} from '@angular/material/core';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { DatePipe } from '@angular/common';
import { first, timeout } from 'rxjs/operators';
import * as moment from 'moment';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { AlertService } from '../../../services/common/alert.service';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { PageEvent } from '@angular/material/paginator';
import { MatAccordion } from '@angular/material/expansion';


// export const MY_FORMATS = {

//   parse: {
//     dateInput: 'DD-MM-YYYY', //HH:mm:ss
//   },
//   display: {
//     dateInput: 'DD-MM-YYYY', //HH:mm:ss
//     monthYearLabel: 'MMM YYYY',
//     dateA11yLabel: 'DD-MM-YYYY HH:mm:ss',
//     monthYearA11yLabel: 'MMMM YYYY',
//   },
// };



// @Component({
//   selector: 'emarking-audit-report',
//   templateUrl: './audit-report.component.html',
//   styleUrls: ['./audit-report.component.css'],

//   providers: [
//     {
//       provide: DateAdapter,
//       useClass: MomentDateAdapter,
//       deps: [MAT_DATE_LOCALE],
//     },
//     { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS },
//     DatePipe,
//   ],
// })

// export class AuditReportComponent implements OnInit {

//   @ViewChild('matSelect') matSelect: any;
//   panelOpenState = false;
//   auditreportList: any[] = [];
//   getevtaudit = new AuditReportModel();
//   pickfromdate!: any;
//   picktodate!: any;
//   logouttemp!: any;
//   temppickfromdate !: any;
//   temppicktodate !: any;
//   loginname: string = '';
//   eventcode: string = '';
//   modulecode: string = '';
//   entitycode: string = '';
//   minimumdate = new Date();
//   tominimumdate = new Date();
//   public maxDate = new Date();
//   serverdatetime: Date = new Date();
//   Isloading: boolean = true;
//   Ispageactive : boolean = false;

//   constructor(
//     private auditreportservice: AuditReportService,
//     public Alert: AlertService,
//     public translate: TranslateService,
//     public commonService: CommonService
//   ) { }
//   selected = 'USERLOGIN';
//   Modulesentries = [
//     { Id: 1, value: 'PROJECTSETUP', title: 'Project Setup' },
//     { Id: 2, value: 'BASICDETAILS', title: 'Basic Details' },
//     { Id: 18, value: 'USERLOGIN', title: 'UserLogin' },
//     { Id: 19, value: 'QIGMANAGEMENT', title: 'QIG Management' },
//     { Id: 20, value: 'LIVEMARKING', title: 'Live Marking' },
//     { Id: 21, value: 'PROJECTUSERMANAGEMENT', title: 'Project User Management' },
//     { Id: 21, value: 'PROJECTUSERMANAGEMENT', title: 'Project User Management' },
//   ]
//   ngOnInit(): void {
//     this.setTitles();
//     this.Ispageactive = true;
//   }
//   setTitles() {
//     this.translate.get('AuditReport.title').subscribe((translated: string) => {
//       this.commonService.setHeaderName(translated);
//     });
//     this.translate.get('AuditReport.pgedesc').subscribe((translated: string) => {
//       this.commonService.setPageDescription(translated);
//     });

//   }
//   Reset() {
//     this.pickfromdate = '';
//     this.picktodate = '';
//     this.loginname = '';
//     this.auditreportList = [];
//     this.selected = 'USERLOGIN'
//     this.Ispageactive = true;
//   }
//   example: string = "";
//   GetAuditReport(fDate: any, tDate: any = null) {
//     debugger
//     this.Isloading = true;
//     this.Ispageactive = false;
//     let objaudit = new AuditReportModel();
//     objaudit.LoginId = this.loginname;
//     objaudit.EventCode = this.eventcode;
//     objaudit.EntityCode = this.entitycode;
//     if (this.matSelect.value == undefined) {
//       objaudit.ModuleCode = "";
//     }
//     objaudit.ModuleCode = this.matSelect.value;
//     // else if (this.matSelect.value == "USERLOGIN") {
//     //   objaudit.ModuleCode = "USERLOGIN";
//     // }
//     // else if (this.matSelect.value == "QIGMANAGEMENT") {
//     //   objaudit.ModuleCode = "USERLOGIN,QIGMANAGEMENT";
//     // }
//     // else if {
//     //   objaudit.ModuleCode = this.matSelect.value
//     // }

//     if (tDate != null) {
//       objaudit.StartDate = fDate;
//       objaudit.EndDate = tDate;
//     }
//     else {
//       objaudit.FromDate = fDate;
//     }
//     this.auditreportservice.GetAuditReport(objaudit).pipe(first()).subscribe({
//       next: (data: any[]) => {
//           this.auditreportList = data;
//           this.Isloading = false;
//       },
//       error: (a: any) => {
//         throw (a);
//       },
//       complete: () => {
//         this.Isloading = false;
//       }
//     });

//   }
//   changeValue(value: any) {
//     let selectedItem: any;
//     selectedItem = this.Modulesentries.filter(item => item.title == value)[0];
//     console.log(selectedItem.id)
//   }
//   fromDatechangeEvent(event: MatDatepickerInputEvent<Date>) {
//     if (event != null && event.value != null)
//       this.tominimumdate = event.value;
//   }


//   onDataChange() {
//     this.Ispageactive = false;
//     const _ = moment();
//     if (this.pickfromdate == '' || this.pickfromdate == undefined) {
//       this.Alert.warning("Select From Date.");
//       this.auditreportList = [];
//       return;
//     }
//     else if (this.picktodate == '' || this.picktodate == undefined) {
//       this.Alert.warning("Select To Date.");
//       this.auditreportList = [];
//       return;
//     }
//     else if (this.pickfromdate > this.picktodate) {
//       this.Alert.warning("To Date cannot be less than From Date.");
//       return;
//     }
//     const StartDate = moment(this.pickfromdate).add({
//       hours: _.hour(),
//       minutes: _.minute(),
//       seconds: _.second(),
//     }).format("YYYY-MM-DD HH:mm:ss");

//     const EndDate = moment(this.picktodate).add({
//       hours: _.hour(),
//       minutes: _.minute(),
//       seconds: _.second(),
//     }).format("YYYY-MM-DD HH:mm:ss");
//     this.temppickfromdate = StartDate;
//     this.temppicktodate = EndDate;
//     this.orgValueChange(StartDate, EndDate);
//   }

//   orgValueChange(fDate: any, tDate: any) {
//     this.GetAuditReport(fDate, tDate);
//   }



// }



export const MY_FORMATS = {
  parse: {
    dateInput: 'DD-MM-YYYY' //HH:mm:ss
  },
  display: {
    dateInput: 'DD-MM-YYYY', //HH:mm:ss
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'DD-MM-YYYY HH:mm:ss',
    monthYearA11yLabel: 'MMMM YYYY'
  }
};

@Component({
  selector: 'emarking-audit-report',
  templateUrl: './audit-report.component.html',
  styleUrls: ['./audit-report.component.css'],

  providers: [
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE]
    },
    { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS },
    DatePipe
  ]
})
export class AuditReportComponent implements OnInit {
  @ViewChild('matSelect') matSelect: any;
  panelOpenState = false;
  auditreportList: any[] = [];
  modulereportlist: any[] = [];
  pickfromdate!: any;
  picktodate!: any;
  temppickfromdate!: any;
  temppicktodate!: any;
  loginname: string = '';
  eventcode: string = '';
  modulecode: string = '';
  entitycode: string = '';
  minimumdate = new Date();
  tominimumdate = new Date();
  public maxDate = new Date();
  Isloading: boolean = true;
  Ispageactive: boolean = false;
  IsUserLevel: boolean = false;
  IsExpanded: boolean = false;

  auditReportRequest: AuditReportRequestModel = new AuditReportRequestModel();
  datalist: { SlNo: number; UserName: any; FunctionPerformed: any }[] = [];
  auditreportListExport!: any[];
  exportClicked: boolean=false;
  ViewReportResult!: boolean;
 

  constructor(
    private auditreportservice: AuditReportService,
    public Alert: AlertService,
    public translate: TranslateService,
    public commonService: CommonService
  ) {}
  Modulesentries = [
       { Id: 2, value:'BASICDETAILS', title: 'Basic Details' },
       { Id: 18, value:'USERLOGIN', title: 'UserLogin' },
       { Id: 19, value:'QIGMANAGEMENT', title: 'QIG Management' },
       { Id: 20, value:'LIVEMARKING', title: 'Live Marking' },
       { Id: 21, value:'PROJECTUSERMANAGEMENT', title: 'Project User Management' },
       { Id: 21, value:'CATEGORISATION', title: 'Categorisation' },
       { Id: 5, value:'PROJECTSCHEDULE', title: 'Project Schedule'},
       { Id: 23, value:'APPLICATIONUSERMANAGEMENT', title:'Application User Management'}, 
       { Id: 13, value:'MARKSCHEMELIB', title: 'Mark Scheme Library'},
       { Id: 36, value:'QIGTEAMMANAGEMENT', title: 'QIG Team Management'},
       { Id: 4, value:'STANDARDISATION', title:'Standardisation'},
       { Id: 8, value:'SETUP', title:'Setup'},
       { Id: 6, value:'PROLVLCONFIG', title:'Project Level Configuration'},
       { Id: 9, value:'RECMND', title:'Sampling'},
       { Id: 10, value:'TRIALMRKNG', title:'Trial Marking'},
       { Id:28, value:'AUTOMATIC', title:'Automatic'},
       { Id:3, value:'QIGSETUP', title:'QIG Setup'}
       ]
  // Pagination
  length = 0;
  pageSize = 10;
  pageIndex = 1;
  pageSizeOptions = [5, 10, 25, 100];
  pageEvent!: PageEvent;

  showPageSizeOptions = true;
  selected = '';
  @ViewChild(MatAccordion) accordion!: MatAccordion;

  ngOnInit(): void {
    
    this.setTitles();
   // this.GetAppModules();
    this.Ispageactive = true;
    this.matSelect.value = '';
  }

  setTitles() {
    this.translate.get('AuditReport.title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate
      .get('AuditReport.pgedesc')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });
  }

  Reset() {
    this.pickfromdate = '';
    this.picktodate = '';
    this.auditreportList = [];
    this.Ispageactive = true;
    this.auditReportRequest.LoginId = '';
    this.selected = '';
    this.IsExpanded = false;
  }

  GetAuditReport() {
    
    this.ViewReportResult=true;
    this.Isloading = true;
    this.Ispageactive = false;
    this.auditReportRequest.ModuleCodes = this.matSelect.value;
    this.auditreportservice
      .GetAuditReport(this.auditReportRequest)
      .pipe(first())
      .subscribe({
        next: (data: any[]) => {
          this.auditreportList = data;
          this.IsExpanded = false;
          this.Isloading = false;
          if (
            this.auditreportList != null &&
            this.auditreportList != undefined
          ) {
            this.length = this.auditreportList[0].TotalRows;
          }
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.Isloading = false;
        }
      });
  }
  fromDatechangeEvent(event: MatDatepickerInputEvent<Date>) {
    if (event != null && event.value != null) this.tominimumdate = event.value;
  }

  ViewReport() {
  
    this.Ispageactive = false;

    if (this.matSelect.value == undefined||this.matSelect.value=="") {
      this.Alert.warning('Select ModuleCode.');
      this.auditReportRequest.ModuleCodes = '';
      return;
    } else {
      this.auditReportRequest.ModuleCodes = 'USERLOGIN,' + this.matSelect.value;
    }

    this.auditReportRequest.PageSize = this.pageSize;
    this.auditReportRequest.PageNo = this.pageIndex == 0 ? this.pageSize : this.pageIndex;

    const _ = moment();
    if (this.pickfromdate == '' || this.pickfromdate == undefined) {
      this.Alert.warning('Select From Date.');
      this.auditreportList = [];
      return;
    } else if (this.picktodate == '' || this.picktodate == undefined) {
      this.Alert.warning('Select To Date.');
      this.auditreportList = [];
      return;
    } else if (this.pickfromdate > this.picktodate) {
      this.Alert.warning('To Date cannot be less than From Date.');
      return;
    }
    const StartDate = moment(this.pickfromdate)
      .add({
        hours: _.hour(),
        minutes: _.minute(),
        seconds: _.second()
      })
      .format('YYYY-MM-DD HH:mm:ss');

    const EndDate = moment(this.picktodate)
      .add({
        hours: _.hour(),
        minutes: _.minute(),
        seconds: _.second()
      })
      .format('YYYY-MM-DD HH:mm:ss');
    this.auditReportRequest.StartDate = StartDate;
    this.auditReportRequest.EndDate = EndDate;
    if(this.exportClicked==false)
      {
        this.GetAuditReport();
      }
   
  }
  SetReportLevel() {
    this.IsUserLevel = !this.IsUserLevel;
    this.matSelect.value = '';
  }

  GetAppModules() {
    this.Isloading = true;
    this.auditreportservice
      .GetAppModules()
      .pipe(first())
      .subscribe({
        next: (data: any[]) => {
          debugger;
          this.modulereportlist = data;
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.Isloading = false;
        }
      });
  }

  handlePageEvent(e: PageEvent) {

    this.pageEvent = e;
    this.length = e.length;
    this.auditReportRequest.PageNo = e.pageIndex + 1;
    this.auditReportRequest.PageSize = e.pageSize;
    this.GetAuditReport();
  }

  ExpandorCollapse() {
    this.IsExpanded == true
      ? this.accordion.openAll()
      : this.accordion.closeAll();
  }
  Export()
  {
    this.auditreportListExport=[]
    this.exportClicked = true;
    this.ViewReport()
   
    this.auditReportRequest.ModuleCodes = this.matSelect.value;
    if(this.matSelect.value!="")
      {
    this.auditReportRequest.PageNo=0;
    this.auditReportRequest.PageSize=0;
    
    this.auditreportservice
    .GetAuditReport(this.auditReportRequest)
    .pipe(first())
    .subscribe({
      next: (data: any[]) => {
        this.auditreportListExport = data;
       
        let csv = '';


        csv += 'SlNo,UserName,FunctionPerformed\r\n';
    
    
        this.auditreportListExport.forEach((element, index) => {

          let slno: number = index + 1;
          this.datalist.push({
            SlNo: slno,
            UserName: element.UserName,
            FunctionPerformed: element.FunctionPerformed.join('\n'), // Join array elements with a separator
          });
    
          // Append data to CSV
          csv += [
            this.datalist[index].SlNo,
            this.datalist[index].UserName,
            `"${this.datalist[index].FunctionPerformed}"`, // Enclose in quotes to handle commas correctly
          ].join(',');
          csv += '\r\n';
        });
    
  
        const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
        const fileName = `${ this.matSelect.value.replace(/\s+/g, '')}|${this.auditReportRequest.StartDate} TO ${this.auditReportRequest.EndDate}.csv`; // Remove spaces from module name for filename
        const link = document.createElement('a');
        const url = URL.createObjectURL(blob);
        link.setAttribute('href', url);
        link.setAttribute('download', fileName);
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        this.exportClicked = false;
        if(this.ViewReportResult==true)
          {
            this.GetAuditReport();
          }
        
      },
     
    });

    }

  }
}
