using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public record UserForAuthenticationDto
{
    [Required(ErrorMessage = "User name is required")]
    public string? UserName { get; init; }
    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; init; }

}
