import { Component, Inject, OnInit } from '@angular/core';
import {
  MatDialog,
  MatDialogRef,
  MAT_DIALOG_DATA,
} from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { first } from 'rxjs/operators';
import { AlertService } from 'src/app/services/common/alert.service';
import { RecommendationService } from 'src/app/services/project/standardisation/std-one/recommendation/recommendation.service';
import { IRecommedData, IBandingScriptResponse, IBanding } from 'src/app/model/project/standardisation/std-one/recommendation/recommendation-model';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';


@Component({
  templateUrl: './banding.component.html',
  styleUrls: ['./banding.component.css']
})
export class BandingComponent implements OnInit {
  constructor(
    @Inject(MAT_DIALOG_DATA) public recmdata: IRecommedData,
    public recommendationService: RecommendationService,
    private dialog: MatDialog,
    public Alert: AlertService,
    public dialogRef: MatDialogRef<BandingComponent>,
    public translate: TranslateService
  ) { }

  scripresponses: IBandingScriptResponse[] = [];
  activeresponse!: IBandingScriptResponse;
  activeresponseindex: number = 0;
  IsQigLevel: boolean = true;
  DefaultBands!: IBanding[];
  S1completed: any;
  IsBandEnable:boolean = true;
  intMessages: any = {
    RecCnfirm: '',
    RecSuccss: '',
    RecUpdSuccss: '',
    ScrAlrRecmd: '',
    BndAlResp: '',
    RecError: '',
    Markingprocesspaused: '',
    IsNotKP: '',
    BndAlQigResp: '',
    NotMappedMarkSchemeMessage: '',
    ScriptTrialMarked: '',
    s1closure: ''
  };
  ngOnInit(): void {
    this.translate.get('Std.band.CnfmRecmn').subscribe((translated: string) => {
      this.intMessages.RecCnfirm = translated;
    });
    this.translate.get('Std.band.RecSuccs').subscribe((translated: string) => {
      this.intMessages.RecSuccss = translated;
    });
    this.translate.get('Std.band.RecAlExt').subscribe((translated: string) => {
      this.intMessages.ScrAlrRecmd = translated;
    });
    this.translate.get('Std.band.SelBnd').subscribe((translated: string) => {
      this.intMessages.BndAlResp = translated;
    });
    this.translate.get('Std.band.SelQigBnd').subscribe((translated: string) => {
      this.intMessages.BndAlQigResp = translated;
    });
    this.translate.get('Std.band.RecErr').subscribe((translated: string) => {
      this.intMessages.RecError = translated;
    });
    this.translate
      .get('Std.SetUp.Markingprocesspaused')
      .subscribe((translated: string) => {
        this.intMessages.Markingprocesspaused = translated;
      });
    this.translate.get('Std.SetUp.IsNotKP').subscribe((translated: string) => {
      this.intMessages.IsNotKP = translated;
    });
    this.translate
      .get('Std.band.RecUpdSuccss')
      .subscribe((translated: string) => {
        this.intMessages.RecUpdSuccss = translated;
      });
    this.translate
      .get('Std.band.NotMappedMarkSchemeMessage')
      .subscribe((translated: string) => {
        this.intMessages.NotMappedMarkSchemeMessage = translated;
      });
    this.translate
      .get('Std.band.ScriptTrialMarked')
      .subscribe((translated: string) => {
        this.intMessages.ScriptTrialMarked = translated;
      });
    this.translate
      .get('Std.QuaAsseCrea.s1closure')
      .subscribe((translated: string) => {
        this.intMessages.s1closure = translated;
      });


    this.getscriptreponses();
  }

  getscriptreponses(): void {
    this.recommendationService
      .getScriptQuestions(this.recmdata.ScriptId, this.recmdata.QigId)
      .subscribe((data: IBandingScriptResponse[]) => {
        this.scripresponses = data;
        if (
          this.scripresponses != null &&
          this.scripresponses != undefined &&
          this.scripresponses.length > 0
        ) {
          this.scripresponses[0].IsActive = true;
          this.IsQigLevel = this.scripresponses[0].IsQigLevel;
          this.getQnsResponse(this.scripresponses[0]);

          if (this.IsQigLevel) {
            this.DefaultBands = this.scripresponses[0]?.Bands;
            this.DefaultBands.forEach((a) => {
              a.IsSelected =
                a.BandId == this.scripresponses[0].RecommendedBand?.BandId;
            });
          }
        }
      });
  }

  getQnsResponse(qns: IBandingScriptResponse) {
    this.scripresponses.forEach((scriptrespelement) => {
      scriptrespelement.IsActive = false;
    });
    qns.IsActive = true;
    this.activeresponse = {} as IBandingScriptResponse;
    this.recommendationService
      .getScriptQuestionResponse(
        this.recmdata.ScriptId,
        qns.ProjectQnsId,
        true
      )
      .subscribe((data: IBandingScriptResponse) => {
        this.activeresponse = data;
       
        if (data != null || data != undefined) {
          this.activeresponseindex = this.scripresponses.findIndex(
            (a) => a.ProjectQnsId == this.activeresponse.ProjectQnsId
          );
          if (this.activeresponseindex >= 0) {
            let selBand =
              this.scripresponses[this.activeresponseindex].RecommendedBand;
            if (selBand != undefined) {
              this.activeresponse.Bands.forEach((activerespelement) => {
                activerespelement.IsSelected =
                  activerespelement.BandId == selBand?.BandId;
              });
            }
          }
        }
        this.IsBandEnable=false;
      });
  }
  recommendCancel() { 
    if (this.confirmDialog != null && this.confirmDialog != undefined) {
      this.confirmDialog.close();
    }
  }

