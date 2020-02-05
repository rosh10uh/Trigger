using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.Score;
using Trigger.Utility;

namespace Trigger.BLL.Questionnaries
{
    public class Questionnaries
    {
        private readonly ILogger<Questionnaries> _logger;
        private readonly IConnectionContext _connectionContext;

        /// <summary>
        /// Use to initialize questionnaires
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="logger"></param>
        public Questionnaries(IConnectionContext connectionContext, ILogger<Questionnaries> logger)
        {
            _connectionContext = connectionContext;
            _logger = logger;
        }

        /// <summary>
        /// Use to get all questionnaires details
        /// </summary>
        /// <returns></returns>
        public async Task<CustomJsonData> GetAllQuestionnaries()
        {
            try
            {
                List<QuestionAnswer> questionAnswars = await Task.FromResult(_connectionContext.TriggerContext.QuestionnariesRepository.InvokeGetQuestionnaries());
                DataTable questionnariesDataTable = Common.ToDataTable(questionAnswars);
                List<QuestionnariesCategory> lstQuestions = GetAllQuestWithAnswers(questionnariesDataTable);
                return JsonSettings.UserCustomDataWithStatusMessage(lstQuestions, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// To get all question with answers
        /// </summary>
        /// <param name="questions"></param>
        /// <returns></returns>
        public List<QuestionnariesCategory> GetAllQuestWithAnswers(DataTable questions)
        {
            try
            {
                DataTable category = questions.DefaultView.ToTable(true, "categoryid", "category").DefaultView.ToTable();
                if (questions.Rows.Count > 0)
                {
                    return (from DataRow dr in category.Select("category <> ''")
                            orderby Convert.ToInt32(dr["categoryid"])
                            select new QuestionnariesCategory()
                            {
                                categoryid = Convert.ToInt32(dr["categoryid"]),
                                category = Convert.ToString(dr["category"]),
                                lstQuestionneries = GetQuestionaries(questions, Convert.ToInt32(dr["categoryid"]))

                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
            return new List<QuestionnariesCategory>();
        }

        /// <summary>
        /// To get list of questionnaires
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        private List<DTO.Questionnaries> GetQuestionaries(DataTable questions, int categoryId)
        {
            return (from DataRow cr in questions.Select("categoryid = " + categoryId + Messages.questionsMsg)
                    orderby Convert.ToInt32(cr["questionid"])
                    select new DTO.Questionnaries()
                    {
                        questions = Convert.ToString(cr["questions"]),
                        id = Convert.ToInt32(cr["questionid"]),
                        categoryid = Convert.ToInt32(cr["categoryid"]),
                        category = Convert.ToString(cr["category"]),

                        answers = (from DataRow ar in questions.Select("questionid = " + Convert.ToInt32(cr["questionid"]))
                                   select new Answers()
                                   {
                                       id = Convert.ToInt32(ar["answerid"]),
                                       questionId = Convert.ToInt32(ar["questionid"]),
                                       answers = Convert.ToString(ar["answer"]),
                                       weightage = Convert.ToDecimal(ar["weightage"])
                                   }).ToList()

                    }).ToList();
        }

        /// <summary>
        /// Method to get list of all categories from category master
        /// </summary>
        /// <returns></returns>
        public async Task<CustomJsonData> GetAllCategories()
        {
            try
            {
                List<Categories> categories = await Task.FromResult(_connectionContext.TriggerContext.QuestionnariesRepository.InvokeGetAllCategories());

                return JsonSettings.UserCustomDataWithStatusMessage(categories, Enums.StatusCodes.status_200.GetHashCode(), Messages.ok);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }


        /// <summary>
        /// To get score ranks 
        /// </summary>
        /// <returns></returns>
        public async Task<CustomJsonData> GetScoreRanks()
        {
            try
            {
                List<ScoreGradeModel> scoreRanks = await Task.FromResult(_connectionContext.TriggerContext.QuestionnariesRepository.InvokeGetScoreRanks());

                return JsonSettings.UserCustomDataWithStatusMessage(scoreRanks, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }
    }
}
