namespace OrderSystem.DTOs
{
    public class FilterOrderDTO
    { 
        public List<GetOrderDTO>? OrderList { get; set; }
        public string? ClientName { get; set; }
        public string? StatusName { get; set; }
    }
}
