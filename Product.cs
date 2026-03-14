public class Product
{
    public bool Active { get; set; }
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Category Category { get; set; }
    public int CategoryId { get; set; }
    public ICollection<Tag> Tags { get; set; }
}
