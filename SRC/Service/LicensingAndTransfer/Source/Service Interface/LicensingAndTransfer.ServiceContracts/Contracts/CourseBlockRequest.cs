using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class CourseBlockRequest
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.CourseBlock> CourseBlockField;
        public List<DataContracts.CourseBlock> LstCourseBlock
        {
            get
            {
                return this.CourseBlockField;
            }
            set
            {
                this.CourseBlockField = value;
            }
        }
    }
}
