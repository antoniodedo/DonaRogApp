import { Injectable } from '@angular/core';
import { RestService, Rest } from '@abp/ng.core';
import { Observable } from 'rxjs';

export interface DonorAttachmentDto {
  id: string;
  donorId: string;
  fileName: string;
  fileExtension: string;
  mimeType: string;
  fileSizeBytes: number;
  attachmentType?: string;
  description?: string;
  displayOrder: number;
  creationTime: string;
  creatorUserName?: string;
}

export interface CreateDonorAttachmentDto {
  donorId: string;
  fileName: string;
  attachmentType?: string;
  description?: string;
  displayOrder: number;
}

export interface UpdateDonorAttachmentDto {
  attachmentType?: string;
  description?: string;
  displayOrder: number;
}

@Injectable({
  providedIn: 'root',
})
export class DonorAttachmentService {
  apiName = 'DonaRogApp';

  constructor(private restService: RestService) {}

  getListByDonor(donorId: string): Observable<{ items: DonorAttachmentDto[] }> {
    const request: Rest.Request<null> = {
      method: 'GET',
      url: `/api/app/donor-attachments/by-donor/${donorId}`,
    };

    return this.restService.request<null, { items: DonorAttachmentDto[] }>(request, {
      apiName: this.apiName,
    });
  }

  get(id: string): Observable<DonorAttachmentDto> {
    const request: Rest.Request<null> = {
      method: 'GET',
      url: `/api/app/donor-attachments/${id}`,
    };

    return this.restService.request<null, DonorAttachmentDto>(request, {
      apiName: this.apiName,
    });
  }

  create(input: CreateDonorAttachmentDto, file: File): Observable<DonorAttachmentDto> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('donorId', input.donorId);
    formData.append('fileName', input.fileName);
    if (input.attachmentType) {
      formData.append('attachmentType', input.attachmentType);
    }
    if (input.description) {
      formData.append('description', input.description);
    }
    formData.append('displayOrder', input.displayOrder.toString());

    const request: Rest.Request<FormData> = {
      method: 'POST',
      url: '/api/app/donor-attachments',
      body: formData,
    };

    return this.restService.request<FormData, DonorAttachmentDto>(request, {
      apiName: this.apiName,
    });
  }

  update(id: string, input: UpdateDonorAttachmentDto): Observable<DonorAttachmentDto> {
    const request: Rest.Request<UpdateDonorAttachmentDto> = {
      method: 'PUT',
      url: `/api/app/donor-attachments/${id}`,
      body: input,
    };

    return this.restService.request<UpdateDonorAttachmentDto, DonorAttachmentDto>(request, {
      apiName: this.apiName,
    });
  }

  delete(id: string): Observable<void> {
    const request: Rest.Request<null> = {
      method: 'DELETE',
      url: `/api/app/donor-attachments/${id}`,
    };

    return this.restService.request<null, void>(request, {
      apiName: this.apiName,
    });
  }

  getDownloadUrl(id: string): string {
    return `/api/app/donor-attachments/${id}/download`;
  }

  reorder(donorId: string, attachmentIds: string[]): Observable<void> {
    const request: Rest.Request<string[]> = {
      method: 'POST',
      url: `/api/app/donor-attachments/reorder`,
      params: { donorId },
      body: attachmentIds,
    };

    return this.restService.request<string[], void>(request, {
      apiName: this.apiName,
    });
  }
}
