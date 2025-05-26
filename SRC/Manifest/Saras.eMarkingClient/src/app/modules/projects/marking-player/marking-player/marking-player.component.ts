import { Component, OnInit, ViewChild } from '@angular/core';

import { ScriptQuestionModel } from 'src/app/model/project/scriptquestion';
import { ResponseDetails, Scorecompnentdetails, QuestionScoreComponentMarkingDetail, Band } from 'src/app/model/project/trialmarking';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { TrialmarkingService } from 'src/app/services/project/trialmarking.service';
import html2canvas from 'html2canvas';
import { PerfectScrollbarComponent } from 'ngx-perfect-scrollbar';
import { first } from 'rxjs/operators';
import { QuestionAnnotatorComponent } from '../question-annotator/question-annotator.component';
import { MatDialog } from '@angular/material/dialog';
import { ViewDownloadMarksSchemeComponent } from '../view-download-marks-scheme/view-download-marks-scheme.component';
import { ViewCandidateResponseComponent } from '../view-candidate-response/view-candidate-response.component';



declare const annotateimg: any;
declare const Loadimg: any;
declare const ClearAll: any;
declare const fnSaveItemAnnotation: any;

declare const drawpreviousobject: any;

declare const DisableTool: any



@Component({
  selector: 'emarking-marking-player',
  templateUrl: './marking-player.component.html',
  styleUrls: ['./marking-player.component.css'],
 
})
export class MarkingPlayerComponent implements OnInit {
  @ViewChild('perfectScroll') perfectScroll!: PerfectScrollbarComponent;
  @ViewChild('perfectScrollp') perfectScrollp!: PerfectScrollbarComponent;
  @ViewChild('perfectScrollq') perfectScrollq!: PerfectScrollbarComponent;
  @ViewChild('perfectScrolla') perfectScrolla!: PerfectScrollbarComponent;

  public questiondet: any[] = [];
  _comment: any;
  _hdrawdata: any;

  isauto:boolean = false;
  _QuestionScoreComponentResponseDetails! : QuestionScoreComponentMarkingDetail[];
  _selectedid: number = 0;
  Group1A: any[] = [];
  _userid?:number;
  _stasticsloading: boolean = true;
  isDisabled: boolean = false;
 
  isDisabledband: boolean = false;
  isDisabledsc:boolean =false;
  isDisabledsubmit : boolean =false;
  isDisabledSave: boolean = false;
  _projectquestionid?:number;
  discrete?:boolean = false;
  Group1B:any [] = []; 
  isview?:boolean = false;
  _QuestionScoreComponentMarkingDetailmarking!:QuestionScoreComponentMarkingDetail[];
  Group2: any[] = [];
  Group3: any[] = [];
  _responseText?:string ;
  _stepvalue!: number ;
  deptment: any;
  correctanswer?:string;
  questiontype?:any;
 _wordcount?:number;
  readmore?:boolean = false;
  responseChinese!: string;
  matches!: RegExpMatchArray | null;
  element!: HTMLElement | null;
  length1!: number;
  constructor(
    public trialmarkingService: TrialmarkingService,
    public _QuestionsAnnotatorComponent: QuestionAnnotatorComponent,
    public router: Router,
    private route: ActivatedRoute,
    public commonService: CommonService,
    private dialog: MatDialog,
    public Alert: AlertService) {
  }
  public _IsScoreComponentExists?:boolean | null;
  public _banddet: any[] = [];
  public Remarks:any;
  public prjquest!: ScriptQuestionModel[];
  public ResponseD!: ResponseDetails;
  public scorecompnentdetail1!: Scorecompnentdetails[];
  public scorecompnentde!: Scorecompnentdetails[];
  public scorecompnentBand?: Scorecompnentdetails[];
  public Band!:Band[];
  public BandT!:Band[];
  recminhide: boolean = false;
  disaplepassage: boolean = false;
  ProjectID!: number;
  questiontext?: string;
  PassageText!:string;
  PassageText2!:string;
  ProjectUserRoleId?: string;
  annotationpath?:any;
  maxmarks_score?: number;
  Createdby!: number;
  UserID!: number;
  disableannotations:boolean = false;
  max_score?: number;
  _marksschemeexist?:boolean;
  _IsScoreComponentbandExists?:boolean = false;
  _marksschmemesubmit?:boolean;
  score_assigned?:any;
  min_score?: any = 0;
  score_awarded?: any = null;
  isShown = false;
  public _IsTestPlayerView?:boolean | null;
  ngOnInit(): void {
    this.UserID = 0;
    var ghid = document.getElementsByTagName("mat-dialog-container");
    ghid[0].setAttribute("style", "overflow:hidden");
    
    annotateimg();
    this.disableannotation();
    
  }
  
