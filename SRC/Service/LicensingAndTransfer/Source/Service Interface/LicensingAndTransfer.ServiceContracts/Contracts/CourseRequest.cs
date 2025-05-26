using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
   public class CourseRequest
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.Course> CourseField;
        public List<DataContracts.Course> LstCourse
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
    }
}
