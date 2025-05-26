import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectClosureComponent } from './project-closure.component';

describe('ProjectLevelConfigurationComponent', () => {
  let component: ProjectClosureComponent;
  let fixture: ComponentFixture<ProjectClosureComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProjectClosureComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProjectClosureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
