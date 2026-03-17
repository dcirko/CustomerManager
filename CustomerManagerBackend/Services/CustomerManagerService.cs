using System;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using CustomerManager.Classes;
using CustomerManager.Data;
using CustomerManager.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CustomerManager.Services;

public class CustomerManagerService
{
    private CustomerManagerDbContext _customerManagerDbContext;

    public CustomerManagerService(CustomerManagerDbContext customerManagerDbContext)
    {
        _customerManagerDbContext = customerManagerDbContext;
    }

    public async Task<List<CustomerDTO>> GetCustomers(CustomerQueryParameters customerQueryParameters)
    {
        var queryCustomers = _customerManagerDbContext.Customers.AsQueryable();

        if (!string.IsNullOrEmpty(customerQueryParameters.Name))
        {
            queryCustomers = queryCustomers.Where(c => EF.Functions.Like(c.FirstName, $"%{customerQueryParameters.Name}%") ||
                                                        EF.Functions.Like(c.LastName, $"%{customerQueryParameters.Name}%"));
        }
        if(!string.IsNullOrEmpty(customerQueryParameters.City))
        {
            queryCustomers = queryCustomers.Where(c => c.City.Equals(customerQueryParameters.City));
        }
        if(!string.IsNullOrEmpty(customerQueryParameters.Country))
        {
            queryCustomers = queryCustomers.Where(c => c.Country.Equals(customerQueryParameters.Country));
        }
        if(customerQueryParameters.IsActive != null)
        {
            queryCustomers = queryCustomers.Where(c => c.IsActive == customerQueryParameters.IsActive);
        }

        if(!string.IsNullOrEmpty(customerQueryParameters.SortColumn) && !string.IsNullOrEmpty(customerQueryParameters.SortOrder))
        {
            bool asc = customerQueryParameters.SortOrder.Equals("asc", StringComparison.CurrentCultureIgnoreCase);

            queryCustomers = customerQueryParameters.SortColumn.ToLower() switch
            { 
                "name" => asc ? queryCustomers.OrderBy(c => c.FirstName) : queryCustomers.OrderByDescending(c => c.FirstName),
                "city" => asc ? queryCustomers.OrderBy(c => c.City) : queryCustomers.OrderByDescending(c => c.City),
                "country" => asc ? queryCustomers.OrderBy(c => c.Country) : queryCustomers.OrderByDescending(c => c.Country),
                "isactive" => asc ? queryCustomers.OrderBy(c => c.IsActive) : queryCustomers.OrderByDescending(c => c.IsActive),
                _ => queryCustomers
            };
        }else
        {
            queryCustomers = queryCustomers.OrderBy(c => c.Id);
        }

        if(customerQueryParameters.PageNumber > 0 && customerQueryParameters.PageSize > 0)
        {
            queryCustomers = queryCustomers.Skip((customerQueryParameters.PageNumber - 1) * customerQueryParameters.PageSize)
                                            .Take(customerQueryParameters.PageSize);
        }

        var customers = await queryCustomers.Select(c => ToDTO(c)!).ToListAsync();

        return customers;

    }

    public async Task<CustomerDTO?> GetById(int id)
    {
        Customer? customer = await _customerManagerDbContext.Customers.FindAsync(id);
        return ToDTO(customer);
    }

    public async Task<CustomerDTO> CreateCustomer(CreateCustomerDTO createCustomerDTO)
    {
        if(await _customerManagerDbContext.Customers.AnyAsync(c => c.Email == createCustomerDTO.Email))
        {
            throw new InvalidOperationException("Customer with the same email already exists");
        }

        Customer newCustomer = new()
        {
            FirstName = createCustomerDTO.FirstName,
            LastName = createCustomerDTO.LastName,
            Email = createCustomerDTO.Email,
            Phone = createCustomerDTO.Phone,
            City = createCustomerDTO.City,
            Country = createCustomerDTO.Country,
            IsActive = true,
            CreatedAt = DateTime.Now,
            LastModifiedAt = null
        };

        await _customerManagerDbContext.AddAsync(newCustomer);

        return ToDTO(newCustomer)!;
    }

    public async Task<CustomerDTO> UpdateCustomer(int id, UpdateCustomerDTO updateCustomerDTO)
    {
        var customer = await _customerManagerDbContext.Customers.FindAsync(id);

        if (customer is null)
        {
            throw new KeyNotFoundException("Customer not found");
        }

        customer.FirstName = updateCustomerDTO.FirstName;
        customer.LastName = updateCustomerDTO.LastName;
        customer.Email = updateCustomerDTO.Email;
        customer.Phone = updateCustomerDTO.Phone;
        customer.City = updateCustomerDTO.City;
        customer.Country = updateCustomerDTO.Country;
        customer.LastModifiedAt = DateTime.Now;

        await _customerManagerDbContext.SaveChangesAsync();

        return ToDTO(customer)!;
    }

    public async Task<bool> DeleteCustomer(int id)
    {
        var customer = await _customerManagerDbContext.Customers.FindAsync(id);

        if(customer is null)
        {
            throw new KeyNotFoundException("Customer not found");
        }

        customer.IsActive = false;
        customer.LastModifiedAt = DateTime.Now;

        await _customerManagerDbContext.SaveChangesAsync();

        return true;
    }

    public async Task<CustomerStatsDTO> GetStats()
    {
        var totalCount = await _customerManagerDbContext.Customers.CountAsync();
        var activeCount = await _customerManagerDbContext.Customers.CountAsync(c => c.IsActive);
        var inactiveCount = await _customerManagerDbContext.Customers.CountAsync(c => c.IsActive.Equals(false));
        var top5Cities = await _customerManagerDbContext.Customers
                                    .GroupBy(c=>c.City)
                                    .Select(group => new {City = group.Key, CustomerCount = group.Count()})
                                    .OrderByDescending(city => city.CustomerCount)
                                    .Take(5)
                                    .ToListAsync();

        List<Top5CitiesDTO> citiesDTOs = top5Cities.Select(c => new Top5CitiesDTO(c.City, c.CustomerCount)).ToList();

        return new CustomerStatsDTO(totalCount, activeCount, inactiveCount, citiesDTOs);
    }

    public async Task<int> BulkDeactivate(int[] BulkDeactivateArray)
    {
        return await _customerManagerDbContext.Customers.Where(c => BulkDeactivateArray.Contains(c.Id))
                                                .ExecuteUpdateAsync(customer => customer.SetProperty(r => r.IsActive, false));
    }

    private static CustomerDTO? ToDTO(Customer? customer)
    {
        if (customer == null)
            throw new KeyNotFoundException("Customer not found");

        return new CustomerDTO(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.Phone,
            customer.City,
            customer.Country,
            customer.IsActive
        );
    }
}
