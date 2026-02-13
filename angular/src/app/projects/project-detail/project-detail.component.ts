import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { ProjectService } from '../../proxy/application/projects/project.service';
import { ProjectDto } from '../../proxy/application/contracts/projects/dto/models';
import { ProjectStatus } from '../../proxy/enums/projects/project-status.enum';
import { ProjectCategory } from '../../proxy/enums/projects/project-category.enum';

@Component({
  selector: 'app-project-detail',
  standalone: false,
  templateUrl: './project-detail.component.html',
  styleUrls: ['./project-detail.component.scss']
})
export class ProjectDetailComponent implements OnInit {
  project: ProjectDto | null = null;
  loading = false;
  projectId: string = '';
  selectedTabIndex = 0;

  // Form modal
  isFormModalVisible = false;

  ProjectStatus = ProjectStatus;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private projectService: ProjectService,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.projectId = this.route.snapshot.params['id'];
    this.loadProject();
  }

  loadProject(): void {
    this.loading = true;
    this.projectService.get(this.projectId).subscribe({
      next: (result) => {
        this.project = result;
        this.loading = false;
      },
      error: (err) => {
        this.message.error('Errore nel caricamento del progetto');
        this.loading = false;
        this.router.navigate(['/admin/projects']);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/admin/projects']);
  }

  editProject(): void {
    this.isFormModalVisible = true;
  }

  closeFormModal(): void {
    this.isFormModalVisible = false;
  }

  onFormSaved(): void {
    this.closeFormModal();
    this.loadProject();
  }

  deleteProject(): void {
    this.projectService.delete(this.projectId).subscribe({
      next: () => {
        this.message.success('Progetto eliminato con successo');
        this.router.navigate(['/admin/projects']);
      },
      error: (err) => {
        this.message.error('Errore nell\'eliminazione del progetto');
      }
    });
  }

  changeStatus(status: ProjectStatus): void {
    this.projectService.changeStatus(this.projectId, status).subscribe({
      next: () => {
        this.message.success('Stato del progetto aggiornato');
        this.loadProject();
      },
      error: (err) => {
        this.message.error('Errore nell\'aggiornamento dello stato');
      }
    });
  }

  getStatusLabel(status: ProjectStatus): string {
    const labels: Record<ProjectStatus, string> = {
      [ProjectStatus.Active]: 'Attivo',
      [ProjectStatus.Inactive]: 'Inattivo',
      [ProjectStatus.Archived]: 'Archiviato',
    };
    return labels[status] ?? String(status);
  }

  getCategoryLabel(category: ProjectCategory): string {
    const labels: Record<ProjectCategory, string> = {
      [ProjectCategory.Education]: 'Educazione',
      [ProjectCategory.Health]: 'Salute',
      [ProjectCategory.Environment]: 'Ambiente',
      [ProjectCategory.SocialWelfare]: 'Assistenza sociale',
      [ProjectCategory.Emergency]: 'Emergenze',
      [ProjectCategory.Infrastructure]: 'Infrastrutture',
      [ProjectCategory.Culture]: 'Cultura',
      [ProjectCategory.Research]: 'Ricerca',
      [ProjectCategory.Sports]: 'Sport',
      [ProjectCategory.Other]: 'Altro',
    };
    return labels[category] ?? String(category);
  }
}
