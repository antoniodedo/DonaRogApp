import { Component, Input, OnInit } from '@angular/core';
import { CampaignService } from 'src/app/proxy/application/campaigns';
import { CampaignDonorDto } from 'src/app/proxy/application/contracts/campaigns/dto/models';
import { PagedResultDto } from '@abp/ng.core';

@Component({
  selector: 'app-campaign-donors-list',
  standalone: false,
  templateUrl: './campaign-donors-list.component.html',
  styleUrls: ['./campaign-donors-list.component.scss']
})
export class CampaignDonorsListComponent implements OnInit {
  @Input() campaignId!: string;
  donors: CampaignDonorDto[] = [];
  loading = false;
  total = 0;
  pageSize = 10;
  pageIndex = 1;

  constructor(private campaignService: CampaignService) {}

  ngOnInit(): void {
    this.loadDonors();
  }

  loadDonors(): void {
    this.loading = true;
    this.campaignService.getCampaignDonors(this.campaignId, {
      skipCount: (this.pageIndex - 1) * this.pageSize,
      maxResultCount: this.pageSize
    }).subscribe({
      next: (result: PagedResultDto<CampaignDonorDto>) => {
        this.donors = result.items;
        this.total = result.totalCount;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  onPageChange(pageIndex: number): void {
    this.pageIndex = pageIndex;
    this.loadDonors();
  }
}
