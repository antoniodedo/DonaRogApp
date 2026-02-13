import { RestService, Rest } from '@abp/ng.core';
import type { ListResultDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { CreateProjectDocumentDto, CreateProjectDto, GetProjectsInput, ProjectDocumentDto, ProjectDto, ProjectListDto, ProjectStatisticsDto, UpdateProjectDocumentDto, UpdateProjectDto } from '../contracts/projects/dto/models';
import type { ProjectAggregateStatisticsDto } from '../contracts/projects/models';
import type { ProjectStatus } from '../../enums/projects/project-status.enum';

@Injectable({
  providedIn: 'root',
})
export class ProjectService {
  apiName = 'Default';
  

  activate = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/project/${id}/activate`,
    },
    { apiName: this.apiName,...config });
  

  addDocument = (projectId: string, input: CreateProjectDocumentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProjectDocumentDto>({
      method: 'POST',
      url: `/api/app/project/document/${projectId}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  archive = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/project/${id}/archive`,
    },
    { apiName: this.apiName,...config });
  

  changeStatus = (id: string, status: ProjectStatus, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/project/${id}/change-status`,
      params: { status },
    },
    { apiName: this.apiName,...config });
  

  create = (input: CreateProjectDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProjectDto>({
      method: 'POST',
      url: '/api/app/project',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  deactivate = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/project/${id}/deactivate`,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/project/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProjectDto>({
      method: 'GET',
      url: `/api/app/project/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getAggregateStatistics = (input?: GetProjectsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProjectAggregateStatisticsDto>({
      method: 'GET',
      url: '/api/app/project/aggregate-statistics',
      params: { filter: input.filter, status: input.status, category: input.category, startDateFrom: input.startDateFrom, startDateTo: input.startDateTo, endDateFrom: input.endDateFrom, endDateTo: input.endDateTo, hasBudget: input.hasBudget, onlyOngoing: input.onlyOngoing, responsiblePerson: input.responsiblePerson, location: input.location, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getDocuments = (projectId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListResultDto<ProjectDocumentDto>>({
      method: 'GET',
      url: `/api/app/project/documents/${projectId}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetProjectsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ProjectDto>>({
      method: 'GET',
      url: '/api/app/project',
      params: { filter: input.filter, status: input.status, category: input.category, startDateFrom: input.startDateFrom, startDateTo: input.startDateTo, endDateFrom: input.endDateFrom, endDateTo: input.endDateTo, hasBudget: input.hasBudget, onlyOngoing: input.onlyOngoing, responsiblePerson: input.responsiblePerson, location: input.location, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getProjectList = (input: GetProjectsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ProjectListDto>>({
      method: 'GET',
      url: '/api/app/project/project-list',
      params: { filter: input.filter, status: input.status, category: input.category, startDateFrom: input.startDateFrom, startDateTo: input.startDateTo, endDateFrom: input.endDateFrom, endDateTo: input.endDateTo, hasBudget: input.hasBudget, onlyOngoing: input.onlyOngoing, responsiblePerson: input.responsiblePerson, location: input.location, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getStatistics = (projectId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProjectStatisticsDto>({
      method: 'GET',
      url: `/api/app/project/statistics/${projectId}`,
    },
    { apiName: this.apiName,...config });
  

  removeDocument = (projectId: string, documentId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/project/document',
      params: { projectId, documentId },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateProjectDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProjectDto>({
      method: 'PUT',
      url: `/api/app/project/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateDocument = (projectId: string, documentId: string, input: UpdateProjectDocumentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProjectDocumentDto>({
      method: 'PUT',
      url: '/api/app/project/document',
      params: { projectId, documentId },
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
