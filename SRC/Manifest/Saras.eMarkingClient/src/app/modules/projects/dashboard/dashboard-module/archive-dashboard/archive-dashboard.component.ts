import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { first } from 'rxjs';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { StudentReportsService } from 'src/app/services/reports/studentreports.service';

@Component({
  templateUrl: './archive-dashboard.component.html',
  styleUrls: ['./archive-dashboard.component.css'],
})
export class ArchiveDashboardComponent implements OnInit {
  title!: string;
  constructor(
    public stdrprt: StudentReportsService,
    public Alert: AlertService,
    public translate: TranslateService,
    public commonservice: CommonService
  ) {}

  archiveloading: boolean = false;

  ngOnInit(): void {
    this.translate.get('Dashboard').subscribe((translated: string) => {
      this.commonservice.setHeaderName(translated);
    });
    this.translate
      .get(
        'To Super Admin/Service Admin/Exam Officer/Exam Manager/Assessment Officer, this dashboard will display two buttons for downloading data reports in CSV and Excel format.'
      )
      .subscribe((translated: string) => {
        this.commonservice.setPageDescription(translated);
      });
  }

  ExcelReport() {
    this.archiveloading = true;
    this.stdrprt
      .GetStudentCompleteArchiveReport()
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.ExportReportFile(
            data,
            'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
            '.xlsx'
          );
        },
        error: (err: any) => {
          throw err;
        },
        complete: () => {
          this.archiveloading = false;
        },
      });
  }

  TextReport() {
    this.archiveloading = true;
    this.stdrprt
      .GetStudentCompleteTextArchiveReport()
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.ExportReportFile(data, 'text/csv', '.csv');
        },
        error: (err: any) => {
          throw err;
        },
        complete: () => {
          this.archiveloading = false;
        },
      });
  }

  ExportReportFile(data: any, format: string, fileExtension: string) {
    const date = new Date();
    const month = date.toLocaleString('default', { month: 'short' });
    var year = date.getFullYear();
    var day = date.getDate();

    var fileName = 'DataReport ' + day + '-' + month + '-' + year;
    const blob = new Blob([data], {
      type: format,
    });
    var link = document.createElement('a');
    if (link.download !== undefined) {
      var url = URL.createObjectURL(blob);
      link.setAttribute('href', url);
      link.setAttribute('download', fileName + '' + fileExtension);
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
  }
}
