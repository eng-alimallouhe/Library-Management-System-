namespace LMS.Domain.HR.Models
{
    public class Holidy
    {
        public Guid HolidayId { get; set; }

        public string Name { get; set; }
        public DateTime StartDate { get; set; }               
        public DateTime EndDate { get; set; }      
        public string Notes { get; set; }


        public Holidy()
        {
            HolidayId = Guid.NewGuid();
            Name = string.Empty;
            Notes = string.Empty;
        }
    }
}
