import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { CustomerDTO } from '../domain/customerDTO';
import { CreateCustomerDTO } from '../domain/createCustomerDTO';
import { UpdateCustomerDTO } from '../domain/updateCustomerDTO';
import { CustomerStatsDTO } from '../domain/customerStatsDTO';
import { QueryParameters } from '../domain/queryParameters';

@Injectable({
  providedIn: 'root'
})
export class ApiServiceService 
{
  private customerApiUrl = 'http://localhost:5178/api/customers'
  constructor(private http: HttpClient) { }

  getCustomers(query: QueryParameters) : Observable<CustomerDTO[]>{
    let params = new HttpParams()

    if (query.pageNumber)
      params = params.set('pageNumber', query.pageNumber)

    if (query.pageSize)
      params = params.set('pageSize', query.pageSize)

    if (query.name)
      params = params.set('name', query.name)

    if (query.city)
      params = params.set('city', query.city)

    if (query.country)
      params = params.set('country', query.country)

    if (query.isActive !== undefined)
      params = params.set('isActive', query.isActive)

    if (query.sortColumn)
      params = params.set('sortColumn', query.sortColumn)

    if (query.sortOrder)
      params = params.set('sortOrder', query.sortOrder)

    console.log(params);
    return this.http.get<CustomerDTO[]>(this.customerApiUrl, {params}).pipe(
      tap(data => console.log('Fetched customers:', data))
    );
  }

  getCustomerById(id: number): Observable<CustomerDTO>{
    return this.http.get<CustomerDTO>(`${this.customerApiUrl}/${id}`).pipe(
      tap(data => console.log('Fetched customer:', data))
    )
  }

  createCustomer(createCustomerDTO: CreateCustomerDTO): Observable<CustomerDTO>{
    return this.http.post<CustomerDTO>(this.customerApiUrl, createCustomerDTO).pipe(
      tap(data => console.log('Created customer:', data))
    )
  }

  updateCustomer(id: number, updateCustomerDTO: UpdateCustomerDTO): Observable<CustomerDTO>{
    return this.http.put<CustomerDTO>(`${this.customerApiUrl}/${id}`, updateCustomerDTO).pipe(
      tap(data => console.log('Updated customer:', data))
    )
  }

  deleteCustomer(id: number): Observable<boolean>{
    return this.http.delete<boolean>(`${this.customerApiUrl}/${id}`).pipe(
      tap(data => console.log('Deleted customer with id:', id, 'Response:', data))
    )
  }

  getStats(): Observable<CustomerStatsDTO>{
    return this.http.get<CustomerStatsDTO>(`${this.customerApiUrl}/stats`).pipe(
      tap(data => console.log('Fetched customer stats:', data))
    )
  }

  bulkDeactivate(bulkDeactivateArray: number[]): Observable<{updatedCount: number}>{
    return this.http.post<{updatedCount: number}>(`${this.customerApiUrl}/bulk-deactivate`, bulkDeactivateArray).pipe(
      tap(data => console.log('Bulk deactivated customers count:', data))
    )
  }
}
