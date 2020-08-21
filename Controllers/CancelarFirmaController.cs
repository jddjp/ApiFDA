using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ApiBack.Models;
using LGT_SDK_NETCORE.Client;
using LGT_SDK_NETCORE.Util;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SimuladorBackOffice.Models;

namespace ApiBack.Controllers
{
    [EnableCors("mipoliticadecors")]
    [Route("api/[controller]")]
    [ApiController]
    public class CancelarFirmaController : ControllerBase
    {
        private readonly ApiBckContext _context;

        public CancelarFirmaController(ApiBckContext context)
        {
            _context = context;
        }
        [EnableCors("cors")]
        [HttpPost]
        public ActionResult Post(
          String IdProcesoEnvioFirm
          , String guidx,
            String Motivo)
        {
            ProcesoEnvioFirma s = new ProcesoEnvioFirma();
            try
            {
                ContratacionLogalty example = new ContratacionLogalty();

                object[] guid = new object[2];
                guid[0] = guidx; //input
                guid[1] = Motivo; //input

                WSBusUpdate WSUpdate = new WSBusUpdate();
                XmlDocument xmlRequest = WSUpdate.CancelRequestDocumentBuilder(guid);

                XMLSign utlSignXml = new XMLSign();
                XmlDocument xmlSigned = utlSignXml.BuildSigned(xmlRequest, example.Certificado);


                LGT_SDK_NETCORE.Entities.DataResponse dataResponse = WSUpdate.PostRequest(xmlSigned, example.Certificado, example.ConnectSetting);

                var x = (((dataResponse.Estado)));
                var y = (( (guid[0].ToString())));

                var IdProcesoEnvioFirma = new SqlParameter("@IdProcesoEnvioFirma", IdProcesoEnvioFirm);
                var State = new SqlParameter("@State", x);
                var GUID = new SqlParameter("@GUID", y);

                var r = _context.DataResponse.FromSqlRaw<DataResponse>("EXEC CancelarEnvioFirma @IdProcesoEnvioFirma,@State,@GUID",
                    IdProcesoEnvioFirma,
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
    }
}