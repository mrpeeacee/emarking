import { Component, OnInit, ElementRef, ViewChild,Inject } from '@angular/core';
import { TrialmarkingService } from 'src/app/services/project/trialmarking.service';
import { DownloadMarkschemeDetails, DownloadMarkschemeModel } from 'src/app/model/project/trialmarking';
import { jsPDF } from "jspdf";


import { MAT_DIALOG_DATA, MatDialogRef , MatDialog} from '@angular/material/dialog';
import { MarkSchemeType } from 'src/app/model/project/mark-scheme/mark-scheme-model';

import html2canvas from 'html2canvas';
import { AlertService } from 'src/app/services/common/alert.service';
import { FileModel } from 'src/app/model/file/file-model';



@Component({
  selector: 'emarking-view-download-marks-scheme',
  templateUrl: './view-download-marks-scheme.component.html',
  styleUrls: ['./view-download-marks-scheme.component.css']
}) 
export class ViewDownloadMarksSchemeComponent implements OnInit {

  constructor(private trailmarkingservice: TrialmarkingService,
    @Inject(MAT_DIALOG_DATA) public data:any, private Alert:AlertService,
    public dialogRef: MatDialogRef<ViewDownloadMarksSchemeComponent>,private dialog: MatDialog) {

  }

  ngOnInit(): void {
    this.Viewdownloadmarkscheme(this.data.projectquestionid,this.data.markschemeid);
  }
 
  public records: any[] = [];
  projectquestionid: number = 0;
  markschemeid:number=0;
  downloadschemedata?: DownloadMarkschemeDetails[];
  jsondatadisplay: any;
  question : string = this.data.questionname;
  datagroup = new DownloadMarkschemeDetails();

  downloadMarkschemeModel!:DownloadMarkschemeModel;
  filelist: FileModel[] = [];

  MarkSchemeType!:MarkSchemeType;
 

  @ViewChild('htmlData', { static: false }) htmlData?: ElementRef;

  Viewdownloadmarkscheme(projectquestionid: number,markschemeid:number) { 
    this.trailmarkingservice.ViewDownloadMarkScheme(projectquestionid,markschemeid).subscribe((data) => {
      this.downloadMarkschemeModel = data;
      
      if(this.downloadMarkschemeModel.MarkSchemes == null || this.downloadMarkschemeModel.MarkSchemes.length <= 0){
        this.Alert.warning("Mark Scheme is not tagged to this question");
      }
    });

    

  }
  clickMethod(evnt: any) {
   
    this.dialogRef.close({ status: 0 });
  }
  public onExport() {
    
    // const doc = new jsPDF("p", "pt", "a1");
    // const source = document.getElementById('content') as HTMLElement;
    // doc.setFontSize(12);
    // doc.html(source
     
    // ).save(this.question);

    this.generarPDF();
  }



  generarPDF() {

    const div = document.getElementById('content') as HTMLElement;
    const options = {
      background: 'white',
      scale: 3
    };

    html2canvas(div, options).then((canvas) => {

      var img = canvas.toDataURL("image/PNG");
      var doc = new jsPDF('l', 'mm', 'a4',false);

      // Add image Canvas to PDF
      const bufferX = 5;
      const bufferY = 5;
      const pdfWidth = doc.internal.pageSize.getWidth() - 2 * bufferX;
      const pdfHeight = doc.internal.pageSize.getHeight() - 2 * bufferY;
      doc.addImage(img, 'PNG', bufferX, bufferY, pdfWidth, pdfHeight, undefined, undefined);

      return doc;
    }).then((doc) => {
      doc.save(this.question);  
    });
  }



}
