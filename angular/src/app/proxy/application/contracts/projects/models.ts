import type { ProjectCategory } from '../../../enums/projects/project-category.enum';

export interface CategoryDistributionDto {
  category?: ProjectCategory;
  projectCount: number;
  totalAmountRaised: number;
  totalTargetAmount: number;
}

export interface ProjectAggregateStatisticsDto {
  totalProjects: number;
  activeProjects: number;
  inactiveProjects: number;
  archivedProjects: number;
  totalAmountRaised: number;
  totalTargetAmount: number;
  totalDonationsCount: number;
  averageDonation: number;
  topProjectsByAmount: TopProjectDto[];
  topProjectsByDonations: TopProjectDto[];
  categoryDistribution: CategoryDistributionDto[];
  projectsNearTarget: number;
  projectsUnderFunded: number;
}

export interface TopProjectDto {
  id?: string;
  code?: string;
  name?: string;
  totalAmountRaised: number;
  totalDonationsCount: number;
  targetCompletionPercentage: number;
}