  onBandSelected(band: number) {
    if (band != 0) {
      this.activeresponse.Bands.forEach((bandselectedelement) => {
        if (bandselectedelement.BandId == band) {
          bandselectedelement.IsSelected = true;
          this.activeresponse.RecommendedBand = bandselectedelement;
        } else {
          bandselectedelement.IsSelected = false;
        }
      });
      this.scripresponses.forEach((bandscriptrespelement) => {
        if (
          bandscriptrespelement.ProjectQnsId == this.activeresponse.ProjectQnsId
        ) {
          bandscriptrespelement.RecommendedBand =
            this.activeresponse.RecommendedBand;
        }
      });
    } else {
      this.scripresponses.forEach((bandelement) => {
        if (bandelement.ProjectQnsId == this.activeresponse.ProjectQnsId) {
          bandelement.RecommendedBand = undefined;
        }
      });
    }
  }

  onQigBandSelected(band: number) {
    if (band != 0) {
      this.activeresponse.Bands.forEach((qigbandelement) => {
        if (qigbandelement.BandId == band) {
          qigbandelement.IsSelected = true;
          this.activeresponse.RecommendedBand = qigbandelement;
        } else {
          qigbandelement.IsSelected = false;
        }
      });
      this.scripresponses.forEach((qigbandscriptelement) => {
        qigbandscriptelement.RecommendedBand =
          this.activeresponse.RecommendedBand;
      });
    } else {
      this.scripresponses.forEach((qigbandscriptresp) => {
        qigbandscriptresp.RecommendedBand = undefined;
      });
    }
  }

  Previousresponse() {
    if (this.activeresponse != null) {
      this.activeresponseindex = this.scripresponses.findIndex(
        (a) => a.ProjectQnsId == this.activeresponse.ProjectQnsId
      );
      if (this.activeresponseindex > 0) {
        this.activeresponse = this.scripresponses[this.activeresponseindex - 1];
        this.getQnsResponse(this.activeresponse);
      }
    }
  }
  nextresponse() {
    if (this.activeresponse != null) {
      this.activeresponseindex = this.scripresponses.findIndex(
        (a) => a.ProjectQnsId == this.activeresponse.ProjectQnsId
      );
      if (this.activeresponseindex < this.scripresponses.length) {
        this.activeresponse = this.scripresponses[this.activeresponseindex + 1];
        this.getQnsResponse(this.activeresponse);
      }
    }
  }

  getBandName(band: IBanding): string {
    let bandname = '';
    if (band != undefined && band != null) {
      if (this.IsQigLevel) {
        bandname = band.BandName;
      } else {
        if (band?.BandFrom == 0 && band?.BandTo == 0) {
          bandname = band.BandName;
        } else {
          bandname =
            band?.BandName + '(' + band?.BandFrom + '-' + band?.BandTo + ')';
        }
      }
    }
    return bandname;
  }

  recminhide: boolean = false;
  IsRecProcessing: boolean = false;
  confirmDialog!: any;
  recommend(evnt: any) {
    let recomnullindex = this.scripresponses.findIndex(
      (a) => a.RecommendedBand == null || a.RecommendedBand == undefined
    );
    if (recomnullindex < 0) {
      this.recminhide = true;
      this.confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
        data: {
          message: this.intMessages.RecCnfirm,
        },
        panelClass: 'confirmationpop',
      });
      this.confirmDialog.afterClosed().subscribe((result: boolean) => {
        this.recminhide = false;
        if (result === true) {
          if (evnt != null) {
            this.IsRecProcessing = true;
            setTimeout(() => (this.IsRecProcessing = false), 5000);
          }
          this.recommendationService
            .recommend(
              this.recmdata.ScriptId,
              this.scripresponses,
              this.recmdata.QigId
            )
            .pipe(first())
            .subscribe({
              next: (data: any) => {
                if (data != null && data != undefined) {
                  if (data == 'SU001') {
                    this.dialogRef.close();
                    this.Alert.success(this.intMessages.RecSuccss);
                  }
                  if (data == 'UP001') {
                    this.dialogRef.close();
                    this.Alert.success(this.intMessages.RecUpdSuccss);
                  }
                  if (data == 'ALEXST') {
                    this.Alert.warning(this.intMessages.ScrAlrRecmd);
                    this.dialogRef.close();
                  }
                  if (data == 'ER001') {
                    this.Alert.error(this.intMessages.RecError);
                  }
                  if (data == 'SELBND') {
                    if (this.IsQigLevel) {
                      this.Alert.warning(this.intMessages.BndAlQigResp);
                    } else {
                      this.Alert.warning(this.intMessages.BndAlResp);
                    }
                  }
                  if (data == 'Paused') {
                    this.Alert.warning(this.intMessages.Markingprocesspaused);
                    this.dialogRef.close();
                  }
                  if (data == 'S1Comp') {
                    this.Alert.warning(this.intMessages.s1closure);
                    this.dialogRef.close();
                  }
                  if (data == 'SINCAT') {
                    this.Alert.warning(this.intMessages.ScriptTrialMarked);
                  }
                  if (data == 'ISOKP') {
                    this.Alert.warning(this.intMessages.IsNotKP);
                    this.dialogRef.close();
                  }
                } else {
                  this.Alert.error(this.intMessages.RecError);
                }
              },
              error: (a: any) => {
                throw (a);
              },
            });
        }
      });
    } else {
      if (evnt != null) {
        this.IsRecProcessing = true;
        setTimeout(() => (this.IsRecProcessing = false), 5000);
      }
      if (this.IsQigLevel) {
        this.Alert.warning(this.intMessages.BndAlQigResp);
      } else {
        this.Alert.warning(this.intMessages.BndAlResp);
      }
    }
  }
}
