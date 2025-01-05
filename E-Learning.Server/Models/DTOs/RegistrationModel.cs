namespace E_Learning.Server.Models.DTOs
{
    public class RegistrationModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Role Role { get; set; }
        public string? Speciality { get; set; }

    }
}