  alertdeleteannotate(){
    this.Alert.warning("Deleting this annotaion will make awarded score negative or exceed the max. marks");
  }
  alertannotate(){
    this.Alert.warning("selected annotation marks will exceed the max. marks or subceeds zero");
  }
  Readmore(event:any){
   
    var det = document.getElementById("qtext") as HTMLAnchorElement;
    if(event.srcElement.innerHTML.trim() == "Read More...")
    {
      event.srcElement.innerHTML = "Read Less";
      det.removeAttribute("style");
    }
    else
    {
      event.srcElement.innerHTML = "Read More...";
      det.setAttribute("style","height:35px;padding-top:9px");
   
    }
    
  }
 
  getminmaxno(event: any) {
    this._QuestionsAnnotatorComponent._change = true;
    const value = event.target.value;
    if (value != '-1') {
      
      var banddet = this._banddet.filter((k) => k.id == value)[0];
      this.min_score = banddet.BandFrom;
      this.max_score = banddet.BandTo;
      this.score_awarded = "";
      
    } else {
      this.min_score = "";
      this.max_score = this.maxmarks_score;
      this.score_awarded = this.min_score;
    }
    this.score_assigned =  this.score_awarded;
  }
  disableannotation() {
    
    if (this.isDisabled || this._QuestionsAnnotatorComponent._isviewmode) {
      DisableTool();
      this.disableannotations = true;
      this.isDisabledsc = true;
      this.isauto = true;
    }
  }
   readBase64(file:any): Promise<any> {
    const reader = new FileReader();
    return  new Promise((resolve, reject) => {
      reader.addEventListener('load', function () {
        resolve(reader.result!);
      }, false);
      reader.addEventListener('error', function (event) {
        reject(event);
      }, false);
    
      reader.readAsDataURL(file);
    });
   
  }
  getannotations()
  {
    
    this.trialmarkingService.Getannoatationdetails(this._QuestionsAnnotatorComponent.data.QigId)
            .pipe(first())
            .subscribe({
              next: (data: any) => {
                if (data) {
              
              for ( let row of data) {
                var det = {
                  Path: row.Path.replace("annotation_icons", this.annotationpath),
                  AnnotationToolName: row.AnnotationToolName,
                  AssociatedMark: row.AssociatedMark
                };
                
                if(row.AnnotationGroupName == "GROUP 1")
                {
                  if((det.Path.indexOf('h_wavy') > -1) ||  (det.Path.indexOf('offpage_comment') > -1) || (det.Path.indexOf('onpage_comment') > -1)  || (det.Path.indexOf('ellipse') > -1) || (det.Path.indexOf('v_wavy') > -1) || (det.Path.indexOf('arrow') > -1) || (det.Path.indexOf('highlight') > -1))
                  {
                    if((det.Path.indexOf('offpage_comment') <= -1))
                    {
                    this.Group1B.push(det);
                    }

                  }
                  else
                  {
                  this.Group1A.push(det);
                  }
                 

                }
                else if (row.AnnotationGroupName == "GROUP 2")
                {
                  this.Group2.push(det);

                }

                else if (row.AnnotationGroupName == "GROUP 3")
                {
                  this.Group3.push(det);

                }
              
                
              }
              
              
                }
                
                this.checkconfiguration();
              },
              error: (a: any) => {
                throw(a);
              },
              
            });


    

  }
  Clearalldata() {
    
    ClearAll();
  }

