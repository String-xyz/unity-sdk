namespace StringSDK
{
    public class Environment
        {
        private const string URL_API_PROD = "https://api.string-api.xyz";
        private const string URL_API_SANDBOX = "https://api.sandbox.string-api.xyz";
        private const string URL_API_DEV = "https://string-api.dev.string-api.xyz";
        // TODO: Remove exposed local env before product launch
        private const string URL_API_LOCAL = "http://localhost:5555";

        public enum Types
        {
            PROD,
            SANDBOX,
            DEV,
            LOCAL
        }

        public const Types DEFAULT = Types.SANDBOX;

        public static string ToUrl(Types env)
        {
            return env switch
            {
                Types.PROD => URL_API_PROD,
                Types.SANDBOX => URL_API_SANDBOX,
                Types.DEV => URL_API_DEV,
                Types.LOCAL => URL_API_LOCAL,
                _ => ToUrl(DEFAULT),
            };
        }

        public static Types ToType(string url)
        {
            return url switch
            {
                    URL_API_PROD => Types.PROD,
                    URL_API_SANDBOX => Types.SANDBOX,
                    URL_API_DEV => Types.DEV,
                    URL_API_LOCAL => Types.LOCAL,
                    _ => DEFAULT,
            };
        }
    }
}
