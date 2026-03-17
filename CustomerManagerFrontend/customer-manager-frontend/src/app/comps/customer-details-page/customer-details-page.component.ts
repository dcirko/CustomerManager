import { Component, OnInit } from '@angular/core';
import { ApiServiceService } from '../../services/api-service.service';
import { CustomerDTO } from '../../domain/customerDTO';
import { CommonModule } from '@angular/common';
import { tap } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { query } from '@angular/animations';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { UpdateCustomerDTO } from '../../domain/updateCustomerDTO';

@Component({
  selector: 'app-customer-details-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './customer-details-page.component.html',
  styleUrl: './customer-details-page.component.css'
})
export class CustomerDetailsPageComponent implements OnInit {
  customerForm!: FormGroup;
  showSuccessToast = false;
  showErrorToast = false;
  updateCustomerDTO!: UpdateCustomerDTO;
  customerId!: number;
  customer!: CustomerDTO;

  constructor(private apiService: ApiServiceService, private fb: FormBuilder, private route: ActivatedRoute, private router: Router) {}

  ngOnInit(): void {
    this.customerForm = this.fb.group({
      firstName: ['', [Validators.minLength(2)]],
      lastName: ['', [Validators.minLength(2)]],
      email: ['', [Validators.email]],
      phone: [''],
      city: [''],
      country: [''],
    })

    this.customerId = Number(this.route.snapshot.paramMap.get('id'))

    if(this.customerId){
      this.apiService.getCustomerById(this.customerId).subscribe(customer => {
        this.customerForm.patchValue(customer)
      });
    }

    console.log(this.customerForm);
  }
  
  saveCustomer(){
    if(this.customerForm.invalid){
      this.showErrorToast = true;
      setTimeout(() => this.showErrorToast = false, 3000);
      return;
    }else{
      console.log(this.customerForm.value);
      this.updateCustomerDTO = this.customerForm.value;
      this.apiService.updateCustomer(this.customerId, this.updateCustomerDTO). subscribe({
            next: () => {
              this.showSuccessToast = true;
              setTimeout(() => this.showSuccessToast = false, 3000);
            }
      });
    }
  }

  cancel(){
    this.router.navigate(['/']);
  }


  
}
