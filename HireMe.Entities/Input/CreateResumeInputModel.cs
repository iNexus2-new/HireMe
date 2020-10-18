namespace HireMe.Entities.Input
{
    using Microsoft.AspNetCore.Http;
    using System.ComponentModel.DataAnnotations;

    public class CreateResumeInputModel : BaseModel
    {
        public string Title { get; set; }

        public string FileId { get; set; }

        public IFormFile FormFile { get; set; }

    }
}
