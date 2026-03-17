using System.ComponentModel.DataAnnotations;

namespace CustomerManager.DTOs;

public record UpdateCustomerDTO(
    [Required] [StringLength(100, MinimumLength = 2, ErrorMessage = "First name must be 2-100 characters")] string FirstName,
    [Required] [StringLength(100, MinimumLength = 2, ErrorMessage = "Last name must be 2-100 characters")] string LastName,
    [Required] [StringLength(255)] [EmailAddress] string Email,
    [Phone] [StringLength(50)] string? Phone,
    [Required] [StringLength(100)] string City,
    [Required] [StringLength(100)] string Country
);
