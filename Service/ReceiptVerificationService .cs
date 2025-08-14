using E_Commers_Adelia.Models;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using Tesseract;
using System.Net.Http.Headers;

namespace E_Commers_Adelia.Service
{
    public class ReceiptVerificationService : IReceiptVerificationService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ReceiptVerificationService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<VerificationResult> VerifyReceiptAsync(string dbimagePath, decimal expectedAmount, DateTime purchaseDateTime)
        {
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, dbimagePath);
            var text = ExtractTextFromImage(imagePath);
            var result = AnalyzeReceiptText(text, expectedAmount: expectedAmount, purchaseDateTime: purchaseDateTime.ToLocalTime());
            if (result != null && result.Status == false)
            {
                // buang file not valid
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            return result;
        }
        private VerificationResult AnalyzeReceiptText(string text, decimal expectedAmount, DateTime purchaseDateTime)
        {

            //// ✅ 2. Cari amount
            //var amountMatch = Regex.Match(text, @"\d+\.\d{2}" + @"\b\d{1,3}(?:[.,]\d{2})\b");
            decimal amount = 0;
            // Cari pattern dua nombor berasingan
            var amountMatch = Regex.Match(text, @"\b\d{1,3}(?:[.,]\d{2})\b");
            if (amountMatch.Success)
            {
                amount = decimal.Parse(amountMatch.Value.Replace(",", "."), CultureInfo.InvariantCulture);
            }
            else
            {
                // 2️⃣ Kalau tak jumpa, cuba gabungkan angka & sen berasingan
                var amountSplitMatch = Regex.Match(text, @"\b(\d{1,3})\s*[\.,]?\s*(\d{2})\b");
                if (amountSplitMatch.Success)
                {
                    string merged = $"{amountSplitMatch.Groups[1].Value}.{amountSplitMatch.Groups[2].Value}";
                    decimal.TryParse(merged, NumberStyles.Any, CultureInfo.InvariantCulture, out amount);
                }
            }

            // ✅ 3. Detect tarikh & masa dalam pelbagai format
            string[] possibleFormats = new[]
             {
                "dd/MM/yyyy HH:mm",
                "dd-MM-yyyy HH:mm",
                "dd/MM/yyyy hh:mm tt",
                "dd-MM-yyyy hh:mm tt",
                "yyyy/MM/dd HH:mm",
                "yyyy-MM-dd HH:mm",
                "yyyy/MM/dd hh:mm tt",
                "yyyy-MM-dd hh:mm tt",
                "dd MMM yyyy HH:mm",
                "dd MMM yyyy hh:mm tt",
                "MMM dd yyyy HH:mm",
                "MMM dd yyyy hh:mm tt",
                "dd MMM yyyy, HH:mm",
                "dd MMM yyyy, hh:mm tt",
                "d MMM yyyy, HH:mm",
                "d MMM yyyy, h:mm tt",
                "dd MMM yyyy HH:mm:ss",        // format dengan seconds (24h)
                "dd MMM yyyy hh:mm:ss tt",     // format CIMB dengan AM/PM
                "d MMM yyyy HH:mm:ss",
                "d MMM yyyy hh:mm:ss tt"
            };

                    var dateMatch = Regex.Match(text, @"\b\d{1,2}[/\-]\d{1,2}[/\-]\d{2,4}(\s+\d{1,2}:\d{2}(:\d{2})?(\s?[APMapm]{2})?)?\b"
                                                   + @"|\b\d{1,2}\s+[A-Za-z]{3,}\s+\d{2,4},?\s+\d{1,2}:\d{2}(:\d{2})?(\s?[APMapm]{2})?\b"
                                                   + @"|\b[A-Za-z]{3,}\s+\d{1,2}\s+\d{2,4},?\s+\d{1,2}:\d{2}(:\d{2})?(\s?[APMapm]{2})?\b");

            DateTime receiptDateTime = DateTime.MinValue;
            bool validDateTime = false;

            if (dateMatch.Success)
            {
                string dateText = dateMatch.Value.Trim();

                if (DateTime.TryParseExact(dateText, possibleFormats,
                                           CultureInfo.InvariantCulture,
                                           DateTimeStyles.None,
                                           out receiptDateTime))
                {
                    // Kira beza minit
                    double diffMinutes = Math.Abs((receiptDateTime - purchaseDateTime).TotalMinutes);
                    if (diffMinutes <= 10)
                    {
                        validDateTime = true;
                    }
                }
                else if (DateTime.TryParse(dateText, out receiptDateTime)) // fallback parse
                {
                    double diffMinutes = Math.Abs((receiptDateTime - purchaseDateTime).TotalMinutes);
                    if (diffMinutes <= 10)
                    {
                        validDateTime = true;
                    }
                }
            }

            // ✅ 4. Validation rules
            //if (!containsBank) return new VerificationResult(false, "Nama bank tidak dijumpai");
            //if (amount != expectedAmount) return new VerificationResult(false, "Jumlah tidak sepadan");
            //if (!validDateTime) return new VerificationResult(false, "Tarikh/Masa resit tidak dalam julat ±10 minit");
            if (!validDateTime || amount != expectedAmount)
            {

                return new VerificationResult(false, "Receipt not valid");
            }

            return new VerificationResult(true, "Receipt is valid");
        }
        private string ExtractTextFromImage(string imagePath)
        {

            string tessDataPath = Path.Combine(_webHostEnvironment.WebRootPath, "AI");
            using var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default);
            try 
            { 
                using var img = Pix.LoadFromFile(imagePath);
                using var page = engine.Process(img);
                return page.GetText();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
    
}

