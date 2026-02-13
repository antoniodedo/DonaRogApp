import { Component, OnInit } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService } from 'ng-zorro-antd/modal';
import { TitleService } from '../../proxy/titles/title.service';
import type { TitleDto, TitleListWithStatsDto, DeleteTitlePreviewDto } from '../../proxy/titles/models';
import { Gender } from '../../proxy/enums/donors/gender.enum';

@Component({
  selector: 'app-title-list',
  standalone: false,
  templateUrl: './title-list.component.html',
  styleUrls: ['./title-list.component.scss']
})
export class TitleListComponent implements OnInit {
  loading = false;
  data: TitleListWithStatsDto | null = null;
  titles: TitleDto[] = [];
  
  // Form modale
  isFormVisible = false;
  editingTitle: TitleDto | null = null;

  // Delete modal
  isDeleteModalVisible = false;
  deletePreview: DeleteTitlePreviewDto | null = null;
  deletingTitleId: string | null = null;
  selectedReplacementTitleId: string | null = null;
  useDefaultReplacement = true;

  // Filtri
  showInactive = false;

  Gender = Gender;

  constructor(
    private titleService: TitleService,
    private message: NzMessageService,
    private modal: NzModalService
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading = true;
    this.titleService.getAllWithStats().subscribe({
      next: (result) => {
        this.data = result;
        this.filterTitles();
        this.loading = false;
      },
      error: () => {
        this.message.error('Errore nel caricamento dei titoli');
        this.loading = false;
      }
    });
  }

  filterTitles(): void {
    if (!this.data) return;
    this.titles = this.showInactive 
      ? this.data.items 
      : this.data.items.filter(t => t.isActive);
  }

  openCreateForm(): void {
    this.editingTitle = null;
    this.isFormVisible = true;
  }

  openEditForm(title: TitleDto): void {
    this.editingTitle = title;
    this.isFormVisible = true;
  }

  closeForm(): void {
    this.isFormVisible = false;
    this.editingTitle = null;
  }

  onFormSaved(): void {
    this.closeForm();
    this.loadData();
  }

  // Gestione eliminazione
  openDeleteModal(title: TitleDto): void {
    this.deletingTitleId = title.id;
    this.selectedReplacementTitleId = null;
    this.useDefaultReplacement = true;
    
    this.titleService.getDeletePreview(title.id).subscribe({
      next: (preview) => {
        this.deletePreview = preview;
        this.isDeleteModalVisible = true;
      },
      error: () => {
        this.message.error('Errore nel caricamento delle informazioni');
      }
    });
  }

  closeDeleteModal(): void {
    this.isDeleteModalVisible = false;
    this.deletePreview = null;
    this.deletingTitleId = null;
  }

  confirmDelete(): void {
    if (!this.deletingTitleId) return;

    const replacementId = this.useDefaultReplacement ? undefined : this.selectedReplacementTitleId || undefined;

    this.titleService.delete(this.deletingTitleId, replacementId).subscribe({
      next: (result) => {
        this.message.success(result.message);
        this.closeDeleteModal();
        this.loadData();
      },
      error: () => {
        this.message.error('Errore nella disattivazione del titolo');
      }
    });
  }

  // Riattivazione
  reactivate(title: TitleDto): void {
    this.titleService.reactivate(title.id).subscribe({
      next: () => {
        this.message.success('Titolo riattivato');
        this.loadData();
      },
      error: () => {
        this.message.error('Errore nella riattivazione');
      }
    });
  }

  // Imposta predefinito
  setAsDefault(title: TitleDto): void {
    this.titleService.setAsDefault(title.id).subscribe({
      next: () => {
        this.message.success(`${title.abbreviation} impostato come predefinito`);
        this.loadData();
      },
      error: (err) => {
        this.message.error(err.error?.error?.message || 'Errore');
      }
    });
  }

  removeDefault(title: TitleDto): void {
    this.titleService.removeDefault(title.id).subscribe({
      next: () => {
        this.message.success('Predefinito rimosso');
        this.loadData();
      },
      error: () => {
        this.message.error('Errore');
      }
    });
  }

  // Helper
  getGenderLabel(gender: Gender | undefined): string {
    switch (gender) {
      case Gender.Male: return 'M';
      case Gender.Female: return 'F';
      default: return '-';
    }
  }

  getGenderFullLabel(gender: Gender | undefined): string {
    switch (gender) {
      case Gender.Male: return 'Maschio';
      case Gender.Female: return 'Femmina';
      case Gender.Other: return 'Altro';
      default: return 'Non specificato';
    }
  }

  getGenderColor(gender: Gender | undefined): string {
    switch (gender) {
      case Gender.Male: return 'blue';
      case Gender.Female: return 'magenta';
      default: return 'default';
    }
  }
}
