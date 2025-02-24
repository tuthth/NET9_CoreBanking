namespace API.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class JWTRequiredAtrribute : Attribute
    {
    }
}
