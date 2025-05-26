import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectLevelConfigurationComponent } from './project-level-configuration.component';

describe('ProjectLevelConfigurationComponent', () => {
  let component: ProjectLevelConfigurationComponent;
  let fixture: ComponentFixture<ProjectLevelConfigurationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProjectLevelConfigurationComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProjectLevelConfigurationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