  onsaveclick(bandvalue: any): string {
   
    return fnSaveItemAnnotation(bandvalue);
  }
  checkconfiguration()
  {
    if(this.Group2.length == 0 && this.discrete )
              {
               
                 this.Alert.warning("Discrete annotations are not configured, Please contact admin");
               
              }
  }
  GetUserScheduleDetails()
  {

    this.trialmarkingService.GetUserScheduleDetails(this._userid!).subscribe({
      next: (data: any) => {
        
        var dat=data;
        var url= dat.Url +"Solutions.aspx?solution="+dat.Solution;
        url = url+"&P2="+dat.AssessmentId;
        url=url+"&SUID="+dat.ScheduleUserId;
        url=url+"&SECID="+dat.SectionId;
        url=url+"&uid="+dat.UserId;
        url=url+"&Theme="+dat.Theme;
        url=url+"&SumType="+dat.SumType;
        url=url+"&TestType="+dat.TestType;
        url=url+"&Page="+dat.Page;
        url=url+"&culture="+dat.culture;
        url=url+"&Key="+dat.Key;
        url=url+"&TestMode="+dat.TestMode;
        url=url+"&TimeStamp="+dat.TimeStamp;
   
        Object.assign(document.createElement('a'), {
          target: '_blank',
          rel: 'noopener noreferrer',
          href: url,
        }).click();
       
      },
      error: (a: any) => {
        throw a;
      },
      
    });
  }
  updatedchange(){

    this._QuestionsAnnotatorComponent._change = true;
    this._QuestionsAnnotatorComponent.isDisabledsubmit = true;
  }
  escapeRegExp(string:any) {
    return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'); // $& means the whole matched string
  }
  converttocanvas(response: any) {

  var iframe = document.createElement('iframe');
    iframe.style.width = "1000px";
    iframe.style.maxWidth = "1000px";
    iframe.style.minWidth = "1000px";
    document.body.appendChild(iframe);

    window.setTimeout(() => {
      var iframedoc: any;
      iframedoc = iframe.contentDocument || iframe.contentWindow!.document;
 
      let divele: any = <HTMLElement>document.createElement("div");
      divele.style.minWidth = "1000px";
      divele.style.maxWidth = "1000px";

      iframedoc.body.append(divele);
      iframedoc.body.style.fontSize = "18";
      iframedoc.body.style.fontFamily = "Times New Roman";
     
      var hdraimg = this._hdrawdata;
      var comments = this._comment;

      var ridn = document.createElement('div') as HTMLElement;

      ridn.style.maxWidth = "800px";
      ridn.style.fontSize = "15";
      ridn.style.fontFamily = "Times New Roman";
     
      ridn.innerHTML = response;
      
      const elementsInsideDiv = ridn.querySelectorAll('*');
    elementsInsideDiv.forEach(function(element) {
   
       if((element as HTMLElement).style.backgroundColor.match(/^(?:white|#fff(?:fff)?|rgba?\(\s*255\s*,\s*255\s*,\s*255\s*(?:,\s*1\s*)?\))$/i))
       { 
        (element as HTMLElement).style.backgroundColor = "";

        }
       
    });
      
      iframedoc.body.append(ridn);
      var color =  this._QuestionsAnnotatorComponent.annotationcolor;
      
   
      this.responseChinese=ridn.innerText.trim()
     
       this.matches = this.responseChinese.match(/[\u4e00-\u9fa5]/g);
if (this.matches && this.matches.length > 0) {
  this._wordcount=this.matches.length
} else {
  this._wordcount = ridn.innerText.trim().split(/\s+/).length ;
}
      
      var isaut = this.isauto;
     
     html2canvas(iframedoc.body,{ scale: 1,useCORS: true }
      ).then(function (canvas) {
        
        Loadimg(canvas.toDataURL(), hdraimg, comments, isaut,color);
        drawpreviousobject(hdraimg);
    (document.getElementById("colorid") as HTMLLabelElement).innerHTML = color;
        var iframes: any = document!.querySelectorAll('iframe');
        for (let i = 0; i < iframes.length; i++) {
          iframes[i].parentNode.removeChild(iframes[i]);
        }
        
      });
      if(this._QuestionScoreComponentResponseDetails.length > 0)
      {
        
        for(let n=0; n< this._QuestionScoreComponentResponseDetails.length; n++)
        {
          let awmar = document.getElementById(this._QuestionScoreComponentResponseDetails[n].ScoreComponentId + "I") as HTMLInputElement;

          if(this._QuestionScoreComponentResponseDetails[n].AwardedMarks != null)
          {
          awmar.value = this._QuestionScoreComponentResponseDetails[n].AwardedMarks!.toString();
          }
          let seletedele = document.getElementById(this._QuestionScoreComponentResponseDetails[n].ScoreComponentId + "S") as HTMLSelectElement;
          if(this._QuestionScoreComponentResponseDetails[n].BandId != null)
          {
          seletedele.value = this._QuestionScoreComponentResponseDetails[n].BandId!.toString();
          if(seletedele.value == "")
          {
            seletedele.value = "-1";
          }
          this.scorecompnentde = this.scorecompnentdetail1.filter(k => k.ScoreComponentId == this._QuestionScoreComponentResponseDetails[n].ScoreComponentId );
         

         if(this.scorecompnentde[0].band.length > 0)
         {
         var bandd = this.scorecompnentde[0].band.filter(l => l.BandId == this._QuestionScoreComponentResponseDetails[n].BandId!);
         var bani = this._QuestionScoreComponentResponseDetails[n].ScoreComponentId.toString() + 'I';
         let dynband = document.getElementById(bani) as HTMLInputElement;
         dynband.min = bandd[0].BandFrom.toString();
         dynband.max = bandd[0].BandTo.toString();
          
         }
        }
        }
      }
      this._QuestionsAnnotatorComponent.isquestiondet = false;
      this._QuestionsAnnotatorComponent.stasticsloading = false;
      this._stasticsloading = false;
      this._QuestionsAnnotatorComponent.Seticonelement();
     this._QuestionsAnnotatorComponent.checksubmit();
     
      
      if(this._QuestionsAnnotatorComponent._isviewmode)
      {
        this.disableannotation();
      }
    }, 300);
  if(this.isDisabledSave)
  {
    this.isDisabled = true;
    this.isDisabledband = true;
    this.isDisabledsc = true;
    this._IsScoreComponentbandExists = true;
    window.setTimeout(() => { this.disableannotation()} , 300);
  }
  
  }
  onChangekey($event:any){
    this._QuestionsAnnotatorComponent._change = true;
    if(($event.currentTarget.value != null) && ($event.currentTarget.value != ""))
    {
    if((+$event.currentTarget.value) > +$event.currentTarget.max)
    {
      $event.currentTarget.value = $event.currentTarget.max;
      this.score_awarded = $event.currentTarget.max;
      
    }
   else if((+$event.currentTarget.value) < +$event.currentTarget.min)
    {
      var x = $event.currentTarget.value.substring(0, 1);
      var y = $event.currentTarget.max.substring(0, 1);
      if(($event.currentTarget.max.length == $event.currentTarget.value.length) || (x != y)  )
      {
        $event.currentTarget.value = $event.currentTarget.min;
        this.score_awarded = $event.currentTarget.min;
      }
     
    }
    this._QuestionsAnnotatorComponent.isDisabledsubmit = true;
    
    if(this._IsScoreComponentExists)
    {
     let tmarks = 0 ;
    let gst = document.getElementsByName("compmarks") ;
   
  if(gst != null)
   {
    
     for (let k =0; k< gst.length ; k++){
   
       var mval = parseFloat((gst[k] as HTMLInputElement).value)
       if(mval.toString() == 'NaN')
       {
        mval = 0;
       }
       
      tmarks =  tmarks  + mval ;
     }

   }
   if((tmarks % this._stepvalue) == 0 )
   {
    this.score_assigned = tmarks;
    
   }
   else
   {
    $event.currentTarget.value = "";
    this.Alert.warning("Score should be in multiples of " + this._stepvalue  );
   }
   
  }
  else
  {
    if((this.score_awarded) > + $event.currentTarget.max)
    {
      this.score_awarded = $event.currentTarget.max;
      
    }
   else if((this.score_awarded) < +$event.currentTarget.min)
    {
      var x1 = this.score_awarded.toString().substring(0, 1);
      var y1 = $event.currentTarget.max.substring(0, 1);
   
      if(($event.currentTarget.max.length == this.score_awarded.length) || (x1 != y1))
      {
        this.score_awarded = $event.currentTarget.min;
      }
     
    }
    if( (this.score_awarded % this._stepvalue) == 0 )
    {
      this.score_assigned = this.score_awarded;
    }
    else
    {
      $event.currentTarget.value = "";
      this.Alert.warning("Score should be in multiples of " + this._stepvalue  );
    }
    
  }
}
else
{
  let tmarks = 0 ;
  let gst = document.getElementsByName("compmarks") ;
  if(gst != null)
   {
    
     for (let k =0; k< gst.length ; k++){
   
       var mvaln = parseFloat((gst[k] as HTMLInputElement).value)
       if(mvaln.toString() == 'NaN')
       {
        mvaln = 0;
       }
       
      tmarks =  tmarks  + mvaln ;
     }

   }
   if((tmarks % this._stepvalue) == 0 )
   {
    this.score_assigned = tmarks;
   }
   if(tmarks == 0)
   {
    this.score_assigned = null;
    
   }
}

}
  onChange($event:any){

    var text = document.getElementById($event.currentTarget.id) as HTMLSelectElement;

    if(text){
      text.classList.remove('red-border');
    }


    this._QuestionsAnnotatorComponent._change = true;
    var ddldet = document.getElementById('_banddet') as HTMLSelectElement;
    if(($event.currentTarget.value != null) && ($event.currentTarget.value != ""))
    {
    if((+$event.currentTarget.value) > +$event.currentTarget.max)
    {
      var text = document.getElementById($event.currentTarget.id) as HTMLSelectElement;

      if(text){
        text.focus();
        text.style.outline='none';
        text.classList.add('red-border');
      }

      ddldet != null ?
      this.Alert.warning(
        'Scores must fall within the specified band range. Score Awarded: ' + $event.currentTarget.value
      ) : this.Alert.warning(
        'Awarded marks cannot be empty and must be within the specified range. Marks Awarded: ' + $event.currentTarget.value
      );
      $event.currentTarget.value = "";
      
    }
   else if((+$event.currentTarget.value) < +$event.currentTarget.min)
    {
      var text = document.getElementById($event.currentTarget.id) as HTMLSelectElement;

      if(text){
        text.focus();
        text.style.outline='none';
        text.classList.add('red-border');
      }

      ddldet != null ?
      this.Alert.warning(
        'Scores must fall within the specified band range. Score Awarded: ' + $event.currentTarget.value
      ) : this.Alert.warning(
        'Awarded marks cannot be empty and must be within the specified range. Marks Awarded: ' + $event.currentTarget.value
      );
      $event.currentTarget.value = "";
    }
    this._QuestionsAnnotatorComponent.isDisabledsubmit = true;
    
    if(this._IsScoreComponentExists)
    {
     let tmarks = 0 ;
    let gst = document.getElementsByName("compmarks") ;
   
  if(gst != null)
   {
    
     for (let k =0; k< gst.length ; k++){
   
       var mval = parseFloat((gst[k] as HTMLInputElement).value)
       if(mval.toString() == 'NaN')
       {
        mval = 0;
       }
       
      tmarks =  tmarks  + mval ;
     }

   }
   
   if((tmarks % this._stepvalue) == 0 )
   {
    this.score_assigned = tmarks;
   }
   else
   {
    
    this.Alert.warning("Score should be in multiples of " + this._stepvalue  );
    const scband_id = $event.currentTarget.id.slice(0, -1) + 'S'
    var ddldetsc = document.getElementById(scband_id) as HTMLSelectElement;
      if(ddldetsc.value != null)
      {
        ddldetsc.value = "-1";
      }
   }
   
  }
  else
  {
 
    if( (this.score_awarded % this._stepvalue) == 0 )
    {
      $event.currentTarget.value == '' ? this.score_assigned = null : this.score_assigned = this.score_awarded;
    }
    else
    {
      
      this.Alert.warning("Score should be in multiples of " + this._stepvalue  );
     
      
      if(ddldet.value != null)
      {
        ddldet.value = "-1";
      }
      
     
    }
    
  }
 
}
else
    {
      if(!this._IsScoreComponentExists)
      {
        this.score_assigned = null;
      }
      else
      {
        const scband_idn = $event.currentTarget.id.slice(0, -1) + 'S'
        var ddldetsc1 = document.getElementById(scband_idn) as HTMLSelectElement;

      if(ddldetsc1.value != null)
      {
        ddldetsc1.value = "-1";
      }
      }
     
    }
   
}
  onSave()
  {
    
    if(!this.isDisabledSave)
    {
      this._QuestionsAnnotatorComponent.isquestiondet = true;
      this._QuestionsAnnotatorComponent.stasticsloading = true;
      this._stasticsloading = true;
      this._QuestionsAnnotatorComponent.savemarking();
   
    }
  }
  viewmarksscheme()
  {
    
    const editorDialog = this.dialog.open(ViewDownloadMarksSchemeComponent, {
      data: {
        projectquestionid: this._projectquestionid,
        questionname: "markScheme"
       
      },
      panelClass: 'fullviewpop',
    });
    editorDialog.afterClosed().subscribe();
  
  }

  viewresponse()
  {    
    const editorDialog = this.dialog.open(ViewCandidateResponseComponent, {
      data: {
        _scheduleid: this._QuestionsAnnotatorComponent._scriptid,
      },
      panelClass: 'fullviewpop',
    });
    editorDialog.afterClosed().subscribe();  
  }

  getminmaxnoscorecomponent(event: any){
    this._QuestionsAnnotatorComponent._change = true;
    const value = event.target.id;
    var checkrow = value.slice(0, -1) ;
    this.scorecompnentde = this.scorecompnentdetail1.filter(k => k.ScoreComponentId == checkrow);
    const bandvalue = event.target.value;

    if(bandvalue != "-1")
    {
      
      var bamddet = this.scorecompnentde[0].band.filter(k => k.BandId ==  bandvalue && k.BandName == event.target.selectedOptions[0].text );
      var bani = value.slice(0, -1) + 'I';
      let dynband = document.getElementById(bani) as HTMLInputElement;
      dynband.min = bamddet[0].BandFrom.toString();
      dynband.max = bamddet[0].BandTo.toString();
      dynband.value = '';
  

      if((event.target.value) > event.target.max)
        {
          this.Alert.warning(
            'Scores must fall within the specified band range. Score Awarded: ' + event.target.value
          );
        event.target.value = "";
          
        }
       else if((event.target.value) < event.target.min)
        {
          this.Alert.warning(
            'Scores must fall within the specified band range. Score Awarded: ' + event.target.value
          );
          event.target.value = "";
        }
    }
    else
    {
      var banid = value.slice(0, -1) + 'I';
      let dynband = document.getElementById(banid) as HTMLInputElement;
      dynband.min = "0";
      dynband.max = this.scorecompnentde[0].MaxMarks.toString();
      dynband.value = "";
    }
    let tmarks = 0 ;
    let gst = document.getElementsByName("compmarks") ;
   
    
   if(gst != null)
   {
    
    for (let k =0; k< gst.length ; k++){
   
      var mval = parseFloat((gst[k] as HTMLInputElement).value)
      if(mval.toString() == 'NaN')
      {
       mval = 0;
      }
     tmarks =  tmarks  + mval ;
    }

   }
   this.score_assigned = tmarks;

  }
  getquestionresponsetext(id: number, scriptid: number, ProjectQuestionId: number, userscriptmarkingrefid?:number) {

    var det = document.getElementById("qtext") as HTMLAnchorElement;
    det.setAttribute("style","height:35px;padding-top:12px");
    
    var detrm = document.getElementById("readmre") as HTMLElement;
    if(detrm != null)
    {
    detrm.innerHTML = "Read More..." ;
    }
    this.perfectScroll.directiveRef!.update(); 
    this.perfectScroll.directiveRef!.scrollToTop(0, 0);
    this.perfectScroll.directiveRef!.scrollToLeft(0, 0);

    // this.perfectScrollp.directiveRef!.update(); 
    // this.perfectScrollp.directiveRef!.scrollToTop(0, 0);
    // this.perfectScrollp.directiveRef!.scrollToLeft(0, 0);

    this.perfectScrollq.directiveRef!.update(); 
    this.perfectScrollq.directiveRef!.scrollToTop(0, 0);
    this.perfectScrollq.directiveRef!.scrollToLeft(0, 0);

    this.perfectScrolla.directiveRef!.update(); 
    this.perfectScrolla.directiveRef!.scrollToTop(0, 0);
    this.perfectScrolla.directiveRef!.scrollToLeft(0, 0);


    
    var colps = document.getElementById("flush-collapseOne") as HTMLElement;
    colps.className = "accordion-collapse collapse";

    this._comment = "";
    this._selectedid = -1;
    this._hdrawdata = "";
    this._QuestionScoreComponentResponseDetails = [];
    this._IsScoreComponentExists = null;
    this._projectquestionid = ProjectQuestionId;
    this._IsTestPlayerView = null;

    this.trialmarkingService.ResponseMarkingDetails(scriptid, id, this._QuestionsAnnotatorComponent._markedby, this._QuestionsAnnotatorComponent._workflowstatusid, userscriptmarkingrefid)
      .pipe(first())
      .subscribe({
        next: (data1: any) => {
          if (data1 != null) {
            this._hdrawdata = "";
            this._comment = "";
            this._selectedid = data1.BandId;
            
            if(data1.BandId != null)
            {
            this._selectedid = data1.BandId;
            }
           
            this.score_awarded = data1.Marks;
            this.score_assigned = data1.Marks;
            this._hdrawdata = data1.Annotation;
            this._comment = data1.Comments;
            this.Remarks = data1.Remarks;
            if(data1.QuestionScoreComponentMarkingDetails != null)
            {
              this._QuestionScoreComponentResponseDetails = data1.QuestionScoreComponentMarkingDetails;
            }
            
          }
          this.trialmarkingService.GetquestionResponeText(scriptid, ProjectQuestionId)
            .pipe(first())
            .subscribe({
              next: (data: any) => {
               this._IsTestPlayerView = data.TestPlayerView;
               this._responseText = data.ResponseText;
               this._QuestionsAnnotatorComponent._markedtype = data.Markedtype;
               this._stepvalue = data.StepValue;
               if((this._stepvalue == null) ||( this._stepvalue == undefined)|| (this._stepvalue == 0))
               {
                 this._stepvalue  = 1;
                 
               }
               if(data.QuestionType == 20 || data.QuestionType == 152)
                  {
                    this._stepvalue  = 0.5;
                  }
               this._userid = data.UserID;
               this.questiontype = data.QuestionType;
               
                if(data.IsScoreComponentExists)
                {
                  
                  this._IsScoreComponentbandExists = false;
                  this._marksschemeexist = false;
                  this._IsScoreComponentExists = true;
                  this.isDisabled = true;
                  this.isDisabledband = true;

                 
                  this._QuestionsAnnotatorComponent.scorecompnentdetail = [];
                  this.scorecompnentdetail1 = [];
                  this.ResponseD = data;
                  if(this.ResponseD.Bands != null)
                  {
                  var ScoCompoId = this.ResponseD.Bands.map(item => item.ScoreComponentId)
                     .filter((value, index, self) => self.indexOf(value) === index)
                     var disable = true;
                  for(let s=0; s<ScoCompoId.length ; s++)
                  {
                    this.Band = [];
                  
                    var sccompandband = this.ResponseD.Bands.filter((itm:any) => itm.ScoreComponentId == ScoCompoId[s] )
                    
                     for(let k= 0 ; k< sccompandband.length; k++)
                     {

                      if(sccompandband[k].BandName != null)
                      {
                       this.BandT = [
                         {
                         
                           BandFrom:sccompandband[k].BandFrom,
                           BandId : sccompandband[k].BandId,
                           BandName : sccompandband[k].BandName + " " + "(" + sccompandband[k].BandFrom + "-" + sccompandband[k].BandTo  + ")",
                           BandTo : sccompandband[k].BandTo,
                           IsSelected : false
                         }

                       ]
                       this.Band.push(this.BandT[0]);
                      }

                     }
                     for(let n = 0; n< this.Band.length ; n++)
                     {
                       if(this.Band[n].BandId != 0)
                       {
                        disable = false;
                       }
                     }
                     
                    this.scorecompnentBand = [
                      {
                        ScoreComponentId : sccompandband[0].ScoreComponentId,
                        ComponentCode : sccompandband[0].ComponentCode,
                        ComponentName : sccompandband[0].ComponentName,
                        MaxMarks:sccompandband[0].MaxMarks,
                        band: this.Band
                      }
                    ]
                   
                   this.scorecompnentdetail1.push(this.scorecompnentBand[0]);
                  }
                  if(disable)
                  {

                    this._IsScoreComponentbandExists = true;

                  }
                  
                }
  
                }
                else
                {

                  this._IsScoreComponentbandExists = false;
                  this.isDisabled = false;
                  
                 this._IsScoreComponentExists = false;
                  
                this._banddet = [];
                
                for (let i = 0; i < data.Bands.length; i++) {

                  var det1 =
                    { "id": data.Bands[i].BandId, "BandFrom": data.Bands[i].BandFrom, "BandTo": data.Bands[i].BandTo, "name": data.Bands[i].BandName + " " + "(" + data.Bands[i].BandFrom + "-" + data.Bands[i].BandTo + ")" };
                  this._banddet.push(det1);

                } 
                if(this._banddet.length != 0)
                {
                var Gfromto = this._banddet.find(k => k.id == this._selectedid);
                if(Gfromto != undefined)
                {
                if (Gfromto.id != -1) {
                  this.min_score = Gfromto.BandFrom;
                  this.max_score = Gfromto.BandTo;
                }
              }
              }
              }
              
              (document.getElementById("questiontext") as HTMLSpanElement).innerHTML =  "<strong>Question:</strong>" + data.QuestionText;

              if(data.QuestionGUID != null)
                {
                  if(data.QuestionType == 20 || data.QuestionType == 152)
                  {
                    this.correctanswer = "Correct answer : " + data.Correctanswers ;       
                  this.readmore = true;
                  }
            
            if(data.Markedtype != 3 && data.Markedtype != null )
            {
        
               this.isDisabled = true;
               this.isDisabledband = true;
               this.isauto = true;
              (document.getElementById("disableNR") as HTMLDivElement).className = "mydisable";
               this.isDisabledsc = true;
               this.isDisabledSave = true;
               this.isDisabledsc = true;
               this.score_assigned = data.FinalizedMarks;
               
               if(data.Markedtype == 4)
               {
                 
                 this.Alert.warning("The discrepancy for this blank is resolved , Hence This cannot be edited.");
               }
            }
            else if(!this._QuestionsAnnotatorComponent._isviewmode)
            {
              this.isDisabled = false;
               this.isDisabledband = false;
               this.isDisabledSave = false;
               this.isDisabledsc = false;
               this.isauto = false;

               (document.getElementById("disableNR") as HTMLDivElement).className = "";
            }
            else if(this._QuestionsAnnotatorComponent._isviewmode)
            {
              (document.getElementById("disableNR") as HTMLDivElement).className = "";
            }
            
              
            }
             
        this.PassageText = data.PassageText;  
        this.PassageText2 = this.PassageText;

                if(data.Bands.length > 0)
                {
                  this._marksschemeexist = true;
                  this.isDisabledband = false;
                }
                else
                {
                  this._marksschemeexist = false;
                  this.isDisabledband = true;
                }
                
                if(data.PassageText == "" || data.PassageText == null )
                {
                  this.disaplepassage = true;
                 
                }
                else
                {
                  this.disaplepassage = false;
                }
   
                this.element = document.getElementById("questiontext") as HTMLElement | null;

                if (this.element) {
                     this.length1 = this.element.innerHTML.length;
                    
                }
                                if((document.getElementById("questiontext") as HTMLElement).innerHTML.length > 130 || data.QuestionText.includes("audio") || data.QuestionText.includes("video") || data.QuestionText.includes("img") || !this.disaplepassage)
                               {
                
                                this.readmore = true;
                               } 
                               //else if ( (document.getElementById("questiontext").innerHTML.length < 130) && (document.getElementById("questionText") as HTMLElement).innerHTML.length>0 )
                               // {
                                 //var det = document.getElementById("qtext") as HTMLAnchorElement;
                                // det.removeAttribute("style");
                                //this.readmore = true;
                                //}
                                else if(this.length1<=130)
                {
                  var det = document.getElementById("qtext") as HTMLAnchorElement;
                   det.removeAttribute("style");
                   this.readmore = false;
                } 
               else
               {
                this.readmore = false;
               }
                if(data.ResponseText == null || data.ResponseText == "-No Response(NR)-" ||  data.IsNullResponse)
                {

                       data.ResponseText = "-No Response(NR)-";
                       (document.getElementById("disableNR") as HTMLDivElement).className = "mydisable";
                       this.isDisabled = true;
                       this.isDisabledband = true;
                       this.isauto = true;
                       this.disableannotations = true;
                       this.isDisabledsc = true;
                       this.isDisabledSave = true;
                       this.isDisabledsc = true;
                       this.score_assigned = data.FinalizedMarks;
                     
                      
                }
                else
                {
                  var click = document.getElementById("Selector")!.hasAttribute("onclick");
                  if(!click)
                  {
                    document.getElementById("Selector")!.setAttribute("onclick", "ToolSelected(this,event);return false");
                    document.getElementById("deleteall")!.setAttribute("onclick", "ClearAllManual(event);return false");
                    document.getElementById("eraser")!.setAttribute("onclick", "RemoveSelected(this,event);return false");
                  }
                 }

                this.converttocanvas(data.ResponseText!);
              },
              error: (a: any) => {
                throw(a);
              },
            
            });
        },
        error: (a: any) => {
          throw(a);
        },
      
      });
  }

  loadcf(){
     
    var stimulusDiv = document.getElementById('innerpassage');
        if (stimulusDiv != null) {
          stimulusDiv.innerHTML = "";
          var newTile = document.createElement("div");
          newTile.setAttribute("class", "stimuluspopup");
          newTile.innerHTML = this.PassageText2.replace("<![CDATA[","").replace("]]>","");
          stimulusDiv.appendChild(newTile);
          setTimeout(() => {
            if (stimulusDiv != null) {
              stimulusDiv.appendChild(newTile);
            
            }
          }, 200);
          
          
        }
  }
}

