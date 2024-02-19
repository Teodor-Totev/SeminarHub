namespace SeminarHub.Models.Seminar
{
    public class SeminarDeleteVM
    {
        public int Id { get; set; }

        public string Topic { get; set; } = null!;

        public string DateAndTime { get; set; } = null!;
    }
}
