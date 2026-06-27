namespace project_cvrde.Models
{
    public class Complaint
    {
        public int ComplaintID { get; set; }

        public int EmployeeID { get; set; }

        public int DepartmentID { get; set; }

        public string? ComplaintDescription { get; set; }

        public DateTime RaisedDate { get; set; }

        public DateTime ExpectedResolutionDate { get; set; }

        public string? Status { get; set; }

        public string? Priority { get; set; }
    }
}