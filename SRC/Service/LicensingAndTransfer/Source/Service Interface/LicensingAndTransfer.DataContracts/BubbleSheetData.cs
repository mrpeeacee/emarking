using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "BubbleSheetData")]
    public class BubbleSheetData
    {
        private Int64 SheetNumberField;
        [DataMember(IsRequired = true, Name = "Sheetnumber", Order = 0)]
        public Int64 SheetNumber
        {
            get
            {
                return SheetNumberField;
            }
            set
            {
                SheetNumberField = value;
            }
        }

        private String NameField;
        [DataMember(IsRequired = true, Name = "Name", Order = 1)]
        public String Name
        {
            get
            {
                return NameField;
            }
            set
            {
                NameField = value;
            }
        }

        private DateTime EntryTimeField;
        [DataMember(IsRequired = true, Name = "EntryTime", Order = 2)]
        public DateTime EntryTime
        {
            get
            {
                return EntryTimeField;
            }
            set
            {
                EntryTimeField = value;
            }
        }

        private String IdentificationNumberField;
        [DataMember(IsRequired = true, Name = "IdentificationNumber", Order = 3)]
        public string IdentificationNumber
        {
            get
            {
                return IdentificationNumberField;
            }
            set
            {
                IdentificationNumberField = value;
            }

        }

        private String ClassNameField;
        [DataMember(IsRequired = true, Name = "ClassName", Order = 4)]
        public String ClassName
        {
            get
            {
                return ClassNameField;
            }
            set
            {
                ClassNameField = value;
            }
        }

        private String FacultyNameField;
        [DataMember(IsRequired = true, Name = "FacultyName", Order = 5)]
        public String FaculatyName
        {
            get
            {
                return FacultyNameField;
            }
            set
            {
                FacultyNameField = value;
            }
        }

        private String TestNameField;
        [DataMember(IsRequired = true, Name = "TestName", Order = 6)]
        public String TestName
        {
            get
            {
                return TestNameField;
            }
            set
            {
                TestNameField = value;
            }
        }
        private String TestMessageField;
        [DataMember(IsRequired = true, Name = "TestMessage", Order = 7)]
        public String TestMessage
        {
            get
            {
                return TestMessageField;
            }
            set
            {
                TestMessageField = value;
            }
        }





    }
}
