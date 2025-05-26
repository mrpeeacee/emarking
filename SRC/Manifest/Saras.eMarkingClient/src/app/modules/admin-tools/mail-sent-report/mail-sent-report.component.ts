import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import { first } from 'rxjs';
import { ClsFilter, ClsMailSent, Mailsentdetails } from 'src/app/model/admin-tools/admin-tools-model';
import { AdminToolsService } from 'src/app/services/admin-tools/admin-tools.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';

@Component({
  selector: 'emarking-mail-sent-report',
  templateUrl: './mail-sent-report.component.html',
  styleUrls: ['./mail-sent-report.component.css']
})
export class MailSentReportComponent implements OnInit {

  constructor( private fb: FormBuilder, private adminService: AdminToolsService, 
    public Alert: AlertService, 
    public translate: TranslateService,public commonService: CommonService) { }
  
  @ViewChild('paginator') paginator!: MatPaginator;
  @ViewChild('menutrigger') menuclose!: ElementRef;
  IsLoading:boolean = false;
  // Pagination
  length = 0;
  pageSize = 5;
  pageIndex = 0;
  pageSizeOptions = [5, 10, 25, 100];
  showPageSizeOptions = true;
  pageEvent!: PageEvent;
  statuslst!:any;
  searchUserForm!: FormGroup;
  objSearch: ClsMailSent = new ClsMailSent();
  MailDetails : Mailsentdetails[] = [];
  SearchValue: string = '';
  IsEnabled = 1;
  IsDisabled = 0;
  Checkedtypes: ClsFilter[] = [];
  title!:string;
  ngOnInit(): void {

    this.pageDescTitle();

    this.objSearch.IsEnabled = 2;
    this.objSearch.PageNo = 1;
    this.objSearch.PageSize = this.pageSize;

    this.GetMailSentDetails();

    var n = new ClsFilter();

    n.Id = 1;
    n.Selected =false;
    n.Text = 'Enabled';
    n.ischecked = false;

    this.Checkedtypes.push(n);
    var m = new ClsFilter();
    m.Id = 2;
    m.Selected =false;
    m.Text = 'Disabled';
    m.ischecked = false;

    this.Checkedtypes.push(m);

  }

  pageDescTitle(){
    this.translate
    .get('MailsentReport.pagedesc')
    .subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });

  this.translate
    .get('MailsentReport.pagetitle')
    .subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
      this.title = translated;
    });
  }

  SearchUserName() {
    this.paginator.pageIndex = 0;
    this.objSearch.SearchText = this.SearchValue.trim();
    this.objSearch.PageNo = 1;
    this.GetMailSentDetails();

  }

  FiltersByUser() {

    var fenable = this.Checkedtypes.filter(a => a.Id == 1 && a.Selected);
    var fdisable = this.Checkedtypes.filter(a => a.Id == 2 && a.Selected);

    if(fenable.length == 1){
      this.objSearch.IsEnabled = 0
    }
    if(fdisable.length == 1){
      this.objSearch.IsEnabled = 1
    }
    if(this.Checkedtypes.filter(a => a.Selected).length == 2 || this.Checkedtypes.filter(a => !a.Selected).length == 2 ){
      this.objSearch.IsEnabled = 2
    }


    this.paginator.pageIndex = 0;
    this.objSearch.PageNo = 1;

    this.GetMailSentDetails();

  }


  handlePageEvent(e: PageEvent) {
    this.pageEvent = e;
    this.length = e.length;
    this.objSearch.PageNo = e.pageIndex + 1;
    this.objSearch.PageSize = e.pageSize;
    this.GetMailSentDetails();
  }

  GetMailSentDetails() {
    this.IsLoading = true;
   //// var obj = new ClsMailSent();
    this.adminService.MailSentDetails(this.objSearch)
      .pipe(first())
      .subscribe({
        next: (data: Mailsentdetails[]) => {
          this.MailDetails = data;
          debugger;
          this.length = this.MailDetails[0].TotalCount;
        },
        complete: () => {
          this.IsLoading = false;
        },
        error: (er: any) => {
          throw er;
        },
      });  
  }


  ExcelReport() {
    const date = new Date();
    const month = date.toLocaleString('default', { month: 'short' });
    var year = date.getFullYear();
    var day = date.getDate();

    this.adminService.ExportMailSentDetails().pipe(first())
      .subscribe({
        next: (data: any) => {
          var fileName = "MailSentDataReport " + day + "-" + month + "-" + year;
          const blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
          var link = document.createElement('a');
          if (link.download !== undefined) {
            var url = URL.createObjectURL(blob);
            link.setAttribute('href', url);
            link.setAttribute('download', fileName);
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            this.Alert.success("Report Downloaded Successfully");
          }


        },
        error: (err: any) => {
          throw err;
        },
        complete: () => {
          this.IsLoading = false;
        }

      });

  }

}
