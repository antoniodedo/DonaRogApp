import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { ProjectService } from '../../proxy/application/projects/project.service';
import { ProjectListDto, ProjectDto, GetProjectsInput } from '../../proxy/application/contracts/projects/dto/models';
import { ProjectStatus } from '../../proxy/enums/projects/project-status.enum';
import { ProjectCategory } from '../../proxy/enums/projects/project-category.enum';

@Component({
  selector: 'app-project-list',
  standalone: false,
  templateUrl: './project-list.component.html',
  styleUrls: ['./project-list.component.scss']
})
export class ProjectListComponent implements OnInit {
  projects: ProjectListDto[] = [];
  loading = false;
  total = 0;
  pageSize = 10;
  pageIndex = 1;
  searchText = '';

  // Filters
  selectedStatus?: ProjectStatus;
  selectedCategory?: ProjectCategory;
  filterDrawerVisible = false;

  // Enum references for template
  ProjectStatus = ProjectStatus;
  ProjectCategory = ProjectCategory;

  // Aggregate statistics
  aggregateStats: any = null;

  // Form modal
  isFormModalVisible = false;
  editingProject: ProjectDto | null = null;

  constructor(
    private projectService: ProjectService,
    private message: NzMessageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadProjects();
    this.loadAggregateStatistics();
  }

  loadProjects(): void {
    this.loading = true;
    
    const input: GetProjectsInput = {
      skipCount: (this.pageIndex - 1) * this.pageSize,
      maxResultCount: this.pageSize,
      filter: this.searchText || undefined,
      status: this.selectedStatus,
      category: this.selectedCategory
    };

    this.projectService.getProjectList(input).subscribe({
      next: (result) => {
        this.projects = result.items || [];
        this.total = result.totalCount || 0;
        this.loading = false;
      },
      error: (err) => {
        this.message.error('Errore nel caricamento dei progetti');
        this.loading = false;
      }
    });
  }

  loadAggregateStatistics(): void {
    this.projectService.getAggregateStatistics({} as GetProjectsInput).subscribe({
      next: (stats) => {
        this.aggregateStats = stats;
      },
      error: (err) => {
        console.error('Errore nel caricamento delle statistiche aggregate', err);
      }
    });
  }

  onSearch(): void {
    this.pageIndex = 1;
    this.loadProjects();
  }

  onPageChange(pageIndex: number): void {
    this.pageIndex = pageIndex;
    this.loadProjects();
  }

  onPageSizeChange(pageSize: number): void {
    this.pageSize = pageSize;
    this.pageIndex = 1;
    this.loadProjects();
  }

  showFilterDrawer(): void {
    this.filterDrawerVisible = true;
  }

  closeFilterDrawer(): void {
    this.filterDrawerVisible = false;
  }

  applyFilters(): void {
    this.pageIndex = 1;
    this.loadProjects();
    this.closeFilterDrawer();
  }

  clearFilters(): void {
    this.selectedStatus = undefined;
    this.selectedCategory = undefined;
    this.searchText = '';
    this.applyFilters();
  }

  viewProject(project: ProjectListDto): void {
    this.router.navigate(['/admin/projects', project.id]);
  }

  openCreateModal(): void {
    this.editingProject = null;
    this.isFormModalVisible = true;
  }

  openEditModal(project: ProjectListDto): void {
    // Load full project data for editing
    this.projectService.get(project.id).subscribe({
      next: (fullProject) => {
        this.editingProject = fullProject;
        this.isFormModalVisible = true;
      },
      error: (err) => {
        this.message.error('Errore nel caricamento del progetto');
      }
    });
  }

  closeFormModal(): void {
    this.isFormModalVisible = false;
    this.editingProject = null;
  }

  onFormSaved(): void {
    this.closeFormModal();
    this.loadProjects();
    this.loadAggregateStatistics();
  }

  deleteProject(project: ProjectListDto): void {
    this.projectService.delete(project.id).subscribe({
      next: () => {
        this.message.success('Progetto eliminato con successo');
        this.loadProjects();
        this.loadAggregateStatistics();
      },
      error: (err) => {
        this.message.error('Errore nell\'eliminazione del progetto');
      }
    });
  }

  archiveProject(project: ProjectListDto): void {
    this.projectService.archive(project.id).subscribe({
      next: () => {
        this.message.success('Progetto archiviato con successo');
        this.loadProjects();
        this.loadAggregateStatistics();
      },
      error: (err) => {
        this.message.error('Errore nell\'archiviazione del progetto');
      }
    });
  }

  getStatusColor(status: ProjectStatus): string {
    switch (status) {
      case ProjectStatus.Active:
        return 'green';
      case ProjectStatus.Inactive:
        return 'orange';
      case ProjectStatus.Archived:
        return 'default';
      default:
        return 'default';
    }
  }

  getCategoryColor(category: ProjectCategory): string {
    switch (category) {
      case ProjectCategory.Education:
        return 'blue';
      case ProjectCategory.Health:
        return 'red';
      case ProjectCategory.Environment:
        return 'green';
      case ProjectCategory.SocialWelfare:
        return 'purple';
      case ProjectCategory.Emergency:
        return 'volcano';
      default:
        return 'default';
    }
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
