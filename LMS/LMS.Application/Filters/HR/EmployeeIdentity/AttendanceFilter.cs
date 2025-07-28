using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Application.Filters.Base;

namespace LMS.Application.Filters.HR.EmployeeIdentity
{
    public class AttendanceFilter : Filter
    {
        public List<Guid>? EmployeeIds { get; set; }
        public bool? IsCheckedIn { get; set; }    
        public bool? IsCheckedOut { get; set; }   

        public bool IsDesc { get; set; } = true;
        public AttendanceOrderBy OrderBy { get; set; } = AttendanceOrderBy.ByDate;
    }
}
