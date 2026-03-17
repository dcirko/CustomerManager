import { Routes } from '@angular/router';
import { CustomerListPageComponent } from './comps/customer-list-page/customer-list-page.component';
import { CustomerDetailsPageComponent } from './comps/customer-details-page/customer-details-page.component';
import { StatsDashboardPageComponent } from './comps/stats-dashboard-page/stats-dashboard-page.component';

export const routes: Routes = [
    {path: '', component: CustomerListPageComponent},
    {path: 'customerDetails/:id', component: CustomerDetailsPageComponent},
    {path: 'statsDashboard', component: StatsDashboardPageComponent}
];
