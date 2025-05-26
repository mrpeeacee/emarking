import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { Breadcrumb } from 'src/app/services/common/breadcrumb/breadcrumb.model';
import { BreadcrumbService } from 'src/app/services/common/breadcrumb/breadcrumb.service';

@Component({
  templateUrl: './authenticated-layout.component.html',
})
export class AuthenticatedLayoutComponent {
  breadcrumbs$: Observable<Breadcrumb[]>;

  constructor(breadcrumbService: BreadcrumbService) {
    this.breadcrumbs$ = breadcrumbService.breadcrumbs$;
  }
}
