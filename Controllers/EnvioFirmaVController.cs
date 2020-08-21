using System;
using Microsoft.AspNetCore.Mvc;
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
//using Getdocumentoscalidad;
//using GetDocumentosPreproduccion;
using GetdocumentosCredi;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiBack.Controllers
{
    [EnableCors("mipoliticadecors")]
    [Route("api/[controller]")]
    [ApiController]
    public class EnvioFirmaVController : ControllerBase
    {
        private readonly ApiBckContext _context;

        public EnvioFirmaVController(ApiBckContext context)
        {
            _context = context;
        }

        private SimulacionSoapClient ws = new SimulacionSoapClient(endpointConfiguration);
        private static SimulacionSoapClient.EndpointConfiguration endpointConfiguration;
       //Credenciales de Web service de credi
        private AuthHeader au = new AuthHeader
        {
            UsuarioNombre = "promotor",
            UsuarioClave = "B4ckcr3d1"
        };
        //Sirve para agregarle cors al endpoint y accedan desde JS
        [EnableCors("cors")]
        [HttpPost]
        public ActionResult Post(DocsExtra item)
        {
            try
            {
             //Jose Daniel de jesus perez DEV:Aqui mando a traer los documentos en base 64 de los servicios de credi ERP
             var documento1 = ws.GetOutputTablaAmortizacionAsync(au, Convert.ToInt32(item.IdCredi)).Result;
            var documento2 = ws.GetOutputEstipulacionAsync(au, Convert.ToInt32(item.IdCredi)).Result;
            var documento3 = ws.GetOutputContratoCONSUMOV14Async(au, Convert.ToInt32(item.IdCredi)).Result;
            var documento4 = ws.GetOutputPagareCONSUMOV14Async(au, Convert.ToInt32(item.IdCredi)).Result;
            var documento5 = ws.GetOutputReferenciaPagoSUTERMAsync(au, Convert.ToInt32(item.IdCredi)).Result;
            var documento6 = ws.GetOutputSolicitudCONSUMOV14Async(au, Convert.ToInt32(item.IdCredi)).Result;
            var documento7 = ws.GetOutputCaratulaCONSUMOV15Async(au, Convert.ToInt32(item.IdCredi)).Result;
            var documento8 = ws.GetOutputArticulosLegalesCONSUMOV15Async(au, Convert.ToInt32(item.IdCredi)).Result;

            var pdfContent = documento1.GetOutputTablaAmortizacionResult.Archivo.Documento;
            var pdfcontent2 = documento2.GetOutputEstipulacionResult.Archivo.Documento;
            var pdfcontent3 = documento3.GetOutputContratoCONSUMOV14Result.Archivo.Documento;
            var pdfcontent4 = documento4.GetOutputPagareCONSUMOV14Result.Archivo.Documento;
            var pdfcontent5 = documento5.GetOutputReferenciaPagoSUTERMResult.Archivo.Documento;
            var pdfcontent6 = documento6.GetOutputSolicitudCONSUMOV14Result.Archivo.Documento;
            var pdfcontent7 = documento7.GetOutputCaratulaCONSUMOV15Result.Archivo.Documento;
            var pdfcontent8 = documento8.GetOutputArticulosLegalesCONSUMOV15Result.Archivo.Documento;
           //Jose daniel de jesus perez no todos los clientes tienen dos 
           //nombres cuando solo tienen uno valido que mary palalia en el front solo me envie una "A"
            var validarname2 = "";
            if (item.SegundoNombr == "A")
            {
                validarname2 = "";
            }
            else
            {
                validarname2 = item.SegundoNombr;
            }
            //Jose daniel de jesus perez Aqui le agregamos el + a cada envio de firma para que si se utiliza por URL el metodo no le de un espacio
            var telcl = "+" + item.TelefonoCelula;

              var documentoextramal = item.pdfcontent12;
            ContratacionLogalty example = new ContratacionLogalty();
            request_meta request_meta = GenerateRequestMeta(item.NumeroPagare.ToString());
            process_meta process_meta = GenerateProcessMeta(item.PrimerNombr,validarname2, item.ApellidoPatern,item.ApellidoMatern, telcl, item.Emai, item.NumeroPagare, item.metodoaviso);
            binarycontents binarycontents = CreateBinaryContents(pdfContent, pdfcontent2, pdfcontent3, pdfcontent4, pdfcontent5, pdfcontent6, pdfcontent7, pdfcontent8, item.pdfcontent9,item.pdfcontent10,item.pdfcontent11/*,item.pdfcontent12*/);

            WSBusIncoming WSBusIncoming = new WSBusIncoming();
            XmlDocument xmlRequest = WSBusIncoming.RequestDocumentBuilder(request_meta, process_meta, binarycontents);
            XMLSign utlSignXml = new XMLSign();
            XmlDocument xmlSigned = utlSignXml.BuildSigned(xmlRequest, example.Certificado);

            LGT_SDK_NETCORE.Entities.DataResponse dataResponse = WSBusIncoming.PostRequest(xmlSigned, example.Certificado, example.ConnectSetting);
            var resultcode = dataResponse.ResultCode;
            var guidresponse = (((((response)dataResponse.Response).response1.result.guid)));
            //Obtener fecha y hora de la firma
            DateTime now = DateTime.Now;
            string date = now.GetDateTimeFormats('d')[0];
            

                var PrimerNombre = new SqlParameter("@PrimerNombre ", item.PrimerNombr);
                var SegundoNombre = new SqlParameter("@SegundoNombre ", validarname2);
                var ApellidoPaterno = new SqlParameter("@ApellidoPaterno  ", item.ApellidoPatern);
                var ApellidoMaterno = new SqlParameter("@ApellidoMaterno ", item.ApellidoMatern);
                var CURP = new SqlParameter("@CURP", item.CUR);
                var NumControl = new SqlParameter("@NumControl ", item.NumeroPagare);
                var TelefonoCasa = new SqlParameter("@TelefonoCasa ", item.TelefonoCas);
                var TelefonoCelular = new SqlParameter("@TelefonoCelular ", telcl);
                var FechaEnvio = new SqlParameter("@FechaEnvio ", date);
                var EstatusProceso = new SqlParameter("@EstatusProceso ", resultcode);
                var Email = new SqlParameter("@Email", item.Emai);
                var State = new SqlParameter("@State", resultcode);
                var GUID = new SqlParameter("@GUID", guidresponse);
                var montoc = new SqlParameter("@monto", item.monto);
                var tipoc = new SqlParameter("@tipo", item.tipo);
                var r = _context.DataResponse.FromSqlRaw<DataResponse>("EXEC GuardarEnvioFirma @PrimerNombre,@SegundoNombre,@ApellidoPaterno,@ApellidoMaterno,@CURP,@NumControl,@TelefonoCasa,@TelefonoCelular,@FechaEnvio,@EstatusProceso,@Email,@State,@GUID,@monto,@tipo",
                   
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
                _context.SaveChanges();
                return Ok(r);
            }
            catch (DbUpdateException)
            {
                return BadRequest("revisar tabla dataresponse");
            }
        }

        private static request_meta GenerateRequestMeta(String NumeroPagare)
        {
            return new request_meta
            {

                service = "PT0005_ACCEPTANCEXPRESS",//input
                time2close = new time2close
                {
                    value = 3,//Define los dias que vive la transaccion:Jose Daniel de jesus perez
                    unit = "d"//define como sera el tiempo valu dias semanas horas
                },
                time2save = new time2save
                {
                    value = 1825,//input
                    unit = "d"//input
                },
                lopd = 1,//input
                retryprotocol = 3,//input
                synchronous = false,//Define que tipo de peticion es
                tsa = 0,
                externalId = NumeroPagare//ID externo nos permite consultar una transaccion que aun no tenga GUID
                
            };

        }
        private static process_meta GenerateProcessMeta(String name1, String name2, String lastname1, String lastname2, string mobile, String email, String NumeroPagare, String metodoaviso)
        {
            return new process_meta
            {
                
                generator = "generator",
                language = "es-ES",//Se puede cambiar el idioma 
                subject = "Numero de Contrato:" + NumeroPagare.ToString(), //input
                enforce_scroll = true,//Se obliga a darle scroll a los documentos 
                enforce_scrollSpecified = true,
                body = "body Processmeta", //Tipo de llamada a realizar
                url = "LOGALTY_DIRECT_ACCESS_DOC_IN_FRAME", //input
                url_back = "https://wvw.aprecia.com.mx/familia-aprecia", //input
                email = "docs.firma@fomepade.com.mx", //Dato para contactarnos  por cualquier duda o comentario respecto a la firma mesa de analisis lo tendra
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
               ,
                extract_pdf_collections = true//Este parámetro indica si en la documentación del email final deben ser enviadas las carteras de Pdfs o deben ser extraidas en sus multiples Pdfs.
               ,
                additional_email_info_uncompressed = true
               ,
                additional_final_email = new string[] {"docs.firma@fomepade.com.mx" }//Arreglo de String para recibir los documentos Firmados
                ,
                metaProperties = generatepropertys(NumeroPagare.ToString())

            };


        }
        private static property[] generatepropertys(String NumeroPagare)
        {
            return new property[]
            {

                //Jose daniel de jesus perez Dev:Aqui se crea una nueva propiedad para enviar via sms el numero de pagare 
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
                        value="Su numero de contrato es :"+NumeroPagare,

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
                    {    //jose Daniel de jesus perez Dev: Aqui se crea el texto para que el usuario visualise que debe de dar scroll a los docs
                         new text
                        {
                            id="label1",
                            label1="Nota:Todos los documentos deben ser revisados para poder activar el Boton de continuar"

                        },
                         //Jose Daniel de jesus perez Dev:Aqui se crea un check para que el cliente pueda aceptar las condiciones
                        new checkbox
                        {
                            id = "chk1",
                            mandatory = "1",//aqui se define que sea obligatorio darle click
                            value = 0,
                            label1 = "Acepto la documentación y estoy de acuerdo con iniciar el proceso de firma (Recibirá un código sms en su teléfono para finalizar el proceso de firma)",
                            hint = "Comentario Chek1"
                        }

                         }
                }



        };
        }

        private static receivers GenerateReceivers(String name1, String name2, String lastname1, String lastname2, String mobile, String email, String NumeroPagare, String metodoaviso)
        {
            return new receivers
            {
                receiver = new receiver[]
                {
                    GenerateReceiver(name1,name2,lastname1,lastname2,mobile,email, NumeroPagare,metodoaviso)
                }
            };
        }

        private static receiver GenerateReceiver(String name1, String name2, String lastname1, String lastname2, String mobile, String email, String NumeroPagare, String metodoaviso)
        {
            return new receiver
            {
                receiverid = 1,
                personalData = new personalData
                {
                    //Jose daniel de jesus perez Dev:Aqui va la ifnormacion correspondiente para el envio a firma 
                    firstname = name1, //Nombre
                    middlename = name2, //Nombre corto o segundo Nombre
                    lastname1 = lastname1, //Apellido Paterno
                    lastname2 = lastname2 //Apellido Materno
                },

                contact = new contact
                {
                    notice_method = metodoaviso,//Parametro con el cual se enviara el acceso a la plataforma de logalty:Jose Daniel de Jesus perez
                    uuid = "uuid1", //input
                    phone = "", //input
                    mobile = mobile.ToString(),//Numero movil donde de va enviar la 
                                               //notificacion y va hacer el metodo de aviso y acceso:Jose Daniel de Jesus perez
                    fax = "fax", //Este campo no se utiliza 
                    email = email //Email donde  van a recibir los documentos firmados:Jose Daniel de Jesus perez

                },

                legalIdentity = new legalIdentity
                {
                                                
                    type = "Numero de Contrato", //Titulo del acceso a la transaccion de cada cliente:Jose Daniel de Jesus perez
                    jurisdictionCountry = "ESP", //input
                    issuer = "issuer", //input
                    id = NumeroPagare.ToString(), //Identificador unico de cada Transaccion (Pagare):Jose Daniel de Jesus perez
                    certificate = "certificate" //input,

                },

                binarycontentrules = new receiverBinarycontentrules
                {
                    binarycontentrule = new binarycontentrule[]
                    {
                        new binarycontentrule
                        //Jose Daniel de jesus Perez Dev 
                        //Aqui se controla que botones aparecen y cuales no en el portal de la firma Logalty
                        {  hide_cancel_button=true,
                          hide_cancel_buttonSpecified=true,
                          hide_change_mobile_button=true,
                          hide_change_mobile_buttonSpecified=true,
                          hide_global_exit_button=true,
                          hide_global_exit_buttonSpecified=true,

                            binarycontentgroupid = 1,//input
                            binarycontentruleid = 1,//input
                            signMethods = new signMethods
                            {
                                signMethod = new string[]
                                {
                                                 //Jose Daniel de jesus Perez Dev
                                     "SMS_VOICE" //El usuario Recibe SMS con el PIN y Una llamada Voz
                                   
                                }

                            }
                        }
                    }
                }
            };
        }
        private static binarycontents CreateBinaryContents(Byte[] pdfContent, Byte[] pdfcontent2, Byte[] pdfcontent3, Byte[] pdfcontent4, Byte[] pdfcontent5, Byte[] pdfcontent6, Byte[] pdfcontent7, Byte[] pdfcontent8,String pdfcontent9,String pdfcontent10, String pdfcontent11)
        {
            ContratacionLogalty example = new ContratacionLogalty();
            //Jose Daniel de jesus perez DEV:
            //Codigo y libreria para subir un documento de una c
            //arpeta seleccioada en la clase  ContratacionLogalty.Cs
            //FileUtl fileUtils = new FileUtl();
            //byte[] pdfContent = fileUtils.ReadFile((example.PathWork + example.FileToSend));

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
                                 ,
                                new binarycontentgroupmember
                                {
                                    binarycontentid = 9
                                }
                                 ,
                                new binarycontentgroupmember
                                {
                                    binarycontentid = 10
                                }
                                 ,
                                new binarycontentgroupmember
                                {
                                    binarycontentid = 11
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
                          ,
                             new binarycontent
                        {
                            binarycontentid = "9",
                            filename =  "EntregaDeObra.pdf",
                            type = "application/pdf",
                            contenttransferencoding = "",
                            Value = pdfcontent9

                        }
                          ,
                             new binarycontent
                        {
                            binarycontentid = "10",
                            filename =  "ProyeccionDeObra.pdf",
                            type = "application/pdf",
                            contenttransferencoding = "",
                            Value = pdfcontent10

                        }
                                ,
                             new binarycontent
                        {
                            binarycontentid = "11",
                            filename =  "EstudioSocioeconomico.pdf",
                            type = "application/pdf",
                            contenttransferencoding = "",
                            Value = pdfcontent11

                        }
                       
                    }
                }
            };
        }
    }
}

  

