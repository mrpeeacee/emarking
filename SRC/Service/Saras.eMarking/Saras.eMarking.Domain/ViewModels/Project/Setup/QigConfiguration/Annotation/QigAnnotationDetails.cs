using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration.Annotation
{
    public class QigAnnotationDetails
    {
        public string TemplateName { get; set; }
        public string TemplateCode { get; set; }
        public List<AnnotationGroups> AnnotationGroup { get; set; }
    }

    public class AnnotationTools
    {
        public string AnnotationToolCode { get; set; }
        public string AnnotationToolName { get; set; }
        public string Path { get; set; }
        public string ColorCode { get; set; }
        public bool isChecked { get; set; }
    }

    public class AnnotationGroups
    {
        public string GroupName { get; set; }
        public string GroupCode { get; set; }
        public List<AnnotationTools> AnnotationTools { get; set; }
    }

    public class SaveAnnotationTools
    {
        public string TemplateName { get; set; }
        public string TemplateCode { get; set; }
        public List<AnnotationTools> LTAnnotationToolCodes { get; set; }

    }

    public class ClsAnnotationToolIds
    {
        public int AnnotationToolID { get; set; }

        public bool isChecked { get; set; }

    }

    public class QigtoAnnotationTemplateMapping
    {
        public long QigId { get; set; }
        public long AnnotationTemplateID { get; set; }
    }

    public class AnnotationgroupIds
    {
        public int? AnnotationGroupID { get; set; }



    }
}
