using System.IO.Enumeration;

namespace StringSDK
{
    public static class Config
        {
        public enum Environment
        {
            PROD, SANDBOX, DEV, LOCAL
        }

        public static Environment ENV_DEFAULT = Environment.SANDBOX;

        public static string EnvironmentURL(Environment env)
        {
            return env switch
            {
                Environment.PROD => "https://api.string-api.xyz",
                Environment.SANDBOX => "https://api.sandbox.string-api.xyz",
                Environment.DEV => "https://string-api.dev.string-api.xyz",
                Environment.LOCAL => "http://localhost:5555",
                _ => EnvironmentURL(ENV_DEFAULT),
            };
        }
    }
}
