import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { LetterTemplateService } from '../../proxy/letter-templates/letter-template.service';
import {
  LetterTemplateDto,
  GetLetterTemplatesInput,
} from '../../proxy/letter-templates/dto/models';
import { TemplateCategory } from '../../proxy/enums/communications/template-category.enum';
import { CommunicationType } from '../../proxy/enums/communications/communication-type.enum';

@Component({
  selector: 'app-template-list',
  standalone: false,
  templateUrl: './template-list.component.html',
  styleUrls: ['./template-list.component.scss'],
})
export class TemplateListComponent implements OnInit {
  templates: LetterTemplateDto[] = [];
  loading = false;
  total = 0;
  pageSize = 10;
  pageIndex = 1;
  searchText = '';

  // Filters
  selectedCategory?: TemplateCategory;
  selectedLanguage?: string;
  selectedCommunicationType?: CommunicationType;
  selectedIsActive?: boolean;
  selectedIsDefault?: boolean;
  filterDrawerVisible = false;

  // Enum references for template
  TemplateCategory = TemplateCategory;
  CommunicationType = CommunicationType;

  // Preview modal
  isPreviewModalVisible = false;
  previewingTemplate: LetterTemplateDto | null = null;

  constructor(
    private templateService: LetterTemplateService,
    private message: NzMessageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadTemplates();
  }

  loadTemplates(): void {
    this.loading = true;

    const input: GetLetterTemplatesInput = {
      skipCount: (this.pageIndex - 1) * this.pageSize,
      maxResultCount: this.pageSize,
      filter: this.searchText || undefined,
      category: this.selectedCategory,
      language: this.selectedLanguage,
      isActive: this.selectedIsActive,
      isDefault: this.selectedIsDefault,
      communicationType: this.selectedCommunicationType,
    };

    this.templateService.getList(input).subscribe({
      next: result => {
        this.templates = result.items || [];
        this.total = result.totalCount || 0;
        this.loading = false;
      },
      error: err => {
        this.message.error('Errore nel caricamento dei template');
        this.loading = false;
        console.error(err);
      },
    });
  }

  onSearch(): void {
    this.pageIndex = 1;
    this.loadTemplates();
  }

  onPageChange(pageIndex: number): void {
    this.pageIndex = pageIndex;
    this.loadTemplates();
  }

  onPageSizeChange(pageSize: number): void {
    this.pageSize = pageSize;
    this.pageIndex = 1;
    this.loadTemplates();
  }

  showFilterDrawer(): void {
    this.filterDrawerVisible = true;
  }

  closeFilterDrawer(): void {
    this.filterDrawerVisible = false;
  }

  applyFilters(): void {
    this.pageIndex = 1;
    this.loadTemplates();
    this.closeFilterDrawer();
  }

  clearFilters(): void {
    this.selectedCategory = undefined;
    this.selectedLanguage = undefined;
    this.selectedCommunicationType = undefined;
    this.selectedIsActive = undefined;
    this.selectedIsDefault = undefined;
    this.searchText = '';
    this.applyFilters();
  }

  clearSearch(): void {
    this.searchText = '';
    this.onSearch();
  }

  hasActiveFilters(): boolean {
    return !!(
      this.searchText ||
      this.selectedCategory !== undefined ||
      this.selectedLanguage !== undefined ||
      this.selectedCommunicationType !== undefined ||
      this.selectedIsActive !== undefined ||
      this.selectedIsDefault !== undefined
    );
  }

  getActiveFiltersCount(): number {
    let count = 0;
    if (this.searchText) count++;
    if (this.selectedCategory !== undefined) count++;
    if (this.selectedLanguage !== undefined) count++;
    if (this.selectedCommunicationType !== undefined) count++;
    if (this.selectedIsActive !== undefined) count++;
    if (this.selectedIsDefault !== undefined) count++;
    return count;
  }

  // Statistics methods
  getActiveCount(): number {
    return this.templates.filter(t => t.isActive).length;
  }

  getDefaultCount(): number {
    return this.templates.filter(t => t.isDefault).length;
  }

