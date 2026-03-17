using CustomerManager.Classes;
using Microsoft.EntityFrameworkCore;

namespace CustomerManager.Data;

public static class DataExtensions
{
    public static async Task MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CustomerManagerDbContext>();

        dbContext.Database.Migrate();

        if (dbContext.Customers.Any()){
            return;
        }
        
        var firstNames = new[]
        {
            "Ivan","Laura","Marko","Petar","Luka","Maja","Nikola","Ivana","Filip","Sara"
        };

        var lastNames = new[]
        {
            "Tomsic","Kovac","Babic","Matic","Jovic","Juric","Bilic","Vukovic","Radic","Pavlovic"
        };

        var cities = new[]
        {
            "Zagreb", "Split", "Rijeka", "Osijek", "Zadar", "Pula", "Slavonski Brod", "Karlovac", "Sisak", "Varaždin"
        };

        var countries = new[]
        {
            "Croatia", "Bosnia and Herzegovina", "Serbia", "Slovenia", "Montenegro", "North Macedonia", "Albania", "Greece", "Italy", "Austria"
        };

        const int customerRecordNumber = 100000;
        const int repeatSeed = 5000;
        var random = new Random();

        var customers = new List<Customer>(repeatSeed);

        for(int i = 0; i < customerRecordNumber; i++)
        {
            var newFirstName = firstNames[i % firstNames.Length];
            var newLastName = lastNames[i % lastNames.Length];

            customers.Add(new Customer
            {
                FirstName = newFirstName,
                LastName = newLastName,
                Email = $"{newFirstName}.{newLastName}{i}@gmail.com",
                Phone = $"09{random.Next(10000000, 99999999)}",
                City = cities[i % cities.Length],
                Country = countries[i % countries.Length],
                IsActive = true,
                CreatedAt = DateTime.Now.AddDays(i),
                LastModifiedAt = null
            });

            if(customers.Count() == repeatSeed)
            {
                await dbContext.Customers.AddRangeAsync(customers);
                await dbContext.SaveChangesAsync();
                customers.Clear();
            }
        }

        if (customers.Count > 0)
        {
            await dbContext.Customers.AddRangeAsync(customers);
            await dbContext.SaveChangesAsync();
        }

        
    }
}
