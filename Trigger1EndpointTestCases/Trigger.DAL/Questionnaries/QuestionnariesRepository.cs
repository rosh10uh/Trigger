using OneRPP.Restful.DAO;
using System.Collections.Generic;
using Trigger.DTO;
using Trigger.DTO.Score;

namespace Trigger.DAL.Questionnaries
{
    public class QuestionnariesRepository : DaoRepository<QuestionAnswer>
    {
        public const string GetAllQuestionnaries = "GetAllQuestionnaries";
        public const string GetAllCategories = "GetAllCategories";
        public const string GetAllScoreRanks = "GetAllScoreRanks";

        public virtual List<QuestionAnswer> InvokeGetQuestionnaries()
        {
            return ExecuteQuery<List<QuestionAnswer>>(null, GetAllQuestionnaries);
        }

        public List<Categories> InvokeGetAllCategories()
        {
            return ExecuteQuery<List<Categories>>(null, GetAllCategories);
        }

        public List<ScoreGradeModel> InvokeGetScoreRanks()
        {
            return ExecuteQuery<List<ScoreGradeModel>>(null, GetAllScoreRanks);
        }
    }
}
