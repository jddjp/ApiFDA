using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ApiBack.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace PruebaApiWebGera.Controllers
{
    [ApiController]
    [EnableCors("mipoliticadecors")]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ApiBckContext _context;

        public WeatherForecastController(ApiBckContext context)
        {
            _context = context;
        }
        private static readonly string[] Summaries = new[]
     {
            "Daniel", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        public ActionResult Get( )
        {
            ProcesoEnvioFirma s = new ProcesoEnvioFirma();
            try
            {

                var r = _context.DataResponse.FromSqlRaw<DataResponse>("select top 1 * from dataResponse ORDER BY GUID DESC");
                _context.SaveChangesAsync();
                _context.SaveChanges();
                return Ok(r);
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }
        }
    }
}
