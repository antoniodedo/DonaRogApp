import { Component, OnInit } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ChartConfiguration, ChartData } from 'chart.js';
import { DonationService } from '@proxy/donations';
import { DonorService } from '@proxy/donors';
import { ProjectService } from '@proxy/application/projects';
import { PrintBatchService } from '@proxy/communications/print-batches';
import { GetDonationsInput } from '@proxy/donations/models';

interface DateFilterOption {
  label: string;
  value: string;
}

@Component({
  selector: 'app-welcome',
  standalone: false,
  templateUrl: './welcome.component.html',
  styleUrl: './welcome.component.scss'
})
export class WelcomeComponent implements OnInit {
  loading = true;
  dateFilter: string = 'thisYear';
  customDateRange: Date[] = [];

  // KPI Data
  totalRaised: number = 0;
  totalDonations: number = 0;
  totalDonors: number = 0;
  retentionRate: number = 0;
  attritionRate: number = 0;
  pendingDonations: number = 0;
  championsCount: number = 0;
  atRiskCount: number = 0;
  averageDonation: number = 0;

  // Chart Data
  monthlyTrendChartData?: ChartData<'line'>;
  channelChartData?: ChartData<'pie'>;
  rfmChartData?: ChartData<'doughnut'>;
  topProjectsChartData?: ChartData<'bar'>;

