namespace E_Commers_Adelia.Models
{
    public class VerificationResult
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public VerificationResult(bool status, string message)
        {
            Status = status;
            Message = message;
        }
    }
}
