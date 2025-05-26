import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LiveMarkingSettingsComponent } from './live-marking-settings.component';
 
describe('LiveMarkingSettingsComponent', () => {
  let component: LiveMarkingSettingsComponent;
  let fixture: ComponentFixture<LiveMarkingSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LiveMarkingSettingsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LiveMarkingSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
