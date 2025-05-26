import { Component, OnInit } from '@angular/core';
import { trigger, transition, style, animate, state } from '@angular/animations';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { ResolutionofCoiService } from 'src/app/services/project/setup/resolution-of-coi.service';
import { first } from 'rxjs/operators';
import { ResolutionOfCoiModel, CoiSchoolModel } from 'src/app/model/project/setup/resolution-of-coi';
import { Router } from '@angular/router';

@Component({
  templateUrl: './resolution-of-coi.component.html',
  styleUrls: ['./resolution-of-coi.component.css'], animations: [
    trigger('sideMenu', [
      state('closed', style({

        right: "-400px"
      })),
      state('open', style({

        right: "0px"
      })),

      transition('open => closed', animate('400ms ease-out')),
      transition('closed => open', animate('300ms ease-out'))
    ]),
  ]
})
export class ResolutionOfCoiComponent implements OnInit {
  panelOpenState = false;
  state = "closed";
  toggletopmenu: boolean = false;
  isShown = false;
  resolutionofcoilst!: ResolutionOfCoiModel[];
  schoollst!: CoiSchoolModel[];
  schoolselectedlst!: CoiSchoolModel[];
  SchoolList!: CoiSchoolModel[];
  SchoolSearchList!: CoiSchoolModel[];
  RoleId!: any;
  Issendingschool!: any;
  SearchSelectedValue: string = "";
  SearchMarkerValue: string = "";
  SearchMarkerLst!: ResolutionOfCoiModel[];
  selected: any[] = [];
  objsclst: any;
  sclschoolslst: any;
  coisLoading: boolean = true;
  scoolsLoading: boolean = true;

  constructor(public translate: TranslateService,
    public commonService: CommonService,
    public resolutionofcoiservice: ResolutionofCoiService,
    public Alert: AlertService, public router: Router) {
  }

  ngOnInit(): void {
    this.translate.get('SetUp.ResolutionCOI.Title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate.get('SetUp.ResolutionCOI.PageDescription').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
    this.GetResolutionCOI();
    this.GetSchoolsCOI();
  }

  CloseFn(schoolselected: any) {
    this.SearchSelectedValue = "";
    this.sclschoolslst = schoolselected;
    this.sclschoolslst = [];
    this.reloadComponent();
  }

  reloadComponent() {
    const currentUrl = this.router.url;
    this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
      this.router.navigate([currentUrl]);
    });
  }
  GetResolutionCOI() {
    this.coisLoading = true;
    this.resolutionofcoiservice.GetResolutionCOI().pipe(first()).subscribe({
      next: (data: any) => {
        this.resolutionofcoilst = data;
        if(this.indexExpanded!=-1 && data[this.indexExpanded].SchoolList.length<=1)
        {
          this.indexExpanded=-1;
        }
        this.SearchMarkerLst = data;
        this.RoleId = this.resolutionofcoilst[0]?.ProjectUserRoleID;
      },
      error: (err: any) => {
        this.Alert.error('Error while getting the project ResolutionCOI');
        throw (err);
      }, complete: () => {
        this.coisLoading = false;
      }

    });
  }

  GetSchoolsCOI() {
    this.scoolsLoading = true;
    this.resolutionofcoiservice.GetSchoolsCOI().pipe(first()).subscribe({
      next: (data: any) => {
        this.SchoolList = data;
        this.SchoolSearchList = data;
        this.schoollst = data;
        this.schoolselectedlst = data;
        this.objsclst = data
      },
      error: (err: any) => {
        this.scoolsLoading = false;
        throw (err);
      }, complete: () => {
        this.scoolsLoading = false;
      }

    });
  }

  checkSchools(markerSchoolList: any, roleid: any,SendingSchoolID:number) {
    this.SearchSelectedValue = "";
    this.schoolselectedlst=this.schoollst;
    this.RoleId = roleid;
    this.schoollst.forEach((element) => {
      if (markerSchoolList.some((sclB: { ExemptionSchoolID: any }) => element.SchoolID == sclB.ExemptionSchoolID)) {
        element.IsSelectedSchool = true;
      }
      else {
        element.IsSelectedSchool = false;
      }
      if (element.SchoolID == SendingSchoolID) {
        element.IsSendingSchool = true;
      }
      else {
        element.IsSendingSchool = false;
      }
    });
  }

  indexExpanded: number = -1;

  togglePanels(index: number) {
    this.indexExpanded = index == this.indexExpanded ? -1 : index;
  }
  isExpansionDisabled(blckext: boolean): string {
    if (blckext) {
      return 'disabled-pointer';
    }
    return '';
  }

  SearchSelectedSchools() {
    var SearchSelectedValue = this.SearchSelectedValue;
    this.schoolselectedlst = this.SchoolSearchList.filter(function (el) { return (el.SchoolName.toLowerCase().includes(SearchSelectedValue.trim().toLowerCase()) || SearchSelectedValue.trim() == "") || el.SchoolCode.toLowerCase().includes(SearchSelectedValue.trim().toLowerCase()) });
    this.schoolselectedlst = this.schoolselectedlst.filter(x => this.selected.includes(x.SchoolCode) || this.selected.length == 0);
  }
  SearchSelectedMarkers() {
    var SearchMarkerValue = this.SearchMarkerValue;
    this.resolutionofcoilst = this.SearchMarkerLst.filter(function (el) { return (el.UserName.toLowerCase().includes(SearchMarkerValue.trim().toLowerCase()) || SearchMarkerValue.trim() == "") });
    this.resolutionofcoilst = this.resolutionofcoilst.filter(x => this.selected.includes(x.UserName) || this.selected.length == 0);
  }

  SaveUpdateResolutionofCOI() {
    this.Alert.clear();
    if (this.schoollst.filter(a => !a.IsSelectedSchool).length == 0) {
      this.translate
        .get('SetUp.ResolutionCOI.AllSchoolSelectedErrMsg')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
    }
    else {
      var selectedprojectcenters = this.schoollst.filter((a: { IsSelectedSchool: boolean; }) => a.IsSelectedSchool);
      this.Alert.clear();
      this.scoolsLoading = true;

      selectedprojectcenters.forEach((element) => {
        element.SchoolName = "";
      });
      document.getElementById('closeComponentCoi')!.click();
      this.resolutionofcoiservice.updateResolutionCOI(
        this.RoleId,
        selectedprojectcenters
      )
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            if (data == 'UP001') {
              this.translate
                .get('SetUp.ResolutionCOI.ResolutionCOIUpdated')
                .subscribe((translated: string) => {
                  this.Alert.clear();
                  this.Alert.success(translated);
                  this.SearchSelectedValue = "";
                  this.GetResolutionCOI();
                  this.GetSchoolsCOI();
                });
            }
            else {
              this.Alert.clear();
              this.Alert.error("Error while updating Conflict schools.");
              this.SearchSelectedValue = "";
              this.GetResolutionCOI();
              this.GetSchoolsCOI();
            }
          },
          error: (a: any) => {
            throw a;
          },
          complete: () => {
            this.scoolsLoading = false;
          }
        });
    }
  }
} 
