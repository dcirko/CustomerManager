using System.Windows.Markup;
using CustomerManager.Classes;
using CustomerManager.DTOs;
using CustomerManager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


[Route("api/customers")]
[ApiController]
public class CustomerManagerController : ControllerBase
{
    private readonly CustomerManagerService _customerManagerService;

    public CustomerManagerController(CustomerManagerService customerManagerService)
    {
        _customerManagerService = customerManagerService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCustomers([FromQuery] CustomerQueryParameters queryParameters)
    {
        var customers = await _customerManagerService.GetCustomers(queryParameters);
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await _customerManagerService.GetById(id);
        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDTO createCustomerDTO)
    {
        CustomerDTO customerDTO = await _customerManagerService.CreateCustomer(createCustomerDTO);

        return Created($"/api/customers/{customerDTO.Id}", customerDTO);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerDTO updateCustomerDTO)
    {
        CustomerDTO customerDTO = await _customerManagerService.UpdateCustomer(id, updateCustomerDTO);

        return Ok(customerDTO);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        await _customerManagerService.DeleteCustomer(id);

        return NoContent();

    }

    [HttpGet("stats")]
    public async Task<IActionResult> TotalStats()
    {
        CustomerStatsDTO customerStatsDTO = await _customerManagerService.GetStats();
        return Ok(customerStatsDTO);
    }

    [HttpPost("bulk-deactivate")]
    public async Task<IActionResult> BulkDeactivate([FromBody] int[] BulkDeactivateArray)
    {
        if(BulkDeactivateArray.Length > 1000)
        {
            return BadRequest("Cannot deactivate more than 1000 customers at once.");
        }
        
        int updatedCount = await _customerManagerService.BulkDeactivate(BulkDeactivateArray);
        return Ok(new { updatedCount });
    }


}
