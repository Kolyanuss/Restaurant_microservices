namespace Web.Utility
{
    public class StaticDetails
    {
        public static string GatewayBase {  get; set; }
        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
        public const string TokenCookie = "JWTToken";
        public enum ApiType
        {
            GET, POST, PUT, DELETE,
        }
    }
}