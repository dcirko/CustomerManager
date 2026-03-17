namespace CustomerManager.DTOs;

public record CustomerDTO(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string City,
    string Country,
    bool IsActive
);
