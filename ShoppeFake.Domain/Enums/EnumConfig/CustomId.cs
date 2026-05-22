namespace ShoppeFake.Domain.Enums.EnumConfig
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CustomId : Attribute
    {
        public string Type { get; set; }
        public CustomId(string type)
        {
            Type = type;
        }
    }
}
