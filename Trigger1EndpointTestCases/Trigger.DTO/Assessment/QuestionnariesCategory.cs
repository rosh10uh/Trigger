using System.Collections.Generic;

namespace Trigger.DTO
{
    public class QuestionnariesCategory
    {
        public int categoryid { get; set; }
        public string category { get; set; }
        public bool isActive { get; set; }
        public string createdby { get; set; }
        public string updatedby { get; set; }

        public List<DTO.Questionnaries> lstQuestionneries { get; set; }


        public QuestionnariesCategory()
        {
            lstQuestionneries = new List<DTO.Questionnaries>();

        }
    }

    public class QuestionnariesQueAnsByCategoryModel
    {
        public int categoryid { get; set; }
        public string category { get; set; }
        public string questions { get; set; }
        public int questionId { get; set; }
        public string answer { get; set; }
        public int answerId { get; set; }
        public decimal weightage { get; set; }
    }

}
