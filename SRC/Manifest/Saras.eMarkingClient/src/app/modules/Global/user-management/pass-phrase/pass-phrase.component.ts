import { Component, OnInit} from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { PassPhrase } from 'src/app/model/Global/UserManagement/UserManagementModel';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { GlobalUserManagementService } from 'src/app/services/Global/UserManagement/UserManagementService';

@Component({
  selector: 'emarking-pass-phrase',
  templateUrl: './pass-phrase.component.html',
  styleUrls: ['./pass-phrase.component.css']
})
export class PassPhraseComponent implements OnInit {

  PassPhraseList: any;
  Message: string = "";
  passphrase: any;
  errorMessage: string = "";
  isloading: boolean = false;

  constructor(public usermanagementService: GlobalUserManagementService,
    public Alert: AlertService,
    public translate: TranslateService, public commonService: CommonService) {

  }

  ngOnInit(): void {
    this.translate.get('usermanage.passphrasetitle').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate.get('usermanage.passphrasedesc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
    this.GetPassPhrase();
  }

  GetPassPhrase() {
    this.isloading = true;
    this.usermanagementService.GetPassPhrase().subscribe(result => {
      this.PassPhraseList = result;
    });
    this.isloading = false;
  }

  AddPassPhraseUsers(passphrase: string) {
    this.isloading = true;
    this.Alert.clear();
    if (passphrase == null || passphrase == undefined || passphrase.length == 0) {
      this.isloading = false;
      this.translate
        .get('usermanage.PassPhraseEmpty')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
    }
    else if (passphrase.trim().length < 8) {
      this.isloading = false;
      this.translate
        .get('usermanage.PassPhraseMinChar')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
    }
    else {
      this.Alert.clear();
      let create = new PassPhrase();
      create.PassPharseCode = passphrase.trim();
      this.usermanagementService.AddPassPhraseUsers(create).subscribe(result => {
        if (result == "success") {
          this.isloading = false;
          this.translate
            .get('usermanage.PassPhraseSuccess')
            .subscribe((translated: string) => {
              this.Alert.success(translated);
            });
          this.passphrase = "";
          this.GetPassPhrase();
        }
        else {
          this.isloading = false;
          this.translate
            .get('usermanage.PassPhraseFailed')
            .subscribe((translated: string) => {
              this.Alert.warning(translated);
            });
          this.GetPassPhrase();
        }
      });
    }
    this.isloading = false;
  }
}
