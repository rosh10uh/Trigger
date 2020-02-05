using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.DAL.Assessment
{
    public class AssessmentContext
    {
        /// <summary>
		/// Use to get service of DbContext
		/// </summary>
		private readonly ILogger<AssessmentContext> _logger;
        private readonly IConnectionContext _connectionContext;
        private readonly string _storageAccountURL;
        private readonly string _storageAccountName;
        private readonly string _storageAccountAccessKey;
        private readonly string _blobContainer = Messages.AssessmentDocPath;

        /// <summary>
        /// Initializes a new instance of the roles class using IDaoContextFactory and Connection.
        /// </summary>
        /// <param name="daoContextFactory"></param>
        /// <param name="connection"></param>
        public AssessmentContext(IConnectionContext connectionContext, IOptions<AppSettings> appSettings, ILogger<AssessmentContext> logger)
        {
            _connectionContext = connectionContext;
            _storageAccountName = appSettings.Value.StorageAccountName;
            _storageAccountAccessKey = appSettings.Value.StorageAccountAccessKey;
            _storageAccountURL = appSettings.Value.StorageAccountURL;
            _logger = logger;
        }

        /// <summary>
        /// Insert Assessment
        /// </summary>
        /// <returns> result</returns>
        public int AddAssessmentMainV1(EmpAssessmentModel empAssessmentModel)
        {
            int result = 0;
            try
            {
                _connectionContext.TriggerContext.BeginTransaction();
                int assessmentID = InsertAssessmentV1(empAssessmentModel);

                if (assessmentID > 0)
                {
                    foreach (var ls in empAssessmentModel.empassessmentdet)
                    {
                        ls.assessmentId = assessmentID;
                    }

                    result = InsertAssessmentDetailsV1(empAssessmentModel.empassessmentdet);

                    //Added by Vivek Bhavsar on 24-12-2018 to update score & rank for employee
                    if (result > 0)
                    {
                        result = UpdateScoreRank(new EmpAssessmentScore { empId = empAssessmentModel.empId, assessmentId = assessmentID });
                    }
                }
                _connectionContext.TriggerContext.Commit();
                return result;
            }
            catch (Exception ex)
            {
                _connectionContext.TriggerContext.RollBack();
                _logger.LogError(ex.Message.ToString());
                throw;
            }
        }

        /// <summary>
        /// For API Version 2.0
        /// Add Assessment Details with Attachments
        /// Upload Documents for General comment & Documents for all question categories
        /// </summary>
        /// <param name="empAssessmentModel"></param>
        /// <returns></returns>
        public async Task<EmpAssessmentModel> AddAssessmentMainV2(EmpAssessmentModel empAssessmentModel)
        {
            EmpAssessmentModel resultEmpAssessment=new EmpAssessmentModel();
            List<DocumentDetails> uploadDocuments = new List<DocumentDetails>();
            try
            {
                _connectionContext.TriggerContext.BeginTransaction();

                if (empAssessmentModel.DocumentName.Length > 0 && empAssessmentModel.DocumentContents.Length > 0)
                {
                    empAssessmentModel.DocumentName = FileActions.GenerateUniqueDocumentName(empAssessmentModel.DocumentName);
                    uploadDocuments.Add(new DocumentDetails { DocumentName = empAssessmentModel.DocumentName, DocumentContents = empAssessmentModel.DocumentContents });
                }

                int assessmentID = InsertAssessmentV2(empAssessmentModel);

                if (assessmentID > 0)
                {
                    resultEmpAssessment.assessmentId = assessmentID;
                  int result = InsertAssessmentDetailsV2(assessmentID, uploadDocuments, empAssessmentModel);
                    if (result > 0)
                    {
                        resultEmpAssessment.Result = result;
                        UpdateScoreRank(new EmpAssessmentScore { empId = empAssessmentModel.empId, assessmentId = assessmentID });
                        UploadDocumentsParrallel(uploadDocuments);
                    }
                }
                _connectionContext.TriggerContext.Commit();
                return await Task.FromResult(resultEmpAssessment);
            }
            catch (Exception ex)
            {
                _connectionContext.TriggerContext.RollBack();
                _logger.LogError(ex.Message.ToString());
                throw;
            }
        }

        /// <summary>
        /// Method to insert assessment details 
        /// </summary>
        /// <param name="assessmentID"></param>
        /// <param name="uploadDocuments"></param>
        /// <param name="empAssessmentModel"></param>
        /// <returns></returns>
        private int InsertAssessmentDetailsV2(int assessmentID,List<DocumentDetails> uploadDocuments, EmpAssessmentModel empAssessmentModel)
        {
            foreach (var ls in empAssessmentModel.empassessmentdet)
            {
                ls.assessmentId = assessmentID;

                if (ls.DocumentName.Length > 0 && ls.DocumentContents.Length > 0)
                {
                    ls.DocumentName = FileActions.GenerateUniqueDocumentName(ls.DocumentName);
                    uploadDocuments.Add(new DocumentDetails { DocumentName = ls.DocumentName, DocumentContents = ls.DocumentContents });
                }
            }

           int result = InsertAssessmentDetailsV2(empAssessmentModel.empassessmentdet);

            return result;
        }

        /// <summary>
        /// Author : Vivek Bhavsar
        /// Created Date : 16-07-2018
        /// Purpose :  Upload document on Azure Blob
        /// </summary>
        /// <param name="documentDetails"></param>
        public async Task UploadDocumentAsync(Trigger.DTO.DocumentDetails documentDetails)
        {
            try
            {
                if (documentDetails.DocumentContents.Length > 0 && documentDetails.DocumentName.Length >0)
                {
                    await FileActions.UploadtoBlobAsync(documentDetails.DocumentName, documentDetails.DocumentContents, _storageAccountName, _storageAccountAccessKey, _blobContainer);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        /// <summary>
        /// method to upload all category attachments parralleli
        /// </summary>
        /// <param name="documentDetails"></param>
        public void UploadDocumentsParrallel(List<Trigger.DTO.DocumentDetails> documentDetails)
        {
            try
            {
                Parallel.ForEach(documentDetails, (currentFile) =>
                {
                    Task.FromResult(UploadDocumentAsync(currentFile));
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Execute Add Assessment
        /// </summary>
        /// <returns> Assessment Id</returns>
        public int InsertAssessmentV1(EmpAssessmentModel empAssessmentModel)
        {
            return _connectionContext.TriggerContext.AssessmentRepository.InsertAssessment(empAssessmentModel).id;
        }

        /// <summary>
        /// Author  : Vivek Bhavsar
        /// Created Date : 16-07-2018
        /// Purpose :  Execute Add Assessment with Attach Document
        /// </summary>
        /// <returns> Assessment Id</returns>
        public int InsertAssessmentV2(EmpAssessmentModel empAssessmentModel)
        {
            return _connectionContext.TriggerContext.AssessmentRepository.InsertAssessmentWithAttachment(empAssessmentModel).id;
        }

        /// <summary>
        /// Execute Add Assessment Details
        /// </summary>
        /// <returns> result</returns>
        public int InsertAssessmentDetailsV1(List<EmpAssessmentDet> empAssessmentDets)
        {
            var empAssessmentDet = _connectionContext.TriggerContext.AssessmentDetailRepository.InsertAssessmentDetails(empAssessmentDets);
            return Convert.ToInt32(empAssessmentDet.Last().result);
        }

        /// <summary>
        /// Author : Vivek Bhavsar
        /// Created Date : 16-07-2018
        /// Purpose :  Execute Add Assessment Details with Attachments
        /// </summary>
        /// <returns> result</returns>
        public int InsertAssessmentDetailsV2(List<EmpAssessmentDet> empAssessmentDets)
        {
            var empAssessmentDet = _connectionContext.TriggerContext.AssessmentDetailRepository.InsertAssessmentDetailsWithAttachment(empAssessmentDets);
            return Convert.ToInt32(empAssessmentDet.Last().result);
        }

        /// <summary>
        /// Author : Vivek Bhavsar
        /// Created Date : 24-12-2018
        /// Purpose : to update score & its rank for employee on assessment
        /// </summary>
        /// <param name="empAssessmentScore"></param>
        /// <returns></returns>
        private int UpdateScoreRank(EmpAssessmentScore empAssessmentScore)
        {
            var empAssessmentScoreResult = _connectionContext.TriggerContext.EmpAssessmentScoreRepository.UpdateScoreRank(empAssessmentScore);
            return Convert.ToInt32(empAssessmentScoreResult.result);
        }

        /// <summary>
        /// Author : Vivek Bhavsar
        /// Created Date : 18-07-2018
        /// Purpose :  Delete document from Azure Blob
        /// </summary>
        /// <param name="documentDetails"></param>
        public async Task<Remarks> DeleteAssessmentDocument(EmpAssessmentDet empAssessmentDet,bool deleteFromDatabase=true)
        {
            int result=0;
            try
            {
                List<EmpAssessmentDet> empAssessmentDets = new List<EmpAssessmentDet>();
                empAssessmentDets.Add(empAssessmentDet);

                var documentName = _connectionContext.TriggerContext.AssessmentDetailRepository.Select(empAssessmentDets);
                if (documentName.Count > 0 && deleteFromDatabase)
                {
                    result = _connectionContext.TriggerContext.AssessmentDetailRepository.DeleteAssessmentAttachment(empAssessmentDets).FirstOrDefault().result;
                }
                if ((result > 0 || !deleteFromDatabase) && documentName.Count > 0 && documentName.FirstOrDefault().DocumentName.Length > 0)
                {
                    await FileActions.DeleteFileFromBlobStorage(documentName.FirstOrDefault().DocumentName, _storageAccountName, _storageAccountAccessKey, _blobContainer);
                }
                var remarks = GetRemarkResponse(empAssessmentDet);
                remarks.status = result;
                return remarks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                throw;
            }
        }


        /// <summary>
        /// Author : Vivek Bhavsar
        /// Created Date : 19-07-2018
        /// Purpose : Update general & category wise comments
        /// </summary>
        /// <param name="EmpAssessmentDet"></param>
        /// <returns></returns>
        public async Task<Remarks> UpdateAssessmentComment(EmpAssessmentDet empAssessmentDet)
        {
            try
            {
                List<EmpAssessmentDet> empAssessmentDets = new List<EmpAssessmentDet>();
                empAssessmentDets.Add(empAssessmentDet);
                if (empAssessmentDet.DocumentName.Length == 0 && empAssessmentDet.DocumentContents.Length == 0)
                {
                    await  DeleteAssessmentDocument(empAssessmentDet);
                }
                else if (empAssessmentDet.DocumentName.Length > 0 && empAssessmentDet.DocumentContents.Length > 0)
                {
                    await DeleteAssessmentDocument(empAssessmentDet,false);
                    DocumentDetails documentDetails = new DocumentDetails { DocumentName = FileActions.GenerateUniqueDocumentName(empAssessmentDet.DocumentName), DocumentContents = empAssessmentDet.DocumentContents };
                    empAssessmentDet.DocumentName = documentDetails.DocumentName;
                    await UploadDocumentAsync(documentDetails);
                }

                var result = GetRemarkResponse(empAssessmentDet);
                result.status = _connectionContext.TriggerContext.AssessmentDetailRepository.UpdateAssessmentComment(empAssessmentDets).FirstOrDefault().result;

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                throw;
            }
        }

        /// <summary>
        /// Generate response to get assessment id,remark id & document name
        /// </summary>
        /// <param name="empAssessmentDet"></param>
        /// <returns></returns>
        private Remarks GetRemarkResponse(EmpAssessmentDet empAssessmentDet)
        {
            var remarks = new Remarks { AssessmentId = empAssessmentDet.assessmentId, RemarkId = empAssessmentDet.remarkId,CloudFilePath=empAssessmentDet.CloudFilePath, assessmentDate = empAssessmentDet.commentUpdDateTime};

            if (empAssessmentDet.DocumentName.Length > 0)
            {
                remarks.DocumentName = string.Concat(_storageAccountURL, Messages.slash, Messages.AssessmentDocPath, Messages.slash, empAssessmentDet.DocumentName);
            }

            return remarks;
        }

        /// <summary>
        /// Author : Vivek Bhavsar
        /// Created Date : 19-07-2018
        /// Purpose :  Delete document from Azure Blob & Comment from  database
        /// </summary>
        /// <param name="documentDetails"></param>
        public async Task<int> DeleteAssessmentComment(EmpAssessmentDet EmpAssessmentDet)
        {
            int result = 0;
            try
            {
                List<EmpAssessmentDet> empAssessmentDets = new List<EmpAssessmentDet>();
                empAssessmentDets.Add(EmpAssessmentDet);
                var documentName = _connectionContext.TriggerContext.AssessmentDetailRepository.Select(empAssessmentDets);
                result = _connectionContext.TriggerContext.AssessmentDetailRepository.DeleteAssessmentComment(empAssessmentDets).FirstOrDefault().result;

                if (result > 0 && documentName.Count > 0 && documentName.FirstOrDefault().DocumentName.Length > 0)
                {
                    await FileActions.DeleteFileFromBlobStorage(documentName.FirstOrDefault().DocumentName, _storageAccountName, _storageAccountAccessKey, _blobContainer);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                throw;
            }
        }
    }
}
