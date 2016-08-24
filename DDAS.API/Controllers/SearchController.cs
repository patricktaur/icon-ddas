using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace DDAS.API.Controllers
{
    public class SearchController : ApiController
    {
        private ISearchEngine _SearchEngine;

        public SearchController(ISearchEngine search)
        {
            _SearchEngine = search; 
        }

        [HttpGet]
        public IHttpActionResult Search([FromUri] SearchName searchName)
        {
            string Name = searchName.FirstName + " " + searchName.LastName;
            return Ok(_SearchEngine.SearchByName(Name));
        }
    }
}