import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  Band,
  MarkScheme,
  MarkSchemeType,
} from 'src/app/model/project/mark-scheme/mark-scheme-model';
import { MarkSchemeService } from 'src/app/services/project/mark-scheme/mark-scheme.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { HtmlEditorComponent } from '../../../shared/html-editor/html-editor.component';
import { first } from 'rxjs/operators';
import { HttpResponse } from '@angular/common/http';
import { FileModel } from 'src/app/model/file/file-model';
@Component({
  templateUrl: './edit-mark-scheme.component.html',
  styleUrls: ['./edit-mark-scheme.component.css'],
})
export class EditMarkSchemeComponent implements OnInit {
  schemeId: any;
  BandModel!: MarkScheme;
  IsBtnvisible!: boolean;
  isdisabled!: boolean;
  isBandRequired: boolean = false;
  filelist: FileModel[] = [];
  message: any;
  progress: number = 0;
  fileformat!: string;
  myFiles!: FormData;
  sMsg: string = '';
  receivedFileId!: string;
  IsView: boolean = false;
  // @Output() nameChange:EventEmitter<String> = new EventEmitter<String>();
  constructor(
    private router: ActivatedRoute,
    private Schemeservice: MarkSchemeService,
    public Alert: AlertService,
    public redirect: Router,
    public commonService: CommonService,
    private dialog: MatDialog,
    public translate: TranslateService
  ) {}

  intMessages: any = {
    SaveSuccss: '',
    MandField: '',
    NotSave: '',
    Error: '',
    Invalid: '',
    schmarblk: '',
    schname: '',
    valbnd: '',
    valbndnm: '',
    valbndfm: '',
    valbndto: '',
    vallessft: '',
    valeqls: '',
    repval: '',
    avobnd: '',
    toexs: '',
    frmexs: '',
    frmtoexs: '',
    savecofm: '',
    bndovl: '',
    unqbnd: '',
    bnddesc: '',
    schdesc: '',
    invdata: '',
    valmiss: '',
    schnam: ''
  };

