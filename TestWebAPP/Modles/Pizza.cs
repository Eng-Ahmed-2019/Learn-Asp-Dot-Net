namespace TestWebAPP.Modles
{
    public class Pizza
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public bool IsGlutenFree { get; set; }
        public string Name { get; set; } = default!;
    }
}