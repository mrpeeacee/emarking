import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { QigManagementModel, QigQuestionsDetails, Tagqigdetails } from 'src/app/model/project/setup/qig-management/qig-management-model';
import { AlertService } from 'src/app/services/common/alert.service';
import { QigManagementService } from 'src/app/services/project/setup/qig-management/qig-management.service';

@Component({
  selector: 'emarking-tagqig',
  templateUrl: './tagqig.component.html'
})
export class TagqigComponent implements OnInit {
  questiondetails = new QigQuestionsDetails();
  tagdataloading: boolean = false;
  status: string = "";
  @ViewChild('matSelect') matSelect: any;
  @ViewChild('closebutton') closebutton: any;

  constructor(public qigmanagementservice: QigManagementService, public Alert: AlertService, @Inject(MAT_DIALOG_DATA) public qigdata: QigManagementModel) { }

  ngOnInit(): void {
    if (this.qigdata != undefined) {
      this.GetQuestionDetails();
    }
  }

  GetQuestionDetails() {
    this.tagdataloading = true;
    this.qigmanagementservice.GetQuestionDetails(this.qigdata.QigType, this.qigdata.ProjectQigId,this.qigdata.QnsType).subscribe({
      next: (data: any) => {
        if (data.Result != undefined || data.Result != null) {
          this.questiondetails.QigName = this.qigdata.QigName;
          this.questiondetails.QigQuestionName = this.qigdata.QuestionCode;
          this.questiondetails.QuestionId = this.qigdata.ProjectQuestionId;
          this.questiondetails.QIGID = this.qigdata.ProjectQigId;
          this.questiondetails.TotalMarks = this.qigdata.QuestionMarks;
          this.questiondetails.QigTotalMarks = data.Result.QigTotalMarks;
          this.questiondetails.QigIds = data.Result.QigIds;
        }
      },
      error: (a: any) => {
        this.tagdataloading = false;
        throw (a);
      },
      complete: () => {
        this.tagdataloading = false;
      }
    });
  }

  MoveorTagQig(questiondetails: QigQuestionsDetails) {
    this.tagdataloading = true;
    var qigid = this.matSelect.value;

    if (qigid == undefined) {
      this.Alert.clear();
      this.Alert.warning("Please select QIG");
      this.tagdataloading = false;
    }
    else {

      var tagqigdetails = new Tagqigdetails();
      tagqigdetails.ProjectQuestionId = questiondetails.QuestionId;
      tagqigdetails.MoveQigId = qigid;
      tagqigdetails.ProjectQigId = questiondetails.QIGID;
      tagqigdetails.QigTotalMarks = questiondetails.QigTotalMarks;
      tagqigdetails.QnsTotalMarks = questiondetails.TotalMarks;

      this.qigmanagementservice.MoveorTagQIG(tagqigdetails).subscribe({
        next: (data: any) => {
          if (data.Result != undefined || data.Result != null) {
            this.Alert.clear();
            if (data.Result == "U001" || data.Result == "S001" || data.Result == "U002" || data.Result == "U003") {
              this.Alert.success("QIG Tagged successfully.");
              this.closebutton.nativeElement.click();
              return;
            }
          }
        },
        error: (a: any) => {
          this.tagdataloading = false;
          throw (a);
        },
        complete: () => {
          this.tagdataloading = false;
        }
      });
    }

  }
}
