using System.ComponentModel.DataAnnotations;

namespace StudyPortal.API.Models;

public class AuthUser
{
    [Required]
    public string UserName { get; init; }= null!;
    
    [Required]
    public string Role { get; init; }= null!;
    
    [Required]
    public string BirthDay { get; init; }= null!;

    [Required] 
    public byte[] PasswordSalt { get; set; } = null!;
    
    [Required]
    public byte[] PasswordHash { get; set; }= null!;

    public string Token { get; set; } = null!;
    
    public DateTime TokenCreated { get; set; }
    public DateTime TokenExpires { get; set; }
}