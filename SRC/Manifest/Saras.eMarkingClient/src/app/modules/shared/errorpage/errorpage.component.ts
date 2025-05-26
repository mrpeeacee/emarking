import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/services/auth/auth.service';

@Component({
  selector: 'emarking-errorpage',
  templateUrl: './errorpage.component.html',
  styleUrls: ['./errorpage.component.css'],
})
export class ErrorpageComponent {
  constructor(
    private router: ActivatedRoute,
    public authService: AuthService,
    private route: ActivatedRoute
  ) {}

  ErrorCode!: string;
  ErrorImg!: string;
  ErrorTitle!: string;
  Datacode!: string;

  showwrongmessagetitle = 'Sorry, Something Went Wrong';
  pageNotfoundtitle = 'Something Went Wrong';
  Unauthorizederrortitle = 'Session Expired!';
  AuthenticationErrortittle='Authentication Failed'

  showwrongmessage = true;

  pageNotfound = false;
  Unauthorizederror = false;
  AuthenticationError=false;

  public ImgPageNotFound = 'assets/images/PageNotFound.png';
  public ImgSomethingwentwrong = 'assets/images/somethingwentwrong.png';
  public ImgUnauthorizederror = 'assets/images/unauthorizederror.png';

  ngOnInit(): void { 
    this.authService.clearAccessToken();
    let errorcode = this.router.snapshot.params['code'];
    if (errorcode != null && errorcode != undefined && errorcode != '') {
      this.ErrorCode = errorcode;
    }
    this.Datacode = this.route.snapshot.data.code;
    if (
      this.Datacode != null &&
      this.Datacode != undefined &&
      this.Datacode != ''
    ) {
      this.ErrorCode = this.Datacode;
    }

    this.showwrongmessage = true;
    this.pageNotfound = false;
    this.Unauthorizederror = false;
    this.AuthenticationError=false;

    switch (this.ErrorCode) {
      case '401':
        this.Unauthorizederror = true;
        this.ErrorImg = this.ImgUnauthorizederror;
        this.ErrorTitle = this.Unauthorizederrortitle;
        break;
      case '404':
        this.pageNotfound = true;
        this.ErrorImg = this.ImgPageNotFound;
        this.ErrorTitle = this.pageNotfoundtitle;
        break;

        case '403':
          this.AuthenticationError=true;
          this.ErrorTitle=this.AuthenticationErrortittle;
          this.ErrorImg=this.ImgUnauthorizederror;
          break;
          
      default:
        this.pageNotfound = true;
        this.ErrorImg = this.ImgSomethingwentwrong;
        this.ErrorTitle = this.showwrongmessagetitle;
        break;
    }
  }
}
