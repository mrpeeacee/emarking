import { Component, Inject, OnInit } from '@angular/core';
import {
  MAT_DIALOG_DATA,
  MatDialogRef,
  MatDialog,
} from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import {
  DownloadOutBoundLogModel,
  ReportsOutboundLogsModel,
} from 'src/app/model/reports/ems-report';
import { AlertService } from 'src/app/services/common/alert.service';
import { EMSReportService } from 'src/app/services/reports/ems-report.service';

@Component({
  selector: 'emarking-report-outbound-logs',
  templateUrl: './report-outbound-logs.component.html',
  styleUrls: ['./report-outbound-logs.component.css'],
})
export class ReportOutboundLogsComponent implements OnInit {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private Alert: AlertService,
    public dialogRef: MatDialogRef<ReportOutboundLogsComponent>,
    private dialog: MatDialog,
    private emsService: EMSReportService,
    public translate: TranslateService
  ) {}
  ApiName!: string;
  ApiLogs!: ReportsOutboundLogsModel[];
  Dwnldloading: boolean = true;
  ngOnInit(): void {
    this.ApiLogs = [] as ReportsOutboundLogsModel[];
    this.getApiLogs(this.data.type);
    this.setTitles();
    if (this.data.type == 1) {
      this.ApiName = this.intMessage.ems1;
    } else if (this.data.type == 2) {
      this.ApiName = this.intMessage.ems2;
    } else if (this.data.type == 3) {
      this.ApiName = this.intMessage.oms;
    }
  }

  intMessage: any = {
    ems1: '',
    ems2: '',
    oms: '',
    inprogress: '',
    success: '',
    failed: '',
    norec: ''
  };

  setTitles() {
    this.translate.get('RptOutbound.ems1').subscribe((translated: string) => {
      this.intMessage.ems1 = translated;
    });
    this.translate.get('RptOutbound.ems2').subscribe((translated: string) => {
      this.intMessage.ems2 = translated;
    });
    this.translate.get('RptOutbound.oms').subscribe((translated: string) => {
      this.intMessage.oms = translated;
    });
    this.translate
      .get('RptOutbound.inprogress')
      .subscribe((translated: string) => {
        this.intMessage.inprogress = translated;
      });
    this.translate
      .get('RptOutbound.success')
      .subscribe((translated: string) => {
        this.intMessage.success = translated;
      });
    this.translate.get('RptOutbound.failed').subscribe((translated: string) => {
      this.intMessage.failed = translated;
    });
    this.translate.get('RptOutbound.norec').subscribe((translated: string) => {
      this.intMessage.norec = translated;
    });
  }
  clickMethod(evnt: any) {
    this.dialogRef.close({ status: 0 });
  }
  Getoutbundstatus(status: number) {
    if (status == null || status == undefined) {
      return this.intMessage.inprogress;
    } else {
      if (status == 1) {
        return this.intMessage.success;
      } else if (status == 2) {
        return this.intMessage.inprogress;
      } else {
        return this.intMessage.failed;
      }
    }
  }

  getApiLogs(reporttype: number) {
    this.Dwnldloading = true;
    this.emsService.GetReportApiLogs().subscribe({
      next: (data: ReportsOutboundLogsModel[]) => {
        if (data != null || data != '' || data != undefined) {
          data.forEach((element: any) => {
            if (element.ReportType == reporttype) {
              this.ApiLogs.push(element);
            }
          });
        }
      },
      error: (ad: any) => {
        throw ad;
      },
      complete: () => {
        this.Dwnldloading = false;
      },
    });
  }

  datalist!: any[];
  downloadDetails!: DownloadOutBoundLogModel[];
  DownloadOutboundLogs(
    correlationid: string,
    fileName: string,
    processedon: any
  ) {
    this.Dwnldloading = true;
    this.emsService.DownloadOutboundLogs(correlationid, processedon).subscribe({
      next: (data: any) => {
        if (data.Status != 'ND001') {
          if (
            data != null &&
            data != undefined &&
            data.Items != null &&
            data.Items != undefined &&
            data.Items.length > 0
          ) {
            this.downloadDetails = data.Items;
            let columnNames = [''];
            let header = columnNames.join(',');

            let csv = header;
            this.datalist = [];
            this.downloadDetails.forEach((element) => {
              this.datalist.push(element == null ? null : element.Results);
            });
            this.datalist.forEach((c) => {
              csv += c;
              csv += '\r\n';
            });

            var blob = new Blob([csv], {
              type: 'text/json;charset=utf-8;',
            });

            var link = document.createElement('a');
            if (link.download !== undefined) {
              var url = URL.createObjectURL(blob);
              link.setAttribute('href', url);
              link.setAttribute('download', fileName + '.txt');
              document.body.appendChild(link);
              link.click();
              document.body.removeChild(link);
            }
          }
        } else {
          this.Alert.warning(this.intMessage.norec);
        }
      },
      error: (ad: any) => {
        throw ad;
      },
      complete: () => {
        this.Dwnldloading = false;
      },
    });
  }
}
