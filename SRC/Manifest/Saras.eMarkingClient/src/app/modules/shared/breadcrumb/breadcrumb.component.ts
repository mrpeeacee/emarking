import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { Breadcrumb } from 'src/app/services/common/breadcrumb/breadcrumb.model';
import { BreadcrumbService } from 'src/app/services/common/breadcrumb/breadcrumb.service';
import { GlobalUserManagementService } from 'src/app/services/Global/UserManagement/UserManagementService';
import { first } from 'rxjs/operators';
import { CreateEditUser } from 'src/app/model/Global/UserManagement/UserManagementModel';

@Component({
  selector: 'emarking-breadcrumb',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.css']
})
export class BreadcrumbComponent {
  breadcrumbs$: Observable<Breadcrumb[]>;
  Myprofile1!: CreateEditUser;

  constructor(
    breadcrumbService: BreadcrumbService,
    public globalUserManagementService: GlobalUserManagementService
  ) {
    this.breadcrumbs$ = breadcrumbService.breadcrumbs$;
  }
  GetProfileData(events: any) {
    if(events=="projects"){
      this.getMyprofileDetailsProject(0);
    }
    
  }

  getMyprofileDetailsProject(evnet:any) {
    this.globalUserManagementService
      .getMyprofileDetailsProject(evnet)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.Myprofile1 = data;
          this.sendDataToHeader();
        },
        error: (err: any) => {
          throw err;
        }
      });
  }

  sendDataToHeader() {
    const data = this.Myprofile1;
    this.globalUserManagementService.updateData(data);
  }
}
