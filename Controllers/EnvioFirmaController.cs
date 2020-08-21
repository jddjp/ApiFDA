using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
//lOGALTY
using LGT_SDK_NETCORE.Entities;
using LGT_SDK_NETCORE.Client;
using LGT_SDK_NETCORE.Util;
using es.logalty.bus.incomingService;
using System.Xml;
using SimuladorBackOffice.Models;
using Microsoft.EntityFrameworkCore;
using ApiBack.Models;
using Microsoft.Data.SqlClient;
using DataResponse = ApiBack.Models.DataResponse;
using Microsoft.AspNetCore.Cors;
//LOGALTY
using GetdocumentosCredi;

//using GetDocumentosPreproduccion;
//using Getdocumentoscalidad;

namespace ApiBack.Controllers
{
    [EnableCors("mipoliticadecors")]
    [Route("api/[controller]")]
    [ApiController]
    public class EnvioFirmaController : ControllerBase
    {
        private readonly ApiBckContext _context;

        public EnvioFirmaController(ApiBckContext context)
        {
            _context = context;
        }

        private SimulacionSoapClient ws = new SimulacionSoapClient(endpointConfiguration);
        private static SimulacionSoapClient.EndpointConfiguration endpointConfiguration;
        private AuthHeader au = new AuthHeader
        {
            UsuarioNombre = "promotor",
            UsuarioClave = "B4ckcr3d1"
        };

        // POST: api/EnviarFirma
        [HttpPost]
        public ActionResult Post(
           String PrimerNombr
           , String SegundoNombr
           , String ApellidoPatern 
           , String ApellidoMatern
           , String CUR
           , String TelefonoCas
           , String TelefonoCelula
           , String Emai
           ,String NumeroPagare
           , int IdCredi
            ,String monto
            ,String tipo
            ,String metodoaviso)
        {
            var _context = new ApiBckContext();
            try
            {
                var documento1 =ws.GetOutputTablaAmortizacionAsync(au, IdCredi).Result;
                var documento2 =ws.GetOutputEstipulacionAsync(au, IdCredi).Result;
                var documento3=ws.GetOutputContratoCONSUMOV14Async(au, IdCredi).Result;
                var documento4=ws.GetOutputPagareCONSUMOV14Async(au, IdCredi).Result;
                var documento5=ws.GetOutputReferenciaPagoSUTERMAsync(au, IdCredi).Result;
                var documento6=ws.GetOutputSolicitudCONSUMOV14Async(au, IdCredi).Result;
                var documento7=ws.GetOutputCaratulaCONSUMOV15Async(au, IdCredi).Result;
                var documento8=ws.GetOutputArticulosLegalesCONSUMOV15Async(au, IdCredi).Result;
                
                var pdfContent = documento1.GetOutputTablaAmortizacionResult.Archivo.Documento;
                var pdfcontent2 = documento2.GetOutputEstipulacionResult.Archivo.Documento;
                var pdfcontent3 = documento3.GetOutputContratoCONSUMOV14Result.Archivo.Documento;
                var pdfcontent4 = documento4.GetOutputPagareCONSUMOV14Result.Archivo.Documento;
                var pdfcontent5 = documento5.GetOutputReferenciaPagoSUTERMResult.Archivo.Documento;
                var pdfcontent6 = documento6.GetOutputSolicitudCONSUMOV14Result.Archivo.Documento;
                var pdfcontent7 = documento7.GetOutputCaratulaCONSUMOV15Result.Archivo.Documento;
                var pdfcontent8 = documento8.GetOutputArticulosLegalesCONSUMOV15Result.Archivo.Documento;
                var validarname2 = "";
                if (SegundoNombr == "A")
                {
                    validarname2 = "";
                }
                else
                {
                    validarname2 = SegundoNombr;
                }
                var telcl = "+"+TelefonoCelula;
                ContratacionLogalty example = new ContratacionLogalty();
                request_meta request_meta = GenerateRequestMeta();
                process_meta process_meta = GenerateProcessMeta(PrimerNombr, validarname2, ApellidoPatern, ApellidoMatern,telcl, Emai, NumeroPagare,metodoaviso);
                binarycontents binarycontents = CreateBinaryContents(pdfContent,pdfcontent2, pdfcontent3,pdfcontent4,pdfcontent5, pdfcontent6, pdfcontent7, pdfcontent8);

                WSBusIncoming WSBusIncoming = new WSBusIncoming();
                XmlDocument xmlRequest = WSBusIncoming.RequestDocumentBuilder(request_meta, process_meta, binarycontents);
                XMLSign utlSignXml = new XMLSign();
                XmlDocument xmlSigned = utlSignXml.BuildSigned(xmlRequest, example.Certificado);

                LGT_SDK_NETCORE.Entities.DataResponse dataResponse = WSBusIncoming.PostRequest(xmlSigned, example.Certificado, example.ConnectSetting);
                var x = dataResponse.ResultCode;
                var y = (((((response)dataResponse.Response).response1.result.guid)));
                //Obtener fecha y hora de la firma
                DateTime now = DateTime.Now;
                string date = now.GetDateTimeFormats('d')[0];
              
              
                var PrimerNombre = new SqlParameter("@PrimerNombre ", PrimerNombr);
                var SegundoNombre = new SqlParameter("@SegundoNombre ", validarname2);
                var ApellidoPaterno = new SqlParameter("@ApellidoPaterno  ", ApellidoPatern);
                var ApellidoMaterno = new SqlParameter("@ApellidoMaterno ", ApellidoMatern);
                var CURP = new SqlParameter("@CURP", CUR);
                var NumControl = new SqlParameter("@NumControl ", NumeroPagare);
                var TelefonoCasa = new SqlParameter("@TelefonoCasa ", TelefonoCas);
                var TelefonoCelular = new SqlParameter("@TelefonoCelular ", telcl);
                var FechaEnvio = new SqlParameter("@FechaEnvio ", date);
                var EstatusProceso = new SqlParameter("@EstatusProceso ", x);
                var Email = new SqlParameter("@Email", Emai);
                var State = new SqlParameter("@State", x);
                var GUID = new SqlParameter("@GUID", y);
                var montoc = new SqlParameter("@monto", monto);
                var tipoc = new SqlParameter("@tipo", tipo);
                var r= _context.DataResponse.FromSqlRaw<DataResponse>("EXEC GuardarEnvioFirma @PrimerNombre,@SegundoNombre,@ApellidoPaterno,@ApellidoMaterno,@CURP,@NumControl,@TelefonoCasa,@TelefonoCelular,@FechaEnvio,@EstatusProceso,@Email,@State,@GUID,@monto,@tipo",
                
                    PrimerNombre,
                    SegundoNombre,
                    ApellidoPaterno,
                    ApellidoMaterno,
                    CURP,
                    NumControl,
                    TelefonoCasa,
                    TelefonoCelular,
                    FechaEnvio,
                    EstatusProceso,
                    Email,
                    State,
                    GUID
                   , montoc
                   , tipoc);
                _context.SaveChangesAsync();
                _context.SaveChanges();
                return Ok(r);
                  }
            catch (DbUpdateException)
            {
                return BadRequest();
            }
        }
       


