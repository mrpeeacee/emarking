import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import { Filedetails, MarkScheme } from 'src/app/model/project/mark-scheme/mark-scheme-model';
import { ScoreComponentLibraryName } from 'src/app/model/project/Scoring-Component/Scoring-Component.model';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { QigService } from 'src/app/services/project/qig.service';
import { ScoringComponentService } from 'src/app/services/project/scoring-component/scoring-component.service';
import { first } from 'rxjs/operators';

@Component({
  selector: 'emarking-scoring-component-library',
  templateUrl: './scoring-component-library.component.html',
  styleUrls: ['./scoring-component-library.component.css']
})
export class ScoringComponentLibraryComponent implements OnInit {
  ScoringComponentLibrary:ScoreComponentLibraryName[]=[]
  isClosed: any;
  

  constructor(
    public translate: TranslateService,
    private Schemeservice: ScoringComponentService,
    public commonService: CommonService,
    public qigservice: QigService,
    public Alert: AlertService,
    private dialog: MatDialog,
  ) { }
  intMessages: any = {
    QustagSuccss: '',
    SelQus: '',
    ErrWhlTag: '',
    ConDelete: '',
    SelMrkSch: '',
    TrlQsn:'',
  };

  ngOnInit(): void {
    this.Getqigworkflowtracking();
    this.translate.get('mark.title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate.get('mark.pgedesc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
    this.translate.get('mark.qustag').subscribe((translated: string) => {
      this.intMessages.QustagSuccss = translated;
    });
    this.translate.get('mark.selqus').subscribe((translated: string) => {
      this.intMessages.SelQus = translated;
    });
    this.translate.get('mark.errtag').subscribe((translated: string) => {
      this.intMessages.ErrWhlTag = translated;
    });
    this.translate.get('mark.condel').subscribe((translated: string) => {
      this.intMessages.ConDelete = translated;
    });
    this.translate.get('mark.selmark').subscribe((translated: string) => {
      this.intMessages.SelMrkSch = translated;
    });
    this.translate.get('mark.trailqsn').subscribe((translated: string) => {
      this.intMessages.TrlQsn = translated;
    });
   this.getallScoringComponentLibrary()

  }



  getallScoringComponentLibrary()
  {
    this.Schemeservice.GetAllScoringComponentLibrary().subscribe((data) => {
     
      this.ScoringComponentLibrary =data
      console.log( this.ScoringComponentLibrary)
    });
    
  }
  
  Getqigworkflowtracking() {
    this.qigservice
      .Getqigworkflowtracking(0, AppSettingEntityType.QIG)
      .subscribe((data) => {
        if (data != null && data != undefined) {
          let WorkFlowStatusTracking = data;
          this.isClosed = WorkFlowStatusTracking.find(
            (x: any) => x.ProjectStatus
          )?.ProjectStatus;
          if (this.isClosed == 3) {
            this.translate
              .get('This project is closed')
              .subscribe((translated: string) => {
                this.Alert.warning(translated);
              });
          }
        }
      });
  }

  

  }



