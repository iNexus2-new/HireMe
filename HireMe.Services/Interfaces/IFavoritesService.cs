namespace HireMe.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HireMe.Core.Helpers;
    using HireMe.Entities.Enums;
    using HireMe.Entities.Models;

    public interface IFavoritesService
    {
        Task AddToFavourite(User user, PostType postType, string postId);
        Task RemoveFromFavourite(User user, PostType postType, string postId);

        IAsyncEnumerable<TViewModel> GetFavouriteBy<TViewModel>(User user, PostType postType);

    }
}
