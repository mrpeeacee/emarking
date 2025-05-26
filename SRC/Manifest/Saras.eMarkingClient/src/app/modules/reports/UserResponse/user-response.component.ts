import { Component, OnInit, ElementRef, ViewChild, SecurityContext, ChangeDetectorRef } from '@angular/core';
import { StudentReportsService } from 'src/app/services/reports/studentreports.service';
import { DomSanitizer } from '@angular/platform-browser';

import { ActivatedRoute } from '@angular/router';


//@ts-ignore
import jsPDF from 'jspdf';

import 'jspdf-autotable';
import { TranslateService } from '@ngx-translate/core';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';




@Component({
  selector: 'emarking-user-response',
  templateUrl: './user-response.component.html',
  styleUrls: ['./user-response.component.css']
})
export class UserResponseComponent implements OnInit {
  download!: boolean;
  QuestionText!: boolean;
 
  constructor(public studentreportservice: StudentReportsService,
    public translate: TranslateService, public Alert: AlertService,
    public commonService: CommonService,
    private _sanitizer: DomSanitizer,
    private route: ActivatedRoute, private cdr: ChangeDetectorRef) {
      
     }
  ispageloading: boolean = false;
  table!: HTMLTableElement;

  UserResponses!: any[];
  UserResponsespdf!: any[];
  htmlFiles!: string[];
  _columns!:any[];
  _scheduleid?:any;
  pdfs: jsPDF[] = [];
  candidateindex:any;
  candidateInfoResponses!: any;
 
  ngOnInit(): void {
    
    this._scheduleid = this.route.snapshot.paramMap.get('candidateid')!;
    this.translate.get('User-Response.PageDescription').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
    this.GetUserResponse();
  }
  @ViewChild('divUserResponse', {static: false}) userresponse!: ElementRef;
  
  @ViewChild('divUserResponses', {static: false}) userresponsedownload!: ElementRef;
  
  downloadPdf() {
    this.DownloadreportasPdf();
  }
  @ViewChild('content') content: ElementRef | undefined;


  DownloadreportasPdf() {

    this.download=true
    this.QuestionText=true
        this.UserResponsespdf=this.UserResponses;
        this.UserResponsespdf.forEach(a => {
          if(a.QuestionType!=10 && a.QuestionType!=154  && a.QuestionType!=92)
            {
              if(a.QuestionText.changingThisBreaksApplicationSecurity!=null)
                {
                 
              a.QuestionText = this._sanitizer.bypassSecurityTrustHtml(a.QuestionText.changingThisBreaksApplicationSecurity);
                }
            }
            else if(a.QuestionType==92)
              {
                if (a.QuestionText?.changingThisBreaksApplicationSecurity) {
            
                  let sanitizedText = a.QuestionText.changingThisBreaksApplicationSecurity;
  
            
                  sanitizedText += `
                  <style> 
                  .blankDiv{
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
                      }
                  </style>
              `;
                  a.QuestionText = this._sanitizer.bypassSecurityTrustHtml(sanitizedText);
              }
              }
            else{
              a.QuestionText= null;
            }
      
        });
        this.cdr.detectChanges(); 
        let pfFilename = "CandidateResponse"
        if (this.candidateInfoResponses != null && this.candidateInfoResponses != undefined) {
    
          pfFilename += this.candidateInfoResponses.CandidateName + '_' + new Date().toDateString();
        }
        pfFilename = pfFilename + '.pdf';
        const contentElement = this.userresponsedownload?.nativeElement;
    
        this.studentreportservice.ExportHtmlToPdf(contentElement.innerHTML, pfFilename);
        this.GetUserResponse()
    
      }
  
  async converttohtm(UserResponses:any)
  {
    var table = document.createElement("table");
    table.style.border = "1px solid #000";
    table.style.marginTop = "30px";
    var thead = document.createElement("thead");
    table.appendChild(thead);
    
    
      var tbody = document.createElement("tbody");
    table.appendChild(tbody);
    for(let i = UserResponses.length - 1 ; i>=0 ;i--)
   {
    var childtable =   document.createElement("table");
    childtable.style.width = "780px";
  
    var row2 = childtable.insertRow(0);
    row2.style.display ="flex;"
    
    var row = table.insertRow(0);
    var cell1 = row.insertCell(0);
     cell1.style.borderRight = "1px solid #000";
     cell1.style.width = "50px";
    var cell2 = row.insertCell(1);
    
      cell1.innerHTML = UserResponses[i].QuestionCode;
      cell2.style.display ="flex;"
    


  
    if(UserResponses[i].QuestionType == 20 || UserResponses[i].QuestionType == 85  )
    {
 
    row2.innerHTML = this._sanitizer.sanitize(SecurityContext.HTML, UserResponses[i].QuestionTextReport)!;
   
    const audioElements = Array.from(row2.getElementsByTagName('audio'));
    for (const audioElement of audioElements) {
      audioElement.replaceWith("[-audio-]");
    }

    const imgElements = Array.from(row2.getElementsByTagName('img'));
    for (const imgElement of imgElements) {
      imgElement.replaceWith("[-img-]");
    }

    const videoElements = Array.from(row2.getElementsByTagName('video'));
    for (const videoElement of videoElements) {
      videoElement.replaceWith("[-video-]");
    }
  }
  else
  {
    var row1 = childtable.insertRow(0);
    row1.style.maxWidth = "200px";
    var divemt = document.createElement("div");
    divemt.innerHTML = UserResponses[i].ResponseText;
    row1.innerHTML = "Candidate Response: " + divemt.innerText;
  }
   
    const parser = new DOMParser();
    const htmlString = row2.innerHTML;
   const docn = parser.parseFromString(htmlString, 'text/html');
   row2.innerHTML = docn.body.innerHTML;


    cell2.append(childtable);
  }
  var childtable2 =   document.createElement("table");
    childtable2.style.width = "780px";
  
    var rows2 = childtable2.insertRow(0);
    rows2.style.display ="flex;"
    
    var rows = table.insertRow(0);
    var cells1 = rows.insertCell(0);
     cells1.style.borderRight = "1px solid #000";
     cells1.style.width = "50px";
    var cells2 = rows.insertCell(1);
    
      cells1.innerHTML = "Candidate Index : " +  this.UserResponses[0].candidateindex;
      cells2.style.display ="flex;"
      rows2.innerHTML = "";
      cells1.colSpan = 2;
      cells1.append(childtable2);
  const doc = new jsPDF();
  
    // Convert the HTML table to an array of arrays (rows and columns)
    (doc as any).autoTable({ html : table ,  theme: 'grid', useCss: false,  },
      );
    
    doc.save('Candidate Summery.pdf');
    
  }
  

  GetUserResponse() {
    this.ispageloading = true;
    this.studentreportservice.GetUserResponse(this._scheduleid,false).subscribe({
      next: (data: any) => {
        this.UserResponses = data;
        
        this.candidateindex = data[0]?.candidateindex;
        this.UserResponses.forEach(a => {
          a.QuestionText = this._sanitizer.bypassSecurityTrustHtml(a.QuestionText);
        });
      },
      error: (a: any) => {
        this.ispageloading = false;
        throw a;
      },
      complete: () => {
        this.ispageloading = false;
      },
    });
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

