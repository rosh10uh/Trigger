﻿using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.DTO;

namespace Trigger.DAL.EmpProfileRepo
{
    /// <summary>
    /// Class to manage employee profile picture
    /// </summary>
    [QueryPath("Trigger.DAL.Query.EmpProfile.EmpProfile")]
    public class EmpProfileRepository : DaoRepository<EmployeeProfilePicture>
    {
    }
}
