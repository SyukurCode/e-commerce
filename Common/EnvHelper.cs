namespace E_Commers_Adelia.Common
{
    public static class EnvHelper
    {
        public static string GetEnv(string key, string defaultValue = "")
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            return value;
        }
    }
}
