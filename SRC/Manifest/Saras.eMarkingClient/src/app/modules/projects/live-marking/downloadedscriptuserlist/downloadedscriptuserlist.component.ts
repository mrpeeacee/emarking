import { Component, OnInit } from '@angular/core';
import { LiveMarkingService } from 'src/app/services/project/live-marking/live-marking.service';
import { TranslateService } from '@ngx-translate/core';
import { Router, ActivatedRoute } from '@angular/router';
import { first } from 'rxjs';
import { Qualitycheckedbyusers, Livepoolscript, Scriptsoflivepool } from 'src/app/model/project/quality-check/marker-tree-view-model';
import { CommonService } from 'src/app/services/common/common.service';


@Component({
  selector: 'emarking-downloadedscriptuserlist',
  templateUrl: './downloadedscriptuserlist.component.html',
  styleUrls: ['./downloadedscriptuserlist.component.css']
})
export class DownloadedscriptuserlistComponent implements OnInit {

  QigId!: number;
  QigName!: string;
  UserList: Qualitycheckedbyusers[] = [];
  SrchList: Qualitycheckedbyusers[] = [];
  selected: any[] = [];
  ScriptSearchValue: string = '';
  title!: string;
  isChecked: boolean = false;
  IsPageLoading: boolean = false;
  scriptsids:Scriptsoflivepool[] = [];
  constructor(private liveMarkingService: LiveMarkingService, public commonService: CommonService,
    public translate: TranslateService, public route: Router, private router: ActivatedRoute) { }

  ngOnInit(): void {
    this.QigId = this.router.snapshot.params['QigId'];
    this.QigName = this.router.snapshot.params['qig'];

    this.GetScriptDownloadedUserList();
    this.textinternationalization();

  }

  textinternationalization() {
    this.translate
      .get('Downloadscriptdetails.Pagedesc')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });

    this.translate
      .get('Downloadscriptdetails.Headername')
      .subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
        this.title = translated;
      });
  }

  GetScriptDownloadedUserList() {
    this.IsPageLoading = true;
    this.liveMarkingService.GetDownloadedScriptUserList(this.QigId)
      .pipe(first()).subscribe({
        next: (data: Qualitycheckedbyusers[]) => {

          this.UserList = data;
          this.SrchList = data;

        },
        error: (err: any) => {
          throw err;
        },
        complete: () => {
          this.IsPageLoading = false;
        }
      })
  }
  SearchScript() {
    var ScriptSearchValue = this.ScriptSearchValue;
    this.UserList = this.SrchList.filter(function (el: { UserName: string; }) {
      return el.UserName.toLowerCase().includes(
        ScriptSearchValue.trim().toLowerCase()
      );
    });
  }

  GetInprogressScript(event: any) {
    //Intentionally empty
  }

  sendscriptstolivepool() {

    var obj = new Livepoolscript();
    obj.QigId = this.QigId;


    this.UserList.forEach(element => {

      if (element.IsChecked) {
        var sobj = new Scriptsoflivepool();
        sobj.ScriptId = element.ScriptId;
        this.scriptsids.push(sobj)
      }

      obj.scriptsids = this.scriptsids;

    });

    if (obj.scriptsids.length > 0) {
      this.liveMarkingService.MoveScriptsToLivePool(obj)
        .pipe(first()).subscribe({
          next: (data: any) => {

            if (data == "SU001") {
              alert("Send script to livepool successfully");
            }
          },
          error: (err: any) => {
            throw err;
          },
        });
    }
    else {
      alert("Please select atleast one script");
    }
  }


}
