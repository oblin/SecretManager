using System;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

namespace SecretManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = GetUserSecretsConfiguration();
            Console.WriteLine($"MySecretKey: {configuration["AppSettings:MySecretKey"]}");
            Console.WriteLine($"Not Exist: {configuration["AppSettings:NotExist"]}");

            string decryptKey = DecryptProtectedKey(configuration["AppSettings:ProtectedKey"]);
            Console.WriteLine($"Protected Key: {decryptKey}");

            GetProtectedKey();

            Console.ReadLine();
        }

        private static IConfiguration GetUserSecretsConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            builder.AddUserSecrets();

            builder.AddEnvironmentVariables();
            return builder.Build();
        }

        private static void GetProtectedKey()
        {
            // var dataProtectionProvider = DataProtectionProvider.Create(Directory.GetCurrentDirectory());
            string destFolder = Path.Combine(
                Environment.GetEnvironmentVariable("LOCALAPPDATA"),
                "AppSecrets");
            var dataProtectionProvider = DataProtectionProvider.Create(
                new DirectoryInfo(destFolder),
                configuration => 
                {
                    configuration.SetApplicationName("SecretsManager");
                    configuration.ProtectKeysWithDpapi();
                }
            );
            var protector = dataProtectionProvider.CreateProtector("General.Protection");
            Console.Write("Enter inputs (empty to leave): ");
            string input = Console.ReadLine();

            if (!string.IsNullOrEmpty(input))
            {
                string protectedInput = protector.Protect(input);
                Console.WriteLine($"Protect returned: {protectedInput}");
                Console.WriteLine($"UnProtect returned: {protector.Unprotect(protectedInput)}");
            }
        }

        private static string DecryptProtectedKey(string protectedKey)
        {
            var dataProtectionProvider = DataProtectionProvider.Create(Directory.GetCurrentDirectory());
            var protector = dataProtectionProvider.CreateProtector("SecretsManager");
            return protector.Unprotect(protectedKey);
        }
    }
}
