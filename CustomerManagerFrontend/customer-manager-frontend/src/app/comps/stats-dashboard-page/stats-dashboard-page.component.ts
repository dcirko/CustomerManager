import { Component, OnInit } from '@angular/core';
import { ApiServiceService } from '../../services/api-service.service';
import { CustomerStatsDTO } from '../../domain/customerStatsDTO';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-stats-dashboard-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './stats-dashboard-page.component.html',
  styleUrl: './stats-dashboard-page.component.css'
})
export class StatsDashboardPageComponent implements OnInit{
  customerStats!: CustomerStatsDTO;

  ngOnInit(): void {
    this.loadStats();
  }

  constructor(private api: ApiServiceService) {}

  private loadStats(){
    this.api.getStats().subscribe({
      next: (data) => {
        this.customerStats = data;
      },
      error: (err) => {
        console.error('Error fetching stats:', err);
      }
    });
  }

}
