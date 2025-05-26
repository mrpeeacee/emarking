import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router, NavigationEnd} from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { TrialmarkingService } from 'src/app/services/project/trialmarking.service';
import { SharedService } from '../shared.service';
import { IRecommedData } from 'src/app/model/standardisation/recommendation';
import { QuestionAnnotatorComponent } from '../../marking-player/question-annotator/question-annotator.component';

@Component({
  selector: 'emarking-view-script-details',
  templateUrl: './view-script-details.component.html',
  styleUrls: ['./view-script-details.component.css']
})
export class ViewScriptDetailsComponent implements OnInit {

  Loader: boolean = false; 
  objectList: any[] = [];
  
  constructor(
    public trialservice : TrialmarkingService,
    public dialog: MatDialog,   
    public translate: TranslateService,
    public commonService: CommonService,
    public router: Router,
    private sharedService: SharedService

  ) { }

  ngOnInit(): void {
    this.setTitles();
    this.loadData();
    if(this.displayList.length===0){
    const storedData = localStorage.getItem('myData');
    if(storedData)
      {
        this.objectList = JSON.parse(storedData);
      }
    }
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.update();
      }
    });
  }

  setTitles() {
    this.translate.get('View-ScriptDetails.title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate.get('View-ScriptDetails.PageDescription').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
  }

  loadData() {
      this.sharedService.objectList$.subscribe(data => {
        this.objectList = data; 
        if(this.displayList.length !==0)
          {
        localStorage.setItem('myData', JSON.stringify(this.displayList));
      }
      });
  }

  get displayList(): any[]{
    const filteredList=this.objectList.filter(obj=> !(obj.Phase==1 && obj.MarksAwarded==null));
    return filteredList.length>0 ? filteredList : this.objectList;
  }

  NavigateToMarkingPlayer(markingdata: IRecommedData){

    const dialogRef = this.dialog.open(
      QuestionAnnotatorComponent,
      {
        data: markingdata,
        panelClass: 'fullviewpop'
      }
    );    
  
  }

  GoToScriptsList(){
    this.router.navigate(['/projects/view-script']);
  }

  update(){
    localStorage.setItem('myData', JSON.stringify([]));
  }
}
