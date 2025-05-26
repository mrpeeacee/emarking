import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ViewScriptModel } from 'src/app/model/project/trialmarking';
import { TrialmarkingService } from 'src/app/services/project/trialmarking.service';
import { first } from 'rxjs';
import { CommonService } from 'src/app/services/common/common.service';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { SharedService } from '../shared.service';
import { TranslateService } from '@ngx-translate/core';
import { AlertService } from 'src/app/services/common/alert.service';

@Component({
  selector: 'emarking-view-script',
  templateUrl: './view-script.component.html',
  styleUrls: ['./view-script.component.css']
})
export class ViewScriptComponent implements OnInit {

  @ViewChild('searchInput') searchInput!: ElementRef;

  ViewScripts:ViewScriptModel[]=[];
  Loader: boolean = false;
  searchString:string ="";
  scriptname:string = "";
  objViewScript :ViewScriptModel = new ViewScriptModel();
  touchedSearchBox:boolean = false;
  view:any[]=[];


  constructor(public trialservice : TrialmarkingService,
    public dialog: MatDialog,  
    public translate: TranslateService,
    private sharedService: SharedService,
    public commonService: CommonService,
    public router: Router,
    public Alert:AlertService
    ) { }

  ngOnInit(): void {
   this.setTitles();
    }
  

  setTitles() {
    this.translate.get('View-Script.title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate.get('View-Script.PageDescription').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
  }

  GetScriptDetails(scriptdata:any,type:number){
    this.touchedSearchBox = true;
    if (!this.searchString || this.searchString.trim() === '') {
      this.Alert.warning('Please enter Candidate Index Number.');
      this.searchInput.nativeElement.focus();
    } 
    else 
    
    {
    if(scriptdata == 0 && type == 0)
    {    
      this.objViewScript.LoginName=this.searchString.trim();
      this.objViewScript.ScriptID=0;
      this.objViewScript.Type=0; 
    }
    else
    {
      this.objViewScript.ScriptID=scriptdata.ScriptID;
      this.objViewScript.Type=1;
      this.objViewScript.ScriptName = scriptdata.ScriptName;
      this.objViewScript.QIGName = scriptdata.QIGName;
    }

    this.trialservice.ViewScript(this.objViewScript).pipe(first()).subscribe({
   
       next:(data:any) =>{
         this.Loader = true;
         this.ViewScripts = data;

         if(this.ViewScripts!=null && this.objViewScript.ScriptID !=0 && this.objViewScript.Type == 1){
           this.nextpagedata();
         }
       },
       error:(err:any) =>{
         throw err;
       },
       complete:()=>{
         this.Loader = false;
       }
    });    
  } 
 }

  nextpagedata(){ 
    this.sharedService.setObjectList(this.ViewScripts);
  }
}
