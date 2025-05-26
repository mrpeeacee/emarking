import { Component, OnInit, Inject } from '@angular/core';
import { UserManagementService } from 'src/app/services/project/setup/user-management.service';
import { first } from 'rxjs/operators';
import { BlankQigIds, MoveMarkingTeamToEmptyQig, CopyMarkingTeamCls } from 'src/app/model/project/setup/user-management';
import { ActivatedRoute } from '@angular/router';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AlertService } from 'src/app/services/common/alert.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Validators } from 'ngx-editor';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'emarking-copymarkingteam',
  templateUrl: './copymarkingteam.component.html',
  styleUrls: ['./copymarkingteam.component.css']
})
export class CopymarkingteamComponent implements OnInit {

  LtBlankQigIds: BlankQigIds[] = [];
  QigId!: number;
  CopyForm!: FormGroup;
  IsLoading: boolean = false;
  InterMessages: any = {
    SelectMessage: ''
  }

  Isdialogclosed: boolean = false;

  constructor(public usermanagementService: UserManagementService,
    private route: ActivatedRoute, public fb: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public copydata: CopyMarkingTeamCls, private dialogRef: MatDialogRef<CopymarkingteamComponent>,
    public Alert: AlertService, public translate: TranslateService) { }

  ngOnInit(): void {

    this.translate.get('SetUp.UserManagement.selqig').subscribe((translated: string) => {
      this.InterMessages.SelectMessage = translated;
    });


    this.formset();
    this.GetQigsTeamList();
  }



  GetQigsTeamList(): any {

    this.usermanagementService.GetBlankQigIds().pipe(first()).subscribe({

      next: (data: any) => {
        this.LtBlankQigIds = data;
      },
      error: (err: any) => {
        throw (err);
      },
    });
  }


  MoveExistingMarkingTeamToEmptyQig() {
    this.IsLoading = true;
    this.Isdialogclosed = false;
    //  var QigVal = Number(((document.getElementById("ddlQigs") as HTMLSelectElement).value));
    var QigVal = 0;
    if (this.CopyForm.valid) {
      QigVal = this.CopyForm.get('Qigs')?.value;
    }



    if (QigVal == 0) {
      this.IsLoading = false;
      this.Alert.warning(this.InterMessages.SelectMessage);
    }
    else {

      if (this.copydata != undefined) {

        var obj = new MoveMarkingTeamToEmptyQig();
        obj.FromQigId = QigVal;
        obj.ToQigId = this.copydata.QigId;


        this.usermanagementService.MoveMarkingTeamToEmptyQig(obj).pipe(first()).subscribe({

          next: (data: any) => {
            if (data == "SU001") {
              this.Alert.success("The team successfully copied from the selected QIG.");
              this.Isdialogclosed = true;
              this.dialogRef.close(this.Isdialogclosed);
            }
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
