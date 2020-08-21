using System;
using System.Linq;
using System.Xml;
using ApiBack.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SimuladorBackOffice.Models;

using LGT_SDK_NETCORE.Client;
using LGT_SDK_NETCORE.Entities;
using System;
using LGT_SDK_NETCORE.Util;
using System.Xml;
using DataResponse = LGT_SDK_NETCORE.Entities.DataResponse;

namespace ApiBack.Controllers
{
    [EnableCors("mipoliticadecors")]
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultarEstadosController : ControllerBase
    {
        private readonly ApiBckContext _context;

        public ConsultarEstadosController(ApiBckContext context)
        {
            _context = context;
        }
        //[EnableCors("cors")]
        [HttpPost]
        public ActionResult Post(String guid1)
        {
            try
            {
                ContratacionLogalty example = new ContratacionLogalty();


                object[] guids = new object[1];
                guids[0] = "";
                //guids[1] = guid2;

                WSBusData WSData = new WSBusData();
            XmlDocument xmlRequest = WSData.DataStateRequestDocumentBuilder(guids);

            XMLSign utlSignXml = new XMLSign();
            XmlDocument xmlSigned = utlSignXml.BuildSigned(xmlRequest, example.Certificado);
            DataResponse dataResponse = WSData.PostRequest(xmlSigned, example.Certificado, example.ConnectSetting);

               var respuestaxml=(dataResponse.ResponseXml);
              var state=(dataResponse.Estado);
                //var Respuestaxml = new SqlParameter("@Xmlrespuesta",respuestaxml);
                //    var r = _context.ResultadoFeedback.FromSqlRaw<ResultadoFeedback>("EXEC GuardarConsultaED @Xmlrespuesta",
                //      Respuestaxml

                //      );/*.ToList();*/

              
                var r = _context.ResultadoFeedback.FromSqlRaw<ResultadoFeedback>("EXEC firma_BuscarGuidFinalizadosSinFirma");/*.ToList();*/
               
                _context.SaveChanges();
            return Ok(r);
            }
            catch (DbUpdateException)
            {
                return BadRequest("revisar tabla dataresponse");
            }
        }

     
    }
}