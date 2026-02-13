import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export enum DonationChannel {
  BankTransfer = 1,
  PostalOrder = 2,
  PostalOrderTelematic = 3,
  CreditCard = 4,
  DirectDebit = 5,
  Cash = 6,
  Check = 7,
  PayPal = 8,
  Stripe = 9,
  Bequest = 10,
  Unknown = 98,
  Other = 99,
}

export enum DonationStatus {
  Pending = 1,
  Verified = 2,
  Rejected = 3,
  Suspended = 4,
}

export enum RejectionReason {
  Duplicate = 1,
  InvalidData = 2,
  Fraudulent = 3,
  Cancelled = 4,
  Other = 99,
}

export interface DonationDto extends FullAuditedEntityDto<string> {
  reference: string;
  externalId?: string;
  donorId: string;
  donorFullName: string;
  campaignId?: string;
  campaignName?: string;
  bankAccountId?: string;
  bankAccountName?: string;
  thankYouTemplateId?: string;
  thankYouTemplateName?: string;
  channel: DonationChannel;
  status: DonationStatus;
  totalAmount: number;
  currency: string;
  donationDate: string;
  creditDate?: string;
  rejectionReason?: RejectionReason;
  rejectionNotes?: string;
  rejectedAt?: string;
  rejectedBy?: string;
  verifiedAt?: string;
  verifiedBy?: string;
  notes?: string;
  internalNotes?: string;
  projects: DonationProjectDto[];
  totalAllocatedAmount: number;
  unallocatedAmount: number;
  isFullyAllocated: boolean;
}

export interface DonationListDto {
  id: string;
  reference: string;
  donorId: string;
  donorFullName: string;
  channel: DonationChannel;
  status: DonationStatus;
  totalAmount: number;
  currency: string;
  donationDate: string;
  creditDate?: string;
  campaignId?: string;
  campaignName?: string;
  projectNames: string[];
  isFullyAllocated: boolean;
}

export interface DonationProjectDto {
  projectId: string;
  projectName: string;
  allocatedAmount: number;
}

export interface CreateDonationDto {
  donorId: string;
  channel: DonationChannel;
  totalAmount: number;
  donationDate: string;
  creditDate?: string;
  campaignId?: string;
  bankAccountId?: string;
  thankYouTemplateId?: string;
  projectAllocations: DonationProjectAllocationDto[];
  notes?: string;
  internalNotes?: string;
  externalId?: string;
  status?: DonationStatus;
}

export interface DonationProjectAllocationDto {
  projectId: string;
  allocatedAmount: number;
}

export interface VerifyDonationDto {
  donorId: string;
  campaignId?: string;
  bankAccountId?: string;
  thankYouTemplateId?: string;
  projectAllocations: DonationProjectAllocationDto[];
  notes?: string;
  internalNotes?: string;
}

export interface RejectDonationDto {
  reason: RejectionReason;
  notes?: string;
}

export interface GetDonationsInput extends PagedAndSortedResultRequestDto {
  status?: DonationStatus;
  channel?: DonationChannel;
  donorId?: string;
  campaignId?: string;
  projectId?: string;
  bankAccountId?: string;
  fromDate?: string;
  toDate?: string;
  minAmount?: number;
  maxAmount?: number;
  search?: string;
}

export interface ExternalDonationDto {
  externalId: string;
  channel: DonationChannel;
  amount: number;
  donationDate: string;
  creditDate?: string;
  donorReference?: string;
  donorId?: string;
  notes?: string;
}

export interface DonationStatisticsDto {
  totalCount: number;
  pendingCount: number;
  verifiedCount: number;
  rejectedCount: number;
  totalAmount: number;
  totalVerifiedAmount: number;
  averageAmount: number;
  firstDonationDate?: string;
  lastDonationDate?: string;
}
