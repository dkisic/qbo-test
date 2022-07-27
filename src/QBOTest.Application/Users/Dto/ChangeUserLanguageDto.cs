using System.ComponentModel.DataAnnotations;

namespace QBOTest.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}