namespace Web.Utility
{
    public class StaticDetails
    {
        public static string CouponApiBase {  get; set; }
        public enum ApiType
        {
            GET, POST, PUT, DELETE,
        }
    }
}