#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class MessagesToSend
    {
        public int Id { get; set; }
        public string Destination { get; set; }
        public string Payload { get; set; }
    }
}
