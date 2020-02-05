namespace Trigger.DTO.Spark
{
    /// <summary>
    /// Class Name   :   ClassificationModel
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   08 Aug 2019
    /// Purpose      :   DTO class for properties require to get list of classifications used for Spark
    /// Revision     : 
    /// </summary>
    public class ClassificationModel : DocumentDetails
    {
        public int ClassificationId { get; set; }

        public string Classification { get; set; }
    }
}
