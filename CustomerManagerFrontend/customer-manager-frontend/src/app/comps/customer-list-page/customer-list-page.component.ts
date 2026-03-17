import { Component, OnInit } from '@angular/core';
import { CustomerDTO } from '../../domain/customerDTO';
import { ApiServiceService } from '../../services/api-service.service';
import { Router } from '@angular/router';
import { debounceTime, distinctUntilChanged, Subject, tap } from 'rxjs';
import { CommonModule } from '@angular/common';
import { CustomerStatsDTO } from '../../domain/customerStatsDTO';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-customer-list-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './customer-list-page.component.html',
  styleUrl: './customer-list-page.component.css'
})
export class CustomerListPageComponent implements OnInit {
  customers: CustomerDTO[] = [];
  countries!: string[];
  selectedCustomers: number[] = [];
  totalRecords!: number;
  totalPages!: number;
  loading = false;
  filterName = '';
  filterCountry = '';
  stats!: CustomerStatsDTO;
  pageNumber = 1;
  pageSize = 50;
  sortColumn = '';
  sortOrder = true;

  constructor(private apiService: ApiServiceService, private router: Router) { }
  private searchSubject = new Subject<string>();

  ngOnInit(): void {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(value => {

      if (value.length >= 2 || value.length === 0) {
        this.filterName = value;
        this.pageNumber = 1;
        this.loadCustomers();
      }

    });
    
    this.loadCustomers();
    this.loadTotalStats();
  }
  
  private loadCustomers(): void {
    this.loading = true;
    this.apiService.getCustomers({
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      name: this.filterName,
      country: this.filterCountry,
      sortColumn: this.sortColumn,
      sortOrder: this.sortOrder ? 'asc' : 'desc'
    }).pipe(
      tap(customers => {
        this.customers = customers;
        const countrySet = new Set(customers.map(c => c.country));
        this.countries = Array.from(countrySet);
      })
    ).subscribe({
      next: () => this.loading = false,
      error: (err) => {
        console.error('Error loading customers:', err);
        this.loading = false;
      }
    });
  }

  sort(sortColumn: string){
    if(this.sortColumn === sortColumn){
      this.sortOrder = !this.sortOrder;
    }else{
      this.sortColumn = sortColumn;
      this.sortOrder = true;
    }
    this.pageNumber = 1;
    this.loadCustomers();
  }

  loadTotalStats(){
    this.apiService.getStats().pipe(tap(data => {
      this.stats = data;
      this.totalRecords = data.totalCount;
      this.totalPages = data.totalCount / this.pageSize
    })).subscribe();  
  }

  onSearch(event: Event){
    const input = event.target as HTMLInputElement;
    this.searchSubject.next(input.value.toLowerCase());
  }

  onCountryChange(event: Event){
    const select = event.target as HTMLSelectElement;
    const value = select.value;
    this.filterCountry = value;
    this.pageNumber = 1;
    this.loadCustomers();
  }

  deactivateSelected(){
    if(this.selectedCustomers.length === 0){
      return;
    }

    this.apiService.bulkDeactivate(this.selectedCustomers).pipe(
      tap(response => {
        console.log('Bulk deactivation response:', response);
        this.selectedCustomers = [];
        this.loadCustomers();
      })
    ).subscribe();
  }

  toggleSelectAll(event: Event){
    const checkbox = event.target as HTMLInputElement;

    if(checkbox.checked){
      this.selectedCustomers = this.customers.map(c => c.id);
    } else {
      this.selectedCustomers = [];
    }
  }

  isSelected(id: number): boolean{
    return this.selectedCustomers.includes(id);
  }

  toggleSelection(customerId: number){
    const index = this.selectedCustomers.indexOf(customerId);

    if (index >= 0) {
      this.selectedCustomers.splice(index, 1);
    } else {
      this.selectedCustomers.push(customerId);
    }
  }

  editCustomer(customerId: number){
    this.router.navigate([`/customerDetails/${customerId}`]);
  }

  deleteCustomer(customerId: number){
      Swal.fire({
        title: 'Are you sure?',
        text: 'This customer will become inactive',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
      }).then((result) => {
        if (result.isConfirmed) {
          this.apiService.deleteCustomer(customerId).pipe(
            tap(() => {
              this.loadCustomers();
            })
          ).subscribe();
        }
      });

  }

  loadStatsPage(){
    this.router.navigate(['/statsDashboard']);
  }

  nextPage(): void {
    this.pageNumber++;
    this.loadCustomers();
  }

  prevPage(): void {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.loadCustomers();
    }
  }
}
