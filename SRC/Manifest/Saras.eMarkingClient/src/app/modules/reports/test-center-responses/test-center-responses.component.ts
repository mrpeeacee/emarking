import { Component, OnInit, SecurityContext } from '@angular/core';
import { StudentReportsService } from 'src/app/services/reports/studentreports.service';
import { DomSanitizer } from '@angular/platform-browser';

import { TestcentrewiseReportService } from 'src/app/services/reports/testcentrewise-report.service';

import jsPDF from 'jspdf'; 
import 'jspdf-autotable';



@Component({
  selector: 'emarking-test-center-responses',
  templateUrl: './test-center-responses.component.html',
  styleUrls: ['./test-center-responses.component.css']
})
export class TestCenterResponsesComponent implements OnInit {

  constructor(public studentreportservice: StudentReportsService,
    public testcentrewiseReportService:TestcentrewiseReportService,

    private _sanitizer: DomSanitizer
    ) { }
  ispageloading: boolean = false;
  table!: HTMLTableElement;

  UserResponses!: any[];
  stasticsloading:boolean = false;
  UserResponsespdf!: any[];
  result!:any[];
  htmlFiles!: string[];
  _columns!:any[];
  _scheduleid?:any = 0;
  pdfs: jsPDF[] = [];
  candidateindex:any;
  public _testcentredet: any[] = [];
  selectedtestcenterid:any = 0;
  questiontext?:any;
  ngOnInit(): void {
    this.GetUserResponse();
  }

  
  viewcandidaterespone() {
    
    this.stasticsloading = true;
    
      this.testcentrewiseReportService.GetUserResponse(this._scheduleid, this.selectedtestcenterid).subscribe({
        next: (data: any) => {
          this.UserResponses = data;

          var groups = new Set(data.map((item:any) => item.candidateindex))
          var result:any = [];
          groups.forEach(g => 
            result.push({
              candidateindex: g, 
              values: data.filter((i:any) => i.candidateindex === g)
            }
          ))
          
          this.result = result;
          for (let resultItem of this.result) {
          for (let valueItem of resultItem.values.reverse()) {
            valueItem.QuestionText = valueItem.QuestionText.replace(/<img[^>]*>/g, "");
            valueItem.QuestionText = valueItem.QuestionText.replace(/<audio[^>]*>/g, "");
            valueItem.QuestionText = valueItem.QuestionText.replace(/<video[^>]*>/g, "");
          }
         }
        
          var imgTags= document.querySelectorAll('img') ;
          imgTags.forEach((img) => {
            document.body.removeChild(img);
          });
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.stasticsloading = false;
        },
      });
   
    
  }
  viewquestion(questionid:any){
     
    this.testcentrewiseReportService.getquestiondetails(questionid).subscribe({
      next: (data: any) => {
        
        this.questiontext = data
      },
      error: (a: any) => {
        throw a;
      },
      complete: () => {
        this.ispageloading = false;
      },
    });
  }
  downloadPdf() {
    
    this.stasticsloading = true;
    if(this.result == undefined)
    {
      this.testcentrewiseReportService.GetUserResponse(this._scheduleid, this.selectedtestcenterid).subscribe({
        next: (data: any) => {
          this.UserResponses = data;

          var groups = new Set(data.map((item:any) => item.candidateindex))
          var result:any = [];
          groups.forEach(g => 
            result.push({
              candidateindex: g, 
              values: data.filter((i:any) => i.candidateindex === g)
            }
          ))
          
          this.result = result;
          this.candidateindex = data[0].candidateindex;
         
          this.converttohtm();
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.stasticsloading = false;
        },
      });
    
    }
    else
    {
      this.converttohtm();
    }
   
    
  }

  async converttohtm()
  {
    var table = document.createElement("table");
    table.style.border = "1px solid #000";
    table.style.marginTop = "30px";
    var thead = document.createElement("thead");
    table.appendChild(thead);
    
    
      var tbody = document.createElement("tbody");
    table.appendChild(tbody);

    for (let j = 0 ; j< this.result.length ; j++)
    {
      var row = table.insertRow();
     
      var cell1 = row.insertCell(0);
      cell1.style.height = "30px";
      var cell2 = row.insertCell(0);
      cell2.style.height = "30px"
      cell2.innerHTML = "Candidate Index : " + this.result[j].candidateindex;
      cell2.colSpan = 2;
      for(let i = 0 ; i< this.result[j].values.length ; i++)
      {
        var row2 = table.insertRow();
        var cell1 = row2.insertCell(0);
        var cell2 = row2.insertCell(0);
        cell2.innerHTML = this.result[j].values[i].QuestionCode;
        
         if(this.result[j].values[i].QuestionType != 10)
        {
          cell1.innerHTML = this._sanitizer.sanitize(SecurityContext.HTML, this.result[j].values[i].QuestionTextReport)!;
         }
        for(let i =0 ; i<cell2.getElementsByTagName('audio').length; i++)
        {
          cell1.getElementsByTagName('audio')[i].replaceWith("[-audio-]");
  
      }
      for(let i =0 ; i<cell2.getElementsByTagName('img').length; i++)
      {
        cell1.getElementsByTagName('img')[i].replaceWith("[-image-]");
  
      }
      for(let i =0 ; i<cell2.getElementsByTagName('video').length; i++)
      {
        cell1.getElementsByTagName('video')[i].replaceWith("[-video-]");
  
      }
      if(this.result[j].values[i].QuestionType == 10)
      {
        cell1.innerHTML = "Candidate Response: " + this.result[j].values[i].ResponseText;

      }
      }
     
    }
     
  const doc = new jsPDF();
  
    // Convert the HTML table to an array of arrays (rows and columns)
    (doc as any).autoTable({ html : table ,  theme: 'grid', useCss: false,  },
      );
    
    doc.save('Candidate Summery.pdf');
    
    this.stasticsloading = false;
    
  }

  GetUserResponse() {
    
    this._testcentredet = [];
    this.ispageloading = true;
    this.testcentrewiseReportService.ProjectCenters().subscribe({
      next: (data: any) => {
        
        for (let i = 0; i < data.length; i++) {

          var det =
            { "id": data[i].ProjectCenterID, "name": data[i].CenterName };
          this._testcentredet.push(det);

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
}