  // Chart Options
  lineChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
        position: 'top',
      },
      title: {
        display: true,
        text: 'Trend Donazioni Mensili'
      }
    },
    scales: {
      y: {
        beginAtZero: true
      }
    }
  };

  pieChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
        position: 'right',
      },
      title: {
        display: true,
        text: 'Donazioni per Canale'
      }
    }
  };

  doughnutChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
        position: 'right',
      },
      title: {
        display: true,
        text: 'Segmentazione RFM Donatori'
      }
    }
  };

  barChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    indexAxis: 'y',
    plugins: {
      legend: {
        display: false
      },
      title: {
        display: true,
        text: 'Top 5 Progetti per Raccolta'
      }
    },
    scales: {
      x: {
        beginAtZero: true
      }
    }
  };

  dateFilterOptions: DateFilterOption[] = [
    { label: 'Questo mese', value: 'thisMonth' },
    { label: 'Ultimi 3 mesi', value: 'last3Months' },
    { label: 'Quest\'anno', value: 'thisYear' },
    { label: 'Ultimi 12 mesi', value: 'last12Months' },
    { label: 'Anno precedente', value: 'lastYear' },
    { label: 'Sempre', value: 'allTime' },
    { label: 'Personalizzato', value: 'custom' }
  ];

  constructor(
    private donationService: DonationService,
    private donorService: DonorService,
    private projectService: ProjectService,
    private printBatchService: PrintBatchService
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  onDateFilterChange(): void {
    if (this.dateFilter !== 'custom') {
      this.customDateRange = [];
      this.loadDashboardData();
    }
  }

  onCustomDateRangeChange(dates: Date[]): void {
    if (dates && dates.length === 2) {
      this.loadDashboardData();
    }
  }

  private getDateRange(): { fromDate?: Date; toDate?: Date } {
    const now = new Date();
    let fromDate: Date | undefined;
    let toDate: Date | undefined = now;

    switch (this.dateFilter) {
      case 'thisMonth':
        fromDate = new Date(now.getFullYear(), now.getMonth(), 1);
        break;
      case 'last3Months':
        fromDate = new Date(now.getFullYear(), now.getMonth() - 3, 1);
        break;
      case 'thisYear':
        fromDate = new Date(now.getFullYear(), 0, 1);
        break;
      case 'last12Months':
        fromDate = new Date(now.getFullYear(), now.getMonth() - 12, 1);
        break;
      case 'lastYear':
        fromDate = new Date(now.getFullYear() - 1, 0, 1);
        toDate = new Date(now.getFullYear() - 1, 11, 31);
        break;
      case 'custom':
        if (this.customDateRange && this.customDateRange.length === 2) {
          fromDate = this.customDateRange[0];
          toDate = this.customDateRange[1];
        }
        break;
      case 'allTime':
      default:
        fromDate = undefined;
        toDate = undefined;
        break;
    }

    return { fromDate, toDate };
  }

  private loadDashboardData(): void {
    this.loading = true;
    const dateRange = this.getDateRange();

    const filter: GetDonationsInput = {
      fromDate: dateRange.fromDate?.toISOString(),
      toDate: dateRange.toDate?.toISOString(),
      maxResultCount: 1000
    };

    // IMPORTANTE: Cambia questo a 'true' dopo aver riavviato il backend!
    // I nuovi endpoint (GetMonthlyTrendAsync e GetRfmStatisticsAsync) 
    // saranno disponibili solo dopo il restart del backend.
    const useNewEndpoints = false;
    
    forkJoin({
      donationStats: this.donationService.getStatistics(filter),
      monthlyTrend: useNewEndpoints ? this.donationService.getMonthlyTrend(filter).pipe(
        catchError(err => {
          console.warn('Monthly trend endpoint not available yet:', err);
          return of([]);
        })
      ) : of([]),
      rfmStats: useNewEndpoints ? this.donorService.getRfmStatistics().pipe(
        catchError(err => {
          console.warn('RFM statistics endpoint not available yet:', err);
          return of({
            totalDonors: 0,
            activeDonors: 0,
            lapsedDonors: 0,
            retentionRate: 0,
            attritionRate: 0,
            championsCount: 0,
            loyalCount: 0,
            potentialCount: 0,
            atRiskCount: 0,
            dormantCount: 0,
            lostCount: 0
          });
        })
      ) : of({
        totalDonors: 0,
        activeDonors: 0,
        lapsedDonors: 0,
        retentionRate: 0,
        attritionRate: 0,
        championsCount: 0,
        loyalCount: 0,
        potentialCount: 0,
        atRiskCount: 0,
        dormantCount: 0,
        lostCount: 0
      }),
      projectStats: this.projectService.getAggregateStatistics({ maxResultCount: 1000 }).pipe(
        catchError(err => {
          console.warn('Project stats not available:', err);
          return of({ topProjectsByAmount: [] });
        })
      ),
      printBatchStats: this.printBatchService.getStatistics().pipe(
        catchError(err => {
          console.warn('Print batch stats not available:', err);
          return of({ pendingCount: 0, generatedCount: 0, printedCount: 0 });
        })
      )
    }).subscribe({
      next: (data) => {
        // KPI Metrics
        this.totalRaised = data.donationStats.totalVerifiedAmount || 0;
        this.totalDonations = data.donationStats.verifiedCount || 0;
        this.pendingDonations = data.donationStats.pendingCount || 0;
        this.averageDonation = data.donationStats.averageAmount || 0;

        this.totalDonors = data.rfmStats.totalDonors || 0;
        this.retentionRate = data.rfmStats.retentionRate || 0;
        this.attritionRate = data.rfmStats.attritionRate || 0;
        this.championsCount = data.rfmStats.championsCount || 0;
        this.atRiskCount = data.rfmStats.atRiskCount || 0;

        // Charts
        this.buildMonthlyTrendChart(data.monthlyTrend);
        this.buildRfmChart(data.rfmStats);
        this.buildTopProjectsChart(data.projectStats);
        
        // Note: Channel chart requires additional data from donations list
        // For now we'll create a placeholder or query separately
        this.buildChannelChartPlaceholder();

        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading dashboard data:', err);
        this.loading = false;
      }
    });
  }

  private buildMonthlyTrendChart(trends: any[]): void {
    if (!trends || trends.length === 0) {
      this.monthlyTrendChartData = undefined;
      return;
    }

    const labels = trends.map(t => `${this.getMonthName(t.month)} ${t.year}`);
    const amounts = trends.map(t => t.amount);
    const counts = trends.map(t => t.count);

    this.monthlyTrendChartData = {
      labels,
      datasets: [
        {
          label: 'Importo (€)',
          data: amounts,
          borderColor: '#1890ff',
          backgroundColor: 'rgba(24, 144, 255, 0.1)',
          tension: 0.4,
          yAxisID: 'y'
        },
        {
          label: 'Numero Donazioni',
          data: counts,
          borderColor: '#52c41a',
          backgroundColor: 'rgba(82, 196, 26, 0.1)',
          tension: 0.4,
          yAxisID: 'y1'
        }
      ]
    };

    this.lineChartOptions = {
      ...this.lineChartOptions,
      scales: {
        y: {
          type: 'linear',
          display: true,
          position: 'left',
          beginAtZero: true,
          title: {
            display: true,
            text: 'Importo (€)'
          }
        },
        y1: {
          type: 'linear',
          display: true,
          position: 'right',
          beginAtZero: true,
          grid: {
            drawOnChartArea: false,
          },
          title: {
            display: true,
            text: 'Numero Donazioni'
          }
        }
      }
    };
  }

  private buildChannelChartPlaceholder(): void {
    // Placeholder data - in a real scenario, fetch channel distribution
    this.channelChartData = {
      labels: ['Bonifico', 'Carta di Credito', 'PayPal', 'Bollettino Postale', 'Altro'],
      datasets: [{
        data: [45, 25, 15, 10, 5],
        backgroundColor: [
          '#1890ff',
          '#52c41a',
          '#fa8c16',
          '#eb2f96',
          '#722ed1'
        ]
      }]
    };
  }

  private buildRfmChart(rfmStats: any): void {
    this.rfmChartData = {
      labels: ['Champions', 'Loyal', 'Potential', 'At Risk', 'Dormant', 'Lost'],
      datasets: [{
        data: [
          rfmStats.championsCount || 0,
          rfmStats.loyalCount || 0,
          rfmStats.potentialCount || 0,
          rfmStats.atRiskCount || 0,
          rfmStats.dormantCount || 0,
          rfmStats.lostCount || 0
        ],
        backgroundColor: [
          '#52c41a',
          '#1890ff',
          '#faad14',
          '#fa8c16',
          '#eb2f96',
          '#f5222d'
        ]
      }]
    };
  }

  private buildTopProjectsChart(projectStats: any): void {
    if (!projectStats.topProjectsByAmount || projectStats.topProjectsByAmount.length === 0) {
      this.topProjectsChartData = undefined;
      return;
    }

    const top5 = projectStats.topProjectsByAmount.slice(0, 5);
    const labels = top5.map((p: any) => p.name);
    const amounts = top5.map((p: any) => p.totalAmount);

    this.topProjectsChartData = {
      labels,
      datasets: [{
        label: 'Importo Raccolto (€)',
        data: amounts,
        backgroundColor: '#1890ff'
      }]
    };
  }

  private getMonthName(month: number): string {
    const months = ['Gen', 'Feb', 'Mar', 'Apr', 'Mag', 'Giu', 'Lug', 'Ago', 'Set', 'Ott', 'Nov', 'Dic'];
    return months[month - 1] || '';
  }

  get isAttritionCritical(): boolean {
    return this.attritionRate > 15;
  }

  get isAtRiskHigh(): boolean {
    return this.totalDonors > 0 && (this.atRiskCount / this.totalDonors * 100) > 10;
  }
}
