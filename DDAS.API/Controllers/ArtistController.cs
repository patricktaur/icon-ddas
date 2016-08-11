using DDAS.Models.ViewModels.Artist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using DDAS.Models;
using DDAS.Models.Entities.Domain;
using System.Threading.Tasks;

namespace DDAS.API.Controllers
{
    [RoutePrefix("api/artist")]
    public class ArtistController : ApiController
    {
        private readonly IUnitOfWork _UOW;
        private readonly IMapper _Mapper;

        public ArtistController(IUnitOfWork uow, IMapper mapper)
        {
            _UOW = uow;
            _Mapper = mapper;
        }

        #region TestEndPoints

        [Route("get")]
        [HttpGet]
        public IHttpActionResult GetGet()
        {
            return Ok("GetGet");
        }

        //[Route("")]
        //[HttpGet]
        //public IHttpActionResult Get()
        //{
        //    return Ok("Get");
        //}

        [Route("")]
        [HttpPost]
        public IHttpActionResult Post()
        {
            return Ok("Post");
        }

        [Route("")]
        [HttpPost]
        public IHttpActionResult Post(long ArtistId)
        {
            return Ok("Post" + ArtistId);
        }

        #endregion


        [Route("")]
        [HttpGet]
        public IHttpActionResult Read(long ArtistID)
        {
            Artist artist = new Artist();

            artist = _UOW.ArtistRepository.FindById(ArtistID);

            var response = _Mapper.Map<Artist, ArtistViewModelRequest>(artist);

            return Ok(response);
        }

        [Route("create")]
        [HttpPost]
        public async Task<IHttpActionResult> Create(ArtistViewModelRequest model)
        {
            if (ModelState.IsValid)
            {
                var artist = _Mapper.Map<ArtistViewModelRequest, Artist>(model);

                if (artist.RecId == 0)
                {
                    //is new record
                    _UOW.ArtistRepository.Add(artist);
                }
                else
                {
                    return BadRequest("Incorrect Artist Recid");
                }

                await _UOW.SaveChangesAsync();

                return Ok(artist);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [Route("update")]
        [HttpPost]
        public async Task<IHttpActionResult> Update(ArtistViewModelRequest model)
        {
            if (ModelState.IsValid)
            {
                var artist = _Mapper.Map<ArtistViewModelRequest, Artist>(model);

                if (artist.RecId != 0)
                {
                    //existing record. update
                    _UOW.ArtistRepository.Update(artist);
                }
                else
                {
                    return BadRequest("Incorrect Artist Recid");
                }


                await _UOW.SaveChangesAsync();

                return Ok("Record updated successfully");
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
