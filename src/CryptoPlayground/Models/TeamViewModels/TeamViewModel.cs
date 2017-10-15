using System.ComponentModel.DataAnnotations;

namespace CryptoPlayground.Models.UserViewModels
{
    public class TeamViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}
