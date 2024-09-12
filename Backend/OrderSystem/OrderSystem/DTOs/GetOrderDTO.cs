namespace OrderSystem.DTOs
{
    public class GetOrderDTO
    {
        public int OrderID { get; set; }
        public string OrderName { get; set; }
        public DateTime OrderDate { get; set; }
        public int NumberOfCopies { get; set; }
        public string OrderStatus { get; set; }
        public string ClientName { get; set; }
        public string UserName { get; set; }

        public DateTime DateIn { get; set; }
    }
}
