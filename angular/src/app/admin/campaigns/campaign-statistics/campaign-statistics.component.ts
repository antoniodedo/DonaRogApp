import { Component, Input, OnInit } from '@angular/core';
import { CampaignService } from 'src/app/proxy/application/campaigns';
import { CampaignStatisticsDto } from 'src/app/proxy/application/contracts/campaigns/dto/models';

@Component({
  selector: 'app-campaign-statistics',
  standalone: false,
  templateUrl: './campaign-statistics.component.html',
  styleUrls: ['./campaign-statistics.component.scss']
})
export class CampaignStatisticsComponent implements OnInit {
  @Input() campaignId!: string;
  statistics: CampaignStatisticsDto | null = null;
  loading = false;

  constructor(private campaignService: CampaignService) {}

  ngOnInit(): void {
    this.loadStatistics();
  }

  loadStatistics(): void {
    this.loading = true;
    this.campaignService.getStatistics(this.campaignId).subscribe({
      next: (stats) => {
        this.statistics = stats;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
}
