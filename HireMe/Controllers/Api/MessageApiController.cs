using HireMe.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace HireMe.Controllers.Api
{
    public class MessageApiController : BaseController
    {
        private readonly BaseDbContext _contextBase;
        private readonly FeaturesDbContext _contextFeatures;
        private readonly IConfiguration _config;

        public MessageApiController(
            BaseDbContext contextBase,
            FeaturesDbContext contextFeatures,
            IConfiguration config)
        {
            this._config = config;

            this._contextBase = contextBase ?? throw new ArgumentNullException(nameof(contextBase));
            this._contextFeatures = contextFeatures ?? throw new ArgumentNullException(nameof(contextFeatures));
        }

        [Produces("application/json")]
        public JsonResult Search()
        {
            string term = HttpContext.Request.Query["term"].ToString();
            var url = _config.GetSection("MySettings").GetSection("SiteImageUrl").Value;

            var nameData = _contextBase.Users
                 .AsQueryable()
                 .Where(X => X.FirstName.Contains(term))
                 .Select(x => new
                 {
                     id = x.Id,
                     firstname = x.FirstName,
                     lastname = x.LastName,
                     picture = x.PictureName,
                     siteurl = url
                 })
                 .ToArray();

            return Json(nameData);
        }

        [Produces("application/json")]
        public JsonResult SearchMessage()
        {
            string term = HttpContext.Request.Query["term"].ToString();
            var nameData = _contextFeatures.Message
                 .AsQueryable()
                 .Where(X => X.Title.Contains(term))
                 .Select(x => new { id = x.Id, title = x.Title })
                 .ToArray();

            return Json(nameData);
        }

        [Produces("application/json")]
        public JsonResult searchUser()
        {
            string term = HttpContext.Request.Query["term"].ToString();

            var isExists = _contextBase.Users.AsQueryable().Where(x => x.Email == term);

            return Json(isExists);
        }
        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public JsonResult CheckUsername(string term)
        {

            bool isValid;
            var users = _contextBase.Users
                .AsQueryable()
                .Where(x => x.UserName == term)
                .Select(x => x)
                .ToList();

            var count = users.Count();
            if (count > 0)
                isValid = true;
            else isValid = false;

            return Json(isValid);
        }
    }
}
