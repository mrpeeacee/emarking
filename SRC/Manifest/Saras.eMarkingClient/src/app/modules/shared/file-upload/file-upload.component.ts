import { Component, ElementRef, Input, ViewChild } from '@angular/core';
import { first } from 'rxjs';
import { Filedetails } from 'src/app/model/project/mark-scheme/mark-scheme-model';
import { AlertService } from 'src/app/services/common/alert.service';
import { FileService } from 'src/app/services/file/file.service';

@Component({
  selector: 'emarking-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.css'],
})
export class FileUploadComponent {
  @Input() files?: Filedetails[] = [];
  @Input() viewmode: boolean = false;
  @ViewChild('fileInput') fileInput: ElementRef | undefined;
  IsFileProcessing: boolean = false;
  fileformat!: string;
  file: File | undefined;
  IsView!: boolean;
  constructor(
    public Alert: AlertService,
    public Fileservice: FileService
  ) {}

  removeFils(sel: any) {
    const indexToRemove = sel;
    this.files?.splice(indexToRemove, 1);
  }

  fileSelected(event: any) {
    this.file = event.target.files[0];
  }

  clearFileInput() {
    if (this.fileInput != undefined && this.fileInput != null) {
      const fileInputElement = this.fileInput.nativeElement as HTMLInputElement;
      fileInputElement.value = '';
      this.file = undefined;
    }
  }

  UploadFile() {
    if (this.file != null && this.file != undefined) {
      this.IsFileProcessing = true;
      this.Fileservice.upload(this.file)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            this.fileformat = data;
            if (this.fileformat == 'F001') {
              this.Alert.warning('An error occurred while uploading the file.');
            } else if (this.fileformat == 'INVFSZE') {
              this.Alert.warning(
                'Files larger than 50MB are not allowed to be uploaded.'
              );
            } else if (this.fileformat == 'INVFTPE') {
              this.Alert.warning('Only PDF files are allowed for upload');
            } else {
              this.Alert.success('File uploaded successfully');
              if( this.file != undefined){
                this.files?.push({
                  FileName: this.file.name,
                  Id: parseInt(this.fileformat),
                });
              }
            }
          },
          error: (err: any) => {
            throw err;
          },
          complete: () => {
            this.clearFileInput();
            this.IsFileProcessing = false;
          },
        });
    } else {
      this.Alert.warning('Please choose a pdf file.');
    }
  }

  downloadPdf(key: string) {
    this.IsFileProcessing = true;
    this.Fileservice.Download(key)
      .pipe(first())
      .subscribe({
        next: (response: any) => {
          if (response != null && response.status == 200) {
            const contentDispositionHeader = response.headers.get(
              'Content-Disposition'
            );
            const filename =
              this.getFileNameFromContentDisposition(
                contentDispositionHeader as string
              ) || 'file';

            const blob = new Blob([response.body as BlobPart], {
              type: 'application/octet-stream',
            });
            const url = window.URL.createObjectURL(blob);

            const link = document.createElement('a');
            link.href = url;
            link.download = filename;
            link.click();
            window.URL.revokeObjectURL(url);
          } else {
            this.Alert.warning('File not found');
          }
        },
        error: (err: any) => {
          throw err;
        },
        complete: () => {
          this.IsFileProcessing = false;
        },
      });
  }

  private getFileNameFromContentDisposition(
    contentDisposition: string
  ): string | null {
    if (contentDisposition) {
      const match = contentDisposition.match(
        /filename\*?=['"]?(?:UTF-\d['"]*)?([^;\r\n"']*)['"]?;?/i
      );
      if (match && match.length > 1) {
        return decodeURIComponent(match[1]);
      }
    }
    return null;
  }
}
