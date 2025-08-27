namespace E_Commers_Adelia.Models
{
    public class UserRole
    {
        public EUser EUser { get; set; }
        public List<string> Roles { get; set; } = new List<string>(); // roles yang dipilih user
    }
}
