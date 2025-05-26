import { Component, OnInit, Inject } from '@angular/core';
import { UserManagementService } from 'src/app/services/project/setup/user-management.service';
import { first } from 'rxjs';
import { BlankQigIds, CopyMarkingTeamCls, MoveMarkingTeamToEmptyQigs } from 'src/app/model/project/setup/user-management';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Validators } from 'ngx-editor';
import { TranslateService } from '@ngx-translate/core';
import { AlertService } from 'src/app/services/common/alert.service';

@Component({
  selector: 'emarking-move-marking-team',
  templateUrl: './move-marking-team.component.html',
  styleUrls: ['./move-marking-team.component.css']
})
export class MoveMarkingTeamComponent implements OnInit {
  LtEmptyQigIds: BlankQigIds[] = [];
  IsLoading: boolean = false;
  CopyForm!: FormGroup;
  dataloading: boolean = false;
  constructor(public usermanagement: UserManagementService, public fb: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public copydata: CopyMarkingTeamCls,
    public Alert: AlertService, public translate: TranslateService) { }

  ngOnInit(): void {
    this.formset();
    this.GetEmptyQigIds();
  }

  GetEmptyQigIds() {
    this.usermanagement.GetEmptyQigIds().pipe(first()).subscribe({

      next: (data: any) => {
        this.LtEmptyQigIds = data;
      },
      error: (err: any) => {
        throw (err);
      },
    })
  }


  MoveExistingMarkingTeamToEmptyQig() {
    this.IsLoading = true;
    var QigVal = this.CopyForm.get('Qigs') ?.value;

    if (QigVal == "") {
      this.IsLoading = false;
       this.Alert.warning("Please select atleast one QIG");
    }
    else {

      if (this.copydata != undefined) {

        var obj = new MoveMarkingTeamToEmptyQigs();
        obj.FromQigId = this.copydata.QigId;
        obj.ToQigId = QigVal;


        this.usermanagement.MoveMarkingTeamToEmptyQigs(obj).pipe(first()).subscribe({

          next: (data: any) => {
            if (data == "S001") {
              this.Alert.success("Team successfully copied to the selected QIG(s).");
            }
            if (data == "R001") {
              this.Alert.warning("Cannot insert heirarchy to the QIG(s)");
            }
          },
          error: (err: any) => {
            throw (err);
          },
          complete: () => {
            this.IsLoading = false;
          }
        });
      }
      else {
        this.IsLoading = false;
      }
    }
  }

  formset() {

    this.CopyForm = this.fb.group({

      Qigs: ['', Validators.required],

    });
  }
}
