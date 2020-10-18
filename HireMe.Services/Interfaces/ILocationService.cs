namespace HireMe.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HireMe.Entities.Models;
    using HireMe.ViewModels.Location;

    public interface ILocationService
    {
        ValueTask<int> GetAllCount();
        IAsyncEnumerable<LocationViewModel> GetAll();
        IQueryable<Location> GetAllAsNoTracking();
        IQueryable<LocationViewModel> GetAllAsNoTrackingMapped();
        ValueTask<T> GetByIdAsync<T>(int id);

        T GetById<T>(int id);
    }
}
