namespace LocalAPI.Model
{
    public class Investment
    {
        public int inv_Id { get; set; }
        public int user_id { get; set; }
        public int type_id { get; set; }
        public int option_id { get; set; }
        public decimal amount_invested { get; set; }
        public DateTime investment_date { get; set; }
    }
}
