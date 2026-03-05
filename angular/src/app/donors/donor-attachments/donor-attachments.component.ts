import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService } from 'ng-zorro-antd/modal';
import { 
  DonorAttachmentService, 
  DonorAttachmentDto, 
  CreateDonorAttachmentDto 
} from '../../proxy/donors/donor-attachment.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-donor-attachments',
  templateUrl: './donor-attachments.component.html',
  styleUrls: ['./donor-attachments.component.scss'],
  standalone: false
})
export class DonorAttachmentsComponent implements OnInit {
  @Input() donorId!: string;

  attachments: DonorAttachmentDto[] = [];
  loading = false;
  uploading = false;
  
  uploadForm!: FormGroup;
  selectedFile: File | null = null;
  showUploadModal = false;
  
  editingAttachment: DonorAttachmentDto | null = null;
  editForm!: FormGroup;
  showEditModal = false;

  attachmentTypes = [
    { value: 'Letter', label: 'Lettera' },
    { value: 'Drawing', label: 'Disegno' },
    { value: 'Document', label: 'Documento' },
    { value: 'Photo', label: 'Foto' },
    { value: 'Other', label: 'Altro' }
  ];

  constructor(
    private attachmentService: DonorAttachmentService,
    private fb: FormBuilder,
    private modal: NzModalService,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.buildForms();
    this.loadAttachments();
  }

  buildForms(): void {
    this.uploadForm = this.fb.group({
      attachmentType: [null],
      description: [''],
      displayOrder: [0]
    });

    this.editForm = this.fb.group({
      attachmentType: [null],
      description: [''],
      displayOrder: [0, Validators.required]
    });
  }

  loadAttachments(): void {
    this.loading = true;
    this.attachmentService.getListByDonor(this.donorId)
      .pipe(finalize(() => this.loading = false))
      .subscribe(response => {
        this.attachments = response.items;
      });
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
    }
  }

  openUploadModal(): void {
    this.uploadForm.reset({ displayOrder: this.attachments.length });
    this.selectedFile = null;
    this.showUploadModal = true;
  }

  closeUploadModal(): void {
    this.showUploadModal = false;
    this.uploadForm.reset();
    this.selectedFile = null;
  }

  uploadAttachment(): void {
    if (!this.selectedFile) {
      this.message.error('Seleziona un file da caricare');
      return;
    }

    const formValue = this.uploadForm.value;
    const input: CreateDonorAttachmentDto = {
      donorId: this.donorId,
      fileName: this.selectedFile.name,
      attachmentType: formValue.attachmentType,
      description: formValue.description,
      displayOrder: formValue.displayOrder || 0
    };

    this.uploading = true;
    this.attachmentService.create(input, this.selectedFile)
      .pipe(finalize(() => this.uploading = false))
      .subscribe({
        next: () => {
          this.message.success('Allegato caricato con successo');
          this.closeUploadModal();
          this.loadAttachments();
        },
        error: (error) => {
          this.message.error(error?.error?.error?.message || 'Errore durante il caricamento');
        }
      });
  }

  openEditModal(attachment: DonorAttachmentDto): void {
    this.editingAttachment = attachment;
    this.editForm.patchValue({
      attachmentType: attachment.attachmentType,
      description: attachment.description,
      displayOrder: attachment.displayOrder
    });
    this.showEditModal = true;
  }

  closeEditModal(): void {
    this.showEditModal = false;
    this.editingAttachment = null;
    this.editForm.reset();
  }

  saveEdit(): void {
    if (!this.editingAttachment || this.editForm.invalid) {
      return;
    }

    const input = this.editForm.value;
    
    this.attachmentService.update(this.editingAttachment.id, input)
      .subscribe({
        next: () => {
          this.message.success('Allegato aggiornato con successo');
          this.closeEditModal();
          this.loadAttachments();
        },
        error: (error) => {
          this.message.error(error?.error?.error?.message || 'Errore durante l\'aggiornamento');
        }
      });
  }

  deleteAttachment(attachment: DonorAttachmentDto): void {
    this.modal.confirm({
      nzTitle: 'Conferma eliminazione',
      nzContent: `Sei sicuro di voler eliminare l'allegato "${attachment.fileName}"?`,
      nzOkText: 'Elimina',
      nzOkDanger: true,
      nzCancelText: 'Annulla',
      nzOnOk: () => {
        this.attachmentService.delete(attachment.id).subscribe({
          next: () => {
            this.message.success('Allegato eliminato con successo');
            this.loadAttachments();
          },
          error: (error) => {
            this.message.error(error?.error?.error?.message || 'Errore durante l\'eliminazione');
          }
        });
      }
    });
  }

  downloadAttachment(attachment: DonorAttachmentDto): void {
    const url = this.attachmentService.getDownloadUrl(attachment.id);
    window.open(url, '_blank');
  }

  getFileIcon(attachment: DonorAttachmentDto): string {
    const ext = attachment.fileExtension.toLowerCase();
    if (['.jpg', '.jpeg', '.png', '.gif', '.bmp', '.webp'].includes(ext)) {
      return 'fa-file-image';
    } else if (['.pdf'].includes(ext)) {
      return 'fa-file-pdf';
    } else if (['.doc', '.docx'].includes(ext)) {
      return 'fa-file-word';
    } else if (['.xls', '.xlsx'].includes(ext)) {
      return 'fa-file-excel';
    } else if (['.zip', '.rar', '.7z'].includes(ext)) {
      return 'fa-file-archive';
    }
    return 'fa-file';
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  }

  getAttachmentTypeLabel(type: string | undefined): string {
    if (!type) return '';
    const found = this.attachmentTypes.find(t => t.value === type);
    return found ? found.label : type;
  }
}
