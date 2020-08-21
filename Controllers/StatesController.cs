using ApiBack.Models;
//logalty

using es.logalty.bus.updateService;
using LGT_SDK_NETCORE.Client;
using LGT_SDK_NETCORE.Util;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SimuladorBackOffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace ApiBack.Controllers
{

    [EnableCors("corsActivate")]
    [Route("api/[controller]")]
    [ApiController]
    public class StatesController : ControllerBase
    {
        private readonly ApiBckContext _context;

        public StatesController(ApiBckContext context)
        {
            _context = context;
        }
   

        // GET: api/States
        [HttpGet]
        public async Task<ActionResult<IEnumerable<State>>> GetState()
        {
            return await _context.State.ToListAsync();
        }
        

        // GET: api/States/5
        [HttpGet("{id}")]
        public async Task<ActionResult<State>> GetState(int id)
        {
            var state = await _context.State.FindAsync(id);

            if (state == null)
            {
                return NotFound();
            }

            return state;
        }

        // PUT: api/States/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        

        [HttpPut("{id}")]
        public async Task<IActionResult> PutState(int id, State state)
        {
            if (id != state.Result)
            {
                return BadRequest();
            }

            _context.Entry(state).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StateExists(id))
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
       

        [HttpPost]
        public ActionResult Post(
           String IdProcesoEnvioFirm,
            String TelefonoCelula
           , String guid)
        {
            ProcesoEnvioFirma s = new ProcesoEnvioFirma();
            try
            {

                var telcl ="+"+TelefonoCelula;
                ContratacionLogalty example = new ContratacionLogalty();
                object[] guids = new object[3];
                guids[0] = guid; //input
                guids[1] = 1; //input
                updateperson updateperson = new updateperson();
                contact contact = new contact
                {
                    mobile = telcl
                };
                updateperson.contact = contact;
                guids[2] = updateperson; //input

                WSBusUpdate WSUpdate = new WSBusUpdate();
                XmlDocument xmlRequest = WSUpdate.UpdateReceiverRequestDocumentBuilder(guids);
                XMLSign utlSignXml = new XMLSign();
                XmlDocument xmlSigned = utlSignXml.BuildSigned(xmlRequest, example.Certificado);
                LGT_SDK_NETCORE.Entities.DataResponse dataResponse = WSUpdate.PostRequest(xmlSigned, example.Certificado, example.ConnectSetting);
                var respuestaxml = dataResponse.ResponseXml;
                var x = (((dataResponse.Estado)));
                var y = (((guids[0].ToString())));
                var IdProcesoEnvioFirma = new SqlParameter("@IdProcesoEnvioFirma", IdProcesoEnvioFirm);
                var TelefonoCelular = new SqlParameter("@TelefonoCelular ", telcl);
                var State = new SqlParameter("@State", x);
                var GUID = new SqlParameter("@GUID", y);

                var r = _context.DataResponse.FromSqlRaw<DataResponse>("EXEC ActualizarEnvioFirma @IdProcesoEnvioFirma,@TelefonoCelular,@State,@GUID",
                    IdProcesoEnvioFirma,
                    TelefonoCelular,
                    State,
                    GUID
                    );
                _context.SaveChangesAsync();
                _context.SaveChanges();
                return Ok(r);
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }
        }

       

        // DELETE: api/States/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<State>> DeleteState(int id)
        {
            var state = await _context.State.FindAsync(id);
            if (state == null)
            {
                return NotFound();
            }

            _context.State.Remove(state);
            await _context.SaveChangesAsync();

            return state;
        }

        private bool StateExists(int id)
        {
            return _context.State.Any(e => e.Result == id);
        }
    }
}