        private static request_meta GenerateRequestMeta()
        {
            return new request_meta
            {
                
                service = "PT0005_ACCEPTANCEXPRESS",//input
                time2close = new time2close
                {
                    value = 3,//input
                    unit = "d"//input
                },
                time2save = new time2save
                {
                    value = 1825,//input
                    unit = "d"//input
                },
                lopd = 1,//input
                retryprotocol = 3,//input
                synchronous = false,//input
                tsa = 0
              
            };

        }
        private static process_meta GenerateProcessMeta(String name1, String name2, String lastname1, String lastname2, string mobile, String email, String NumeroPagare,String metodoaviso)
        {
            return new process_meta
            {
                generator = "generator",//input
                language = "es-ES",
                subject = "Numero de Contrato:" + NumeroPagare.ToString(), 
                enforce_scroll = true,
                enforce_scrollSpecified = true,
                body = "body Processmeta", 
                url = "LOGALTY_DIRECT_ACCESS_DOC_IN_FRAME", 
                url_back ="https://wvw.aprecia.com.mx/familia-aprecia", 
                email = "docs.firma@fomepade.com.mx", 
                userdefined = new userdefined1[]
                {


                    new userdefined1
                    {
                        Value = "value Processmeta", //input
                        name = "LGT_SDK_NETCORE" //input
                    }
                },
                Item = GenerateReceivers(name1, name2, lastname1, lastname2, mobile, email, NumeroPagare, metodoaviso),
                xforms_flow = GenerateXForn(),
                additional_email_info = "DOC_CERT"//Informacion adicional, documentos y certificado
               , extract_pdf_collections = true//Este parámetro indica si en la documentación del email final deben ser enviadas las carteras de Pdfs o deben ser extraidas en sus multiples Pdfs.
               , additional_email_info_uncompressed = true,
               additional_final_email = new string[] {"docs.firma@fomepade.com.mx" }
               , metaProperties = generatepropertys(NumeroPagare.ToString())
               
            };


        }
        private static property[] generatepropertys(String NumeroPagare)
        {
            return new property[]
            {
              new property
              {
                  id="sms1",
                  locations= new string[]{"SMS"},
                  localizedProperties=new localizedProperty[]
                  {
                      new localizedProperty
                      {
                        label="label1",
                        locale="es_ES",
                        value="Su numero de Contrato es :" + NumeroPagare.ToString(),

                      }
                   
                  }
                  
              }

            };
        }
    
