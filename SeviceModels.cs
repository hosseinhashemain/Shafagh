using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ServiceManager
{
    [DataContract]
    public class ProjectEntity
    {
        [DataMember]
        public int ProjectId { get; set; }
        [DataMember]
        public string ProjectTitle { get; set; }
        [DataMember]
        public string ProjectDescription { get; set; }
        [DataMember]
        public string ProjectUrl { get; set; }
        [DataMember]
        public List<ListEntity> ProjectLists = new List<ListEntity>();
    }

    [DataContract]
    public class CredentialEntity
    {
        [DataMember]
        public string Domain { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public List<ProjectEntity> ProjectAccessList = new List<ProjectEntity>();
    }

    [DataContract]
    public class ListEntity
    {
        [DataMember]
        public int ProjectId { get; set; }
        [DataMember]
        public int ListId { get; set; }
        [DataMember]
        public string ListTitle { get; set; }
        [DataMember]
        public string ListDescrption { get; set; }
        [DataMember]
        public List<FieldEntity> ListFields = new List<FieldEntity>();
        [DataMember]
        public string ProjectUrl { get; set; }
    }

    [DataContract]
    public class FieldEntity
    {
        [DataMember]
        public int ListId { get; set; }
        [DataMember]
        public int FieldId { get; set; }
        [DataMember]
        public string FieldTitle { get; set; }
        [DataMember]
        public string FieldDescription { get; set; }
        //[DataMember]
        //public string FieldCSharpType { get; set; }
        //[DataMember]
        //public SharepointDataType FieldSharepointType { get; set; }
        [DataMember]
        public bool FieldIsNullable { get; set; }
    }

    [DataContract]
    public class FieldContentEntry
    {
        [DataMember]
        public int DataId { get; set; }
        [DataMember]
        //public Dictionary<string, string> Contents { get; set; }
        public List<KeyValueEntity> Contents { get; set; }
        [DataMember]
        public List<AttachFileEntry> AttachFiles = new List<AttachFileEntry>();
    }

    [DataContract]
    public class AttachFileEntry
    {
        [DataMember]
        public byte[] FileContent { get; set; }
        [DataMember]
        public string FileName { get; set; }
    }

    [DataContract]
    public class StreamFileEntry
    {
        [DataMember]
        public System.IO.Stream StreamContent { get; set; }
        [DataMember]
        public string FileName { get; set; }
    }

    public enum SharepointDataType
    {
        Boolean = 1, //bool
        DateTime, //datetime 
        Integer, //positive or negative integer
        Number, //decimals, float
        Text //string
    }

    [DataContract]
    public class ResultEntity
    {
        [DataMember]
        public int DataId { get; set; }
        [DataMember]
        public string ResultStatus { get; set; }
    }

    [DataContract]
    public class ExceptionEntity
    {
        [DataMember]
        public int DataId { get; set; }
        [DataMember]
        public string Exception { get; set; }
    }

    [DataContract]
    public class ListFieldsContentEntry
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public List<ContentEntry> Contents { get; set; }
    }

    [DataContract]
    public class ContentEntry
    {
        [DataMember]
        public string ContentKey { get; set; }
        [DataMember]
        public string ContentValue { get; set; }
    }

    [DataContract]
    public class ItemEntity
    {
        [DataMember]
        public int ItemId { get; set; }
        [DataMember]
        public List<KeyValueEntity> ItemContents { get; set; }
    }

    [DataContract]
    public class ItemEntityStructure
    {
        [DataMember]
        public string ListCount { get; set; }
        [DataMember]
        public List<ItemEntity> ListContents { get; set; }
    }

    [DataContract]
    public class KeyValueEntity
    {
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Value { get; set; }
    }
        
    [DataContract]
    public class RecallEntity
    {
        [DataMember]
        public int RecallID { get; set; }//ID
        [DataMember]
        public string RecallTitle { get; set; }//Title
        [DataMember]
        public string RecallNumber { get; set; }//_x0634__x0645__x0627__x0631__x06
        [DataMember]
        public string RecallStatus { get; set; }//_x0648__x0636__x0639__x06cc__x06
        [DataMember]
        public string SecretariatDemander { get; set; }//_x062f__x0628__x06cc__x0631__x06
    }

    [DataContract]
    public class RecallEntityStructure
    {
        [DataMember]
        public string ListCount { get; set; }
        [DataMember]
        public List<RecallEntity> RecallInformationList { get; set; }
    }

    [DataContract]
    public class SuggestionFieldEntity
    {
        [DataMember]
        public int SuggestionFieldID { get; set; }//ID
        [DataMember]
        public string SuggestionFieldTitle { get; set; }//Title
        [DataMember]
        public string SuggestionFieldStatus { get; set; }//_x0648__x0636__x0639__x06cc__x06
        [DataMember]
        public string SuggestionFieldNumber { get; set; }//_x0634__x0645__x0627__x0631__x06
    }

    [DataContract]
    public class SuggestionFieldEntityStructure
    {
        [DataMember]
        public string ListCount { get; set; }
        [DataMember]
        public List<SuggestionFieldEntity> SuggestionFieldList { get; set; }
    }

    [DataContract]
    public class PersonnelPostEntity
    {
        [DataMember]
        public string PersonnelCode { get; set; }
        [DataMember]
        public string NationalCode { get; set; }
        [DataMember]
        public string BusinessLocationChart { get; set; }
    }

    [DataContract]
    public class PersonnelStatusEntity
    {
        [DataMember]
        public string PersonnelCode { get; set; }
        [DataMember]
        public string NationalCode { get; set; }
        [DataMember]
        public string RecruitmentType { get; set; }
    }

    [DataContract]
    public class NationalCodeEntity
    {
        [DataMember]
        public string MainSuggesterNationalCode { get; set; }
    }

    [DataContract]
    public class SomeNationalCodeEntity
    {
        [DataMember]
        public string SubSuggester_1NationalCode { get; set; }
        [DataMember]
        public string SubSuggester_2NationalCode { get; set; }
        [DataMember]
        public string SubSuggester_3NationalCode { get; set; }
        [DataMember]
        public string SubSuggester_4NationalCode { get; set; }
    }

    [DataContract]
    public class CellPhoneEntity
    {
        [DataMember]
        public string MainSuggesterPhone { get; set; }//_x062a__x0644__x0641__x0646__x06
        [DataMember]
        public string MainSuggesterCellPhone { get; set; }//_x062a__x0644__x0641__x0646__x00        
    }

    [DataContract]
    public class SomeCellPhoneEntity
    {
        [DataMember]
        public string SubSuggester_1CellPhone { get; set; }//_x062a__x0644__x0641__x0646__x064
        [DataMember]
        public string SubSuggester_2CellPhone { get; set; }//_x062a__x0644__x0641__x0646__x060
        [DataMember]
        public string SubSuggester_3CellPhone { get; set; }//_x062a__x0644__x0641__x0646__x062
        [DataMember]
        public string SubSuggester_4CellPhone { get; set; }//_x062a__x0644__x0641__x0646__x063
    }

    [DataContract]
    public class UserNameEntity
    {
        [DataMember]
        public string MainSuggesterUserName { get; set; }//_x067e__x06cc__x0634__x0646__x060
    }

    [DataContract]
    public class SomeUserNameEntity
    {
        [DataMember]
        public string SubSuggester_1UserName { get; set; }//_x067e__x06cc__x0634__x0646__x061
        [DataMember]
        public string SubSuggester_2UserName { get; set; }//_x067e__x06cc__x0634__x0646__x062
        [DataMember]
        public string SubSuggester_3UserName { get; set; }//_x067e__x06cc__x0634__x0646__x063
        [DataMember]
        public string SubSuggester_4UserName { get; set; }//_x067e__x06cc__x0634__x0646__x064
    }

    [DataContract]
    public class BusinessLocationEntity
    {
        [DataMember]
        public string MainSuggesterBusinessLocation { get; set; }//_x0645__x062d__x0644__x062e__x06
    }

    [DataContract]
    public class SomeBusinessLocationEntity
    {
        public SomeBusinessLocationEntity() { }
        public SomeBusinessLocationEntity(string sub1, string sub2, string sub3, string sub4)
        {
            SubSuggester_1BusinessLocation = sub1;
            SubSuggester_2BusinessLocation = sub2;
            SubSuggester_3BusinessLocation = sub3;
            SubSuggester_4BusinessLocation = sub4;
        }
        [DataMember]
        public string SubSuggester_1BusinessLocation { get; set; }//_x0645__x062d__x0644__x062e__x060
        [DataMember]
        public string SubSuggester_2BusinessLocation { get; set; }//_x0645__x062d__x0644__x062e__x061
        [DataMember]
        public string SubSuggester_3BusinessLocation { get; set; }//_x0645__x062d__x0644__x062e__x062
        [DataMember]
        public string SubSuggester_4BusinessLocation { get; set; }//_x0645__x062d__x0644__x062e__x063
    }

    [DataContract]
    public class SuggesterNameEntity
    {
        [DataMember]
        public string MainSuggesterName { get; set; }
    }

    [DataContract]
    public class SomeNameEntity : SuggesterNameEntity
    {
        [DataMember]
        public string SubSuggester_1Name { get; set; }
        [DataMember]
        public string SubSuggester_2Name { get; set; }
        [DataMember]
        public string SubSuggester_3Name { get; set; }
        [DataMember]
        public string SubSuggester_4Name { get; set; }
    }

    [DataContract]
    public class SuggesterFamilyEntity
    {
        [DataMember]
        public string MainSuggesterFamily { get; set; }
    }

    [DataContract]
    public class SomeFamilyEntity : SuggesterFamilyEntity
    {
        [DataMember]
        public string SubSuggester_1Family { get; set; }
        [DataMember]
        public string SubSuggester_2Family { get; set; }
        [DataMember]
        public string SubSuggester_3Family { get; set; }
        [DataMember]
        public string SubSuggester_4Family { get; set; }
    }

    [DataContract]
    public class RecruitmentTypeEntity
    {
        [DataMember]
        public string MainSuggesterRecruitmentType { get; set; }//_x0646__x0648__x0639__x0627__x06
    }

    [DataContract]
    public class SomeRecruitmentTypeEntity
    {
        public SomeRecruitmentTypeEntity() { }
        public SomeRecruitmentTypeEntity(string sub1, string sub2, string sub3, string sub4)
        {
            SubSuggester_1RecruitmentType = sub1;
            SubSuggester_2RecruitmentType = sub2;
            SubSuggester_3RecruitmentType = sub3;
            SubSuggester_4RecruitmentType = sub4;
        }
        [DataMember]
        public string SubSuggester_1RecruitmentType { get; set; }//_x0646__x0648__x0639__x0627__x060
        [DataMember]
        public string SubSuggester_2RecruitmentType { get; set; }//_x0646__x0648__x0639__x0627__x061
        [DataMember]
        public string SubSuggester_3RecruitmentType { get; set; }//_x0646__x0648__x0639__x0627__x062
        [DataMember]
        public string SubSuggester_4RecruitmentType { get; set; }//_x0646__x0648__x0639__x0627__x063

    }

    [DataContract]
    public class ParticipationPercentage
    {
        [DataMember]
        public string MainParticipationPercentage { get; set; }//_x062f__x0631__x0635__x062f__x06
    }

    [DataContract]
    public class SomeParticipationPercentage
    {
        public SomeParticipationPercentage() { }
        public SomeParticipationPercentage(string main, string sub1, string sub2, string sub3, string sub4)
        {
            mainParticipationPercentage = main;
            Sub_1ParticipationPercentage = sub1;
            Sub_2ParticipationPercentage = sub2;
            Sub_3ParticipationPercentage = sub3;
            Sub_4ParticipationPercentage = sub4;
        }
        [DataMember]
        public string mainParticipationPercentage { get; set; }//_x062f__x0631__x0635__x062f__x06
        [DataMember]
        public string Sub_1ParticipationPercentage { get; set; }//_x062f__x0631__x0635__x062f__x060
        [DataMember]
        public string Sub_2ParticipationPercentage { get; set; }//_x062f__x0631__x0635__x062f__x062
        [DataMember]
        public string Sub_3ParticipationPercentage { get; set; }//_x062f__x0631__x0635__x062f__x061
        [DataMember]
        public string Sub_4ParticipationPercentage { get; set; }//_x062f__x0631__x0635__x062f__x063
    }

    [DataContract]
    public class CitizenSuggestionEntity : SuggestionInformation
    {
        //[DataMember]
        //public string SuggestionType { get; set; }//_x0646__x0648__x0639__x0020__x06
        //[DataMember]
        //public string SuggesterType { get; set; }//_x0646__x0648__x0639__x067e__x06
    }

    public class SomeCitizenSuggestionEntity : CitizenSuggestionEntity
    {
        [DataMember]
        public string SuggestionDistributorCount { get; set; }//_x062a__x0639__x062f__x0627__x06
        [DataMember]
        public SomeNationalCodeEntity citizensNationalCode { get; set; }
        [DataMember]
        public SomeCellPhoneEntity citizensCellPhone { get; set; }
        [DataMember]
        public SomeUserNameEntity citizensUserName { get; set; }
        [DataMember]
        public SomeNameEntity citizensName { get; set; }
        [DataMember]
        public SomeFamilyEntity citizensFamily { get; set; }
        [DataMember]
        public SomeParticipationPercentage citizensParticipationPercentage { get; set;}
    }

    [DataContract]
    public class EmployeeSuggetionEntity : SuggestionInformation
    {
    }

    [DataContract]
    public class SomeEmployeeSuggetionEntity : EmployeeSuggetionEntity
    {
        [DataMember]
        public string SuggestionDistributorCount { get; set; }//_x062a__x0639__x062f__x0627__x06
        [DataMember]
        public SomeNationalCodeEntity employeesNationalCode { get; set; }
        [DataMember]
        public SomeCellPhoneEntity employeesCellPhone { get; set; }
        [DataMember]
        public SomeUserNameEntity employeesUserName { get; set; }
        [DataMember]
        public SomeNameEntity employeesName { get; set; }
        [DataMember]
        public SomeFamilyEntity employeesFamily { get; set; }
        [DataMember]
        public SomeParticipationPercentage employeesParticipationPercentage { get; set; }
    }

    [DataContract]
    public class SuggestionInformation
    {
        [DataMember]
        public string SuggestionTitle { get; set; }//Title
        [DataMember]
        public string SuggestionField { get; set; }//_x0632__x0645__x06cc__x0646__x060
        [DataMember]
        public string Recall { get; set; }//_x0641__x0631__x0627__x062e__x06
        [DataMember]
        public string MainSuggesterPhone { get; set; }//_x062a__x0644__x0641__x0646__x06
        [DataMember]
        public string mainSuggesterNationalCode { get; set; }
        [DataMember]
        public string mainSuggesterCellPhone { get; set; }
        [DataMember]
        public string mainSuggesterUserName { get; set; }//_x067e__x06cc__x0634__x0646__x060
        [DataMember]
        public string mainSuggesterName { get; set; }
        [DataMember]
        public string mainSuggesterFamily { get; set; }
        [DataMember]
        public string implementationEffect { get; set; }
        [DataMember]
        public string moneySavedFirstYearImplementation { get; set; }
        [DataMember]
        public string implementedBefore { get; set; }
        [DataMember]
        public string currentSituationProblemsShortComings { get; set; }
        [DataMember]
        public string prosAndCons { get; set; }
        [DataMember]
        public string facilityEquipmentManpower { get; set; }
        [DataMember]
        public string fullDescription { get; set; }
        [DataMember]
        public string urgency { get; set; }
        [DataMember]
        public string urgencyReason { get; set; }
    }

    [DataContract]
    public class SuggestionImpactEntity
    {
        [DataMember]
        public int SuggestionImpactID { get; set; }
        [DataMember]
        public string SuggestionImpactTitle { get; set; }
    }

    [DataContract]
    public class SuggestionImpactStructure
    {
        [DataMember]
        public int SuggestionImpactsCount { get; set; }
        [DataMember]
        public List<SuggestionImpactEntity> SuggestionImpacts { get; set; }
    }

    [DataContract]
    public class SuggestionPriorityEntity
    {
        [DataMember]
        public int SuggestionPriorityID { get; set; }
        [DataMember]
        public string SuggestionPriorityTitle { get; set; }
    }

    [DataContract]
    public class SuggestionPrioritySturcture
    {
        [DataMember]
        public int SuggestionPriorityCount { get; set; }
        [DataMember]
        public List<SuggestionPriorityEntity> SuggestionPriorities { get; set; }
    }

    [DataContract]
    public class SearchResultStructure
    {
        [DataMember]
        public int ResultCount { get; set; }
        [DataMember]
        public List<string> SearchResults { get; set; }
    }

    [DataContract]
    public class SuggestionInsertionStatus
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string SuggestionField { get; set; }
        [DataMember]
        public string Recall { get; set; }
        [DataMember]
        public string SuggestionType { get; set; }
        [DataMember]
        public string SuggesterType { get; set; }
        [DataMember]
        public string InsertionStatus { get; set; }
    }
}

