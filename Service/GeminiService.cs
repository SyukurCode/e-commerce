using E_Commers_Adelia.Common;
using E_Commers_Adelia.Models;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Serilog;

namespace E_Commers_Adelia.Service
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public GeminiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ReceiptData> ExtractReceiptAsync(string imagePath, decimal amount, DateTime purchaseDate)
        {
            var apiKey = EnvHelper.GetEnv("GEMINI_API_KEY");
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            // Baca image dan convert ke base64
            var imageBytes = await File.ReadAllBytesAsync(imagePath);
            var base64Image = Convert.ToBase64String(imageBytes);

            // Prompt
            var requestBody = new
            {
                contents = new[]
                {
                new {
                    parts = new object[]
                    {
                        new { text = "You are given a photo of payment receipt." +
                                     "Extract these fields in json" +
                                     "{ ReferenceNo" +
                                     " Amount(without RM)," +
                                     " Datetime(format YYYY-MM-DD H:mm:ss)," +
                                     " BankName," +
                                     " RecipientName," +
                                     " isValid(true/false)" +
                                     " Explaination : (Identify receipt originality accept for screenshoot.)}"

                        },
                        new {
                            inline_data = new {
                                mime_type = "image/jpeg",
                                data = base64Image
                            }
                        }
                    }
                }
            }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            string? text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            text = text.Replace("```", "").Replace("json", "");
            // terus deserialize ke class
            var obj = JsonSerializer.Deserialize<JsonElement>(text);

            ReceiptData data = new ReceiptData
            {
                RecipientName = obj.GetProperty("RecipientName").GetString(),
                Amount = Math.Round(Decimal.Parse(obj.GetProperty("Amount").GetString()), 2),
                BankName = obj.GetProperty("BankName").GetString(),
                Datetime = DateTime.Parse(obj.GetProperty("Datetime").GetString()),
                isValid = obj.GetProperty("isValid").GetBoolean(),
                ReferenceNo = obj.GetProperty("ReferenceNo").GetString(),
                Explanation = obj.GetProperty("Explanation").GetString()
            };

            // verify time payment || purchase date must be lower than receiptdate
            double diffMinutes = Math.Abs((data.Datetime - purchaseDate).TotalMinutes);
            if(diffMinutes < 0)
            {
                data.isValid = false;
                data.Explanation = "Receipt not valid, payment already make before purchase.";
            }

            if(amount != data.Amount)
            {
                data.isValid = false;
                data.Explanation = "Receipt not valid, amount not valid";
            }

            Log.Information($"Payment not valid.{data.Explanation}");

            return data;
        }
    }
}