       private static form[] GenerateXForn()
        {

            ItemsChoiceType3[] ItemsChoiceType3 = new ItemsChoiceType3[13];
            ItemsChoiceType3[0] = es.logalty.bus.incomingService.ItemsChoiceType3.steps;
            ItemsChoiceType3[1] = es.logalty.bus.incomingService.ItemsChoiceType3.validatedocument;
            ItemsChoiceType3[2] = es.logalty.bus.incomingService.ItemsChoiceType3.validateface;
            ItemsChoiceType3[3] = es.logalty.bus.incomingService.ItemsChoiceType3.validateidentificationnumber;
            ItemsChoiceType3[4] = es.logalty.bus.incomingService.ItemsChoiceType3.validateparams;
            ItemsChoiceType3[5] = es.logalty.bus.incomingService.ItemsChoiceType3.capturemethod;
            ItemsChoiceType3[6] = es.logalty.bus.incomingService.ItemsChoiceType3.noticemethod;
            ItemsChoiceType3[7] = es.logalty.bus.incomingService.ItemsChoiceType3.idcardtime2save;
            ItemsChoiceType3[8] = es.logalty.bus.incomingService.ItemsChoiceType3.idcardtime2close;
            ItemsChoiceType3[9] = es.logalty.bus.incomingService.ItemsChoiceType3.ocrmethod;
            ItemsChoiceType3[10] = es.logalty.bus.incomingService.ItemsChoiceType3.validationparams;
            ItemsChoiceType3[11] = es.logalty.bus.incomingService.ItemsChoiceType3.image;
            ItemsChoiceType3[12] = es.logalty.bus.incomingService.ItemsChoiceType3.documentscategory;

            return new form[]
            {
                new form {
                    id = "xform_1",
                    locale = "es-ES",
                    placement = "BOTTOM",
                    step = "SHOW_DOC",

                    Items = new object[]
                    {
                         new text
                        {
                            id="label1",
                            label1="Nota:Todos los documentos deben ser revisados para poder activar el Boton de continuar"

                        },
                        new checkbox
                        {
                            id = "chk1",
                            mandatory = "1",
                            value = 0,
                            label1 = "Acepto la documentación y estoy de acuerdo con iniciar el proceso de firma (Recibirá un código sms en su teléfono para finalizar el proceso de firma)",
                            hint = "Comentario Chek1"
                        }

                         }
                }



        };
        }

        private static receivers GenerateReceivers(String name1, String name2, String lastname1, String lastname2, String mobile, String email, String NumeroPagare,String metodoaviso)
        {
            return new receivers
            {
                receiver = new receiver[]
                {
                    GenerateReceiver(name1,name2,lastname1,lastname2,mobile,email, NumeroPagare,metodoaviso)
                }
            };
        }

