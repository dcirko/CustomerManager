using System;
using CustomerManager.Classes;
using Microsoft.EntityFrameworkCore;

namespace CustomerManager.Data;

public class CustomerManagerDbContext(DbContextOptions<CustomerManagerDbContext> dbContextOptions) 
        : DbContext(dbContextOptions)
{
    public DbSet<Customer> Customers => Set<Customer>();
}
