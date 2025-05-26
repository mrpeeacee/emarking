import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { SemiAutomaticQuestionsService } from 'src/app/services/project/response-processing/semi-automatic.service';
import { first } from 'rxjs/operators';
import { ViewFrequencyDistributionModel, ViewAllBlankSummaryModel } from 'src/app/model/project/response-processing/semi-automatic-model';
import { QigQuestionModel } from 'src/app/model/project/qig';
import { ActivatedRoute,Router } from '@angular/router';

@Component({
  selector: 'emarking-semi-automatic-questions',
  templateUrl: './semi-automatic-questions.component.html',
  styleUrls: ['./semi-automatic-questions.component.css']
})

export class SemiAutomaticQuestionsComponent implements OnInit {

  panelOpenState = false;

  Viewfrequencydistributionlst!: ViewFrequencyDistributionModel[];
  ViewallBlanksummarylst!: ViewAllBlankSummaryModel[];
  Status: any;
  allviewquestions: QigQuestionModel[] = [];
  ProjectQuestionId!: number;
  Hidesemiauto: boolean = false;
  questionLoading: boolean = false;
  scoolsLoading: boolean = true;
  showFlyout: boolean = false;

  constructor(public translate: TranslateService,
    public commonService: CommonService,
    public Alert: AlertService,
    public semiautomaticquestionsservice: SemiAutomaticQuestionsService,
    private route: Router,private router: ActivatedRoute) { }

  ngOnInit(): void {
    this.translate.get('SetUp.SemiAutomatic.Title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate.get('SetUp.SemiAutomatic.SemiPageDescription').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
    this.GetAllViewQuestions();

    this.showFlyout = false;
  }

  GetAllViewQuestions() {
    this.questionLoading = true;
    this.semiautomaticquestionsservice.GetAllViewQuestions()
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.allviewquestions = data;
        },
        error: (a: any) => {
          throw (a);
        },
        complete: () => {
          this.questionLoading = false;
        }
      });
  }


  clickFrequencydistribution(projectQuestionId: number) {
    this.ProjectQuestionId = projectQuestionId;
    this.route.navigateByUrl('projects/semi-automatic-questions/'+this.ProjectQuestionId+'/frequency-distribution');
    
    this.ShowHideSemiAuto();
  }
  ShowHideSemiAuto() {
    this.Hidesemiauto = true;
  }

  ViewStimulus(available_qu: any) {
        var stimulusDiv = document.getElementById('divstimulus');
        if (stimulusDiv != null) {
          stimulusDiv.innerHTML = "";
          var newTile = document.createElement("div");
          newTile.setAttribute("class", "stimuluspopup");
          newTile.innerHTML = available_qu.PassageText.replace("<![CDATA[","").replace("]]>","");
          setTimeout(() => {
            if (stimulusDiv != null) {
              stimulusDiv.appendChild(newTile);
            }
          }, 100);   
        }
      }
}
