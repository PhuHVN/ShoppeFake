namespace ShoppeFake.Domain.Enums.EnumConfig
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CustomName : Attribute
    {
        public string Names { get; set; }

        public CustomName(string name)
        {
            Names = name;
        }
    }
}
