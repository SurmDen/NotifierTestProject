using System.ComponentModel.DataAnnotations;

namespace NotifierTestProject.Models
{
    public class CreateNoticeDTO
    {
        [Required]
        public string NotifierName { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;
    }
}
