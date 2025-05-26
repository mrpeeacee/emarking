namespace Saras.eMarking.Domain.Entities
{
    public partial class IndexMaster
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string SchemaName { get; set; }
        public string IndexColumns { get; set; }
        public string IncludeColumns { get; set; }
        public string Filter { get; set; }
        public bool DevInstance { get; set; }
        public bool BuildInstance { get; set; }
        public bool CloudInstance { get; set; }
        public bool UatInstance { get; set; }
        public bool ProdInstance { get; set; }
    }
}
