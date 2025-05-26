import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { MarkSchemeService } from 'src/app/services/project/mark-scheme/mark-scheme.service';

@Component({
  templateUrl: './mark-scheme-questions.component.html',
  styleUrls: ['./mark-scheme-questions.component.css']
})
export class MarkSchemeQuestionsComponent implements OnInit {
  Alert: any;

  constructor(
    private service: MarkSchemeService,
    public translate: TranslateService,
    private dialogRef: MatDialogRef<MarkSchemeQuestionsComponent>
  ) {
  }
  
  public Question: any = [];
  public Marksschemes: any;
  result: any;
  projectquestionid: any = 0;
  projectmarkschemeid: any;
  questionid: any;
  selectedMarkScheme: number = 0;
  mark: any;
  questionlength: any = 0;
  pagenumber: number = 1;

  ngOnInit(): void {
    this.getQuestionMarkSchemes(this.pagenumber);
  }

  getQuestionMarkSchemes(pagenumber: number) {
    this.service.GetQuestionsUnderProject(pagenumber).subscribe((data) => {
      this.questionlength = data.Count;
      for (let item of data.Items) {
        this.Question.push(item);
      }

    });
  }

  onScrollDown(ev: any) {
    this.pagenumber = this.pagenumber + 1;
    this.getQuestionMarkSchemes(this.pagenumber);
  }

  closeeditor(){
    this.dialogRef.close();
  }
}
