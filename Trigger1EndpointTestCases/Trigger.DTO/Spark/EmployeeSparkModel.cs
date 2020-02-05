using System;

namespace Trigger.DTO.Spark
{
    /// <summary>
    /// Class Name   :   EmployeeSparkModel
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   09 Aug 2019
    /// Purpose      :   DTO class for properties require to perform CRUD operations for Spark
    /// Revision     : 
    /// </summary>
    public class EmployeeSparkModel : ClassificationModel
    {
        public int CompanyId { get; set; }

        public int EmpId { get; set; }

        public string EmployeeId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int SparkId { get; set; }

        public int CategoryId { get; set; }

        public string Category { get; set; }

        public DateTime SparkDate { get; set; }

        public int SparkBy { get; set; }

        public string SparkByFirstName { get; set; }

        public string SparkByLastName { get; set; }

        public string Remarks { get; set; }

        public bool ViaSms { get; set; }

        public bool BActive { get; set; }

        public int ApprovalStatus { get; set; }

        public int ApprovalBy { get; set; }

        public DateTime ApprovalDate { get; set; }

        public string RejectionRemark { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public int Result { get; set; }

        public string PhoneNumber { get; set; }

        public string SenderPhoneNumber { get; set; }

        public string SparkByImgPath { get; set; }

    }

    public class SparkRejectionDetails
    {
        public int SparkId { get; set; }

        public int EmpId { get; set; }

        public string EmployeeName { get; set; }

        public int ApprovalBy { get; set; }

        public string RejectedByName { get; set; }

        public string RejectionRemark { get; set; }

        public string SenderPhoneNumber { get; set; }
    }

    public class AspnetUserDetails : Claims
    {
        public string Email { get; set; }
    }
}
