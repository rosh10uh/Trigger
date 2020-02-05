using System;
using System.Collections.Generic;
using System.Text;

namespace Trigger.DTO.ServerSideValidation
{
   public static class ValidationResource
    {
        // Employee

        public const string dateOfBirth = "dateOfBirth";
        public const string lastIncDate = "lastIncDate";
        public const string lastPromodate = "lastPromodate";
        public const string empId = "empId";
        public const string joiningDate = "joiningDate";
        public const string countryId = "countryId";

        // Company  
        public const string contractEndDate = "contractEndDate";
        public const string compId = "compId";

        //Contry IDS
        public const int Canada = 37;
        public const int US = 215;

        //Team configuration
        public const string TeamStartDate = "StartDate";
        public const string TeamEndDate = "EndDate";
        public const string TeamId = "TeamId";
    }
}
