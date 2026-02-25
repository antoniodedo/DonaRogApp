import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzUploadModule, NzUploadFile } from 'ng-zorro-antd/upload';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzMessageService } from 'ng-zorro-antd/message';
import { DonationDocumentType } from '../../proxy/donations/models';

@Component({
  selector: 'app-upload-document-modal',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NzFormModule,
    NzSelectModule,
    NzInputModule,
    NzUploadModule,
    NzButtonModule,
    NzIconModule
  ],
  template: `
    <div style="padding: 24px; background: linear-gradient(to bottom, #fafafa, white);">
      <form nz-form nzLayout="vertical">
        
        <!-- Tipo Documento -->
        <nz-form-item>
          <nz-form-label [nzRequired]="true">
            <span style="display: flex; align-items: center; gap: 6px; font-weight: 700; font-size: 13px; color: #0c4a6e;">
              <span nz-icon nzType="folder-open" nzTheme="outline" style="color: #14b8a6;"></span>
              Tipo Documento
            </span>
          </nz-form-label>
          <nz-form-control>
            <nz-select 
              [(ngModel)]="documentType" 
              name="documentType" 
              nzPlaceHolder="Seleziona tipo documento"
              nzSize="large"
              style="width: 100%;">
              <nz-option [nzValue]="DonationDocumentType.BankReceipt">
                <span style="display: flex; align-items: center; gap: 8px;">
                  <span nz-icon nzType="bank" nzTheme="outline" style="color: #3b82f6;"></span>
                  Ricevuta Bonifico
                </span>
              </nz-option>
              <nz-option [nzValue]="DonationDocumentType.PostalReceipt">
                <span style="display: flex; align-items: center; gap: 8px;">
                  <span nz-icon nzType="mail" nzTheme="outline" style="color: #ec4899;"></span>
                  Ricevuta Postale
                </span>
              </nz-option>
              <nz-option [nzValue]="DonationDocumentType.PayPalReceipt">
                <span style="display: flex; align-items: center; gap: 8px;">
                  <span nz-icon nzType="paypal" nzTheme="outline" style="color: #0070ba;"></span>
                  Ricevuta PayPal
                </span>
              </nz-option>
              <nz-option [nzValue]="DonationDocumentType.CheckImage">
                <span style="display: flex; align-items: center; gap: 8px;">
                  <span nz-icon nzType="dollar" nzTheme="outline" style="color: #f59e0b;"></span>
                  Immagine Assegno
                </span>
              </nz-option>
              <nz-option [nzValue]="DonationDocumentType.CashReceipt">
                <span style="display: flex; align-items: center; gap: 8px;">
                  <span nz-icon nzType="wallet" nzTheme="outline" style="color: #10b981;"></span>
                  Ricevuta Contanti
                </span>
              </nz-option>
              <nz-option [nzValue]="DonationDocumentType.Other">
                <span style="display: flex; align-items: center; gap: 8px;">
                  <span nz-icon nzType="file" nzTheme="outline" style="color: #6b7280;"></span>
                  Altro
                </span>
              </nz-option>
            </nz-select>
          </nz-form-control>
        </nz-form-item>

        <!-- File Upload -->
        <nz-form-item>
          <nz-form-label [nzRequired]="true">
            <span style="display: flex; align-items: center; gap: 6px; font-weight: 700; font-size: 13px; color: #0c4a6e;">
              <span nz-icon nzType="file-add" nzTheme="outline" style="color: #14b8a6;"></span>
              File da caricare
            </span>
          </nz-form-label>
          <nz-form-control>
            <div style="padding: 24px; background: linear-gradient(135deg, #f0f9ff, #e0f2fe); border: 2px dashed #0ea5e9; border-radius: 12px; text-align: center; transition: all 0.3s ease;">
              <nz-upload
                [nzBeforeUpload]="beforeUpload"
                [nzShowUploadList]="true"
                [nzFileList]="fileList"
                [nzRemove]="handleRemove"
                nzAccept=".jpg,.jpeg,.png,.pdf,.gif"
                [nzMultiple]="false"
                nzListType="picture-card">
                <div style="padding: 16px;">
                  <span nz-icon nzType="cloud-upload" nzTheme="outline" style="font-size: 48px; color: #0284c7; margin-bottom: 12px; display: block;"></span>
                  <div style="font-size: 14px; font-weight: 600; color: #0c4a6e; margin-bottom: 8px;">Trascina file qui o clicca per selezionare</div>
                  <div style="font-size: 12px; color: #075985;">JPG, PNG, PDF, GIF • Max 10MB</div>
                </div>
              </nz-upload>
            </div>
          </nz-form-control>
        </nz-form-item>

        <!-- Note -->
        <nz-form-item>
          <nz-form-label>
            <span style="display: flex; align-items: center; gap: 6px; font-weight: 700; font-size: 13px; color: #0c4a6e;">
              <span nz-icon nzType="edit" nzTheme="outline" style="color: #14b8a6;"></span>
              Note (opzionale)
            </span>
          </nz-form-label>
          <nz-form-control>
            <textarea
              nz-input
              [(ngModel)]="notes"
              name="notes"
              [nzAutosize]="{ minRows: 3, maxRows: 5 }"
              placeholder="Aggiungi note o informazioni aggiuntive sul documento..."
              style="border-radius: 8px; border: 2px solid #e5e7eb; padding: 12px; font-size: 13px;">
            </textarea>
          </nz-form-control>
        </nz-form-item>

      </form>
    </div>
  `,
  styles: [`
    ::ng-deep .ant-select-selector {
      border-radius: 8px !important;
      border: 2px solid #e5e7eb !important;
      transition: all 0.3s ease !important;
    }
    
    ::ng-deep .ant-select-focused .ant-select-selector {
      border-color: #14b8a6 !important;
      box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1) !important;
    }

    ::ng-deep .ant-upload.ant-upload-select {
      width: 100% !important;
      margin: 0 !important;
    }

    ::ng-deep .ant-upload-list-picture-card-container {
      width: 100% !important;
      margin: 0 !important;
    }

    ::ng-deep textarea.ant-input:focus {
      border-color: #14b8a6 !important;
      box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1) !important;
    }
  `]
})
export class UploadDocumentModalComponent implements OnInit {
  DonationDocumentType = DonationDocumentType;
  
