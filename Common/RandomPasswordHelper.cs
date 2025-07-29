using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data.SqlTypes;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace E_Commers_Adelia.Common
{
    public static class RandomPasswordHelper
    {
        public static string Generate(PasswordOptions opts = null)
        {
            //const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            //const string lower = "abcdefghijklmnopqrstuvwxyz";
            //const string digits = "0123456789";
            //const string symbols = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            //string allChars = upper + lower + digits + symbols;
            //Random rand = new Random();

            //return new string(Enumerable.Range(0, length)
            //    .Select(x => allChars[rand.Next(allChars.Length)]).ToArray());
            if (opts == null)
            {
                opts = new PasswordOptions
                {
                    RequiredLength = 8,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireUppercase = true,
                    RequireNonAlphanumeric = true
                };
            }

            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$?_-"                        // non-alphanumeric
        };

            var rand = new Random();
            var chars = new StringBuilder();

            if (opts.RequireUppercase)
                chars.Append(randomChars[0][rand.Next(randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Append(randomChars[1][rand.Next(randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Append(randomChars[2][rand.Next(randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Append(randomChars[3][rand.Next(randomChars[3].Length)]);

            while (chars.Length < opts.RequiredLength)
            {
                string rcs = randomChars[rand.Next(randomChars.Length)];
                chars.Append(rcs[rand.Next(rcs.Length)]);
            }

            // Shuffle the result
            return new string(chars.ToString().OrderBy(x => rand.Next()).ToArray());
        }
    }
}
