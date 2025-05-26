import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Filedetails } from 'src/app/model/project/mark-scheme/mark-scheme-model';
import { ScoreComponentLibraryName } from 'src/app/model/project/Scoring-Component/Scoring-Component.model';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { ScoringComponentService } from 'src/app/services/project/scoring-component/scoring-component.service';

@Component({
  selector: 'emarking-view-scoring-component',
  templateUrl: './view-scoring-component.component.html',
  styleUrls: ['./view-scoring-component.component.css']
})
export class ViewScoringComponentComponent implements OnInit {
  ScoreComponentLibrary!:ScoreComponentLibraryName
  ScoreComponentID:any
  IsfromQig: boolean=false;
 
  constructor(
    @Inject(MAT_DIALOG_DATA) public data:any, 
    public dialogRef: MatDialogRef<ViewScoringComponentComponent>,private dialog: MatDialog,
    public translate: TranslateService,
    public Alert: AlertService,
    public commonService: CommonService,
    private router: ActivatedRoute,
    private Schemeservice:ScoringComponentService
  ) { }
 // @ViewChild('htmlData', { static: false }) htmlData?: ElementRef;
  ngOnInit(): void {
    this.translate.get('viewsch.title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });

    this.translate.get('viewsch.pgedesc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
    this.ScoreComponentID = this.router.snapshot.params['ComponentId'];
   
  
    if(this.data.IsfromQig==true)
    {
      this.IsfromQig=this.data.IsfromQig
      this.ScoreComponentID=this.data.markschemeid
    }
    this.ViewScoringComponentLibrary();

    
  }

  ViewScoringComponentLibrary()
  {
    debugger;
    this.Schemeservice.ViewScoringComponentLibrary(this.ScoreComponentID).subscribe((data) => {
     
     
      this.ScoreComponentLibrary =data
      console.log(this.ScoreComponentLibrary)
    });

  }

  clickMethod(evnt: any) {
   
    this.dialogRef.close({ status: 0 });
  }

}
