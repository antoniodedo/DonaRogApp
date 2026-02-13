import { Component, Input, OnInit } from '@angular/core';
import { ProjectService } from '../../proxy/application/projects/project.service';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-project-statistics',
  standalone: false,
  templateUrl: './project-statistics.component.html',
  styleUrls: ['./project-statistics.component.scss']
})
export class ProjectStatisticsComponent implements OnInit {
  @Input() projectId: string = '';
  
  statistics: any = null;
  loading = false;

  constructor(
    private projectService: ProjectService,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    // Temporaneamente disabilitato - endpoint non ancora configurato
    // this.loadStatistics();
  }

  loadStatistics(): void {
    this.loading = true;
    this.projectService.getStatistics(this.projectId).subscribe({
      next: (result) => {
        this.statistics = result;
        this.loading = false;
      },
      error: (err) => {
        console.warn('Endpoint statistiche non ancora disponibile');
        this.loading = false;
      }
    });
  }
}
