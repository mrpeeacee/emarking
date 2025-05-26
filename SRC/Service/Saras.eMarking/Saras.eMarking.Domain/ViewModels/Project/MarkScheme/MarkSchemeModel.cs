using Microsoft.AspNetCore.Http;
using Saras.eMarking.Domain.Extensions;
using Saras.eMarking.Domain.ViewModels.File;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.MarkScheme
{
    public class MarkSchemeModel
    {
        public MarkSchemeModel()
        {
        }

        public long? ProjectMarkSchemeId { get; set; }
        [XssTextValidation]

        public string SchemeCode { get; set; }
        [XssTextValidation]
        [Required]
        public string SchemeName { get; set; }
        [Required]
        public decimal? Marks { get; set; }
        
        [MaxLength(2000)]
        public string SchemeDescription { get; set; }
        public long? ProjectQuestionId { get; set; }
        public long CountOfTaggedQuestions { get; set; }
        [Required]
        public List<BandModel> Bands { get; set; }
        public bool Selected { get; set; }
        public int MarkScheme { get; set; }
        public bool IsBandExist { get; set; }
        public bool IsQuestionTagged { get; set; }
        public MarkSchemeType MarkSchemeType { get; set; }      
        public List<FileModel> filedetails { get; set; }
    }


    public class Filedetails
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public IFormFile FileContent { get; set; }
    }

}
