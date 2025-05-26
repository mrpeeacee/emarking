import { CommonModule } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS, HttpClient } from '@angular/common/http';
import { ErrorHandler, NgModule } from '@angular/core';
import { BrowserModule, Title } from '@angular/platform-browser';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthModule } from './modules/auth/auth.module';
import { HeaderComponent } from './modules/shared/header/header.component';
import { FooterComponent } from './modules/shared/footer/footer.component';
import { NotificationsComponent } from './modules/shared/notifications/notifications.component';
import { SharedModule } from './modules/shared/shared.module';
import { AuthInterceptor } from './services/auth/auth.interceptor';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { EncryptInterceptor } from './services/interceptor/encrypt.interceptor';
import { BreadcrumbComponent } from './modules/shared/breadcrumb/breadcrumb.component';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { AlertComponent } from './modules/shared/alert/alert.component';
import { StoreModule } from '@ngrx/store';
import { I18nModule } from './i18n/i18n.module';
import { metaReducers, ROOT_REDUCERS } from './i18n/metaReducers';
import { RouterState, StoreRouterConnectingModule } from '@ngrx/router-store';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { MatExpansionModule } from '@angular/material/expansion';
import { CdkAccordionModule } from '@angular/cdk/accordion';
import { GlobalErrorHandler } from './services/common/global-error-handler';
import { HttpLoadingInterceptor } from './services/interceptor/http-loading.interceptor';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { TimeZoneInterceptor } from './services/auth/time-zone-interceptor';
import { MatToolbarModule } from '@angular/material/toolbar';
import { AuthenticatedLayoutComponent } from './modules/authenticated/authenticated-layout/authenticated-layout.component';
import { AuthenticatedRoutingModule } from './modules/authenticated/authenticated.module';



 
export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/app/', '.json');
}

@NgModule({
  declarations: [
    AppComponent,
    NotificationsComponent,
    HeaderComponent,
    BreadcrumbComponent,
    FooterComponent,
    AlertComponent,
    AuthenticatedLayoutComponent 
  ],
  imports: [
    CommonModule,
    BrowserModule,
    MatMenuModule,
    MatButtonModule,
    HttpClientModule,
    MatToolbarModule,
    BrowserAnimationsModule,
    I18nModule,
    AppRoutingModule,
    SharedModule,
    HttpClientModule,
    AuthModule,
    NgbModule,
    MatExpansionModule,
    CdkAccordionModule,
    InfiniteScrollModule,
    AuthenticatedRoutingModule,
    StoreModule.forRoot(ROOT_REDUCERS, {
      metaReducers,
      runtimeChecks: {
        strictStateImmutability: true,
        strictActionImmutability: true,
        strictStateSerializability: true,
        strictActionSerializability: true
      }
    }),
    StoreRouterConnectingModule.forRoot({
      routerState: RouterState.Minimal
    }),
    StoreDevtoolsModule.instrument({
      name: 'NgRx Book Store App'
    }),

    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient]
      },
      isolate: true
    })
  ],
  providers: [
    Title,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: EncryptInterceptor,
      multi: true
    }, {
      provide: ErrorHandler,
      useClass: GlobalErrorHandler,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpLoadingInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TimeZoneInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
