namespace WebApplication5.Attributes
{
    public class ExcusionAttribute : Attribute
    {
        public string _attribute { get; set; }
        public ExcusionAttribute(string attribute)
        {
            _attribute = attribute;
        }
    }
}
