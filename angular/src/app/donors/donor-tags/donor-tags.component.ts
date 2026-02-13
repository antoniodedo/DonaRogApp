import { Component, Input, OnInit } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { DonorService } from '../../proxy/donors/donor.service';
import { TagService } from '../../proxy/tags/tag.service';
import type { DonorTagDto } from '../../proxy/donors/dtos/models';
import type { TagDto } from '../../proxy/tags/dto/models';

@Component({
  selector: 'app-donor-tags',
  standalone: false,
  templateUrl: './donor-tags.component.html',
  styleUrls: ['./donor-tags.component.scss']
})
export class DonorTagsComponent implements OnInit {
  @Input() donorId!: string;

  donorTags: DonorTagDto[] = [];
  availableTags: TagDto[] = [];
  loading = false;

  // Modal
  isModalVisible = false;
  selectedTagId: string | null = null;

  constructor(
    private donorService: DonorService,
    private tagService: TagService,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.loadDonorTags();
    this.loadAvailableTags();
  }

  loadDonorTags(): void {
    this.loading = true;
    this.donorService.getTags(this.donorId).subscribe({
      next: (tags) => {
        this.donorTags = tags;
        this.loading = false;
      },
      error: () => {
        this.message.error('Errore nel caricamento dei tag');
        this.loading = false;
      }
    });
  }

  loadAvailableTags(): void {
    this.tagService.getActiveList().subscribe({
      next: (tags) => this.availableTags = tags,
      error: () => {}
    });
  }

  getUnassignedTags(): TagDto[] {
    const assignedIds = new Set(this.donorTags.map(t => t.tagId));
    return this.availableTags.filter(t => !assignedIds.has(t.id));
  }

  openAddModal(): void {
    this.selectedTagId = null;
    this.isModalVisible = true;
  }

  closeModal(): void {
    this.isModalVisible = false;
  }

  assignTag(): void {
    if (!this.selectedTagId) return;

    this.donorService.addTag(this.donorId, { tagId: this.selectedTagId }).subscribe({
      next: () => {
        this.message.success('Tag assegnato');
        this.closeModal();
        this.loadDonorTags();
      },
      error: (err) => {
        this.message.error(err?.error?.error?.message || 'Errore nell\'assegnazione');
      }
    });
  }

  removeTag(tag: DonorTagDto): void {
    this.donorService.deleteTag(this.donorId, tag.tagId).subscribe({
      next: () => {
        this.message.success('Tag rimosso');
        this.loadDonorTags();
      },
      error: (err) => {
        this.message.error(err?.error?.error?.message || 'Errore nella rimozione');
      }
    });
  }

  formatDate(date: string): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('it-IT');
  }

  getContrastColor(hex: string | undefined): string {
    if (!hex) return '#000';
    const r = parseInt(hex.slice(1, 3), 16);
    const g = parseInt(hex.slice(3, 5), 16);
    const b = parseInt(hex.slice(5, 7), 16);
    const luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;
    return luminance > 0.5 ? '#000' : '#fff';
  }
}
