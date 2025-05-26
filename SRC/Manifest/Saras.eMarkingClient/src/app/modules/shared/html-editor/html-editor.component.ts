import {
  Component,
  Inject,
  OnDestroy,
  OnInit,
  ViewEncapsulation,
} from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Editor, toDoc, toHTML, Toolbar, Validators } from 'ngx-editor';

export type DialogDataSubmitCallback<T> = (row: T) => void;

@Component({
  selector: 'emarking-html-editor',
  templateUrl: './html-editor.component.html',
  styleUrls: ['./html-editor.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class HtmlEditorComponent implements OnInit, OnDestroy {
  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: {
      callback: DialogDataSubmitCallback<any>;
      defaultValue: any;
      htmpagetitle: any;
      disabled: boolean;
    },
    private dialogRef: MatDialogRef<HtmlEditorComponent>
  ) {}

  htmltitle: string = '';
  html: any;
  editor!: Editor;
  toolbar: Toolbar = [
    ['bold', 'italic'],
    ['underline', 'strike'],
    ['ordered_list', 'bullet_list'],
    [{ heading: ['h1', 'h2', 'h3', 'h4', 'h5', 'h6'] }],
  ];

  ngOnInit(): void {
    this.editor = new Editor();
    this.htmltitle = this.data.htmpagetitle;

    if (this.data.defaultValue == undefined) {
      this.data.defaultValue = '';
    }

    this.form.setValue({ editorContent: this.data.defaultValue });
  }

  form = new FormGroup({
    editorContent: new FormControl(
      {
        value: toDoc(
          this.data.defaultValue == undefined ? '' : this.data.defaultValue
        ),
        disabled: this.data.disabled,
      },
      Validators.required()
    ),
  });

  eddata: any;
  editorChange() { 
    var data = this.docdata();
    if (
      String(data).replace(/\s/g, '') == '' ||
      String(data).replace(/\s/g, '') === '<p></p>'
    ) {
      this.data.callback('');
    } else {
      this.data.callback(toHTML(toDoc(data)));
    }
  }

  docdata() {
    return String(this.form.value.editorContent);
  }

  closeeditor() {
    this.dialogRef.close();
  }

  ngOnDestroy(): void {
    this.editor.destroy();
  }
}
