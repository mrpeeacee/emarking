import { Component, OnInit, ElementRef, ViewChild, SecurityContext, Inject } from '@angular/core';
import { StudentReportsService } from 'src/app/services/reports/studentreports.service';
import { DomSanitizer } from '@angular/platform-browser';

import { ActivatedRoute } from '@angular/router';


//@ts-ignore
import jsPDF from 'jspdf';

import 'jspdf-autotable';
import { TranslateService } from '@ngx-translate/core';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';




@Component({
  selector: 'emarking-user-response',
  templateUrl: './view-candidate-response.component.html',
  styleUrls: ['./view-candidate-response.component.css']
})
export class ViewCandidateResponseComponent implements OnInit {
 
  constructor(public studentreportservice: StudentReportsService,
    public translate: TranslateService, public Alert: AlertService,
    public commonService: CommonService,
    private _sanitizer: DomSanitizer,
    private route: ActivatedRoute,public dialogRef: MatDialogRef<ViewCandidateResponseComponent>,@Inject(MAT_DIALOG_DATA) public data: any) {
      
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
  
 
  ngOnInit(): void {    
    this._scheduleid = this.route.snapshot.paramMap.get('candidateid')!;
    if(this._scheduleid == null)
    {
        this._scheduleid = this.data._scheduleid;
    }
    this.translate.get('User-Response.PageDescription').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
    this.GetUserResponse();
  }
  @ViewChild('divUserResponse', {static: false}) userresponse!: ElementRef;
  
 
  
  downloadPdf() {
    this.converttohtm(this.UserResponses);
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
  
  clickMethod(evnt: any) {   
    this.dialogRef.close({ status: 0 });
  }

  GetUserResponse() {
    this.ispageloading = true;
    this.studentreportservice.GetUserResponse(this._scheduleid,true).subscribe({
      next: (data: any) => {
        this.UserResponses = data;

        this.candidateindex = data[0]?.candidateindex;
        this.UserResponses.forEach(a => {
          a.QuestionText = this._sanitizer.bypassSecurityTrustHtml(a.QuestionText);
        });

        if(this.UserResponses.length <= 0){
            this.Alert.warning("No Response found.");
          }
      },
      error: (a: any) => {
        throw a;
      },
      complete: () => {
        this.ispageloading = false;
      },
    });
  }

  isChecked(index: number, responseText: string): boolean {
    if (responseText) {
        // Convert the responseText into an array of numbers
        const selectedIndices = responseText.split(',').map(val => parseInt(val.trim(), 10));
        // Check if the current index is in the array
        return selectedIndices.includes(index);
    }
    return false;
} 
}

