using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "User")]
    public class User
    {
        public User()
        {
        }
        public User(System.Int64 UserID)
        {
            this.UserIDField = UserID;
        }

        private System.Int64 UserIDField;
        [DataMember(IsRequired = true, Name = "UserID", Order = 0)]
        public System.Int64 UserID
        {
            get
            {
                return this.UserIDField;
            }
            set
            {
                this.UserIDField = value;
            }
        }

        private System.String FirstNameField;
        [DataMember(IsRequired = true, Name = "FirstName", Order = 1)]
        public System.String FirstName
        {
            get
            {
                return this.FirstNameField;
            }
            set
            {
                this.FirstNameField = value;
            }
        }

        private System.String LoginNameField;
        [DataMember(IsRequired = true, Name = "LoginName", Order = 2)]
        public System.String LoginName
        {
            get
            {
                return this.LoginNameField;
            }
            set
            {
                this.LoginNameField = value;
            }
        }

        private System.String PasswordField;
        [DataMember(IsRequired = true, Name = "Password", Order = 3)]
        public System.String Password
        {
            get
            {
                return this.PasswordField;
            }
            set
            {
                this.PasswordField = value;
            }
        }

        private System.String EmailField;
        [DataMember(IsRequired = true, Name = "Email", Order = 4)]
        public System.String Email
        {
            get
            {
                return this.EmailField;
            }
            set
            {
                this.EmailField = value;
            }
        }

        private System.String LastNameField;
        [DataMember(IsRequired = true, Name = "LastName", Order = 5)]
        public System.String LastName
        {
            get
            {
                return this.LastNameField;
            }
            set
            {
                this.LastNameField = value;
            }
        }

        private System.Int64 ClassIDField;
        [DataMember(IsRequired = true, Name = "ClassID", Order = 6)]
        public System.Int64 ClassID
        {
            get
            {
                return this.ClassIDField;
            }
            set
            {
                this.ClassIDField = value;
            }
        }

        private System.Int32 YearIDField;
        [DataMember(IsRequired = true, Name = "YearID", Order = 7)]
        public System.Int32 YearID
        {
            get
            {
                return this.YearIDField;
            }
            set
            {
                this.YearIDField = value;
            }
        }

        private System.Int64 UserTypeField;
        [DataMember(IsRequired = true, Name = "UserType", Order = 8)]
        public System.Int64 UserType
        {
            get
            {
                return this.UserTypeField;
            }
            set
            {
                this.UserTypeField = value;
            }
        }

        private System.Int64 OrganizationIDField;
        [DataMember(IsRequired = true, Name = "OrganizationID", Order = 9)]
        public System.Int64 OrganizationID
        {
            get
            {
                return this.OrganizationIDField;
            }
            set
            {
                this.OrganizationIDField = value;
            }
        }

        private System.Int64 CreatedByField;
        [DataMember(IsRequired = true, Name = "CreatedBy", Order = 10)]
        public System.Int64 CreatedBy
        {
            get
            {
                return this.CreatedByField;
            }
            set
            {
                this.CreatedByField = value;
            }
        }

        private System.DateTime CreatedDateField;
        [DataMember(IsRequired = true, Name = "CreatedDate", Order = 11)]
        public System.DateTime CreatedDate
        {
            get
            {
                return this.CreatedDateField;
            }
            set
            {
                this.CreatedDateField = value;
            }
        }

        private System.Int64 ModifiedByField;
        [DataMember(IsRequired = true, Name = "ModifiedBy", Order = 12)]
        public System.Int64 ModifiedBy
        {
            get
            {
                return this.ModifiedByField;
            }
            set
            {
                this.ModifiedByField = value;
            }
        }

        private System.DateTime ModifiedDateField;
        [DataMember(IsRequired = true, Name = "ModifiedDate", Order = 13)]
        public System.DateTime ModifiedDate
        {
            get
            {
                return this.ModifiedDateField;
            }
            set
            {
                this.ModifiedDateField = value;
            }
        }

        private System.Boolean IsDeletedField;
        [DataMember(IsRequired = true, Name = "IsDeleted", Order = 14)]
        public System.Boolean IsDeleted
        {
            get
            {
                return this.IsDeletedField;
            }
            set
            {
                this.IsDeletedField = value;
            }
        }

        private System.Boolean IsOffLineAuthoringField;
        [DataMember(IsRequired = true, Name = "IsOffLineAuthoring", Order = 15)]
        public System.Boolean IsOffLineAuthoring
        {
            get
            {
                return this.IsOffLineAuthoringField;
            }
            set
            {
                this.IsOffLineAuthoringField = value;
            }
        }

        private System.Boolean IsLoggedINField;
        [DataMember(IsRequired = true, Name = "IsLoggedIN", Order = 16)]
        public System.Boolean IsLoggedIN
        {
            get
            {
                return this.IsLoggedINField;
            }
            set
            {
                this.IsLoggedINField = value;
            }
        }

        private System.Boolean IsActiveField;
        [DataMember(IsRequired = true, Name = "IsActive", Order = 17)]
        public System.Boolean IsActive
        {
            get
            {
                return this.IsActiveField;
            }
            set
            {
                this.IsActiveField = value;
            }
        }

        private System.Boolean IsApproveField;
        [DataMember(IsRequired = true, Name = "IsApprove", Order = 18)]
        public System.Boolean IsApprove
        {
            get
            {
                return this.IsApproveField;
            }
            set
            {
                this.IsApproveField = value;
            }
        }

        private System.Boolean IsAllowEditField;
        [DataMember(IsRequired = true, Name = "IsAllowEdit", Order = 19)]
        public System.Boolean IsAllowEdit
        {
            get
            {
                return this.IsAllowEditField;
            }
            set
            {
                this.IsAllowEditField = value;
            }
        }

        private System.Int64 ManagerIDField;
        [DataMember(IsRequired = true, Name = "ManagerID", Order = 20)]
        public System.Int64 ManagerID
        {
            get
            {
                return this.ManagerIDField;
            }
            set
            {
                this.ManagerIDField = value;
            }
        }

        private System.String UserCodeField;
        [DataMember(IsRequired = true, Name = "UserCode", Order = 21)]
        public System.String UserCode
        {
            get
            {
                return this.UserCodeField;
            }
            set
            {
                this.UserCodeField = value;
            }
        }

        private System.Boolean IsFirstTimeLoggedInField;
        [DataMember(IsRequired = true, Name = "IsFirstTimeLoggedIn", Order = 22)]
        public System.Boolean IsFirstTimeLoggedIn
        {
            get
            {
                return this.IsFirstTimeLoggedInField;
            }
            set
            {
                this.IsFirstTimeLoggedInField = value;
            }
        }

        private System.Int64 OfficeIDField;
        [DataMember(IsRequired = true, Name = "OfficeID", Order = 23)]
        public System.Int64 OfficeID
        {
            get
            {
                return this.OfficeIDField;
            }
            set
            {
                this.OfficeIDField = value;
            }
        }

        private System.Int64 SectionIDField;
        [DataMember(IsRequired = true, Name = "SectionID", Order = 24)]
        public System.Int64 SectionID
        {
            get
            {
                return this.SectionIDField;
            }
            set
            {
                this.SectionIDField = value;
            }
        }

        private System.Boolean IsManagerField;
        [DataMember(IsRequired = true, Name = "IsManager", Order = 25)]
        public System.Boolean IsManager
        {
            get
            {
                return this.IsManagerField;
            }
            set
            {
                this.IsManagerField = value;
            }
        }

        private System.Int32 LoginCountField;
        [DataMember(IsRequired = true, Name = "LoginCount", Order = 26)]
        public System.Int32 LoginCount
        {
            get
            {
                return this.LoginCountField;
            }
            set
            {
                this.LoginCountField = value;
            }
        }

        private System.DateTime PasswordLastModifiedDateField;
        [DataMember(IsRequired = true, Name = "PasswordLastModifiedDate", Order = 27)]
        public System.DateTime PasswordLastModifiedDate
        {
            get
            {
                return this.PasswordLastModifiedDateField;
            }
            set
            {
                this.PasswordLastModifiedDateField = value;
            }
        }

        private System.Decimal AdditionalTimeInPercentField;
        [DataMember(IsRequired = true, Name = "AdditionalTimeInPercent", Order = 28)]
        public System.Decimal AdditionalTimeInPercent
        {
            get
            {
                return this.AdditionalTimeInPercentField;
            }
            set
            {
                this.AdditionalTimeInPercentField = value;
            }
        }

        private System.Boolean IsBlockField;
        [DataMember(IsRequired = true, Name = "IsBlock", Order = 29)]
        public System.Boolean IsBlock
        {
            get
            {
                return this.IsBlockField;
            }
            set
            {
                this.IsBlockField = value;
            }
        }

        private System.String CourseTypeField;
        [DataMember(IsRequired = true, Name = "CourseType", Order = 30)]
        public System.String CourseType
        {
            get
            {
                return this.CourseTypeField;
            }
            set
            {
                this.CourseTypeField = value;
            }
        }

        private System.String CourseField;
        [DataMember(IsRequired = true, Name = "Course", Order = 31)]
        public System.String Course
        {
            get
            {
                return this.CourseField;
            }
            set
            {
                this.CourseField = value;
            }
        }

        private System.String EnrollmentNumberField;
        [DataMember(IsRequired = true, Name = "EnrollmentNumber", Order = 32)]
        public System.String EnrollmentNumber
        {
            get
            {
                return this.EnrollmentNumberField;
            }
            set
            {
                this.EnrollmentNumberField = value;
            }
        }

        private System.DateTime LastLoginDateField;
        [DataMember(IsRequired = true, Name = "LastLoginDate", Order = 33)]
        public System.DateTime LastLoginDate
        {
            get
            {
                return this.LastLoginDateField;
            }
            set
            {
                this.LastLoginDateField = value;
            }
        }

        private System.DateTime LastLogoutDateField;
        [DataMember(IsRequired = true, Name = "LastLogoutDate", Order = 34)]
        public System.DateTime LastLogoutDate
        {
            get
            {
                return this.LastLogoutDateField;
            }
            set
            {
                this.LastLogoutDateField = value;
            }
        }

        private System.Int64 LocationIDField;
        [DataMember(IsRequired = true, Name = "LocationID", Order = 35)]
        public System.Int64 LocationID
        {
            get
            {
                return this.LocationIDField;
            }
            set
            {
                this.LocationIDField = value;
            }
        }

        private System.String AnnotationSettingsField;
        [DataMember(IsRequired = true, Name = "AnnotationSettings", Order = 36)]
        public System.String AnnotationSettings
        {
            get
            {
                return this.AnnotationSettingsField;
            }
            set
            {
                this.AnnotationSettingsField = value;
            }
        }

        private List<UserProfile> ListUserProfileField;
        [DataMember(IsRequired = true, Name = "ListUserProfile", Order = 37)]
        public List<UserProfile> ListUserProfile
        {
            get
            {
                return this.ListUserProfileField;
            }
            set
            {
                this.ListUserProfileField = value;
            }
        }
    }

    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "UserProfile")]
    public class UserProfile
    {
        private System.Int64 UserIDField;
        [DataMember(IsRequired = true, Name = "UserID", Order = 0)]
        public System.Int64 UserID
        {
            get
            {
                return this.UserIDField;
            }
            set
            {
                this.UserIDField = value;
            }
        }

        private System.Boolean GenderField;
        [DataMember(IsRequired = true, Name = "Gender", Order = 1)]
        public System.Boolean Gender
        {
            get
            {
                return this.GenderField;
            }
            set
            {
                this.GenderField = value;
            }
        }

        private System.String TelephoneNUM1Field;
        [DataMember(IsRequired = true, Name = "TelephoneNUM1", Order = 2)]
        public System.String TelephoneNUM1
        {
            get
            {
                return this.TelephoneNUM1Field;
            }
            set
            {
                this.TelephoneNUM1Field = value;
            }
        }

        private System.String TelephoneNUM2Field;
        [DataMember(IsRequired = true, Name = "TelephoneNUM2", Order = 3)]
        public System.String TelephoneNUM2
        {
            get
            {
                return this.TelephoneNUM2Field;
            }
            set
            {
                this.TelephoneNUM2Field = value;
            }
        }

        private System.String MobileNUMField;
        [DataMember(IsRequired = true, Name = "MobileNUM", Order = 4)]
        public System.String MobileNUM
        {
            get
            {
                return this.MobileNUMField;
            }
            set
            {
                this.MobileNUMField = value;
            }
        }

        private System.String PersonalNUMField;
        [DataMember(IsRequired = true, Name = "PersonalNUM", Order = 5)]
        public System.String PersonalNUM
        {
            get
            {
                return this.PersonalNUMField;
            }
            set
            {
                this.PersonalNUMField = value;
            }
        }

        private System.String PostNUMField;
        [DataMember(IsRequired = true, Name = "PostNUM", Order = 6)]
        public System.String PostNUM
        {
            get
            {
                return this.PostNUMField;
            }
            set
            {
                this.PostNUMField = value;
            }
        }

        private System.String RemarksField;
        [DataMember(IsRequired = true, Name = "Remarks", Order = 7)]
        public System.String Remarks
        {
            get
            {
                return this.RemarksField;
            }
            set
            {
                this.RemarksField = value;
            }
        }

        private System.String Photo1Field;
        [DataMember(IsRequired = true, Name = "Photo1", Order = 8)]
        public System.String Photo1
        {
            get
            {
                return this.Photo1Field;
            }
            set
            {
                this.Photo1Field = value;
            }
        }

        private System.String MyVideoField;
        [DataMember(IsRequired = true, Name = "MyVideo", Order = 9)]
        public System.String MyVideo
        {
            get
            {
                return this.MyVideoField;
            }
            set
            {
                this.MyVideoField = value;
            }
        }

        private System.String SecreatQuestionField;
        [DataMember(IsRequired = true, Name = "SecreatQuestion", Order = 10)]
        public System.String SecreatQuestion
        {
            get
            {
                return this.SecreatQuestionField;
            }
            set
            {
                this.SecreatQuestionField = value;
            }
        }

        private System.String HintAnswerField;
        [DataMember(IsRequired = true, Name = "HintAnswer", Order = 11)]
        public System.String HintAnswer
        {
            get
            {
                return this.HintAnswerField;
            }
            set
            {
                this.HintAnswerField = value;
            }
        }

        private System.String TimeZoneField;
        [DataMember(IsRequired = true, Name = "TimeZone", Order = 12)]
        public System.String TimeZone
        {
            get
            {
                return this.TimeZoneField;
            }
            set
            {
                this.TimeZoneField = value;
            }
        }

        private System.String TimeFormatField;
        [DataMember(IsRequired = true, Name = "TimeFormat", Order = 13)]
        public System.String TimeFormat
        {
            get
            {
                return this.TimeFormatField;
            }
            set
            {
                this.TimeFormatField = value;
            }
        }

        private System.String DateFormatField;
        [DataMember(IsRequired = true, Name = "DateFormat", Order = 14)]
        public System.String DateFormat
        {
            get
            {
                return this.DateFormatField;
            }
            set
            {
                this.DateFormatField = value;
            }
        }

        private System.String EducationalLevelField;
        [DataMember(IsRequired = true, Name = "EducationalLevel", Order = 15)]
        public System.String EducationalLevel
        {
            get
            {
                return this.EducationalLevelField;
            }
            set
            {
                this.EducationalLevelField = value;
            }
        }

        private System.Boolean IsDeletedField;
        [DataMember(IsRequired = true, Name = "IsDeleted", Order = 16)]
        public System.Boolean IsDeleted
        {
            get
            {
                return this.IsDeletedField;
            }
            set
            {
                this.IsDeletedField = value;
            }
        }

        private System.Int64 PAssetIDField;
        [DataMember(IsRequired = true, Name = "PAssetID", Order = 17)]
        public System.Int64 PAssetID
        {
            get
            {
                return this.PAssetIDField;
            }
            set
            {
                this.PAssetIDField = value;
            }
        }

        private System.Int64 VAssetIDField;
        [DataMember(IsRequired = true, Name = "VAssetID", Order = 18)]
        public System.Int64 VAssetID
        {
            get
            {
                return this.VAssetIDField;
            }
            set
            {
                this.VAssetIDField = value;
            }
        }

        private System.String LanguageField;
        [DataMember(IsRequired = true, Name = "Language", Order = 19)]
        public System.String Language
        {
            get
            {
                return this.LanguageField;
            }
            set
            {
                this.LanguageField = value;
            }
        }

        private System.DateTime DOBField;
        [DataMember(IsRequired = true, Name = "DOB", Order = 20)]
        public System.DateTime DOB
        {
            get
            {
                return this.DOBField;
            }
            set
            {
                this.DOBField = value;
            }
        }

        private System.Int64 TeacherCodeField;
        [DataMember(IsRequired = true, Name = "TeacherCode", Order = 21)]
        public System.Int64 TeacherCode
        {
            get
            {
                return this.TeacherCodeField;
            }
            set
            {
                this.TeacherCodeField = value;
            }
        }

        private System.String InitialField;
        [DataMember(IsRequired = true, Name = "Initial", Order = 22)]
        public System.String Initial
        {
            get
            {
                return this.InitialField;
            }
            set
            {
                this.InitialField = value;
            }
        }

        private System.String KeywordField;
        [DataMember(IsRequired = true, Name = "Keyword", Order = 23)]
        public System.String Keyword
        {
            get
            {
                return this.KeywordField;
            }
            set
            {
                this.KeywordField = value;
            }
        }

        private System.String AlternateEmailField;
        [DataMember(IsRequired = true, Name = "AlternateEmail", Order = 24)]
        public System.String AlternateEmail
        {
            get
            {
                return this.AlternateEmailField;
            }
            set
            {
                this.AlternateEmailField = value;
            }
        }

        private System.String DesignationField;
        [DataMember(IsRequired = true, Name = "Designation", Order = 25)]
        public System.String Designation
        {
            get
            {
                return this.DesignationField;
            }
            set
            {
                this.DesignationField = value;
            }
        }

        private System.Int64 OfficeIDField;
        [DataMember(IsRequired = true, Name = "OfficeID", Order = 26)]
        public System.Int64 OfficeID
        {
            get
            {
                return this.OfficeIDField;
            }
            set
            {
                this.OfficeIDField = value;
            }
        }

        private System.String AssessmentsEnrolledForField;
        [DataMember(IsRequired = true, Name = "AssessmentsEnrolledFor", Order = 27)]
        public System.String AssessmentsEnrolledFor
        {
            get
            {
                return this.AssessmentsEnrolledForField;
            }
            set
            {
                this.AssessmentsEnrolledForField = value;
            }
        }

        private System.DateTime EnrolledDateField;
        [DataMember(IsRequired = true, Name = "EnrolledDate", Order = 28)]
        public System.DateTime EnrolledDate
        {
            get
            {
                return this.EnrolledDateField;
            }
            set
            {
                this.EnrolledDateField = value;
            }
        }

        private System.String ManagerNameField;
        [DataMember(IsRequired = true, Name = "ManagerName", Order = 29)]
        public System.String ManagerName
        {
            get
            {
                return this.ManagerNameField;
            }
            set
            {
                this.ManagerNameField = value;
            }
        }

        private System.String OfficeNameField;
        [DataMember(IsRequired = true, Name = "OfficeName", Order = 30)]
        public System.String OfficeName
        {
            get
            {
                return this.OfficeNameField;
            }
            set
            {
                this.OfficeNameField = value;
            }
        }

        private System.String RoleNameField;
        [DataMember(IsRequired = true, Name = "RoleName", Order = 31)]
        public System.String RoleName
        {
            get
            {
                return this.RoleNameField;
            }
            set
            {
                this.RoleNameField = value;
            }
        }

        private System.String HallTicketNumberField;
        [DataMember(IsRequired = true, Name = "HallTicketNumber", Order = 32)]
        public System.String HallTicketNumber
        {
            get
            {
                return this.HallTicketNumberField;
            }
            set
            {
                this.HallTicketNumberField = value;
            }
        }

        private System.String TeachingModeField;
        [DataMember(IsRequired = true, Name = "TeachingMode", Order = 33)]
        public System.String TeachingMode
        {
            get
            {
                return this.TeachingModeField;
            }
            set
            {
                this.TeachingModeField = value;
            }
        }

        private System.String StreamNameField;
        [DataMember(IsRequired = true, Name = "StreamName", Order = 34)]
        public System.String StreamName
        {
            get
            {
                return this.StreamNameField;
            }
            set
            {
                this.StreamNameField = value;
            }
        }

        private System.Int64 BranchIDField;
        [DataMember(IsRequired = true, Name = "BranchID", Order = 35)]
        public System.Int64 BranchID
        {
            get
            {
                return this.BranchIDField;
            }
            set
            {
                this.BranchIDField = value;
            }
        }

        private System.Int64 SemesterIDField;
        [DataMember(IsRequired = true, Name = "SemesterID", Order = 36)]
        public System.Int64 SemesterID
        {
            get
            {
                return this.SemesterIDField;
            }
            set
            {
                this.SemesterIDField = value;
            }
        }

        private System.String FingerPrint1Field;
        [DataMember(IsRequired = true, Name = "FingerPrint1", Order = 37)]
        public System.String FingerPrint1
        {
            get
            {
                return this.FingerPrint1Field;
            }
            set
            {
                this.FingerPrint1Field = value;
            }
        }

        private System.String FingerPrint2Field;
        [DataMember(IsRequired = true, Name = "FingerPrint2", Order = 38)]
        public System.String FingerPrint2
        {
            get
            {
                return this.FingerPrint2Field;
            }
            set
            {
                this.FingerPrint2Field = value;
            }
        }

        private System.Int64 VerificationPhotoIDField;
        [DataMember(IsRequired = true, Name = "VerificationPhotoID", Order = 39)]
        public System.Int64 VerificationPhotoID
        {
            get
            {
                return this.VerificationPhotoIDField;
            }
            set
            {
                this.VerificationPhotoIDField = value;
            }
        }
    }

    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "Role")]
    public class Role
    {
        public Role()
        {
        }
        public Role(System.Int64 RoleID)
        {
            this.RoleIDField = RoleID;
        }

        private List<RoleToPrivilages> ListRoleToPrivilagesField;
        [DataMember(IsRequired = true, Name = "ListRoleToPrivilages", Order = 1)]
        public List<RoleToPrivilages> ListRoleToPrivilages
        {
            get
            {
                return this.ListRoleToPrivilagesField;
            }
            set
            {
                this.ListRoleToPrivilagesField = value;
            }
        }

        private System.Int64 RoleIDField;
        [DataMember(IsRequired = true, Name = "RoleID", Order = 2)]
        public System.Int64 RoleID
        {
            get
            {
                return this.RoleIDField;
            }
            set
            {
                this.RoleIDField = value;
            }
        }

        private System.String RoleNameField;
        [DataMember(IsRequired = true, Name = "RoleName", Order = 3)]
        public System.String RoleName
        {
            get
            {
                return this.RoleNameField;
            }
            set
            {
                this.RoleNameField = value;
            }
        }

        private System.String RoleDescriptionField;
        [DataMember(IsRequired = true, Name = "RoleDescription", Order = 4)]
        public System.String RoleDescription
        {
            get
            {
                return this.RoleDescriptionField;
            }
            set
            {
                this.RoleDescriptionField = value;
            }
        }

        private System.Boolean IsDeletedField;
        [DataMember(IsRequired = true, Name = "IsDeleted", Order = 5)]
        public System.Boolean IsDeleted
        {
            get
            {
                return this.IsDeletedField;
            }
            set
            {
                this.IsDeletedField = value;
            }
        }

        private System.Int64 RoleTypeField;
        [DataMember(IsRequired = true, Name = "RoleType", Order = 6)]
        public System.Int64 RoleType
        {
            get
            {
                return this.RoleTypeField;
            }
            set
            {
                this.RoleTypeField = value;
            }
        }

        private System.Int64 CustomerIDField;
        [DataMember(IsRequired = true, Name = "CustomerID", Order = 7)]
        public System.Int64 CustomerID
        {
            get
            {
                return this.CustomerIDField;
            }
            set
            {
                this.CustomerIDField = value;
            }
        }

        private System.Int64 DocumentIdField;
        [DataMember(IsRequired = true, Name = "DocumentId", Order = 8)]
        public System.Int64 DocumentId
        {
            get
            {
                return this.DocumentIdField;
            }
            set
            {
                this.DocumentIdField = value;
            }
        }

        private System.String DocumentUrlField;
        [DataMember(IsRequired = true, Name = "DocumentUrl", Order = 9)]
        public System.String DocumentUrl
        {
            get
            {
                return this.DocumentUrlField;
            }
            set
            {
                this.DocumentUrlField = value;
            }
        }

        private System.String CodeField;
        [DataMember(IsRequired = true, Name = "Code", Order = 10)]
        public System.String Code
        {
            get
            {
                return this.CodeField;
            }
            set
            {
                this.CodeField = value;
            }
        }

        private System.Int64 MetadataIDField;
        [DataMember(IsRequired = true, Name = "MetadataID", Order = 11)]
        public System.Int64 MetadataID
        {
            get
            {
                return this.MetadataIDField;
            }
            set
            {
                this.MetadataIDField = value;
            }
        }
    }

    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "RoleToPrivilages")]
    public class RoleToPrivilages
    {
        public RoleToPrivilages()
        {
        }        
        public RoleToPrivilages(System.Int64 RtoPID)
        {
            this.RtoPIDField = RtoPID;
        }

        private System.Int64 RtoPIDField;
        [DataMember(IsRequired = true, Name = "RtoPID", Order = 0)]
        public System.Int64 RtoPID
        {
            get { return this.RtoPIDField; }
            set { this.RtoPIDField = value; }
        }

        private System.Int64 RoleIdField;
        [DataMember(IsRequired = true, Name = "RoleId", Order = 1)]
        public System.Int64 RoleId
        {
            get { return this.RoleIdField; }
            set { this.RoleIdField = value; }
        }

        private System.Int64 PrivilageIdField;
        [DataMember(IsRequired = true, Name = "PrivilageId", Order = 2)]
        public System.Int64 PrivilageId
        {
            get { return this.PrivilageIdField; }
            set { this.PrivilageIdField = value; }
        }

        private System.Boolean IsDeletedField;
        [DataMember(IsRequired = true, Name = "IsDeleted", Order = 3)]
        public System.Boolean IsDeleted
        {
            get { return this.IsDeletedField; }
            set { this.IsDeletedField = value; }
        }
    }

    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "Organization")]
    public class Organization
    {
        public Organization()
        {
        }
        public Organization(System.Int64 OrganizationID)
        {
            this.OrganizationIDField = OrganizationID;
        }

        private System.Int64 OrganizationIDField;
        [DataMember(IsRequired = true, Name = "OrganizationID", Order = 0)]
        public System.Int64 OrganizationID
        {
            get
            {
                return this.OrganizationIDField;
            }
            set
            {
                this.OrganizationIDField = value;
            }
        }

        private System.String OrganizationNameField;
        [DataMember(IsRequired = true, Name = "OrganizationName", Order = 1)]
        public System.String OrganizationName
        {
            get
            {
                return this.OrganizationNameField;
            }
            set
            {
                this.OrganizationNameField = value;
            }
        }

        private System.String CustomerNumberField;
        [DataMember(IsRequired = true, Name = "CustomerNumber", Order = 2)]
        public System.String CustomerNumber
        {
            get
            {
                return this.CustomerNumberField;
            }
            set
            {
                this.CustomerNumberField = value;
            }
        }

        private System.String PostCodeField;
        [DataMember(IsRequired = true, Name = "PostCode", Order = 3)]
        public System.String PostCode
        {
            get
            {
                return this.PostCodeField;
            }
            set
            {
                this.PostCodeField = value;
            }
        }

        private System.String OrganizationCodeField;
        [DataMember(IsRequired = true, Name = "OrganizationCode", Order = 4)]
        public System.String OrganizationCode
        {
            get
            {
                return this.OrganizationCodeField;
            }
            set
            {
                this.OrganizationCodeField = value;
            }
        }

        private System.String PhoneNoField;
        [DataMember(IsRequired = true, Name = "PhoneNo", Order = 5)]
        public System.String PhoneNo
        {
            get
            {
                return this.PhoneNoField;
            }
            set
            {
                this.PhoneNoField = value;
            }
        }

        private System.String EmailIDField;
        [DataMember(IsRequired = true, Name = "EmailID", Order = 6)]
        public System.String EmailID
        {
            get
            {
                return this.EmailIDField;
            }
            set
            {
                this.EmailIDField = value;
            }
        }

        private System.String WebAddressField;
        [DataMember(IsRequired = true, Name = "WebAddress", Order = 7)]
        public System.String WebAddress
        {
            get
            {
                return this.WebAddressField;
            }
            set
            {
                this.WebAddressField = value;
            }
        }

        private System.String NotesField;
        [DataMember(IsRequired = true, Name = "Notes", Order = 8)]
        public System.String Notes
        {
            get
            {
                return this.NotesField;
            }
            set
            {
                this.NotesField = value;
            }
        }

        private System.Boolean IsDeletedField;
        [DataMember(IsRequired = true, Name = "IsDeleted", Order = 9)]
        public System.Boolean IsDeleted
        {
            get
            {
                return this.IsDeletedField;
            }
            set
            {
                this.IsDeletedField = value;
            }
        }

        private System.String StandardPasswordField;
        [DataMember(IsRequired = true, Name = "StandardPassword", Order = 10)]
        public System.String StandardPassword
        {
            get
            {
                return this.StandardPasswordField;
            }
            set
            {
                this.StandardPasswordField = value;
            }
        }

        private System.Boolean IsBlockedField;
        [DataMember(IsRequired = true, Name = "IsBlocked", Order = 11)]
        public System.Boolean IsBlocked
        {
            get
            {
                return this.IsBlockedField;
            }
            set
            {
                this.IsBlockedField = value;
            }
        }

        private System.Int64 ParentIDField;
        [DataMember(IsRequired = true, Name = "ParentID", Order = 12)]
        public System.Int64 ParentID
        {
            get
            {
                return this.ParentIDField;
            }
            set
            {
                this.ParentIDField = value;
            }
        }

        private System.String LocationCodeField;
        [DataMember(IsRequired = true, Name = "LocationCode", Order = 13)]
        public System.String LocationCode
        {
            get
            {
                return this.LocationCodeField;
            }
            set
            {
                this.LocationCodeField = value;
            }
        }

        private System.Boolean AllowStudentsToCreatePasswordField;
        [DataMember(IsRequired = true, Name = "AllowStudentsToCreatePassword", Order = 14)]
        public System.Boolean AllowStudentsToCreatePassword
        {
            get
            {
                return this.AllowStudentsToCreatePasswordField;
            }
            set
            {
                this.AllowStudentsToCreatePasswordField = value;
            }
        }

        private System.Boolean AllowTeachersToCreatePasswordField;
        [DataMember(IsRequired = true, Name = "AllowTeachersToCreatePassword", Order = 15)]
        public System.Boolean AllowTeachersToCreatePassword
        {
            get
            {
                return this.AllowTeachersToCreatePasswordField;
            }
            set
            {
                this.AllowTeachersToCreatePasswordField = value;
            }
        }

        private System.Int32 OrganizationTypeField;
        [DataMember(IsRequired = true, Name = "OrganizationType", Order = 16)]
        public System.Int32 OrganizationType
        {
            get
            {
                return this.OrganizationTypeField;
            }
            set
            {
                this.OrganizationTypeField = value;
            }
        }

        private System.Int64 ContactPersonField;
        [DataMember(IsRequired = true, Name = "ContactPerson", Order = 17)]
        public System.Int64 ContactPerson
        {
            get
            {
                return this.ContactPersonField;
            }
            set
            {
                this.ContactPersonField = value;
            }
        }

        private System.Int64 LogoField;
        [DataMember(IsRequired = true, Name = "Logo", Order = 18)]
        public System.Int64 Logo
        {
            get
            {
                return this.LogoField;
            }
            set
            {
                this.LogoField = value;
            }
        }

        private System.Int64 CountryIDField;
        [DataMember(IsRequired = true, Name = "CountryID", Order = 19)]
        public System.Int64 CountryID
        {
            get
            {
                return this.CountryIDField;
            }
            set
            {
                this.CountryIDField = value;
            }
        }

        private System.String LocationHeadField;
        [DataMember(IsRequired = true, Name = "LocationHead", Order = 20)]
        public System.String LocationHead
        {
            get
            {
                return this.LocationHeadField;
            }
            set
            {
                this.LocationHeadField = value;
            }
        }

        private System.DateTime CreatedDateField;
        [DataMember(IsRequired = true, Name = "CreatedDate", Order = 21)]
        public System.DateTime CreatedDate
        {
            get
            {
                return this.CreatedDateField;
            }
            set
            {
                this.CreatedDateField = value;
            }
        }

        private System.DateTime ModifiedDateField;
        [DataMember(IsRequired = true, Name = "ModifiedDate", Order = 22)]
        public System.DateTime ModifiedDate
        {
            get
            {
                return this.ModifiedDateField;
            }
            set
            {
                this.ModifiedDateField = value;
            }
        }

        private System.String ThemeField;
        [DataMember(IsRequired = true, Name = "Theme", Order = 23)]
        public System.String Theme
        {
            get
            {
                return this.ThemeField;
            }
            set
            {
                this.ThemeField = value;
            }
        }

        private System.Int64 TestPlayerConcurrentUsersField;
        [DataMember(IsRequired = true, Name = "TestPlayerConcurrentUsers", Order = 24)]
        public System.Int64 TestPlayerConcurrentUsers
        {
            get
            {
                return this.TestPlayerConcurrentUsersField;
            }
            set
            {
                this.TestPlayerConcurrentUsersField = value;
            }
        }

        private System.String LegalNameField;
        [DataMember(IsRequired = true, Name = "LegalName", Order = 25)]
        public System.String LegalName
        {
            get
            {
                return this.LegalNameField;
            }
            set
            {
                this.LegalNameField = value;
            }
        }

        private System.DateTime StartDateField;
        [DataMember(IsRequired = true, Name = "StartDate", Order = 26)]
        public System.DateTime StartDate
        {
            get
            {
                return this.StartDateField;
            }
            set
            {
                this.StartDateField = value;
            }
        }

        private System.DateTime EndDateField;
        [DataMember(IsRequired = true, Name = "EndDate", Order = 27)]
        public System.DateTime EndDate
        {
            get
            {
                return this.EndDateField;
            }
            set
            {
                this.EndDateField = value;
            }
        }

        private System.Int32 OrganizationTypeIDField;
        [DataMember(IsRequired = true, Name = "OrganizationTypeID", Order = 28)]
        public System.Int32 OrganizationTypeID
        {
            get
            {
                return this.OrganizationTypeIDField;
            }
            set
            {
                this.OrganizationTypeIDField = value;
            }
        }

        private System.Int64 MetadataIDField;
        [DataMember(IsRequired = true, Name = "MetadataID", Order = 29)]
        public System.Int64 MetadataID
        {
            get
            {
                return this.MetadataIDField;
            }
            set
            {
                this.MetadataIDField = value;
            }
        }

        private System.Int64 CreatedByField;
        [DataMember(IsRequired = true, Name = "CreatedBy", Order = 30)]
        public System.Int64 CreatedBy
        {
            get
            {
                return this.CreatedByField;
            }
            set
            {
                this.CreatedByField = value;
            }
        }

        private System.Int64 ModifiedByField;
        [DataMember(IsRequired = true, Name = "ModifiedBy", Order = 31)]
        public System.Int64 ModifiedBy
        {
            get
            {
                return this.ModifiedByField;
            }
            set
            {
                this.ModifiedByField = value;
            }
        }

        private System.Int64 LocationIDField;
        [DataMember(IsRequired = true, Name = "LocationID", Order = 32)]
        public System.Int64 LocationID
        {
            get
            {
                return this.LocationIDField;
            }
            set
            {
                this.LocationIDField = value;
            }
        }
    }
}
