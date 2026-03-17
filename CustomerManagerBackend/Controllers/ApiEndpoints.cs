using System;
using CustomerManager.Classes;
using CustomerManager.Data;
using CustomerManager.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CustomerManager.Controllers;

public static class ApiEndpoints
{
    public static void Endpoints(this WebApplication app)
    {
        app.MapGet("/Customers", async (CustomerManagerDbContext customerManagerDbContext) => 
                await customerManagerDbContext.Customers
                                            .Select(customer => new CustomerDTO(
                                                    customer.Id,
                                                    customer.FirstName,
                                                    customer.LastName,
                                                    customer.Email,
                                                    customer.Phone,
                                                    customer.City,
                                                    customer.Country,
                                                    customer.IsActive
                                                    ))
                                            .ToListAsync());

        app.MapGet("/Customers/{id}", async (int id, CustomerManagerDbContext customerManagerDbContext) =>
        {
            var customer = await customerManagerDbContext.Customers.FindAsync(id);

            return customer is null ? Results.NotFound() : Results.Ok(new CustomerDTO(
                                                    customer.Id,
                                                    customer.FirstName,
                                                    customer.LastName,
                                                    customer.Email,
                                                    customer.Phone,
                                                    customer.City,
                                                    customer.Country,
                                                    customer.IsActive
                                                    ));
        });
        
        app.MapPost("/CreateCustomer", async (CreateCustomerDTO createCustomerDTO, CustomerManagerDbContext customerManagerDbContext) =>
        {
            Customer customer = new()
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

            customerManagerDbContext.Customers.Add(customer);
            customerManagerDbContext.SaveChanges();

            return Results.Created($"/GetCustomer/{customer.Id}", customer);
        });
    }
}
