import type { AddressType } from '../../enums/shared/address-type.enum';
import type { SubjectType } from '../../enums/donors/subject-type.enum';
import type { Gender } from '../../enums/donors/gender.enum';
import type { OrganizationType } from '../../enums/donors/organization-type.enum';
import type { LegalForm } from '../../enums/donors/legal-form.enum';
import type { DonorOrigin } from '../../enums/donors/donor-origin.enum';
import type { EmailType } from '../../enums/shared/email-type.enum';
import type { DonorStatus } from '../../enums/donors/donor-status.enum';
import type { DonorCategory } from '../../enums/donors/donor-category.enum';
import type { EntityDto } from '@abp/ng.core';

export interface AssignTagDto {
  tagId?: string;
}

export interface CreateDonorAddressDto {
  street?: string;
  city?: string;
  postalCode?: string;
  country?: string;
  type?: AddressType;
  province?: string;
  region?: string;
  notes?: string;
}

export interface CreateDonorDto {
  subjectType?: SubjectType;
  firstName?: string;
  lastName?: string;
  middleName?: string;
  titleId?: string;
  gender?: Gender;
  birthDate?: string;
  birthPlace?: string;
  taxCode?: string;
  companyName?: string;
  organizationType?: OrganizationType;
  legalForm?: LegalForm;
  businessSector?: string;
  vatNumber?: string;
  email?: string;
  phoneNumber?: string;
  origin?: DonorOrigin;
  preferredLanguage?: string;
  notes?: string;
}

export interface CreateDonorEmailDto {
  emailAddress?: string;
  type?: EmailType;
}

export interface DonorAddressDto {
  id?: string;
  street?: string;
  city?: string;
  province?: string;
  region?: string;
  postalCode?: string;
  country?: string;
  type?: AddressType;
  isDefault: boolean;
  isVerified: boolean;
  verifiedDate?: string;
  startDate?: string;
  endDate?: string;
  isActive: boolean;
  notes?: string;
}

export interface DonorDto {
  id?: string;
  donorCode?: string;
  subjectType?: SubjectType;
  firstName?: string;
  middleName?: string;
  lastName?: string;
  fullName?: string;
  gender?: Gender;
  birthDate?: string;
  birthPlace?: string;
  age?: number;
  titleId?: string;
  titleName?: string;
  companyName?: string;
  organizationType?: OrganizationType;
  legalForm?: LegalForm;
  businessSector?: string;
  taxCode?: string;
  vatNumber?: string;
  status?: DonorStatus;
  category?: DonorCategory;
  origin?: DonorOrigin;
  totalDonated: number;
  donationCount: number;
  averageDonationAmount: number;
  firstDonationAmount?: number;
  lastDonationAmount?: number;
  firstDonationDate?: string;
  lastDonationDate?: string;
  recencyScore: number;
  frequencyScore: number;
  monetaryScore: number;
  rfmSegment?: string;
  preferredLanguage?: string;
  preferredChannel?: string;
  lettersSentCount: number;
  emailsSentCount: number;
  lastThankYouLetterDate?: string;
  lastEmailSentDate?: string;
  privacyConsent: boolean;
  privacyConsentDate?: string;
  privacyConsentRevokedDate?: string;
  newsletterConsent: boolean;
  newsletterConsentDate?: string;
  phoneConsent: boolean;
  mailConsent: boolean;
  mailConsentDate?: string;
  isAnonymized: boolean;
  primaryEmail?: string;
  primaryAddress?: string;
  primaryCity?: string;
  creationTime?: string;
  lastModificationTime?: string;
  notes?: string;
}

export interface DonorEmailDto {
  id?: string;
  emailAddress?: string;
  type?: EmailType;
  isDefault: boolean;
  isVerified: boolean;
  verifiedDate?: string;
  bounceCount: number;
  lastBounceDate?: string;
  lastBounceReason?: string;
  isInvalid: boolean;
  dateAdded?: string;
  notes?: string;
}

export interface DonorStatusHistoryDto extends EntityDto<string> {
  donorId?: string;
  oldStatus: number;
  newStatus: number;
  note?: string;
  changedAt?: string;
  creationTime?: string;
  creatorId?: string;
}

export interface DonorTagDto {
  tagId?: string;
  tagName?: string;
  tagCode?: string;
  colorCode?: string;
  category?: string;
  taggedAt?: string;
  isAutomatic: boolean;
  taggingReason?: string;
  priority: number;
}

export interface UpdateDonorDto {
  firstName?: string;
  lastName?: string;
  middleName?: string;
  titleId?: string;
  gender?: Gender;
  birthDate?: string;
  birthPlace?: string;
  companyName?: string;
  organizationType?: OrganizationType;
  legalForm?: LegalForm;
  businessSector?: string;
  preferredLanguage?: string;
  preferredChannel?: string;
  notes?: string;
}

export interface DonorRfmStatisticsDto {
  totalDonors: number;
  activeDonors: number;
  lapsedDonors: number;
  retentionRate: number;
  attritionRate: number;
  championsCount: number;
  loyalCount: number;
  potentialCount: number;
  atRiskCount: number;
  dormantCount: number;
  lostCount: number;
}
