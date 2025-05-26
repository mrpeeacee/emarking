using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LicensingAndTransfer.API.BusinessEntities
{
    internal class DOBDateFormat : Newtonsoft.Json.Converters.IsoDateTimeConverter
    {
        public DOBDateFormat()
        {
            base.DateTimeFormat = "yyyy-MM-dd";
        }
    }

    /// <summary>
    /// Candidate test result.
    /// </summary>
    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class Result
    {
        public Candidate Candidate;

        public Admission Admission;

        public List<AccommodationAssistant> AccommodationAssistant;

        public TestCentre TestCentre;

        public Appointment Appointment;

        public TestInformation TestInformation;

        public MCQTestResult MCQTestResult;

        public HPTTestResult HPTTestResult;

        public TrialTest TrialTest;

        public SurveyTest SurveyTest;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class Candidate
    {
        public String CandidateID;

        public String Name;

        public String Surname;

        [DataType(DataType.Date)]
        [Newtonsoft.Json.JsonConverter(typeof(DOBDateFormat))]
        public DateTime DOB;

        /// <summary>
        /// Gender Type 0-Female,1-Male,2-Other
        /// </summary>
        public GenderType Gender;

        public String DrivingLicenseNumber;

        public String PersonalReferenceNumber;

        public String EntitlementConfirmation;

        public String Address;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class Admission
    {
        public DateTime? DateTime;

        public String AdmittedBy;

        public String CandidateSignature;

        public String CandidatePhoto;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class AccommodationAssistant
    {
        public DateTime? DateTime;

        public String AdmittedBy;

        /// <summary>
        /// Accommodations 0-ExtraLength,1-VoiceOverLanguage,2-BSL,3-PauseHPT,4-OLM,5-reader,6-recorder,7-BSLTranslator,8-lipSpeaker,9-listeningAid,10-separateRoom,11-atHomeTesting,12-languageTranslator
        /// </summary>
        public List<Accommodation> AccommodationType;

        public String AssistantSignature;

        public String AssistantName;

        public String Organisation;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class TestCentre
    {
        public String TestCentreCode;

        public Region Region;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class Appointment
    {
        private String ID;
        public DateTime? DateTime;
        public string getAppointmentID()
        {
            return ID;
        }
        public void setAppointmentID(string _ID)
        {
            ID = _ID;
        }
    }

    /// <summary>
    /// Test level information.
    /// </summary>
    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class TestInformation
    {
        public String WorkStation;

        public SystemPerformance WorkStationPerformance;

        public DateTime? StartTime;

        public DateTime? EndTime;

        public String Invigilator;

        /// <summary>
        /// Language of the Test 0-English,1-Welsh
        /// </summary>
        public Language? TextLanguage;

        /// <summary>
        /// Test Accommodations 0-ExtraLength,1-VoiceoverLanguage,2-BSL,3-PauseHPT,4-OLM,5-Reader,6-Recorder,7-BSLTranslator,8-LipSpeaker,9-ListeningAid,10-SeparateRoom,11-AtHomeTesting,12-LanguageTranslator
        /// </summary>
        public List<Accommodation> AccommodationType;

        /// <summary>
        /// Type of the Test 0-Car,2-MotorCycle,3-LGVMC,4-LGVHPT,5-LGVCPC,6-LGVCPCC,7-PCVMC,8-PCVHPT,9-PCVCPC,10-PCVCPCC,11-ADI1,12-ADIHPT,13-ERS,14-AMI1,15-Taxi,16-ExaminerCar
        /// </summary>
        public TestType TestType;

        /// <summary>
        /// Test delivery mode set for the booking 0-CENTRALIZED,3-DISTRIBUTEDONLINE
        /// </summary>
        public DeliveryMode DeliveryMode;

        /// <summary>
        /// Language of the Test 0-English,1-Welsh,2- Arabic, 3-Farsi, 4-Cantonese, 5-Turkish,6-Polish, 7-Portuguese
        /// </summary>
        public Language? VoiceOverLanguage;

        public String CertificationID;

        public ColourFormat? ColourFormat;

        public ResultStatus? OverallStatus;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class SystemPerformance
    {
        public int? CPU;

        public int? RAM;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class TestResult
    {
        private Int64 AssessmentID;

        public Int64 getAssessmentID() { return AssessmentID; }

        public void setAssessmentID(Int64 _assessmentID) { AssessmentID = _assessmentID; }

        public String FormID;

        public Decimal? TestScore;

        public Decimal? TotalScore;

        public Decimal? Percentage;

        /// <summary>
        /// Result staus 0-Fail 1-Pass
        /// </summary>
        public ResultStatus? ResultStatus;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class MCQTestResult : TestResult
    {
        public List<MCQSection> Sections;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class HPTTestResult : TestResult
    {
        public List<HPTSection> Sections;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class Section
    {
        private Int64 SectionID;

        public Int64 getSectionID() { return SectionID; }

        public void setSectionID(Int64 _sectionID) { SectionID = _sectionID; }

        public String Name;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class ItemResponse
    {
        public String Code;

        /// <summary>
        /// Item type 0-MultipleChoiceStatic 1-HazardPerception 2-Trial 3-Survey
        /// </summary>
        public ItemType? Type;

        public Decimal? Version;

        public String Topic;

        public Boolean? Attempted;

        public List<String> UserResponse;

        public Int32? Order;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class ItemResponses
    {
        public String Code;

        /// <summary>
        /// Item type 0-MultipleChoiceStatic 1-HazardPerception 2-Trial 3-Survey
        /// </summary>
        public ItemType? Type;

        public Decimal? Version;

        public String Topic;

        public Boolean? Attempted;

        public List<String> UserResponses;

        public Int32? Order;
    }


    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class MCQSection : Section
    {
        public int? Order;

        public Decimal? MaxScore;

        public Decimal? TotalScore;

        public Decimal? Percentage;

        public DateTime? StartTime;

        public DateTime? EndTime;

        public List<MCQItemResponse> Items;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class HPTSection : Section
    {
        public Int32? Order;

        public Decimal? MaxScore;

        public Decimal? TotalScore;

        public Decimal? Percentage;

        public DateTime? StartTime;

        public DateTime? EndTime;

        public List<HPTItemResponse> Items;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class MCQItemResponse : ItemResponses
    {
        public Int32? TimeSpent;

        public Decimal? Score;

        public List<String> CorrectChoice;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class HPTItemResponse : ItemResponses
    {
        public Decimal? Score;

        public Int32? FrameRate;

        public Int32? InappropriateKeyingsInvoked;

        //TODO:  Will be released in V3
        //TODO:  public List<ScoringWindow> ScoringWindows;

        //TODO:  Will be released in V3
        //TODO:  public List<HPTUserScore> UsersScore;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class ScoringWindow
    {
        public String Code;

        public Int32? StartFrame;

        public Int32? EndFrame;

        public Decimal? Score;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class HPTUserScore
    {
        public String Code;

        public Decimal? Score;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class TrialTest
    {
        private Int64 AssessmentID;

        public Int64 getAssessmentID() { return AssessmentID; }

        public void setAssessmentID(Int64 _assessmentID) { AssessmentID = _assessmentID; }

        public List<TrialSection> Sections;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class TrialSection : Section
    {
        public List<ItemResponse> Items;
        //TODO:  Will be released in V3 - ItemResponses
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class SurveyTest
    {
        private Int64 AssessmentID;

        public Int64 getAssessmentID() { return AssessmentID; }

        public void setAssessmentID(Int64 _assessmentID) { AssessmentID = _assessmentID; }

        public List<SurveySection> Sections;
    }

    [Newtonsoft.Json.JsonObject]
    [Serializable, System.Runtime.Serialization.DataContract]
    public class SurveySection : Section
    {
        public List<ItemResponse> Items;
        //TODO:  Will be released in V3 - ItemResponses
    }

    [Serializable, System.Runtime.Serialization.DataContract]
    public enum GenderType { Female = 0, Male = 1, Other = 2, Unknown = 3 }

    /// <summary>
    /// Test Accommodation
    /// </summary>
    [Serializable, System.Runtime.Serialization.DataContract]
    public enum Accommodation
    {
        ExtraLength = 0, VoiceoverLanguage = 1, BSL = 2, PauseHPT = 3, OLM = 4, Reader = 5, Recorder = 6, BSLTranslator = 7, LipSpeaker = 8,
        ListeningAid = 9, SeparateRoom = 10, AtHomeTesting = 11, LanguageTranslator = 12
    }

    [Serializable, System.Runtime.Serialization.DataContract]
    public enum Region { NA = 0, A = 1, B = 2, C = 3 }

    /// <summary>
    ///Type of test
    ///</summary>
    [Serializable, System.Runtime.Serialization.DataContract]
    public enum TestType
    {
        Car = 0, MotorCycle = 2, LGVMC = 3, LGVHPT = 4, LGVCPC = 5, LGVCPCC = 6, PCVMC = 7, PCVHPT = 8, PCVCPC = 9, PCVCPCC = 10, ADI1 = 11,
        ADIHPT = 12, ERS = 13, AMI1 = 14, Taxi = 15, ExaminerCar = 16
    }

    /// <summary>
    /// Mode of Test delivery types
    /// </summary>
    [Serializable, System.Runtime.Serialization.DataContract]
    public enum DeliveryMode { IHTTC = 0, PermanentTestCentre = 1, OccasionalTestCentre = 2, AtHome = 3 }

    [Serializable, System.Runtime.Serialization.DataContract]
    public enum Language { English = 0, Welsh = 1, Arabic = 2, Farsi = 3, Cantonese = 4, Turkish = 5, Polish = 6, Portuguese = 7 }

    [Serializable, System.Runtime.Serialization.DataContract]
    public enum ColourFormat { NA = 0, FormatOne = 1, FormatTwo = 2, FormatThree = 3 }

    [Serializable, System.Runtime.Serialization.DataContract]
    public enum ResultStatus { Fail = 0, Pass = 1, NotStarted = 2, InComplete = 3 }

    [Serializable, System.Runtime.Serialization.DataContract]
    public enum ItemType { MultipleChoiceStatic = 0, HazardPerception = 1, Trial = 2, Survey = 3 }
}