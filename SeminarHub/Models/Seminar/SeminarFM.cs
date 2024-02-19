namespace SeminarHub.Models.Seminar
{
    using System.ComponentModel.DataAnnotations;

    using static SeminarHub.Common.EntityValidationConstants.SeminarConstants;

    public class SeminarFM
	{
        public SeminarFM()
        {
			this.Categories = new HashSet<CategoryVM>();
            DateAndTime = DateTime.UtcNow;
        }

        [Required]
		[StringLength(TopicMaxLength, MinimumLength = TopicMinLength,
            ErrorMessage = "Topic must be between 3 and 100 characters long.")]
		public string Topic { get; set; } = null!;

		[Required]
		[StringLength(LecturerMaxLength, MinimumLength = LecturerMinLength,
            ErrorMessage = "Lecturer must be between 5 and 60 characters long.")]
		public string Lecturer { get; set; } = null!;

		[Required]
		[StringLength(DetailsMaxLength, MinimumLength = DetailsMinLength,
            ErrorMessage = "Details must be between 10 and 500 characters long.")]
		public string Details { get; set; } = null!;

		[Required]
		[Display(Name = "Date of Seminar")]
		[DataType(DataType.DateTime)]
		public DateTime DateAndTime { get; set; }

		[Required]
		[Range(MinDuration, MaxDuration, ErrorMessage = "Duration must be between 30 and 180")]
		public int Duration { get; set; }

		[Required]
		[Display(Name = "Select Category")]
		public int CategoryId { get; set; }

		public IEnumerable<CategoryVM> Categories { get; set; }
	}

	public class CategoryVM
	{
        public int Id { get; set; }

        public string Name { get; set; } = null!;
    }
}
