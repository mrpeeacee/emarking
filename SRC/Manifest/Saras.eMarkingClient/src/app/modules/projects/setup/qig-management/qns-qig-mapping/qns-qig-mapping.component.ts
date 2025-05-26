import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { QigDetails, QigManagementModel, QigQuestionModel } from 'src/app/model/project/setup/qig-management/qig-management-model';
import { QigManagementService } from 'src/app/services/project/setup/qig-management/qig-management.service';
import { TagqigComponent } from '../tagqig/tagqig.component';
import { AlertService } from 'src/app/services/common/alert.service';

@Component({
  selector: 'emarking-qns-qig-mapping',
  templateUrl: './qns-qig-mapping.component.html',
  styleUrls: ['./qns-qig-mapping.component.css']
})

export class QnsQigMappingComponent implements OnInit {

  constructor(public dialog: MatDialog, public qigmanagementservice: QigManagementService, public Alert: AlertService) { }

  panelOpenState = false;
  Qigquestions: QigManagementModel[] = [];
  defaultqnstype: number = 3;
  Qigdetails!: QigDetails;
  Questiontxt!: QigQuestionModel;
  qnstype!: number;
  compostionloading: boolean = false;
  @ViewChild('matSelect') matSelect: any;
  FilteredQigQuestions: QigManagementModel[] = [];
  activeQig!: any;
  defaultcomposition: number = 1;
  qigtype!: number;
  remarks: string = '';

  ngOnInit(): void {
    this.compostionloading = true;
    this.Getqigquestions(this.defaultqnstype, this.defaultcomposition);
  }

  Getqigquestions(qnstype: number, qigtype: number) {
    this.compostionloading = true;
    this.qnstype = qnstype;
    this.qigtype = qigtype;
    this.qigmanagementservice.GetQigQuestions(qnstype).subscribe({
      next: (data: any) => {
        if (data.Result != null && data.Result != undefined) {
          this.Qigquestions = data.Result;
          this.FilteredQigQuestions = data.Result;
          this.GetQigs();
        }
      },
      error: (a: any) => {
        this.compostionloading = false;
        throw (a);
      },
      complete: () => {
        this.compostionloading = false;
      }
    });
  }

  GetQigs() {
    this.compostionloading = true;
    this.qigmanagementservice.getQigs(this.qigtype).subscribe({
      next: (data: any) => {
        if (data != null && data != undefined) {
          this.activeQig = data;
        }
      },
      error: (a: any) => {
        this.compostionloading = false;
        throw (a);
      },
      complete: () => {
        this.compostionloading = false;
      }
    });
  }

  getqigdetails(ProjectQigId: number) {
    this.qigmanagementservice.GetQigDetails(ProjectQigId).subscribe({
      next: (data: any) => {
        if (data.Result != null && data.Result != undefined) {
          this.Qigdetails = data.Result;
        }
      },
      error: (a: any) => {
        throw (a);
      },
    });
  }


  onQIGChange(event: any) {
    if ((event.value != null || event.value != undefined) && (event.value != 0)) {
      this.Qigquestions = this.FilteredQigQuestions.filter(a => a.ProjectQigId == event.value);
    }
    if (event.value == 0) {
      this.Qigquestions = this.FilteredQigQuestions;
    }
  }


  SaveQigdetails(qigdetails: QigDetails) {
    if (qigdetails.MandatoryQuestion == 0 && qigdetails.qigQuestions.length > 0) {
      this.Alert.warning("Mandatory question should not be zero.");
    }
    else {
      this.qigmanagementservice.SaveQigDetails(qigdetails).subscribe({
        next: (data: any) => { },
        error: (a: any) => {
          throw (a);
        },
      });
    }
  }

  ViewQuestion(ProjectQigId: number, ProjQnsId: number, templateRef: any) {
    this.qigmanagementservice.GetQuestionText(ProjectQigId, ProjQnsId).subscribe({
      next: (data: any) => {
        if (data.Result != null && data.Result != undefined) {
          this.Questiontxt = data.Result;
        }
      },
      error: (a: any) => {
        throw (a);
      },
    });

    this.dialog.open(templateRef, {
      panelClass: 'alert_class',
      width: '450px'
    });
  }

  openDialog(enterAnimationDuration: string, exitAnimationDuration: string, Qnsdetails: QigManagementModel): void {
    Qnsdetails.QigType = this.qigtype;
    Qnsdetails.QnsType = this.qnstype;
    const confirmDialog = this.dialog.open(TagqigComponent, {
      data: Qnsdetails,
      panelClass: ['tag_move', 'modal-lg', 'modal-dialog'],
      enterAnimationDuration,
      exitAnimationDuration,
    });
    confirmDialog.afterClosed().subscribe(result => {
      this.Getqigquestions(this.qnstype, this.qigtype);
    });
  }

  SaveQigQuestions(remarks: string) {
    this.Alert.clear();
  }

}
