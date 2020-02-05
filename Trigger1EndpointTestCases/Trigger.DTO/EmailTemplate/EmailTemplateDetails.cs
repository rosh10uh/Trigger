using System;
using System.Collections.Generic;
using System.Text;

namespace Trigger.DTO.EmailTemplate
{
    public class EmailTemplateDetails
    {
        public string templateName { get; set; }
        public int companyId { get; set; }
        /// <summary>
        /// Purpose :SubjectId used for get reset password email template and it is optional field.
        /// </summary>
        public string subjectId { get; set; }
    }
}
