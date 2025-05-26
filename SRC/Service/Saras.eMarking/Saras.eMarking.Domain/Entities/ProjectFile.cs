using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectFile
{
    public long FileId { get; set; }

    public long? EntityId { get; set; }

    /// <summary>
    /// 0--&gt; TEMP Files , 1 --&gt; Markscheme
    /// </summary>
    public short? EntityType { get; set; }

    public string FileName { get; set; }

    public string FileExtention { get; set; }

    public string FileType { get; set; }

    public string FilePath { get; set; }

    public long? ParentFileId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public long? ProjectId { get; set; }

    /// <summary>
    /// 0--&gt; Local Repo  1--&gt; AWS  2 --&gt; Azure 
    /// </summary>
    public byte? Repository { get; set; }

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ProjectUserRoleinfo ModifiedByNavigation { get; set; }

    public virtual ProjectInfo Project { get; set; }
}
