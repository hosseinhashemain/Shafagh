using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using Microsoft.SharePoint.Client;

namespace ServiceManager
{
    /// <summary>
    /// Summary description for EdeskServices
    /// </summary>
    [WebService(Namespace = "http://ServiceLayer.Portal/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class EdeskServices : Portal.Services.BaseSoapService
    {
        private readonly ServiceManager.SharepointRepository _sharepointRepository = new SharepointRepository();

        [WebMethod]
        [SoapHeader("Authentication", Direction = SoapHeaderDirection.In, Required = true)]
        public RecallEntityStructure RecallsInformation()
        {
            try
            {
                Authenticate();
                return _sharepointRepository.RecallsInformation();
            }
            catch (Exception ex)
            {
                Portal.Services.Aspects.AspectExtensions.ThrowExceptionsSoapCall(ex);
                return null;
            }
        }

        [WebMethod]
        [SoapHeader("Authentication", Direction = SoapHeaderDirection.In, Required = true)]
        public SuggestionFieldEntityStructure SuggestionFieldInformation()
        {
            try
            {
                Authenticate();
                return _sharepointRepository.SuggestionFieldInformation();
            }
            catch (Exception ex)
            {
                Portal.Services.Aspects.AspectExtensions.ThrowExceptionsSoapCall(ex);
                return null;
            }
        }

        [WebMethod]
        [SoapHeader("Authentication", Direction = SoapHeaderDirection.In, Required = true)]
        public SuggestionInsertionStatus SubmitCitizenSuggestion(string SuggestionTitle, string SuggestionField = "",
            string Recall = "",
            string MainSuggesterPhone = "", string mainSuggesterNationalCode = "", string mainSuggesterCellPhone = "",
            string mainSuggesterUserName = "", string mainSuggesterName = "", string mainSuggesterFamily = "",
            string SuggestionImplementationEffect = "",
            string MoneySavedFirstYearImplementation = "",
            string SuggestionImplementedBefore = "",
            string CurrentSituationProblemsShortComings = "",
            string ProsAndCons = "",
            string FacilityEquipmentManpower = "",
            string FullDescription = "",
            string urgency = "",
            string urgencyReason = "")
        {
            try
            {
                Authenticate();
                return _sharepointRepository.SubmitCitizenSuggestion(SuggestionTitle, SuggestionField, Recall,
                    MainSuggesterPhone, mainSuggesterNationalCode, mainSuggesterCellPhone,
                    mainSuggesterUserName, mainSuggesterName, mainSuggesterFamily,
                    SuggestionImplementationEffect,
                    MoneySavedFirstYearImplementation,
                    SuggestionImplementedBefore,
                    CurrentSituationProblemsShortComings,
                    ProsAndCons,
                    FacilityEquipmentManpower,
                    FullDescription,
                    urgency,
                    urgencyReason);
            }
            catch (Exception ex)
            {
                Portal.Services.Aspects.AspectExtensions.ThrowExceptionsSoapCall(ex);
                SuggestionInsertionStatus returnValue = new SuggestionInsertionStatus();
                returnValue.Title = SuggestionTitle;
                returnValue.InsertionStatus = @"Unsuccessful calling SubmitCitizenSuggestion
                                                " + ex.Message;
                return returnValue;
            }
        }

        [WebMethod]
        [SoapHeader("Authentication", Direction = SoapHeaderDirection.In, Required = true)]
        public SuggestionInsertionStatus SubmitSomeCitizensSuggestion(string SuggestionTitle, string SuggestionField = "", string Recall = "",
            string MainSuggesterPhone = "", string mainSuggesterNationalCode = "", string mainSuggesterCellPhone = "",
            string mainSuggesterUserName = "", string mainSuggesterName = "", string mainSuggesterFamily = "",
            string mainParticipationPercentage = "", string SuggestionDistributorCount = "",
            string SuggestionImplementationEffect = "",
            string MoneySavedFirstYearImplementation = "",
            string SuggestionImplementedBefore = "",
            string CurrentSituationProblemsShortComings = "",
            string ProsAndCons = "",
            string FacilityEquipmentManpower = "",
            string FullDescription = "",
            string urgency = "",
            string urgencyReason = "",
            string SubSuggester_1CellPhone = "", string SubSuggester_2CellPhone = "", string SubSuggester_3CellPhone = "", 
            string SubSuggester_4CellPhone = "", string SubSuggester_1UserName = "", 
            string SubSuggester_2UserName = "", string SubSuggester_3UserName = "", string SubSuggester_4UserName = "",
            string Sub_1ParticipationPercentage = "", string Sub_2ParticipationPercentage = "", string Sub_3ParticipationPercentage = "",
            string Sub_4ParticipationPercentage = "")
        {
            try
            {
                Authenticate();
                return _sharepointRepository.SubmitSomeCitizensSuggestion(SuggestionTitle, SuggestionField, Recall,
                MainSuggesterPhone, mainSuggesterNationalCode, mainSuggesterCellPhone,
                mainSuggesterUserName, mainSuggesterName, mainSuggesterFamily, mainParticipationPercentage, SuggestionDistributorCount,
                SuggestionImplementationEffect,
                MoneySavedFirstYearImplementation,
                SuggestionImplementedBefore,
                CurrentSituationProblemsShortComings,
                ProsAndCons,
                FacilityEquipmentManpower,
                FullDescription,
                urgency,
                urgencyReason,
                SubSuggester_1CellPhone, SubSuggester_2CellPhone, SubSuggester_3CellPhone, SubSuggester_4CellPhone,
                SubSuggester_1UserName, SubSuggester_2UserName, SubSuggester_3UserName, SubSuggester_4UserName,
                Sub_1ParticipationPercentage, Sub_2ParticipationPercentage, Sub_3ParticipationPercentage,
                Sub_4ParticipationPercentage);
            }
            catch (Exception ex)
            {
                Portal.Services.Aspects.AspectExtensions.ThrowExceptionsSoapCall(ex);
                SuggestionInsertionStatus returnValue = new SuggestionInsertionStatus();
                returnValue.Title = SuggestionTitle;
                returnValue.InsertionStatus = @"Unsuccessful calling SubmitSomeCitizensSuggestion
                                                " + ex.Message;
                return returnValue;
            }
        }

        [WebMethod]
        [SoapHeader("Authentication", Direction = SoapHeaderDirection.In, Required = true)]
        public SuggestionInsertionStatus SubmitEmployeeSuggestion(string SuggestionTitle, string SuggestionField = "", string Recall = "",
            string MainSuggesterPhone = "", string mainSuggesterNationalCode = "", string mainSuggesterCellPhone = "",
            string mainSuggesterUserName = "", string mainSuggesterName = "", string mainSuggesterFamily = "",
            string SuggestionImplementationEffect = "",
            string MoneySavedFirstYearImplementation = "",
            string SuggestionImplementedBefore = "",
            string CurrentSituationProblemsShortComings = "",
            string ProsAndCons = "",
            string FacilityEquipmentManpower = "",
            string FullDescription = "",
            string urgency = "",
            string urgencyReason = "")
        {
            try
            {
                Authenticate();
                return 
                    _sharepointRepository.SubmitEmployeeSuggestion(SuggestionTitle, SuggestionField, Recall,
                    MainSuggesterPhone, mainSuggesterNationalCode, mainSuggesterCellPhone, mainSuggesterUserName,
                    mainSuggesterName, mainSuggesterFamily,
                    SuggestionImplementationEffect,
                    MoneySavedFirstYearImplementation,
                    SuggestionImplementedBefore,
                    CurrentSituationProblemsShortComings,
                    ProsAndCons,
                    FacilityEquipmentManpower,
                    FullDescription,
                    urgency,
                    urgencyReason);
            }
            catch (Exception ex)
            {
                Portal.Services.Aspects.AspectExtensions.ThrowExceptionsSoapCall(ex);
                SuggestionInsertionStatus returnValue = new SuggestionInsertionStatus();
                returnValue.Title = SuggestionTitle;
                returnValue.InsertionStatus = @"Unsuccessful calling SubmitEmployeeSuggestion
                                                " + ex.Message;
                return returnValue;
            }
        }

        [WebMethod]
        [SoapHeader("Authentication", Direction = SoapHeaderDirection.In, Required = true)]
        public SuggestionInsertionStatus SubmitSomeEmployeesSuggestion(string SuggestionTitle, string SuggestionField = "", string Recall = "",
            string MainSuggesterPhone = "", string mainSuggesterNationalCode = "", string mainSuggesterCellPhone = "",
            string mainSuggesterUserName = "", string mainSuggesterName = "", string mainSuggesterFamily = "",
            string mainParticipationPercentage = "",
            string SuggestionDistributorCount = "",
            string SuggestionImplementationEffect = "",
            string MoneySavedFirstYearImplementation = "",
            string SuggestionImplementedBefore = "",
            string CurrentSituationProblemsShortComings = "",
            string ProsAndCons = "",
            string FacilityEquipmentManpower = "",
            string FullDescription = "",
            string urgency = "",
            string urgencyReason = "",
            string SubSuggester_1CellPhone = "",
            string SubSuggester_2CellPhone = "",
            string SubSuggester_3CellPhone = "",
            string SubSuggester_4CellPhone = "",
            string SubSuggester_1NationalCode = "",
            string SubSuggester_2NationalCode = "",
            string SubSuggester_3NationalCode = "",
            string SubSuggester_4NationalCode = "",
            string SubSuggester_1UserName = "",
            string SubSuggester_2UserName = "",
            string SubSuggester_3UserName = "",
            string SubSuggester_4UserName = "",
            string Sub_1ParticipationPercentage = "",
            string Sub_2ParticipationPercentage = "",
            string Sub_3ParticipationPercentage = "",
            string Sub_4ParticipationPercentage = "")
        {
            try
            {
                Authenticate();
                return _sharepointRepository.SubmitSomeEmployeesSuggestion(SuggestionTitle, SuggestionField, Recall, MainSuggesterPhone, 
                    mainSuggesterNationalCode, mainSuggesterCellPhone, mainSuggesterUserName, mainSuggesterName,
                    mainSuggesterFamily, mainParticipationPercentage, SuggestionDistributorCount,
                    SuggestionImplementationEffect,
                    MoneySavedFirstYearImplementation,
                    SuggestionImplementedBefore,
                    CurrentSituationProblemsShortComings,
                    ProsAndCons,
                    FacilityEquipmentManpower,
                    FullDescription,
                    urgency,
                    urgencyReason,
                    SubSuggester_1CellPhone, SubSuggester_2CellPhone, SubSuggester_3CellPhone,
                    SubSuggester_4CellPhone, SubSuggester_1NationalCode, SubSuggester_2NationalCode, SubSuggester_3NationalCode,
                    SubSuggester_4NationalCode, SubSuggester_1UserName, SubSuggester_2UserName, SubSuggester_3UserName,
                    SubSuggester_4UserName, Sub_1ParticipationPercentage, Sub_2ParticipationPercentage, Sub_3ParticipationPercentage,
                    Sub_4ParticipationPercentage);
            }
            catch (Exception ex)
            {
                Portal.Services.Aspects.AspectExtensions.ThrowExceptionsSoapCall(ex);
                SuggestionInsertionStatus returnValue = new SuggestionInsertionStatus();
                returnValue.Title = SuggestionTitle;
                returnValue.InsertionStatus = @"Unsuccessful calling SubmitSomeEmployeesSuggestion
                                                " + ex.Message;
                return returnValue;
            }
        }

        [WebMethod]
        [SoapHeader("Authentication", Direction = SoapHeaderDirection.In, Required = true)]
        public SuggestionImpactStructure SuggestionImpacts()
        {
            try
            {
                Authenticate();
                return _sharepointRepository.SuggestionImpacts();
            }
            catch (Exception ex)
            {
                Portal.Services.Aspects.AspectExtensions.ThrowExceptionsSoapCall(ex);
                return null;
            }
        }

        [WebMethod]
        [SoapHeader("Authentication", Direction = SoapHeaderDirection.In, Required = true)]
        public SuggestionPrioritySturcture SuggestionPriorities()
        {
            try
            {
                Authenticate();
                return _sharepointRepository.SuggestionPriorities();
            }
            catch (Exception ex)
            {
                Portal.Services.Aspects.AspectExtensions.ThrowExceptionsSoapCall(ex);
                return null;
            }
        }

        public SearchResultStructure SearchSuggestionTitle(string title)
        {
            try
            {
                Authenticate();
                return _sharepointRepository.SearchSuggestionTitle(title);
            }
            catch (Exception ex)
            {
                Portal.Services.Aspects.AspectExtensions.ThrowExceptionsSoapCall(ex);
                return null;
            }
        }

        public override string Name { get { return "Offers"; } }
    }
}
