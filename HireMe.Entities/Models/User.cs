namespace HireMe.Entities.Models
{
    using Microsoft.AspNetCore.Identity;

    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PictureName { get; set; }
        public bool isEmployer { get; set; }
        public bool profileConfirmed { get; set; }

        // Favourites
        public string FavouriteJobs { get; set; } = "0";
        public string FavouriteContestants { get; set; } = "0";
        public string FavouriteCompanies { get; set; } = "0";

    }
}
