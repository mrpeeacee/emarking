import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
  Filedetails,
  MarkScheme,
  MarkSchemeType,
} from 'src/app/model/project/mark-scheme/mark-scheme-model';
import { MarkSchemeService } from 'src/app/services/project/mark-scheme/mark-scheme.service';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { HtmlEditorComponent } from '../../../shared/html-editor/html-editor.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  templateUrl: './view-mark-scheme.component.html',
  styleUrls: ['./view-mark-scheme.component.css'],
})
export class ViewMarkSchemeComponent implements OnInit {
  BandModel!: MarkScheme;
  schemeId: any;
  isBandRequired: boolean = false;
  filelist: Filedetails[] = [];
  constructor(
    private router: ActivatedRoute,
    private Schemeservice: MarkSchemeService,
    public commonService: CommonService,
    public translate: TranslateService,
    private dialog: MatDialog
  ) {}

  public get MarkSchemeType() {
    return MarkSchemeType;
  }

  ngOnInit(): void {
    this.translate.get('viewsch.title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });

    this.translate.get('viewsch.pgedesc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });

    this.schemeId = this.router.snapshot.params['schemeid'];
    this.getMarkSchemeAndBand();
  }

  getMarkSchemeAndBand() {
    this.Schemeservice.getMarkSchemeAndBand(this.schemeId).subscribe((data) => {
      this.BandModel = data;
      this.filelist = this.BandModel.filedetails;
      this.isBandRequired = this.BandModel.IsBandExist;
    });
  }

  activeband: any;
  openeditor(band: any) {
    this.activeband = band;
    const editorDialog = this.dialog.open(HtmlEditorComponent, {
      data: {
        defaultValue: band.BandDescription,
        htmpagetitle: band.BandName,
        disabled: true,
      },
    });
    editorDialog.afterClosed().subscribe();
  }
}
