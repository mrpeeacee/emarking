import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import * as fromI18n from '../../reducers';
import { LanguageActions } from '../../actions';
import { Language } from '../../models/language.model';

@Component({
  selector: 'app-language-selector',
  templateUrl: './language-selector.component.html'
})
export class LanguageSelectorComponent {
  constructor(private readonly store: Store<fromI18n.State>) { }


  setLanguage(language: Language) {
    this.store.dispatch(LanguageActions.set({ language }));
  }
}
