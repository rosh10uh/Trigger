using System.Collections.Generic;

namespace Trigger.DTO
{
    public class Questionnaries
    {
        public int id { get; set; }
        public int categoryid { get; set; }
        public string category { get; set; }
        public string questions { get; set; }
        public bool isActive { get; set; }
        public int createdby { get; set; }
        public int updatedby { get; set; }
        
        public List<Answers> answers { get; set; }
              
        public Questionnaries()
        {
            answers = new List<Answers>();
         
        }

    }

    public class Answers
    {
        public int id { get; set; }
        public int questionId { get; set; }
        public string answers { get; set; }
        public decimal weightage { get; set; }
        public bool isActive { get; set; }
        public int createdby { get; set; }
        public int updatedby { get; set; }

    }


    public class QuestionAnswer
    {        
        public string category { get; set; }
        public string questions { get; set; }
        public string answer { get; set; }
        public decimal weightage { get; set; }
        public int questionid { get; set; }        
        public int categoryid { get; set; }
        public int answerid { get; set; }
        
    }

    public class Categories
    {
        public int categoryId { get; set; }
        public string category { get; set; }
    }
}
