using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Domain.HR.Models
{
    public class LeaveBalance
    {
        public Guid LeaveBalanceId { get; set; }
        public Guid EmployeeId { get; set; }
        public int Year { get; set; }


        //Base Balance for current year:
        public int BaseAnnualBalance { get; set; }

        // Remain balance from the previos year
        public int CarriedOverBalance { get; set; }


        [NotMapped]
        public int TotalBalance => BaseAnnualBalance + CarriedOverBalance;

        public int UsedBalance { get; set; }

        
        [NotMapped]
        public int RemainingBalance => TotalBalance - UsedBalance;

        public Employee Employee { get; set; }


        public LeaveBalance()
        {
            LeaveBalanceId = Guid.NewGuid();
            Employee = null!;
        }
    }

}
