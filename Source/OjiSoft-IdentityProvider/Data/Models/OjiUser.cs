using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace OjiSoft.IdentityProvider.Data.Models;

public class OjiUser : IdentityUser
{
    [MaxLength(25)]
    public string? Nickname { get; set; }

    public string? NicknameColor { get; set; }

    public string? ProfileMainColor { get; set; }

    public string? ProfileSecondaryColor { get; set; }

    public int Level { get; set; } = 1;
}