import { Component, OnInit} from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { AutomaticQuestionsService } from 'src/app/services/project/response-processing/automatic.service';
import { first } from 'rxjs/operators';
import { AutomaticQuestionsModel } from 'src/app/model/project/response-processing/automatic-model';
import { Router, ActivatedRoute } from '@angular/router';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { QigConfigService } from 'src/app/services/project/qigconfig.service';

@Component({
  selector: 'emarking-automatic-questions',
  templateUrl: './automatic-questions.component.html',
  styleUrls: ['./automatic-questions.component.css']
})
export class AutomaticQuestionsComponent implements OnInit {

  panelOpenState = false;
  allviewquestions: AutomaticQuestionsModel[] = [];
  allViewquestionsSearchlst: AutomaticQuestionsModel[] = [];
  selected: any[] = [];
  AutomaticSearchValue: string = '';
  FilterValue: number = 0;
  automaticQuestionloading: boolean = true;
  ProjectQuestionId!: number;
  ParentQuestionId?: number;
  ProjectQId?:number;
  selectedoptionindex: number = 0;
  StimulusText: any;
  psgText: any;
  qigquestions: any;

  constructor(public translate: TranslateService,
    public commonService: CommonService,
    private sanitizer: DomSanitizer,
    private router: ActivatedRoute,
    public qigconfigservice: QigConfigService,
    public Alert: AlertService, public automaticquestionsservice: AutomaticQuestionsService, private route: Router) { }


  ngOnInit(): void {
    this.translate.get('SetUp.Automatic.Title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate.get('SetUp.Automatic.AutomaticPageDescription').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
    this.GetViewAllAutomaticQuestions();
    this.ProjectQId = this.router.snapshot.params['pqid'];
  }

  GetViewAllAutomaticQuestions() {
    this.automaticQuestionloading = true;
    this.automaticquestionsservice.GetViewAllAutomaticQuestions()
      .pipe(first())
      .subscribe({
        next: (data: any) => {      
          this.allviewquestions = data;
          this.allViewquestionsSearchlst = data;
               
        },
        error: (a: any) => {
          throw (a);
        },
        complete: () => {
          this.automaticQuestionloading = false;
        }
      });
  }
  trustedHtmlCache: { [key: string]: SafeHtml } = {};
  getTrustedHtml(htmlcode:string): SafeHtml {
    if (!this.trustedHtmlCache[htmlcode]) {
      this.trustedHtmlCache[htmlcode] = this.sanitizer.bypassSecurityTrustHtml(htmlcode);
    }
    return this.trustedHtmlCache[htmlcode];
  }
  clickModeratescore(projectQuestionId: number) {
    this.ProjectQuestionId = projectQuestionId;
    this.route.navigateByUrl('projects/automatic-questions/' + this.ProjectQuestionId + '/moderate-score');
  }

  indexExpanded: number = -1;

  togglePanels(index: number) {
      this.indexExpanded = index == this.indexExpanded ? -1 : index;
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
