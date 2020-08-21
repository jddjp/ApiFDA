using System;
using System.Collections.Generic;
using System.Linq;
using ApiBack.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Nest;
using SimuladorBackOffice.Models;
using LGT_SDK_NETCORE.Client;
using LGT_SDK_NETCORE.Entities;
using es.logalty.bus.dataService;
using System;
using LGT_SDK_NETCORE.Util;
using System.Xml;

namespace ApiBack.Controllers
{
    [EnableCors("mipoliticadecors")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnDocController : ControllerBase
    {
        private readonly ApiBckContext _context;
      

        public ReturnDocController(ApiBckContext context)
        {
            _context = context;
   
        }


        [HttpPost]
        public ActionResult Post(Object parameters)
        {
            //try
            //{
           
                //Respuesta de incersiondelfeedback
               var respuest= resultado(parameters);
               var z = respuest.ElementAt(0);
                //respuesta Guardarxml           

            var guidprueba = z.DescRespuesta;
            var documentos = resultadoDescarga(guidprueba);
            var res = documentos.ElementAt(0);

            //Pruebasd e que se ejecuten lso dos metodos
            var comprobacion = "GuidEntrante:" + z.DescRespuesta + "GuardadoDocumentos:" + res.DescRespuesta;
            //if (z.CodigoRespuesta != 0)
            //{


            //    respuesta =
            //        "{" +
            //        "\"version\":0," +
            //        "\"severity\":\"\"," +
            //        "\"http-status\":403," +
            //        "\"error-code\":\"\"," +
            //        "\"error-message\":\"\"," +
            //        "\"system-error-code\":\"\"," +
            //        "\"system-error-description\":\"\"" +
            //        "}";


            //}
            //else
            //{

            //    var documentos = resultadoDescarga(guidprueba);
            //    respuesta =
            //        "{" +
            //        "\"version\":0," +
            //        "\"severity\":\"\"," +
            //        "\"http-status\":200," +
            //        "\"error-code\":\"\"," +
            //        "\"error-message\":\"\"," +
            //        "\"system-error-code\":\"\"," +
            //        "\"system-error-description\":\"\"" +
            //        "}";

            //}
            //return StatusCode(200);
            return StatusCode(200,comprobacion);
            //}
            //catch (DbUpdateException)
            //{
            //   var  respuesta =
            //    "{" +
            //    "\"version\":0," +
            //    "\"severity\":\"\"," +
            //    "\"http-status\":403," +
            //    "\"error-code\":\"\"," +
            //    "\"error-message\":\"\"," +
            //    "\"system-error-code\":\"\"," +
            //    "\"system-error-description\":\"\"" +
            //    "}";
            //    return BadRequest(respuesta);
            //}
        }
        public List<ResultadoFeedback> resultado(Object parameters)
        {
            var Respuesta = new SqlParameter("@Respuesta", parameters.ToString());
            var r = _context.ResultadoFeedback.FromSqlRaw<ResultadoFeedback>("EXEC firma_GuardarRespuestaFirmaD @Respuesta",
                  Respuesta
                  ).ToList();
            _context.SaveChanges();
            return r;
        }

        public List<ResultadoFeedback>resultadoDescarga(String Guid)
        {
            ContratacionLogalty example = new ContratacionLogalty();

            guid[] guids = new guid[1];
            guid guid = new guid
            {
                Value = Guid//Dato del guid a descargar
            };
            guids[0] = guid;
            WSBusData WSData = new WSBusData();
            XmlDocument xmlRequest = WSData.StampedBinaryPackRequestDocumentBuilder(guids);

            XMLSign utlSignXml = new XMLSign();
            XmlDocument xmlSigned = utlSignXml.BuildSigned(xmlRequest, example.Certificado);
            LGT_SDK_NETCORE.Entities.DataResponse dataResponse = WSData.PostRequest(xmlSigned, example.Certificado, example.ConnectSetting);

            var state = (("State: " + (dataResponse.Estado)));
            var respuestaguid = (("GUID: " + (guids[0].Value)));

            var Respuesta = new SqlParameter("@XMLRespuesta", dataResponse.ResponseXml);
            var r = _context.ResultadoFeedback.FromSqlRaw<ResultadoFeedback>("EXEC  GuardarDocumentoFirmadoD @XMLRespuesta",
                  Respuesta
                  ).ToList();

            _context.SaveChanges();

            return r;
        }


    }
}
