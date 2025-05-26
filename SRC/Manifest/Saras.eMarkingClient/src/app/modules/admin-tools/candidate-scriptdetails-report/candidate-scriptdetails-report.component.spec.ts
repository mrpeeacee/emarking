import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CandidateScriptDetailsComponent } from './candidate-scriptdetails-report.component';


describe('CandidateScriptDetailsComponent', () => {
  let component: CandidateScriptDetailsComponent;
  let fixture: ComponentFixture<CandidateScriptDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CandidateScriptDetailsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CandidateScriptDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
