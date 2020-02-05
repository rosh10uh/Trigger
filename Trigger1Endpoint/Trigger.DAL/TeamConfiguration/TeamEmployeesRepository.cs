using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System;
using System.Collections.Generic;
using Trigger.DTO.TeamConfiguration;

namespace Trigger.DAL.TeamConfiguration
{
    [QueryPath("Trigger.DAL.Query.TeamConfiguration.TeamConfiguration")]
    public class TeamEmployeesRepository : DaoRepository<List<TeamEmployeesModel>>
    {
        private const string invokeAddTeamEmployeesConfiguration = "AddTeamEmployees";
        private const string invokeUpdateTeamEmployeesConfiguration = "UpdateTeamEmployees";

        public List<TeamEmployeesModel> InsertTeamEmployees(List<TeamEmployeesModel> teamEmployees)
        {
            return ExecuteQuery<List<TeamEmployeesModel>>(teamEmployees, invokeAddTeamEmployeesConfiguration);
        }

        public List<TeamEmployeesModel> UpdateTeamEmployees(List<TeamEmployeesModel> teamEmployees)
        {
            return ExecuteQuery<List<TeamEmployeesModel>>(teamEmployees, invokeAddTeamEmployeesConfiguration);
        }



    }
}
