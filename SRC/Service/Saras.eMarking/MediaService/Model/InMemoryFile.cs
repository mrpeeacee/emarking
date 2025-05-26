namespace MediaLibrary.Model
{
    public class InMemoryFile
    {
        public InMemoryFile()
        {
            FileName = string.Empty;
            Content = Array.Empty<byte>();
        }
        public string FileName { get; set; }
        public byte[] Content { get; set; }
    }
}
