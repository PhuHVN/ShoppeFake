namespace ShoppeFake.Application.DTOs.ValueDtos
{
    public class ValueResponse
    {
        public int Id { get; set; }
        public int AttributeId { get; set; }
        public string ValueText { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

    }
}
