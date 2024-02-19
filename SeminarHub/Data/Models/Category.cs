namespace SeminarHub.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static SeminarHub.Common.EntityValidationConstants.CategoryConstants;

    public class Category
	{
        public Category()
        {
            this.Seminars = new HashSet<Seminar>();
		}

		[Key]
        public int Id { get; set; }

		[Required]
		[MaxLength(NameMaxLength)]
		public string Name { get; set; } = null!;

		public ICollection<Seminar> Seminars { get; set; }
	}
}