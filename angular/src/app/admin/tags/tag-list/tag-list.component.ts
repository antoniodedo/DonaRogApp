import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { TagService } from '../../../proxy/tags/tag.service';
import type { TagDto, CreateUpdateTagDto } from '../../../proxy/tags/dto/models';

@Component({
  selector: 'app-tag-list',
  standalone: false,
  templateUrl: './tag-list.component.html',
  styleUrls: ['./tag-list.component.scss']
})
export class TagListComponent implements OnInit {
  tags: TagDto[] = [];
  loading = false;
  totalCount = 0;
  pageIndex = 1;
  pageSize = 10;

  // Modal
  isModalVisible = false;
  isEditing = false;
  editingTag: TagDto | null = null;
  form!: FormGroup;
  saving = false;

  // Categorie esistenti per autocomplete
  categories: string[] = [];

  // Colori predefiniti
  presetColors = [
    '#f5222d', '#fa541c', '#fa8c16', '#faad14', '#fadb14',
    '#a0d911', '#52c41a', '#13c2c2', '#1890ff', '#2f54eb',
    '#722ed1', '#eb2f96', '#8c8c8c', '#434343'
  ];

  constructor(
    private tagService: TagService,
    private message: NzMessageService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadTags();
    this.loadCategories();
  }

  private initForm(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(64)]],
      code: ['', [Validators.required, Validators.maxLength(32)]],
      description: ['', Validators.maxLength(256)],
      colorCode: ['#1890ff'],
      category: [''],
      displayOrder: [0],
      isActive: [true]
    });
  }

  loadTags(): void {
    this.loading = true;
    this.tagService.getList({
      skipCount: (this.pageIndex - 1) * this.pageSize,
      maxResultCount: this.pageSize,
      sorting: 'Name'
    }).subscribe({
      next: (result) => {
        this.tags = result.items;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: () => {
        this.message.error('Errore nel caricamento dei tag');
        this.loading = false;
      }
    });
  }

  loadCategories(): void {
    this.tagService.getCategories().subscribe({
      next: (cats) => this.categories = cats,
      error: () => {}
    });
  }

  onPageChange(page: number): void {
    this.pageIndex = page;
    this.loadTags();
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageIndex = 1;
    this.loadTags();
  }

  openCreateModal(): void {
    this.isEditing = false;
    this.editingTag = null;
    this.form.reset({ 
      colorCode: '#1890ff',
      displayOrder: 0,
      isActive: true
    });
    this.isModalVisible = true;
  }

  openEditModal(tag: TagDto): void {
    this.isEditing = true;
    this.editingTag = tag;
    this.form.patchValue({
      name: tag.name,
      code: tag.code,
      description: tag.description,
      colorCode: tag.colorCode || '#1890ff',
      category: tag.category,
      displayOrder: 0,
      isActive: tag.isActive
    });
    this.isModalVisible = true;
  }

  closeModal(): void {
    this.isModalVisible = false;
    this.editingTag = null;
  }

  save(): void {
    if (this.form.invalid) {
      Object.keys(this.form.controls).forEach(key => {
        const control = this.form.get(key);
        control?.markAsDirty();
        control?.updateValueAndValidity();
      });
      return;
    }

    this.saving = true;
    const dto: CreateUpdateTagDto = this.form.value;

    const request = this.isEditing
      ? this.tagService.update(this.editingTag!.id, dto)
      : this.tagService.create(dto);

    request.subscribe({
      next: () => {
        this.message.success(this.isEditing ? 'Tag aggiornato' : 'Tag creato');
        this.closeModal();
        this.loadTags();
        this.loadCategories();
        this.saving = false;
      },
      error: (err) => {
        this.message.error(err?.error?.error?.message || 'Errore nel salvataggio');
        this.saving = false;
      }
    });
  }

  toggleActive(tag: TagDto): void {
    const request = tag.isActive
      ? this.tagService.deactivate(tag.id)
      : this.tagService.activate(tag.id);

    request.subscribe({
      next: () => {
        this.message.success(tag.isActive ? 'Tag disattivato' : 'Tag attivato');
        this.loadTags();
      },
      error: (err) => {
        this.message.error(err?.error?.error?.message || 'Errore');
      }
    });
  }

  selectColor(color: string): void {
    this.form.patchValue({ colorCode: color });
  }

  getContrastColor(hex: string): string {
    if (!hex) return '#000';
    const r = parseInt(hex.slice(1, 3), 16);
    const g = parseInt(hex.slice(3, 5), 16);
    const b = parseInt(hex.slice(5, 7), 16);
    const luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;
    return luminance > 0.5 ? '#000' : '#fff';
  }

  // Statistics methods
  getActiveCount(): number {
    return this.tags.filter(t => t.isActive).length;
  }

  getCategoriesCount(): number {
    const uniqueCategories = new Set(
      this.tags
        .filter(t => t.category)
        .map(t => t.category!)
    );
    return uniqueCategories.size;
  }

  getTotalUsage(): number {
    return this.tags.reduce((sum, tag) => sum + (tag.usageCount || 0), 0);
  }

  get modalTitle(): string {
    return this.isEditing ? 'Modifica Tag' : 'Nuovo Tag';
  }

  handleCancel(): void {
    this.closeModal();
  }

  handleSave(): void {
    this.save();
  }

  deleteTag(id: string): void {
    this.tagService.delete(id).subscribe({
      next: () => {
        this.message.success('Tag eliminato');
        this.loadTags();
      },
      error: (err) => {
        this.message.error(err?.error?.error?.message || 'Errore nell\'eliminazione');
      }
    });
  }
}
