using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiBack.Models;
using Microsoft.AspNetCore.Cors;

namespace ApiBack.Controllers
{
    [EnableCors("mipoliticadecors")]
    [Route("api/[controller]")]
    [ApiController]
    public class DataResponsesController : ControllerBase
    {
        private readonly ApiBckContext _context;

        public DataResponsesController(ApiBckContext context)
        {
            _context = context;
        }

        // GET: api/DataResponses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DataResponse>>> GetDataResponse()
        {
            return await _context.DataResponse.ToListAsync();
        }

        // GET: api/DataResponses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DataResponse>> GetDataResponse(string id)
        {
            var dataResponse = await _context.DataResponse.FindAsync(id);

            if (dataResponse == null)
            {
                return NotFound();
            }

            return dataResponse;
        }

        // PUT: api/DataResponses/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDataResponse(string id, DataResponse dataResponse)
        {
            if (id != dataResponse.Guid)
            {
                return BadRequest();
            }

            _context.Entry(dataResponse).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DataResponseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/DataResponses
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [EnableCors("cors")]
        [HttpPost]
        public async Task<ActionResult<DataResponse>> PostDataResponse(DataResponse dataResponse)
        {
            _context.DataResponse.Add(dataResponse);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DataResponseExists(dataResponse.Guid))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetDataResponse", new { id = dataResponse.Guid }, dataResponse);
        }

        // DELETE: api/DataResponses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DataResponse>> DeleteDataResponse(string id)
        {
            var dataResponse = await _context.DataResponse.FindAsync(id);
            if (dataResponse == null)
            {
                return NotFound();
            }

            _context.DataResponse.Remove(dataResponse);
            await _context.SaveChangesAsync();

            return dataResponse;
        }

        private bool DataResponseExists(string id)
        {
            return _context.DataResponse.Any(e => e.Guid == id);
        }
    }
}
