import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth/auth.service';
import { Observable } from 'node_modules/rxjs';
import { TranslateService } from '@ngx-translate/core';
@Component({
  selector: 'emarking-preloader',
  templateUrl: './preloader.component.html',
  styleUrls: ['./preloader.component.css']
})
export class PreloaderComponent implements OnInit {
  EnablepreLoading$!: Observable<boolean>;
  IsLoaderRequired: boolean = false;

  constructor(public authService: AuthService,
    public translate: TranslateService) {}

  ngOnInit(): void {
    this.EnablepreLoading$ = this.authService.getLoading;
    this.translate
    .get('Loader.IsLoaderRequired')
    .subscribe((translated: string) => {
      this.IsLoaderRequired = JSON.parse(translated);
    });

    this.authService.setLoading(false);
  }
}
