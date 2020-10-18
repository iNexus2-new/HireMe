using HireMe.Data;
using Microsoft.Extensions.Configuration;
using System;

namespace HireMe.Controllers.Api
{
    public class ChartsApiController : BaseController
    {
        private readonly BaseDbContext _contextBase;
        private readonly FeaturesDbContext _contextFeatures;
        private readonly IConfiguration _config;

        public ChartsApiController(
            BaseDbContext contextBase,
            FeaturesDbContext contextFeatures,
            IConfiguration config)
        {
            this._config = config;

            this._contextBase = contextBase ?? throw new ArgumentNullException(nameof(contextBase));
            this._contextFeatures = contextFeatures ?? throw new ArgumentNullException(nameof(contextFeatures));
        }


    }
}
