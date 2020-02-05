using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trigger.BLL.ContactUs;
using Trigger.BLL.Country;
using Trigger.BLL.Shared;
using Trigger.DTO;
using Trigger.Middleware;
using Trigger.Utility;

namespace Trigger.Controllers.V1
{

    /// <summary>
    /// Class Name      :   ContactUsController
    /// Author          :   Mayur Patel
    /// Creation Date   :   04 Jun 2019
    /// Purpose         :   API to Send support email to support team
    /// Revision        :   
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        private readonly ContactUs _contactUs;
        /// <summary>
        /// Name            :   ContactUsController
        /// Author          :    Mayur Patel
        /// Creation Date   :   04 Jun 2019
        /// Purpose         :   constructor for ContactUsController
        /// Revision        :   
        /// </summary>
        /// <param name="contactUs"></param>
        public ContactUsController(ContactUs contactUs)
        {
            _contactUs = contactUs;
        }

        /// <summary>
        /// This API is used to send support email to support team
        /// Require Parameters :Full Name, Email, Subject,Comments
        /// </summary>
        /// <param name="contactDetails"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ParameterValidation]
        public async Task<CustomJsonData> post(ContactDetails contactDetails)
        {
            return await _contactUs.InvokeContactUsSupport(contactDetails);
        }

    }
}