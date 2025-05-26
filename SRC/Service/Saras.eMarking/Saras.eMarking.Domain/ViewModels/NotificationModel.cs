using Saras.eMarking.Domain.Configuration;

namespace Saras.eMarking.Domain.ViewModels
{
    public class NotificationModel
    {
    }

    public class ServerTimeModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Date { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public int Millisecond { get; set; }
    }
    public class BrandingModel
    {
        public bool DisplayBuildNumber { get; set; }
        public string BuildNumber { get; set; }
        public Branding Branding { get; set; }
    }

}
