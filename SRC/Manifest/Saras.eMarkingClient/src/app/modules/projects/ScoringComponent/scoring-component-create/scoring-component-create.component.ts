import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { first } from 'rxjs';
import { ScoreComponentLibraryName, ScoringComponentDetails } from 'src/app/model/project/Scoring-Component/Scoring-Component.model';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { ScoringComponentService } from 'src/app/services/project/scoring-component/scoring-component.service';

@Component({
  selector: 'emarking-scoring-component-create',
  templateUrl: './scoring-component-create.component.html',
  styleUrls: ['./scoring-component-create.component.css']
})
export class ScoringComponentCreateComponent implements OnInit {

  ScoringComponentLibrary!:ScoreComponentLibraryName
  totalMarks: number=0;
  constructor(
    private Schemeservice: ScoringComponentService,
    public Alert: AlertService,
    public redirect: Router,
    public commonService: CommonService,
    private dialog: MatDialog,
    public translate: TranslateService,
  ) { }
  ScoringComponent: ScoreComponentLibraryName = {
    ScoreComponentId :0,
    ComponentName: '',
    ComponentCode: '',
    Marks: 0,
    ProjectID: 0,
    IsTagged: false,
    ScoringComponentDetails: [],  // Ensure this is initialized as an empty array
    IsActive: true,
    IsDeleted: false,
    CreatedBy: 0,
    //IsBandExist: false,
    IsQuestionTagged: false,
  };
  ngOnInit(): void {
    this.onAddScoringComponent()
  }


  
onAddScoringComponent(): void {
  const length = this.ScoringComponent.ScoringComponentDetails.length;
  const newComponent: ScoringComponentDetails = {
    
    ComponentDetailID:0,
    Order: length+1,
    ScoringComponentName: undefined,
    Marks: undefined,
    ComponentCode: undefined,
    IsQuestionTagged:false
};
  this.ScoringComponent.ScoringComponentDetails.push(newComponent);
  this.updateTotalMarks()
}

  onDeleteRow(index: number) {
    // Ensure the array has at least one element before attempting to delete
    if (this.ScoringComponent.ScoringComponentDetails.length > 0) {
      this.ScoringComponent.ScoringComponentDetails.splice(index, 1);
    }
    this.updateTotalMarks() ;
    this.updateIndexes();
  }
  SaveScoringCompponent() {
    debugger;
    this.updateTotalMarks()
    if (this.ScoringComponent.ComponentName.trim() === '') {
      this.Alert.warning('Please provide a valid Component Name.');
      return;
    }
    debugger;
    this.ScoringComponent.ComponentCode = this.ScoringComponent.ComponentName.trim().toUpperCase().replace(/\s+/g, '');
    for(let i=0;i< this.ScoringComponent.ScoringComponentDetails.length;i++)
      {
        if(this.ScoringComponent.ScoringComponentDetails[i].ScoringComponentName!=null &&this.ScoringComponent.ScoringComponentDetails[i].ScoringComponentName!="")
        {
        this.ScoringComponent.ScoringComponentDetails[i].ComponentCode=this.ScoringComponent.ScoringComponentDetails[i].ScoringComponentName.trim().toUpperCase().replace(/\s+/g, '')
        }
        else{
          this.Alert.warning("ComponentName is Empty")
          return;
        }
        if(this.ScoringComponent.ScoringComponentDetails[i].Marks==null)
        {
          this.Alert.warning("Marks cannot be  is Empty")
          return
        }
      }
      this.ScoringComponent.Marks=this.totalMarks
this.Schemeservice.SaveScoringCompponent(this.ScoringComponent).pipe(first())
.subscribe({
  next: (data: any) => {
   // this.Clear();
    if (data == 's001') {
      this.Alert.success("ScoreComponent library created sucessfully")
    }
    else if(data=='Exists')
    {
      this.Alert.warning("ScoreComponent library Name Already Exists")
    }
    else
    {
      this.Alert.warning('ScoreComponent Library creation Falied');
    }

  }
})
   
    console.log('Saved ScoringComponentLibrary:', this.ScoringComponent);
  }
  updateTotalMarks() {
    this.totalMarks=0
    debugger;
   

    for(let i=0;i< this.ScoringComponent.ScoringComponentDetails.length;i++)
    {
      const marks = this.ScoringComponent.ScoringComponentDetails[i].Marks;

      
      // if (!isNaN(marks)) {
        this.totalMarks += marks==undefined?0:marks;
      // }
    }
    
  }
  updateIndexes(): void {
    this.ScoringComponent.ScoringComponentDetails.forEach((component, i) => {
      component.Order = i + 1; // Reassign indices starting from 1
    });
  }
}
