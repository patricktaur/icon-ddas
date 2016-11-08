using DDAS.Models.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using DDAS.API.Identity;

namespace DDAS.API.Controllers
{
    
    [RoutePrefix("api/Identity")]
    public class TestOneController : ApiController
    {
        [Route("get")]
        [HttpGet]
         public IHttpActionResult Get()
        {
            return Ok("Get");
        }

        [Route("put")]
        [HttpPut]
        public IHttpActionResult Put()
        {
            return Ok("Put");
        }

        [Route("post")]
        [HttpPost]
        public IHttpActionResult Post()
        {
            return Ok("Post");
        }

        [Route("delete")]
        [HttpDelete]
        public IHttpActionResult Delete()
        {
            return Ok("Delete");
        }

    }
}
