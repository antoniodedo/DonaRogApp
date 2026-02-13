import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { SubjectType } from '../../enums/donors/subject-type.enum';
import type { DonorStatus } from '../../enums/donors/donor-status.enum';
import type { DonorCategory } from '../../enums/donors/donor-category.enum';

export interface GetDonorsInput extends PagedAndSortedResultRequestDto {
  filter?: string;
  subjectType?: SubjectType;
  status?: DonorStatus;
  category?: DonorCategory;
  titleId?: string;
  donorCode?: string;
  email?: string;
  phoneNumber?: string;
  city?: string;
  postalCode?: string;
  province?: string;
  country?: string;
}
