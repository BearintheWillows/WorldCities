namespace WorldCitiesApi.Data.DTOs.AuthDTO;

using System.ComponentModel.DataAnnotations;

public class LoginRequestDTO {
	[Required( ErrorMessage = "Email is Required." )]
	public string Email { get; set; } = null!;

	[Required( ErrorMessage = "Password is Required." )]
	public string Password { get; set; } = null!;
}