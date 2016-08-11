using DDAS.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DDAS.Models.ViewModels.Artist
{
    public class ArtistViewModelRequest
    {
        [Required(ErrorMessage = "Id required.")]
        public long Recid { get; set; }

        [Required(ErrorMessage = "Artist Name is required.")]
        [StringLength(100, ErrorMessage = "Artist Name cannot be longer than 100 characters.")]
        public string ArtistName { get; set; }

        public int GenderPId { get; set; }

        public List<Param> Genders { get; private set; }

        public int? YearOfBirth { get; set; }
        public int? YearOfDeath { get; set; }
    }
}
