import {
  Component,
  ElementRef,
  OnInit,
  SecurityContext,
  ViewChild,
  Injectable
} from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { jsPDF } from 'jspdf';
import { first } from 'rxjs';
import {
  SchoolInfoDetails,
  UserResponses,
} from 'src/app/model/reports/studentreports';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { StudentReportsService } from 'src/app/services/reports/studentreports.service';
import { PageEvent, MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { SearchFilterModel } from 'src/app/model/project/setup/user-management';
import { SchoolDetails } from 'src/app/model/Global/UserManagement/UserManagementModel';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'emarking-all-users-response',
  templateUrl: './all-users-response.component.html',
  styleUrls: ['./all-users-response.component.css'],
})
@Injectable()
export class AllUsersResponseComponent
  extends MatPaginatorIntl
  implements OnInit
{
  [x: string]: any;
  @ViewChild('printDiv')
  block!: ElementRef;
  @ViewChild('paginator') paginator!: MatPaginator;

  AllUseresponses: any;
  IMG1!: any;
  Index!: any[];
  responsetext!: string;
  responseCellQuestioncode: any;
  imgElements!: any;
  IMG2: any;
  result: any = [];
  stasticsloading: boolean = false;
  /// Pagination
  length = 0;
  pageSize = 5;
  pageIndex = 0;
  pageNo = 0;
  pageSizeOptions = [5, 10, 25, 100];
  showPageSizeOptions = true;
  pageEvent!: PageEvent;
  objSearch: SearchFilterModel = new SearchFilterModel();
  schooldetails!: SchoolInfoDetails[];

  constructor(
    public studentreportservice: StudentReportsService,
    public translate: TranslateService,
    public Alert: AlertService,
    public commonService: CommonService,
    private _sanitizer: DomSanitizer,
    private route: ActivatedRoute
  ) {
    super();
    this.itemsPerPageLabel = 'candidates / page';
  }

  ngOnInit(): void {
    this.translate.get('UserResponse.title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate.get('UserResponse.desc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });

    this.objSearch.PageNo = 1;
    this.objSearch.PageSize = this.pageSize;
    this.GetSchoolInfoDetails();
    this.GetAllUseresponses();
    this.searchUserForm = this.fb.group({
      fschool: new FormControl(''),
    });
  }

  @ViewChild('divUserResponse', { static: false }) userresponse!: ElementRef;
  ispageloading: boolean = false;
  table!: HTMLTableElement;

  UserResponses!: UserResponses[];
  UserResponsespdf!: any[];
  htmlFiles!: string[];
  _columns!: any[];
  groupedResponses: string[] = [];
  candidateindex: string[] = [];
  pdfs: jsPDF[] = [];
  searchUserForm!: FormGroup;
  @ViewChild('matSelectschool') matSelectschool: any;
  schoolcodes: string = '';
  questiontextreportsanitized: any
  GetSchoolInfoDetails() {
    this.studentreportservice
      .GetSchoolSDetails()
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.schooldetails = data;
        },
        error: (err: any) => {
          throw err;
        },
        complete: () => {
          this.stasticsloading = false;
        },
      });
  }

  IscheckedSchool(event: any, el: SchoolDetails) {
    el.IsSchoolSelected = event.source.selected;
  }

  Clear() {
    this.GetSchoolInfoDetails();
  }

  GetAllUseresponses() {
    this.ispageloading = true;
    this.stasticsloading = true;
    this.studentreportservice.GetAllUseresponses(this.objSearch).subscribe({
      next: (data: any) => {
        this.UserResponses = data;
        if (this.UserResponses.length > 0) {
          this.UserResponses.forEach((response) => {
            response.UserQuestionResponses.forEach((element: any) => {
              element.QuestionText = this._sanitizer.bypassSecurityTrustHtml(
                element.QuestionText
              );
              this.length = response.UserQuestionResponses[0].TotalRows;
            });
          });
        }
      },
      error: (error: any) => {
        this.ispageloading = false;
        this.stasticsloading = false;
        throw error;
      },
      complete: () => {
        this.ispageloading = false;
        this.stasticsloading = false;
      },
    });
  }

  downloadPdf() {
    this.generatePdf();
    this.Clear();
  }

  handlePageEvent(e: PageEvent) {
    this.pageEvent = e;
    this.length = e.length;
    this.objSearch.PageNo = e.pageIndex + 1;
    this.objSearch.PageSize = e.pageSize;
    this.GetAllUseresponses();
  }

  DownloadreportasPdf(_dataResponse: any) {
    let pfFilename = 'AllUserResponse';
    if (
      this.candidateInfoResponses != null &&
      this.candidateInfoResponses != undefined
    ) {
      pfFilename +=
        this.candidateInfoResponses.CandidateName +
        '_' +
        new Date().toDateString();
    }
    pfFilename = pfFilename + '.pdf';
    const contentElement = _dataResponse;
    this.studentreportservice.ExportHtmlToPdf(
      contentElement.innerHTML,
      pfFilename
    );
  }

  generatePdf() {
    this.ispageloading = true;
    this.stasticsloading = true;
    this.objSearch.PageNo = 0;
    this.objSearch.PageSize = 0;
    this.schoolcodes = '';
    this.result = [];

    //Start To filter response as per school code
    this.schooldetails
      .filter((x) => x.IsSchoolSelected)
      .map((a) => a.SchoolID)
      .forEach((element) => {
        this.schoolcodes += element + ',';
      });

    this.schoolcodes = this.schoolcodes.slice(0, -1);
    if (this.matSelectschool.value != undefined) {
      this.objSearch.SchoolCode = this.schoolcodes;
    }

    //End
    this.studentreportservice.GetAllUseresponses(this.objSearch).subscribe({
      next: (data: any) => {
        /// this.AllUserResponses = data; 
        var groups = new Set(data.map((item: any) => item.candidateindex));

        data.forEach((response: { UserQuestionResponses: any[] }) => {
          response.UserQuestionResponses.forEach((element: any) => {
            element.QuestionText = this._sanitizer.bypassSecurityTrustHtml(
              element.QuestionText
            );
          });
        });

        groups.forEach((g) =>
          this.result.push({
            candidateindex: g,
            values: data.filter((i: any) => i.candidateindex === g),
          })
        );

        ///this.result = this.result;
        this.candidateindex = data[0].candidateindex;

        this.converttohtm(this.result);
        this.menuclose.nativeElement.click();
        this.GetSchoolInfoDetails();
      },
      error: (error: any) => {
        this.ispageloading = false;
        this.stasticsloading = false;
        throw error;
      },
      complete: () => {
        this.ispageloading = false;
        this.stasticsloading = false;
      },
    });
  }
 
  async converttohtm(_dataResponse: any) {
    
    var divElement = document.createElement('div');
    var table = document.createElement('table');
    divElement.appendChild(table);
    table.style.background = '#fff';
    table.style.width = '100%';
    table.style.border = '1px solid #efebeb';
    table.style.marginBottom = '10px';
    var thead = document.createElement('thead');
    table.appendChild(thead);

    var tbody = document.createElement('tbody');
    table.appendChild(tbody);

    for (let j = 0; j < _dataResponse.length; j++) {
      var row = table.insertRow();
      var headerRow = document.createElement('tr');
      var headerCell1 = document.createElement('td');
      headerCell1.style.width = '50px';
      headerCell1.style.paddingRight = '20px';
      headerCell1.style.verticalAlign = 'top';
      headerCell1.textContent = 'Question Code';

      var headerCell2 = document.createElement('td');
      headerCell2.textContent = 'Candidate Response';

      headerRow.appendChild(headerCell1);
      headerRow.appendChild(headerCell2);

      // Append the header row to the table
      table.appendChild(headerRow);

      var cell1 = row.insertCell(0);
      cell1.style.height = '30px';
      var cell2 = row.insertCell(0);
      cell2.style.height = '30px';
      cell2.innerHTML = 'Candidate Index : ' + _dataResponse[j].candidateindex;
      cell1.colSpan = 1;
      cell2.colSpan = 2;
      for (
        let i = 0;
        i < _dataResponse[j].values[0].UserQuestionResponses.length;
        i++
      ) {
        var row2 = table.insertRow();
        cell1 = row2.insertCell(0);
        cell2 = row2.insertCell(0);
        cell2.innerHTML =
          _dataResponse[0].values[0].UserQuestionResponses[i].QuestionCode;

        if ( ((_dataResponse[j].values[0].UserQuestionResponses[i].QuestionType != 10) && (_dataResponse[j].values[0].UserQuestionResponses[i].QuestionType != 154)&& (_dataResponse[j].values[0].UserQuestionResponses[i].QuestionType !=92)&&(_dataResponse[j].values[0].UserQuestionResponses[i].QuestionType != 16))) {
          cell1.innerHTML = this._sanitizer.sanitize(
            SecurityContext.HTML,
            _dataResponse[j].values[0].UserQuestionResponses[i]
              .QuestionTextReport
          )!;
        }
        if (_dataResponse[j].values[0].UserQuestionResponses[i].QuestionType == 92||_dataResponse[j].values[0].UserQuestionResponses[i].QuestionType == 16) {
          const questionResponse = _dataResponse[j].values[0].UserQuestionResponses[i];
          if (questionResponse.QuestionTextReport) {
            // Get the raw HTML content
            let sanitizedText = questionResponse.QuestionTextReport;
        
            // Append CSS styles for proper display of images and divs
            if(questionResponse.QuestionType==92)
              {
                   sanitizedText += `
                     <style>
                       .blankDiv {
                         border: 2px solid #bebebe;
                         margin: 10px;
                         padding: 2px;
                         display: inline-block;
                         margin: 0 4px;
                       }
                       img {
                         display: block;
                         max-width: 100%;
                         height: auto;
                         page-break-inside: avoid;
                       }
                     </style>
                   `;
              }
        
            // Sanitize the HTML
         
            questionResponse.QuestionTextReport= this._sanitizer.bypassSecurityTrustHtml(sanitizedText);
        
            // Set the sanitized HTML to the table cell
            cell1.innerHTML = questionResponse.QuestionTextReport.changingThisBreaksApplicationSecurity;
          }
        }
        for (let a = 0; a < cell2.getElementsByTagName('audio').length; a++) {
          cell1.getElementsByTagName('audio')[a].replaceWith('[-audio-]');
        }
        for (let b = 0; b < cell2.getElementsByTagName('img').length; b++) {
          cell1.getElementsByTagName('img')[b].replaceWith('[-image-]');
        }
        for (let c = 0; c < cell2.getElementsByTagName('video').length; c++) {
          cell1.getElementsByTagName('video')[c].replaceWith('[-video-]');
        }
        if (
          (_dataResponse[j].values[0].UserQuestionResponses[i].QuestionType == 10) || ( _dataResponse[j].values[0].UserQuestionResponses[i].QuestionType == 154)
        ) {
          cell1.innerHTML =
            _dataResponse[j].values[0].UserQuestionResponses[i].ResponseText;
        }
      }
    }

    this.DownloadreportasPdf(divElement);

    this.stasticsloading = false;
  }
  isChoiceSelected(responseText: string, choiceIndex: number): boolean {
    if (!responseText) {
      return false;
    }
    // Convert ResponseText to a Set of numbers for quick lookup
    const selectedChoices = new Set(responseText.split(',').map(val => parseInt(val, 10)));
    return selectedChoices.has(choiceIndex);
  }
}
