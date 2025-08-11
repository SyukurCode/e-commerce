namespace E_Commers_Adelia.Common
{
    public enum SizeUnit
    {
        Bytes,
        KB,
        MB,
        GB
    }

    public static class SizeConverter
    {
        public static long Convert(double value, SizeUnit unit)
        {
            switch (unit)
            {
                case SizeUnit.KB:
                    return (long)(value * 1024);
                case SizeUnit.MB:
                    return (long)(value * 1024 * 1024);
                case SizeUnit.GB:
                    return (long)(value * 1024 * 1024 * 1024);
                default:
                    return (long)value;
            }
        }

        public static long ConvertFromString(string sizeText)
        {
            // buang space dan uppercase semua
            sizeText = sizeText.Trim().ToUpper();

            // pisahkan nombor dan unit
            string numberPart = new string(sizeText.TakeWhile(char.IsDigit).ToArray());
            string unitPart = new string(sizeText.SkipWhile(char.IsDigit).ToArray());

            if (string.IsNullOrEmpty(numberPart))
                throw new ArgumentException("Invalid size format");

            double value = double.Parse(numberPart);

            // tentukan unit
            SizeUnit unit = unitPart switch
            {
                "B" => SizeUnit.Bytes,
                "KB" => SizeUnit.KB,
                "MB" => SizeUnit.MB,
                "GB" => SizeUnit.GB,
                _ => throw new ArgumentException("Unknown unit")
            };

            return Convert(value, unit);
        }
    }

}
