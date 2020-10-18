namespace HireMe.Entities.Models
{
    using HireMe.Entities.Models.Chart;

    public class Views : Charts
    {
        public string PostId { get; set; }
        public int Value { get; set; }
    }
   
}