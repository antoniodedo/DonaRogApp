import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProjectService } from '../../proxy/application/projects/project.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { ProjectDocumentDto, CreateProjectDocumentDto } from '../../proxy/application/contracts/projects/dto/models';

@Component({
  selector: 'app-project-documents',
  standalone: false,
  templateUrl: './project-documents.component.html',
  styleUrls: ['./project-documents.component.scss']
})
export class ProjectDocumentsComponent implements OnInit {
  @Input() projectId: string = '';
  
  documents: ProjectDocumentDto[] = [];
  loading = false;

  // Document form modal
  isDocumentModalVisible = false;
  documentForm!: FormGroup;
  savingDocument = false;

  constructor(
    private projectService: ProjectService,
    private message: NzMessageService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.initDocumentForm();
    // Temporaneamente disabilitato - endpoint non ancora configurato
    // this.loadDocuments();
  }

  private initDocumentForm(): void {
    this.documentForm = this.fb.group({
      fileName: ['', [Validators.required, Validators.maxLength(255)]],
      fileUrl: ['', [Validators.required, Validators.maxLength(1000)]],
      fileType: [''],
      fileSize: [0, [Validators.required, Validators.min(1)]],
      description: ['', Validators.maxLength(500)]
    });
  }

  loadDocuments(): void {
    this.loading = true;
    this.projectService.getDocuments(this.projectId).subscribe({
      next: (result) => {
        this.documents = result.items || [];
        this.loading = false;
      },
      error: (err) => {
        console.warn('Endpoint documenti non ancora disponibile');
        this.loading = false;
      }
    });
  }

  openAddDocumentModal(): void {
    this.documentForm.reset({
      fileName: '',
      fileUrl: '',
      fileType: 'application/pdf',
      fileSize: 0,
      description: ''
    });
    this.isDocumentModalVisible = true;
  }

  closeDocumentModal(): void {
    this.isDocumentModalVisible = false;
  }

  saveDocument(): void {
    if (this.documentForm.invalid) {
      Object.keys(this.documentForm.controls).forEach(key => {
        const control = this.documentForm.get(key);
        control?.markAsDirty();
        control?.updateValueAndValidity();
      });
      return;
    }

    this.savingDocument = true;
    const dto: CreateProjectDocumentDto = this.documentForm.value;

    this.projectService.addDocument(this.projectId, dto).subscribe({
      next: () => {
        this.message.success('Documento aggiunto con successo');
        this.savingDocument = false;
        this.closeDocumentModal();
        this.loadDocuments();
      },
      error: (err) => {
        this.message.error('Errore nell\'aggiunta del documento');
        this.savingDocument = false;
      }
    });
  }

  deleteDocument(document: ProjectDocumentDto): void {
    this.projectService.removeDocument(this.projectId, document.id).subscribe({
      next: () => {
        this.message.success('Documento eliminato');
        this.loadDocuments();
      },
      error: (err) => {
        this.message.error('Errore nell\'eliminazione del documento');
      }
    });
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  }
}
