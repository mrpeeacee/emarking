using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class MarkSchemeFile
{
    public long FileId { get; set; }

    public long ProjectMarkSchemeId { get; set; }

    public string FilePath { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string FileName { get; set; }

    public string FileExtention { get; set; }

    public string FileType { get; set; }

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ProjectUserRoleinfo ModifiedByNavigation { get; set; }

    public virtual ProjectMarkSchemeTemplate ProjectMarkScheme { get; set; }
}
