using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.ViewModels.Project.ScoringComponentLibrary
{
    public class ScoringComponentLibraryModel
    {
		public class ScoringComponentDetails
		{
			public long ComponentDetailID { get; set; }
			public string ComponentCode { get; set; }
			public string ScoringComponentName { get; set; }
			public decimal? Marks { get; set; }
			
			public long CreatedBy { get; set; }
			public DateTime CreatedDate{ get; set; }

			public int Order {  get; set; }
			public long ModifiedBy { get; set; }
			public DateTime ModifiedDate { get; set; }

			public bool? IsDeleted { get; set; }

			public bool IsQuestionTagged { get; set; }
		}

		
		public class ScoreComponentLibraryName
		{
			public long ScoreComponentId { get; set; }
			public string ComponentName { get; set; }
			public string ComponentCode { get; set; }
			public decimal? Marks { get; set; }
			public int ProjectID { get; set; }
			public bool IsTagged { get; set; }
			public List<ScoringComponentDetails> ScoringComponentDetails { get; set; }
			public bool IsActive { get; set; }
			public bool IsDeleted { get; set; }
			public long  CreatedBy { get; set; }
			public bool IsBandExist { get; set; }
			public bool IsQuestionTagged { get; set; }
		}
	
	}
}
