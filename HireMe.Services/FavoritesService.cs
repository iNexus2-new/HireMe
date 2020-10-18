namespace HireMe.Services
{
    using HireMe.Entities.Enums;
    using HireMe.Entities.Models;
    using HireMe.Mapping.Utility;
    using HireMe.Services.Interfaces;
    using Microsoft.AspNetCore.Identity;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class FavoritesService : IFavoritesService
    {
        private readonly UserManager<User> _userManager;
        private readonly IJobsService _jobsService;
        private readonly IContestantsService _contestantService;
        private readonly ICompanyService _companiesService;

        public FavoritesService(
            UserManager<User> userManager,
            IJobsService jobsService,
            IContestantsService contestantService,
            ICompanyService companiesService)
        {
            _userManager = userManager;
            _jobsService = jobsService;
            _contestantService = contestantService;
            _companiesService = companiesService;
        }

        public async Task AddToFavourite(User user, PostType postType, string postId)
        {
            string postIdComplate = ',' + postId;

            switch (postType)
            {
                case PostType.Company:
                    {
                        user.FavouriteCompanies += postIdComplate;
                        await _userManager.UpdateAsync(user);
                    }
                    break;
                case PostType.Contestant:
                    {
                        user.FavouriteContestants += postIdComplate;
                        await _userManager.UpdateAsync(user);
                    }
                    break;
                case PostType.Job:
                    {
                        user.FavouriteJobs += postIdComplate;
                        await _userManager.UpdateAsync(user);
                    }
                    break;
            }

        }

        public async Task RemoveFromFavourite(User user, PostType postType, string postId)
        {
            string postIdComplate = ',' + postId;

            switch (postType)
            {
                case PostType.Company:
                    {
                        if (user.FavouriteCompanies?.IndexOf(postId) > -1)
                        {
                            user.FavouriteCompanies = user.FavouriteCompanies.Remove(user.FavouriteCompanies.Contains(',') ? user.FavouriteCompanies.IndexOf(postIdComplate) : user.FavouriteCompanies.IndexOf(postId));
                            await _userManager.UpdateAsync(user);
                        }
                    }
                    break;
                case PostType.Contestant:
                    {
                        if (user.FavouriteContestants?.IndexOf(postId) > -1)
                        {
                            user.FavouriteContestants = user.FavouriteContestants.Remove(user.FavouriteContestants.Contains(',') ? user.FavouriteContestants.IndexOf(postIdComplate) : user.FavouriteContestants.IndexOf(postId));
                            await _userManager.UpdateAsync(user);
                        }
                    }
                    break;
                case PostType.Job:
                    {
                        if (user.FavouriteJobs?.IndexOf(postId) > -1)
                        {
                            user.FavouriteJobs = user.FavouriteJobs.Remove(user.FavouriteJobs.Contains(',') ? user.FavouriteJobs.IndexOf(postIdComplate) : user.FavouriteJobs.IndexOf(postId));
                            await _userManager.UpdateAsync(user);
                        }
                    }
                    break;
            }

        }

        public IAsyncEnumerable<TViewModel> GetFavouriteBy<TViewModel>(User user, PostType postType)
        {
            switch (postType)
            {
                case PostType.Company:
                    {
                        string[] items = user.FavouriteCompanies?.Split(',');
                        if (items == null)
                            return null;

                        var entities = _companiesService.GetAllAsNoTracking()
                        .Where(x => ((IList)items).Contains(x.Id.ToString()))
                        .To<TViewModel>()
                        .ToAsyncEnumerable();

                        return entities;
                    }

                case PostType.Contestant:
                    {
                        string[] items = user.FavouriteContestants?.Split(',');
                        if (items == null)
                            return null;

                        var entities = _contestantService.GetAllAsNoTracking()
                         .Where(x => ((IList)items).Contains(x.Id.ToString()))
                         .To<TViewModel>()
                         .ToAsyncEnumerable();

                        return entities;
                    }

                case PostType.Job:
                    {
                        string[] items = user.FavouriteJobs?.Split(',');
                        if (items == null)
                            return null;

                        var entities = _jobsService.GetAllAsNoTracking()
                         .Where(x => ((IList)items).Contains(x.Id.ToString()))
                         .To<TViewModel>()
                         .ToAsyncEnumerable();

                        return entities;
                    }
                    
            }
            return null;
        }

    }
}