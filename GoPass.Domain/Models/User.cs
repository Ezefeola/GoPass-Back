using System.ComponentModel.DataAnnotations;

namespace GoPass.Domain.Models;

public class User : BaseModel
{
    [EmailAddress]
    public string Email { get; set; } = default!;

    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;
    public string? Name { get; set; } = default!;
    public string? DNI { get; set; } = default!;
    public string? PhoneNumber { get; set; } = default!;
    public string? Image { get; set; }
    public string? City { get; set; } = default!;
    public string? Country { get; set; } = default!;
    public bool IsVerified { get; set; } = false;
    public bool IsEmailVerified { get; set; } = false;
    public bool IsSmsVerified { get; set; } = false;
    public bool IsReset { get; set; } = false;

    //Navigation Properties

    public virtual List<Ticket>? Tickets { get; set; }
    public virtual List<Resale>? Resales { get; set; }
}
