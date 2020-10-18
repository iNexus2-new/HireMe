namespace HireMe.Services
{
    using HireMe.Data.Repository.Interfaces;
    using HireMe.Entities.Models;
    using HireMe.Mapping.Utility;
    using HireMe.Services.Interfaces;
    using HireMe.ViewModels.Location;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class LocationService : ILocationService
    {
        private readonly IRepository<Location> LocationRepository;

        public LocationService(IRepository<Location> LocationRepository)
        {
            this.LocationRepository = LocationRepository;
        }

        public async ValueTask<int> GetAllCount()
        {
            var ent = await GetAllAsNoTracking().CountAsync().ConfigureAwait(false);
            return ent;
        }
        public IAsyncEnumerable<LocationViewModel> GetAll()
        {
            var ent = GetAllAsNoTrackingMapped().ToAsyncEnumerable();
            return ent;
        }
        public IQueryable<Location> GetAllAsNoTracking()
        {
            return LocationRepository.Set().AsNoTracking();
        }
        public IQueryable<LocationViewModel> GetAllAsNoTrackingMapped()
        {
            return LocationRepository.Set().AsNoTracking().To<LocationViewModel>().OrderByDescending(x => x.Id);
        }
        public async ValueTask<T> GetByIdAsync<T>(int id)
        {
            var ent = await LocationRepository.Set().Where(p => p.Id == id).To<T>().FirstOrDefaultAsync().ConfigureAwait(false);

            return ent;
        }

        public T GetById<T>(int id)
        {
            var ent = LocationRepository.Set().Where(p => p.Id == id).To<T>().FirstOrDefault();

            return ent;
        }

    }
}