  getUniqueCategoriesCount(): number {
    const categories = new Set(this.templates.map(t => t.category));
    return categories.size;
  }

  openCreatePage(): void {
    this.router.navigate(['/letter-templates/create']);
  }

  openEditPage(template: LetterTemplateDto): void {
    this.router.navigate(['/letter-templates', template.id, 'edit']);
  }

  duplicateTemplate(template: LetterTemplateDto): void {
    this.templateService.duplicate(template.id).subscribe({
      next: () => {
        this.message.success('Template duplicato con successo');
        this.loadTemplates();
      },
      error: err => {
        this.message.error('Errore nella duplicazione del template');
        console.error(err);
      },
    });
  }

  deleteTemplate(template: LetterTemplateDto): void {
    this.templateService.delete(template.id).subscribe({
      next: () => {
        this.message.success('Template eliminato con successo');
        this.loadTemplates();
      },
      error: err => {
        this.message.error("Errore nell'eliminazione del template");
        console.error(err);
      },
    });
  }

  openPreviewModal(template: LetterTemplateDto): void {
    this.templateService.get(template.id).subscribe({
      next: fullTemplate => {
        this.previewingTemplate = fullTemplate;
        this.isPreviewModalVisible = true;
      },
      error: err => {
        this.message.error('Errore nel caricamento del template');
        console.error(err);
      },
    });
  }

  closePreviewModal(): void {
    this.isPreviewModalVisible = false;
    this.previewingTemplate = null;
  }

  getCategoryLabel(category: TemplateCategory): string {
    const labels: Record<TemplateCategory, string> = {
      [TemplateCategory.ThankYou]: 'Ringraziamento',
      [TemplateCategory.Reminder]: 'Promemoria',
      [TemplateCategory.Newsletter]: 'Newsletter',
      [TemplateCategory.HolidayGreeting]: 'Auguri',
      [TemplateCategory.ProjectUpdate]: 'Aggiornamento Progetto',
      [TemplateCategory.Solicitation]: 'Sollecito',
      [TemplateCategory.Confirmation]: 'Conferma',
      [TemplateCategory.InformationRequest]: 'Richiesta Info',
      [TemplateCategory.Birthday]: 'Compleanno',
      [TemplateCategory.Anniversary]: 'Anniversario',
      [TemplateCategory.Survey]: 'Sondaggio',
      [TemplateCategory.EventInvitation]: 'Invito Evento',
      [TemplateCategory.FiscalReceipt]: 'Ricevuta Fiscale',
      [TemplateCategory.Other]: 'Altro',
    };
    return labels[category] ?? String(category);
  }

  getCommunicationTypeLabel(type?: CommunicationType): string {
    if (!type) return 'Entrambi';
    const labels: Record<CommunicationType, string> = {
      [CommunicationType.Email]: 'Email',
      [CommunicationType.Letter]: 'Lettera',
      [CommunicationType.SMS]: 'SMS',
      [CommunicationType.WhatsApp]: 'WhatsApp',
      [CommunicationType.PhoneCall]: 'Telefonata',
      [CommunicationType.PushNotification]: 'Notifica Push',
      [CommunicationType.InApp]: 'In-App',
      [CommunicationType.SocialMedia]: 'Social Media',
    };
    return labels[type] ?? String(type);
  }

  getCategoryColor(category: TemplateCategory): string {
    switch (category) {
      case TemplateCategory.ThankYou:
        return 'green';
      case TemplateCategory.Reminder:
        return 'orange';
      case TemplateCategory.Newsletter:
        return 'blue';
      case TemplateCategory.HolidayGreeting:
        return 'gold';
      case TemplateCategory.ProjectUpdate:
        return 'cyan';
      case TemplateCategory.Solicitation:
        return 'red';
      default:
        return 'default';
    }
  }

  getLanguageLabel(language: string): string {
    const labels: Record<string, string> = {
      it: 'Italiano',
      en: 'English',
    };
    return labels[language] ?? language;
  }
}
