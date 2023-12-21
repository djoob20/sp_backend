using System.ComponentModel.DataAnnotations;

namespace StudyPortal.API.Models;

public class Register
{
    [Required] public string Firstname { get; set; } = null!;

    [Required] public string Lastname { get; set; } = null!;

    [Required] public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    [Required] public string Password { get; set; } = null!;

    [Required] public string ConfirmPassword { get; set; } = null!;
}