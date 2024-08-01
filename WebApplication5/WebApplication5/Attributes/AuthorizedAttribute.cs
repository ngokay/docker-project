namespace WebApplication5.Attributes
{
    public class AccessDeniedAttribute : Attribute
    {
       public string accessDenied { get; set; }
        public AccessDeniedAttribute(string accessDenied)
        {
            this.accessDenied = accessDenied;
        }
    }
}
