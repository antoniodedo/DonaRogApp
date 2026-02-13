import type { Gender } from '../enums/donors/gender.enum';

export interface AffectedDonorsByGenderDto {
  gender?: Gender;
  count: number;
  defaultTitleId?: string;
  defaultTitleName?: string;
}

export interface CreateUpdateTitleDto {
  code?: string;
  name?: string;
  abbreviation?: string;
  associatedGender?: Gender;
  displayOrder: number;
  notes?: string;
}

export interface DeleteTitlePreviewDto {
  titleId?: string;
  titleName?: string;
  totalAffectedDonors: number;
  affectedByGender: AffectedDonorsByGenderDto[];
  alternativeTitles: TitleDto[];
}

export interface DeleteTitleResultDto {
  success: boolean;
  affectedDonors: number;
  message?: string;
}

export interface TitleDto {
  id?: string;
  code?: string;
  name?: string;
  abbreviation?: string;
  associatedGender?: Gender;
  displayOrder: number;
  isDefault: boolean;
  isActive: boolean;
  notes?: string;
}

export interface TitleListWithStatsDto {
  items: TitleWithStatsDto[];
  donorsWithoutTitle: number;
  totalDonors: number;
}

export interface TitleWithStatsDto extends TitleDto {
  donorCount: number;
}
