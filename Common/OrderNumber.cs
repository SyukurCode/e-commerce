namespace E_Commers_Adelia.Common
{
    public static class OrderNumber
    {
        public static string Generate()
        {
            string prefix = "ORD";
            string datePart = DateTime.Now.ToString("yyyyMMdd"); // Contoh: 20250729
            string randomPart = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(); // Contoh: AB1234

            return $"{prefix}{datePart}-{randomPart}";
        }

    }
}
