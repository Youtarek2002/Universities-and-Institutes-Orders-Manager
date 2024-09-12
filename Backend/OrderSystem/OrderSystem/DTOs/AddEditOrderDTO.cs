namespace OrderSystem.DTOs
{
    public class AddEDITOrderDTO
    {
        public String OrderName { get; set; }
        public DateTime OrderDate { get; set; }
        public int NumberOfCopies { get; set; }
        public int ClientID { get; set; }
        public int OrgId { get; set; }

    }
}
