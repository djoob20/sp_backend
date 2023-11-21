using System.ComponentModel.DataAnnotations;

namespace StudyPortal.API.Models;

public class Register
{
    [Required]
    public string Firstname { get; set; }
    
    [Required]
    public string Lastname { get; set; }
    
    [Required]
    public string Email { get; set; }
    [Required]
    public string Role { get; set; }
    [Required]
    public string BirthDay { get; set; }
    [Required]
    public string Password { get; set; }
    
    [Required]
    public string ConfirmPassword { get; set; }
}