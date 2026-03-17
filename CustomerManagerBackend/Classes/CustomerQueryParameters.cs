using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerManager.Classes;

public class CustomerQueryParameters
{
    public string? Name { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public bool? IsActive { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; }
}
