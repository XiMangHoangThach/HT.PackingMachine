namespace HT.PackingMachine.Data.Models
{
    public class SalesOrderViewModel
    {
        public DateTime? DELIVERY_DATE { get; set; }
        public string STATUS { get; set; } = string.Empty;



        public string DRIVER_NAME { get; set; } = string.Empty;
        public string VEHICLE_CODE { get; set; } = string.Empty;
        public string DELIVERY_CODE { get; set; } = string.Empty;
        public string CUSTOMER_NAME { get; set; } = string.Empty;

        public string ItemName { get; set; } = string.Empty;

        public string VEHICLECODE { get; set; } = string.Empty;

    }
}