  ngOnInit(): void {
    this.translate.get('editsch.title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate.get('editsch.pgedesc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
    this.translate.get('editsch.succ').subscribe((translated: string) => {
      this.intMessages.SaveSuccss = translated;
    });
    this.translate.get('editsch.mand').subscribe((translated: string) => {
      this.intMessages.MandField = translated;
    });
    this.translate.get('editsch.unsucc').subscribe((translated: string) => {
      this.intMessages.NotSave = translated;
    });
    this.translate.get('editsch.error').subscribe((translated: string) => {
      this.intMessages.Error = translated;
    });
    this.translate.get('editsch.invald').subscribe((translated: string) => {
      this.intMessages.Invalid = translated;
    });
    this.translate.get('editsch.schmarblk').subscribe((translated: string) => {
      this.intMessages.schmarblk = translated;
    });
    this.translate.get('editsch.schname').subscribe((translated: string) => {
      this.intMessages.schname = translated;
    });
    this.translate.get('editsch.valbnd').subscribe((translated: string) => {
      this.intMessages.valbnd = translated;
    });
    this.translate.get('editsch.valbndnm').subscribe((translated: string) => {
      this.intMessages.valbndnm = translated;
    });
    this.translate.get('editsch.valbndfm').subscribe((translated: string) => {
      this.intMessages.valbndfm = translated;
    });
    this.translate.get('editsch.valbndto').subscribe((translated: string) => {
      this.intMessages.valbndto = translated;
    });
    this.translate.get('editsch.vallessft').subscribe((translated: string) => {
      this.intMessages.vallessft = translated;
    });
    this.translate.get('editsch.valeqls').subscribe((translated: string) => {
      this.intMessages.valeqls = translated;
    });
    this.translate.get('editsch.repval').subscribe((translated: string) => {
      this.intMessages.repval = translated;
    });
    this.translate.get('editsch.avobnd').subscribe((translated: string) => {
      this.intMessages.avobnd = translated;
    });
    this.translate.get('editsch.toexs').subscribe((translated: string) => {
      this.intMessages.toexs = translated;
    });
    this.translate.get('editsch.frmexs').subscribe((translated: string) => {
      this.intMessages.frmexs = translated;
    });
    this.translate.get('editsch.frmtoexs').subscribe((translated: string) => {
      this.intMessages.frmtoexs = translated;
    });
    this.translate.get('editsch.savecofm').subscribe((translated: string) => {
      this.intMessages.savecofm = translated;
    });
    this.translate.get('editsch.bndovl').subscribe((translated: string) => {
      this.intMessages.bndovl = translated;
    });
    this.translate.get('editsch.unqbnd').subscribe((translated: string) => {
      this.intMessages.unqbnd = translated;
    });
    this.translate.get('editsch.bnddesc').subscribe((translated: string) => {
      this.intMessages.bnddesc = translated;
    });
    this.translate.get('editsch.schdesc').subscribe((translated: string) => {
      this.intMessages.schdesc = translated;
    });
    this.translate.get('editsch.invdata').subscribe((translated: string) => {
      this.intMessages.invdata = translated;
    });
    this.translate.get('editsch.valmiss').subscribe((translated: string) => {
      this.intMessages.valmiss = translated;
    });
    this.translate.get('createsch.schnam').subscribe((translated: string) => {
      this.intMessages.schnam = translated;
    });

    this.schemeId = this.router.snapshot.params['schemeid'];

    this.BandModel = {} as MarkScheme;
    this.BandModel.Bands = [{}] as Band[];

    this.getMarkSchemeAndBand();

    this.isdisabled = true;
  }
  onAddBand() {
    var length = this.BandModel.Bands.length;

    if (length > 0) {
      var toVal =
        this.BandModel.Bands[this.BandModel.Bands.length - 1].BandFrom;
      this.BandModel.Bands.push({} as Band);
      this.BandModel.Bands[length].BandTo = toVal - 1;
    } else {
      this.BandModel.Bands.push({} as Band);
      this.BandModel.Bands[0].BandTo = this.BandModel.Marks;
    }
  }

  VisibleBtn(fromval: number, eindex: number) {
    var length = this.BandModel.Bands.length;

    let istrue = this.BandModel.Bands.filter((a) => a.BandFrom == 0);
    if (fromval == 0 || istrue.length > 0) {
      this.IsBtnvisible = false;
    } else {
      this.IsBtnvisible = true;
    }

    if (length > eindex && fromval > 0) {
      var i = eindex + 1;
      if (
        this.BandModel.Bands[i] != null &&
        this.BandModel.Bands[i] != undefined
      ) {
        this.BandModel.Bands[i].BandTo = fromval - 1;
      }
    }
  }

  getMarkSchemeAndBand() {
    this.Schemeservice.getMarkSchemeAndBand(this.schemeId).subscribe((data) => {
      this.BandModel = data;
      this.isBandRequired = this.BandModel.IsBandExist;
      this.filelist = this.BandModel.filedetails;
      let istrue = this.BandModel.Bands.filter((a) => a.BandFrom == 0);
      if (istrue.length > 0) {
        this.IsBtnvisible = false;
      } else {
        this.IsBtnvisible = true;
      }

      this.CheckBandEmpty();
    });
  }

  schemesaving: boolean = false;
  updateMarkSchemeAndBands() {
    this.BandModel.IsBandExist = this.isBandRequired;
    if (!this.schemesaving && this.validateScheme()) {
      const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
        data: {
          message: this.intMessages.savecofm,
        },
      });
      confirmDialog.afterClosed().subscribe((res) => {
        if (res === true) {
          this.schemesaving = true;
          this.BandModel.filedetails = this.filelist;
          this.Schemeservice.updateMarkSchemeAndBands(
            this.BandModel,
            this.schemeId
          )
            .pipe(first())
            .subscribe({
              next: (data: any) => {
                if (data == 'SU001') {
                  this.Alert.success(this.intMessages.SaveSuccss);
                  this.getMarkSchemeAndBand();
                  this.redirect.navigate(['/projects/', 'mark-schemes']);
                } else if (data == 'EMPTY') {
                  this.Alert.warning(this.intMessages.invdata);
                } else if (data == 'VALMISS') {
                  this.Alert.warning(this.intMessages.valmiss);
                } else if (data == 'SN001') {
                  this.Alert.warning(this.intMessages.schnam);
                } else {
                  this.Alert.warning(this.intMessages.NotSave);
                }
              },
              error: (a: any) => {
                throw a;
              },
              complete: () => {
                this.schemesaving = false;
              },
            });
        }
      });
    }
  }

  public get MarkSchemeType() {
    return MarkSchemeType;
  }

  private validateScheme(): boolean {
    if (this.BandModel == null || this.BandModel == undefined) {
      this.Alert.warning(this.intMessages.Invalid);
      return false;
    }
    if (this.BandModel.SchemeName.trim() == '') {
      this.Alert.warning(this.intMessages.schname);
      return false;
    }
    if (this.BandModel.Marks <= 0) {
      this.Alert.warning(this.intMessages.schmarblk);
      return false;
    }
    if (this.BandModel.IsBandExist) {
      if (this.BandModel.Bands == null || this.BandModel.Bands == undefined) {
        this.Alert.warning(this.intMessages.valbnd);
        return false;
      }
      if (this.BandModel.Bands.length <= 0) {
        this.Alert.warning(this.intMessages.valbnd);
        return false;
      }
      let emptyBname = this.BandModel.Bands.findIndex(
        (a) =>
          a.BandName == null ||
          a.BandName == undefined ||
          a.BandName.trim() == ''
      );
      if (emptyBname >= 0) {
        this.Alert.warning(this.intMessages.valbndnm);
        return false;
      }

      let emptyBfrom = this.BandModel.Bands.findIndex(
        (a) => a.BandFrom == null || a.BandFrom == undefined || a.BandFrom < 0
      );
      if (emptyBfrom >= 0) {
        this.Alert.warning(this.intMessages.valbndfm);
        return false;
      }
      let emptyBto = this.BandModel.Bands.findIndex(
        (a) => a.BandTo == null || a.BandTo == undefined || a.BandTo < 0
      );
      if (emptyBto >= 0) {
        this.Alert.warning(this.intMessages.valbndto);
        return false;
      }

      let frmrange = false;
      let torange = false;
      let frmto = false;
      this.BandModel.Bands.forEach((a) => {
        let frmcount = 0;
        let tocount = 0;
        let frmtocount = 0;
        this.BandModel.Bands.forEach((z) => {
          if (a.BandFrom == z.BandFrom) {
            frmcount++;
          }
          if (a.BandTo == z.BandTo) {
            tocount++;
          }
          if (z.BandFrom != 0 && z.BandTo != 0) {
            if (a.BandFrom == z.BandTo || a.BandTo == z.BandFrom) {
              frmtocount++;
            }
          }
        });
        if (frmcount >= 2) {
          frmrange = true;
        }
        if (tocount >= 2) {
          torange = true;
        }
        if (frmtocount >= 1) {
          frmto = true;
        }
      });
      if (torange) {
        this.Alert.warning(this.intMessages.avobnd);
        return false;
      }
      if (frmrange) {
        this.Alert.warning(this.intMessages.frmexs);
        return false;
      }
      // if (frmto) {
      //   this.Alert.warning(this.intMessages.frmtoexs);
      //   return false;
      // }

      let cmprBto = this.BandModel.Bands.findIndex(
        (a) => 
        (a.BandFrom != 0 && a.BandTo != 0) ? a.BandFrom >= a.BandTo : 0
      );
      // if (cmprBto >= 0) {
      //   this.Alert.warning(this.intMessages.vallessft);
      //   return false;
      // }

      let scMrCnt = this.BandModel.Bands.findIndex(
        (a) => a.BandTo > this.BandModel.Marks
      );
      if (scMrCnt >= 0) {
        this.Alert.warning(this.intMessages.valeqls);
        return false;
      }
      let isnincalidrange = false;
      let isnincalidname = false;
      this.BandModel.Bands.forEach((dupband) => {
        let indcount = 0;
        let bnamecount = 0;
        this.BandModel.Bands.forEach((innerbnd) => {
          if (
            dupband.BandFrom == innerbnd.BandFrom &&
            dupband.BandTo == innerbnd.BandTo
          ) {
            indcount = indcount + 1;
          }

          if (dupband.BandName == innerbnd.BandName) {
            bnamecount = bnamecount + 1;
          }
        });
        if (indcount >= 2) {
          isnincalidrange = true;
        }
        if (bnamecount >= 2) {
          isnincalidname = true;
        }
      });
      if (isnincalidrange) {
        this.Alert.warning(this.intMessages.repval);
        return false;
      }
      if (isnincalidname) {
        this.Alert.warning(this.intMessages.avobnd);
        return false;
      }

      var j = 0;
      const sortFunction1 = (a: any, b: any) => (a.BandTo < b.BandTo ? 1 : -1);
      this.BandModel.Bands.sort(sortFunction1);

      const sortFunction2 = (a: any, b: any) => (a.BandTo < b.BandTo ? 1 : -1);
      this.BandModel.Bands.sort(sortFunction2);

      for (var i = 0; i < this.BandModel.Bands.length; i++) {
        if (i > 0) {
          j = i - 1;
        }

        if (this.BandModel.Bands[i].BandFrom > this.BandModel.Bands[i].BandTo) {
          this.Alert.warning(this.intMessages.frmexs);
          return false;
        } else if (
          this.BandModel.Bands[i].BandTo >= this.BandModel.Bands[j].BandFrom &&
          i > 0
        ) {
          this.Alert.warning(this.intMessages.bndovl);
          return false;
        } else if (
          this.BandModel.Bands[j].BandFrom - this.BandModel.Bands[i].BandTo >=
            2 &&
          i > 0
        ) {
          this.Alert.warning('Invalid band range.');
          return false;
        }
      }
      const lower = this.BandModel.Bands.map((element) => {
        return element.BandName.toLowerCase();
      });
      if (this.hasDuplicates(lower)) {
        this.Alert.warning(this.intMessages.unqbnd);

        return false;
      }
    }

    return true;
  }
  hasDuplicates = (arr: any) => {
    return new Set(arr).size !== arr.length;
  };
  keyPressAlphaNumericWithCharacters(event: KeyboardEvent) {
    var inp = event.key;
    // Allow numbers, alpahbets, space, underscore
    if (/[a-zA-Z0-9-_ ]/.test(inp)) {
      return true;
    } else {
      event.preventDefault();
      return false;
    }
  }

  activeband: any;
  openeditor(band: any) {
    this.activeband = band;
    const editorDialog = this.dialog.open(HtmlEditorComponent, {
      data: {
        callback: this.callBack.bind(this),
        defaultValue: band.BandDescription,
        htmpagetitle: band.BandName,
        disabled: false,
      },
    });
    editorDialog.afterClosed().subscribe();
  }

  callBack(name: any) {
    if (name != null && name != undefined) {
      this.activeband.BandDescription = name;
    }
  }

  CheckBandEmpty() {
    var length = this.BandModel.Bands.length;
    if (length <= 0) {
      this.BandModel.Bands.push({} as Band);
      this.BandModel.Bands[0].BandTo = this.BandModel.Marks;
    }
  }

  onRemoveBand(BandName: string, event: any, eindex: number) {
    this.IsBtnvisible = true;
    this.isBandRequired = true;
    const checked = event.target.checked;
    if (eindex == 0) {
      this.IsBtnvisible = true;
    }
    if (checked) {
      this.BandModel.Bands.push({ BandName: BandName } as Band);
    } else {
      const index = this.BandModel.Bands.findIndex(
        (list) => list.BandName == BandName
      );
      this.BandModel.Bands.splice(index, 1);
      this.Alert.success(this.intMessages.delsucc);
    }
  }

  DownloadFile(fileurl: string) {
    this.Schemeservice.download(fileurl).subscribe((event: any) => {
      let data = event as HttpResponse<Blob>;
      const downloadedFile = new Blob([data.body as BlobPart], {
        type: data.body?.type,
      });

      console.log('ddd', downloadedFile);
      if (downloadedFile.type != '') {
        const a = document.createElement('a');
        a.setAttribute('style', 'display:none;');
        document.body.appendChild(a);
        a.download = fileurl;
        a.href = URL.createObjectURL(downloadedFile);
        a.target = '_blank';
        a.click();
        document.body.removeChild(a);
      }
    });
  }

  getMark(mark: number) {
    this.BandModel.Bands[0].BandTo = mark;
  }
}
