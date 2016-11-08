using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SecretManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            builder.AddUserSecrets();

            builder.AddEnvironmentVariables();
            var configuration = builder.Build();

            Console.WriteLine($"MySecretKey: {configuration["AppSettings:MySecretKey"]}");
            Console.WriteLine($"Not Exist: {configuration["AppSettings:NotExist"]}");
            Console.ReadLine();
        }
    }
}
