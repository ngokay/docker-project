namespace WebApplication5.Attributes
{
    public class RevockTokenAttribute : Attribute
    {
        public string revockToken;
        public RevockTokenAttribute(string revockToken)
        {
            this.revockToken = revockToken;
        }
    }
}
