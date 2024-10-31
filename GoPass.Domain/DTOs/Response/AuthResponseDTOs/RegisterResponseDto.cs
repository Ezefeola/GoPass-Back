namespace GoPass.Domain.DTOs.Response.AuthResponseDTOs
{
    public class RegisterResponseDto
    {
        //public string Name { get; set; }
        public string Email { get; set; } = default!;
        public string Token { get; set; } = default!;
    }
}