  documentType: DonationDocumentType = DonationDocumentType.Other;
  notes = '';
  fileList: NzUploadFile[] = [];
  selectedFile: File | null = null;

  constructor(
    private modal: NzModalRef,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {}

  beforeUpload = (file: File, fileList: File[]): boolean => {
    console.log('beforeUpload called with file:', file, 'fileList:', fileList);
    
    // Validate file size (10MB)
    const maxSize = 10 * 1024 * 1024;
    if (file.size && file.size > maxSize) {
      this.message.error('Il file supera il limite di 10MB');
      return false;
    }

    // Validate extension
    const allowedExtensions = ['.jpg', '.jpeg', '.png', '.pdf', '.gif'];
    const fileName = file.name.toLowerCase();
    const hasValidExtension = allowedExtensions.some(ext => fileName.endsWith(ext));
    
    if (!hasValidExtension) {
      this.message.error('Formato file non valido. Usa: JPG, PNG, PDF, GIF');
      return false;
    }

    // Store the actual File object
    this.selectedFile = file;
    this.fileList = [file as any];
    
    console.log('selectedFile set to:', this.selectedFile);
    console.log('selectedFile instanceof File:', this.selectedFile instanceof File);
    
    return false; // Prevent auto upload
  };

  handleRemove = (): boolean => {
    this.fileList = [];
    this.selectedFile = null;
    return true;
  };

  getResult() {
    console.log('getResult called, selectedFile:', this.selectedFile);
    
    if (!this.selectedFile) {
      this.message.error('Seleziona un file da caricare');
      return null;
    }

    const result = {
      file: this.selectedFile,
      documentType: this.documentType,
      notes: this.notes
    };
    
    console.log('getResult returning:', result);
    return result;
  }
}
