namespace MediaLibrary.Model
{
    public sealed class CloudMediaRequestModel
    {
        public string? ContainerName { get; set; }
        public string? StorageAccountName { get; set; }
        public string? StorageAccountKey { get; set; }
        public string? StorageSecret { get; set; }
        public string? RegionEndPoint { get; set; }
        public string? APIInvokeURL { get; set; }
        public string? Signature { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string? FolderPath { get; set; }
        public string? Key { get; set; }
    }
}
