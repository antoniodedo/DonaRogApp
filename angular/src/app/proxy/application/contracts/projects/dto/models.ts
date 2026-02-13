import type { ProjectCategory } from '../../../../enums/projects/project-category.enum';
import type { EntityDto, FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { ProjectStatus } from '../../../../enums/projects/project-status.enum';

export interface CreateProjectDocumentDto {
  fileName?: string;
  fileUrl?: string;
  fileType?: string;
  fileSize: number;
  description?: string;
}

export interface CreateProjectDto {
  code: string;
  name: string;
  description?: string;
  category: ProjectCategory;
  startDate: string;
  endDate?: string;
  targetAmount?: number;
  currency?: string;
  responsiblePerson?: string;
  responsibleEmail?: string;
  responsiblePhone?: string;
  mainImageUrl?: string;
  thumbnailUrl?: string;
  location?: string;
  latitude?: number;
  longitude?: number;
}

export interface GetProjectsInput extends PagedAndSortedResultRequestDto {
  filter?: string;
  status?: ProjectStatus;
  category?: ProjectCategory;
  startDateFrom?: string;
  startDateTo?: string;
  endDateFrom?: string;
  endDateTo?: string;
  hasBudget?: boolean;
  onlyOngoing?: boolean;
  responsiblePerson?: string;
  location?: string;
}

export interface MonthlyTrendDto {
  year: number;
  month: number;
  amountRaised: number;
  donationsCount: number;
}

export interface ProjectDocumentDto extends FullAuditedEntityDto<string> {
  projectId?: string;
  fileName?: string;
  fileUrl?: string;
  fileType?: string;
  fileSize: number;
  description?: string;
  displayOrder: number;
}

export interface ProjectDto extends FullAuditedEntityDto<string> {
  code?: string;
  name?: string;
  description?: string;
  category?: ProjectCategory;
  status?: ProjectStatus;
  startDate?: string;
  endDate?: string;
  targetAmount?: number;
  currency?: string;
  responsiblePerson?: string;
  responsibleEmail?: string;
  responsiblePhone?: string;
  mainImageUrl?: string;
  thumbnailUrl?: string;
  location?: string;
  latitude?: number;
  longitude?: number;
  totalAmountRaised: number;
  totalDonationsCount: number;
  averageDonation: number;
  lastDonationDate?: string;
  targetCompletionPercentage: number;
  remainingAmount: number;
  hasReachedTarget: boolean;
  isOngoing: boolean;
  documents: ProjectDocumentDto[];
}

export interface ProjectListDto extends EntityDto<string> {
  code?: string;
  name?: string;
  category?: ProjectCategory;
  status?: ProjectStatus;
  startDate?: string;
  endDate?: string;
  targetAmount?: number;
  currency?: string;
  totalAmountRaised: number;
  totalDonationsCount: number;
  targetCompletionPercentage: number;
  thumbnailUrl?: string;
  responsiblePerson?: string;
  location?: string;
  isOngoing: boolean;
  creationTime?: string;
}

export interface ProjectStatisticsDto {
  projectId?: string;
  code?: string;
  name?: string;
  totalAmountRaised: number;
  totalDonationsCount: number;
  averageDonation: number;
  largestDonation: number;
  smallestDonation: number;
  lastDonationDate?: string;
  firstDonationDate?: string;
  targetAmount?: number;
  remainingAmount: number;
  targetCompletionPercentage: number;
  hasReachedTarget: boolean;
  yearlyStatistics: YearlyStatisticsDto[];
  topDonors: ProjectTopDonorDto[];
  monthlyTrend: MonthlyTrendDto[];
}

export interface ProjectTopDonorDto {
  donorId?: string;
  donorName?: string;
  totalAmount: number;
  donationsCount: number;
  lastDonationDate?: string;
}

export interface UpdateProjectDocumentDto {
  fileName?: string;
  description?: string;
  displayOrder: number;
}

export interface UpdateProjectDto {
  code: string;
  name: string;
  description?: string;
  category: ProjectCategory;
  status: ProjectStatus;
  startDate: string;
  endDate?: string;
  targetAmount?: number;
  currency?: string;
  responsiblePerson?: string;
  responsibleEmail?: string;
  responsiblePhone?: string;
  mainImageUrl?: string;
  thumbnailUrl?: string;
  location?: string;
  latitude?: number;
  longitude?: number;
}

export interface YearlyStatisticsDto {
  year: number;
  amountRaised: number;
  donationsCount: number;
  averageDonation: number;
}
