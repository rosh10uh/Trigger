using System;
using System.Collections.Generic;
using Trigger.DTO.ServerSideValidation;

namespace Trigger.DTO
{
    public class EmpAssessmentModel: DocumentDetails
    {
        public int id { get; set; }
        public int empId { get; set; }
        public DateTime assessmentDate { get; set; }
        public string assessmentPeriod { get; set; }
        public int assessmentBy { get; set; }
        public string remarks { get; set; }
        public int generalStatus { get; set; }
        public int remarksType { get; set; }
        public bool bactive { get; set; }
        public int createdBy { get; set; }
        public int updatedBy { get; set; }
        public int assessmentId { get; set; }
        public int questionId { get; set; }
        public List<EmpAssessmentDet> empassessmentdet { get; set; }
        public List<QuestionnariesCategory> questCategories { get; set; }
        public int Result { get; set; }


        public EmpAssessmentModel()
        {
            empassessmentdet = new List<EmpAssessmentDet>();
            questCategories = new List<QuestionnariesCategory>();
        }

    }

    public class EmpAssessmentDet: DocumentDetails
    {
        public int assessmentId { get; set; }
        public int questionId { get; set; }
        public DateTime commentUpdDateTime { get; set; }
        public int remarkId { get; set; }
        public int answerid { get; set; }
        public string remarks { get; set; }
        public int kpiStatus { get; set; }
        public bool bactive { get; set; }
        public int createdBy { get; set; }
        public int updatedBy { get; set; }
        public int result { get; set; }

    }

    public class PartialAssessmentDet
    {
        public int empId { get; set; }
        public int assessmentBy { get; set; }
        public DateTime assessmentDate { get; set; }
        public string kpiRemarks { get; set; }
        public int kpiStatus { get; set; }
        public string generalRemarks { get; set; }
        public int generalStatus { get; set; }
        public int createdBy { get; set; }
        public int result { get; set; }
    }

    public class LstEmpAssessment
    {
        public int id { get; set; }
        public int empId { get; set; }
        public DateTime assessmentDate { get; set; }
        public string assessmentPeriod { get; set; }
        public int assessmentBy { get; set; }
        public string GeneralRemarks { get; set; }
        public int generalStatus { get; set; }
        public int categoryId { get; set; }
        public string category { get; set; }
        public int questionId { get; set; }
        public string questions { get; set; }
        public int answerId { get; set; }
        public string answer { get; set; }
        public int weightage { get; set; }
        public string KPIRemarks { get; set; }
        public int KPIStatus { get; set; }
        public int isSelected { get; set; }
    }
    public class LastEmpAssessmentMain
    {
        public int id { get; set; }
        public int empId { get; set; }
        public DateTime assessmentDate { get; set; }
        public string assessmentPeriod { get; set; }
        public int assessmentBy { get; set; }
        public string GeneralRemarks { get; set; }
        public int generalStatus { get; set; }

        public List<LastEmpAssessmentDetails> lstAssessmentdet { get; set; }

        public LastEmpAssessmentMain()
        {
            lstAssessmentdet = new List<LastEmpAssessmentDetails>();
        }

    }

    public class LastEmpAssessmentDetails
    {
        public int categoryId { get; set; }
        public string category { get; set; }
        public int questionId { get; set; }
        public string questions { get; set; }
        public int answerId { get; set; }
        public string answers { get; set; }
        public int weightage { get; set; }
        public string kpiRemarks { get; set; }
        public int kpiStatus { get; set; }
        public int isSelected { get; set; }

    }

    public class EmpAssessmentScore
    {
        public int empId { get; set; }
        public int assessmentId { get; set; }
        public int result { get; set; }
    }

    /// <summary>
    /// For document details
    /// </summary>
    public class DocumentDetails
    {
        private string _documentName;
        private string _documentContents;
        private string _cloudFilePath;

        [RequiredDocumentNameAttribute(ValidationMessage.DocumentNameRequired,nameof(DocumentContents))]
        public string DocumentName
        {
            get
            {
               return _documentName == null ? "" : _documentName;
            }
            set { _documentName = value; }
        }

        public string DocumentContents
        {
            get
            {
                return _documentContents == null ? "" : _documentContents;
            }
            set { _documentContents = value; }
        }

        [RequiredCloudFilePath(ValidationMessage.CloudFilePathRequired, nameof(DocumentName))]
        public string CloudFilePath
        {
            get
            {
                return _cloudFilePath == null ? string.Empty : _cloudFilePath;
            }
            set { _cloudFilePath = value; }
        }
    }
}
