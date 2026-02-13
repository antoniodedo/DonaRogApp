import type { ContactType } from '../enums/shared/contact-type.enum';

export interface CreateDonorContactDto {
  phoneNumber?: string;
  type?: ContactType;
}

export interface DonorContactDto {
  id?: string;
  phoneNumber?: string;
  type?: ContactType;
  isDefault: boolean;
  isVerified: boolean;
  verifiedDate?: string;
  dateAdded?: string;
  notes?: string;
}
