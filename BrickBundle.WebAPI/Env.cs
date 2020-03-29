namespace BrickBundle.WebAPI
{
    public static class Env
    {
        public static string JwtIssuer { get; set; }
        public static string JwtAudience { get; set; }
        public static string JwtSigningKey { get; set; }
    }
}