        private static receiver GenerateReceiver(String name1, String name2, String lastname1, String lastname2, String mobile, String email, String NumeroPagare,String metodoaviso)
        {
            return new receiver
            {
                receiverid = 1,//input
                personalData = new personalData
                {
                    firstname = name1, //input
                    middlename = name2, //input
                    lastname1 = lastname1, //input
                    lastname2 = lastname2 //input
                },

                contact = new contact
                {
                    notice_method= metodoaviso,
                    uuid = "uuid1", //input
                    phone = "", //input
                    mobile = mobile.ToString(), //input
                    fax = "fax", //input
                    email = email //input
                    
                },

                legalIdentity = new legalIdentity
                {
                    type = "Numero de Contrato", //input
                    jurisdictionCountry = "ESP", //input
                    issuer = "issuer", //input
                    id = NumeroPagare.ToString(), //input
                    certificate = "certificate" //input,
                
                },

                binarycontentrules = new receiverBinarycontentrules
                {
                    binarycontentrule = new binarycontentrule[]
                    {
                        new binarycontentrule
                        { 
                          hide_cancel_button=true,
                          hide_cancel_buttonSpecified=true,
                          hide_change_mobile_button=true,
                          hide_change_mobile_buttonSpecified=true,
                          hide_global_exit_button=false,
                          hide_global_exit_buttonSpecified=false,

                            binarycontentgroupid = 1,//input
                            binarycontentruleid = 1,//input
                            signMethods = new signMethods
                            {
                                signMethod = new string[]
                                {
                                     "SMS_VOICE" //input
                                 
                                }
                               
                            }
                        }
                    }
                }
            };
        }
        private static binarycontents CreateBinaryContents(Byte[] pdfContent, Byte[] pdfcontent2,Byte[] pdfcontent3, Byte[] pdfcontent4, Byte[] pdfcontent5, Byte[] pdfcontent6, Byte[] pdfcontent7, Byte[] pdfcontent8)
        {
            ContratacionLogalty example = new ContratacionLogalty();
         
             return new binarycontents
            {
                binarycontentgroups = new binarycontentgroups
                {
                    binarycontentgroup = new binarycontentgroup[]
                    {
                        new binarycontentgroup
                        {
                            binarycontentgroupid = 1,
                            binarycontentgroupmember = new binarycontentgroupmember[]
                            {
                              new binarycontentgroupmember
                                {
                                    binarycontentid = 1
                                },
                                new binarycontentgroupmember
                                {
                                    binarycontentid = 2
                                }
                                ,
                                new binarycontentgroupmember
                                {
                                    binarycontentid = 3
                                }
                                 ,
                                new binarycontentgroupmember
                                {
                                    binarycontentid = 4
                                }
                                ,
                                new binarycontentgroupmember
                                {
                                    binarycontentid = 5
                                }
                                 ,
                                new binarycontentgroupmember
                                {
                                    binarycontentid = 6
                                }
                                 ,
                                new binarycontentgroupmember
                                {
                                    binarycontentid = 7
                                }
                                 ,
                                new binarycontentgroupmember
                                {
                                    binarycontentid = 8
                                }
                            }
                        }
                    }
                },
                binarycontentitems = new binarycontentitems
                { 
                    Items = new object[]
                    {
                        new binarycontent
                        {
                            binarycontentid ="1",
                            filename = "TablaAmortizacion.pdf",
                            type = "application/pdf",
                            contenttransferencoding = "base64",
                            Value = Convert.ToBase64String(pdfContent)
                        },
                          new binarycontent
                        {
                            binarycontentid = "2",
                            filename =  "Estipulacion.pdf",
                            type = "application/pdf",
                            contenttransferencoding = "base64",
                            Value = Convert.ToBase64String(pdfcontent2)
                        }
                          ,
                          new binarycontent
                        {
                            binarycontentid = "3",
                            filename =  "Contrato.pdf",
                            type = "application/pdf",
                            contenttransferencoding = "base64",
                            Value = Convert.ToBase64String(pdfcontent3)
                        }
                           ,
                          new binarycontent
                        {
                            binarycontentid = "4",
                            filename =  "Pagare.pdf",
                            type = "application/pdf",
                            contenttransferencoding = "base64",
                            Value = Convert.ToBase64String(pdfcontent4)
                        }
                              ,
                          new binarycontent
                        {
                            binarycontentid = "5",
                            filename =  "ReferenciaPago.pdf",
                            type = "application/pdf",
                            contenttransferencoding = "base64",
                            Value = Convert.ToBase64String(pdfcontent5)
                        }
                               ,
                          new binarycontent
                        {
                            binarycontentid = "6",
                            filename =  "Solicitud.pdf",
                            type = "application/pdf",
                            contenttransferencoding = "base64",
                            Value = Convert.ToBase64String(pdfcontent6)
                        }
                               ,
                          new binarycontent
                        {
                            binarycontentid = "7",
                            filename =  "Caratula.pdf",
                            type = "application/pdf",
                            contenttransferencoding = "base64",
                            Value = Convert.ToBase64String(pdfcontent7)
                        }
                               ,
                          new binarycontent
                        {
                            binarycontentid = "8",
                            filename =  "ArticulosLegales.pdf",
                            type = "application/pdf",
                            contenttransferencoding = "base64",
                            Value = Convert.ToBase64String(pdfcontent8)
                        }
                    }
                }
            };
        }
    } 
}
