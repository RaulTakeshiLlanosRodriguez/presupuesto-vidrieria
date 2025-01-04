using ApiPresupuesto.Models;
using ApiPresupuesto.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;

namespace ApiPresupuesto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PresupuestoController : ControllerBase
    {
        private readonly PresupuestoService _service;
        private readonly PdfGeneratorService _pdfGeneratorService;
        private readonly CloudinaryService _cloudinaryService;

        public PresupuestoController(PresupuestoService service, PdfGeneratorService pdfGeneratorService, CloudinaryService cloudinaryService)
        {
            _service = service;
            _pdfGeneratorService = pdfGeneratorService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost]
        [Route("newPresupuesto")]
        public async Task<IActionResult> insertPresupuesto([FromBody] Presupuesto presupuesto)
        {
            try
            {
                var id = await _service.insertPresupuesto(presupuesto);
                return Ok(new { id });
            }catch (Exception ex) {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpGet]
        [Route("getPresupuesto")]
        public async Task<ActionResult<Presupuesto>> getPresupuesto(int iDPresupuesto)
        {
            try
            {
                var presupuesto = await _service.getPresupuesto_ById(iDPresupuesto);

                if (presupuesto == null)
                {
                    return NotFound(new { message = "Presupuesto no encontrado." });
                }

                return Ok(presupuesto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        [Route("generarPdf")]
        public async Task<IActionResult> generatePdf([FromBody] Presupuesto presupuesto)
        {
            try
            {
                // Generar contenido HTML dinámico basado en el presupuesto
                int numero = await _service.getNumeroPropuesta();
                string htmlContent = GenerateHtmlFromPresupuesto(presupuesto,numero);

                // Generar PDF
                var pdfData = _pdfGeneratorService.GeneratePdf(htmlContent);

                // Subir a Cloudinary
                var fileName = $"Presupuesto_{presupuesto.tDniCliente}_{DateTime.UtcNow.Ticks}.pdf";
                var pdfUrl = await _cloudinaryService.UploadPdfAsync(pdfData, fileName);

                return Ok(new { pdfUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private string GenerateHtmlFromPresupuesto(Presupuesto presupuesto, int numero)
        {
            string itemsHtml = string.Join("", presupuesto.items.Select(item => $@"
        <tr>
            <td>{item.iDProducto}</td>
            <td class='imgBody'>
                <img src='{item.urlImagenProducto}' alt='{item.tProducto}' />
            </td>
            <td class='descripBody'>{item.descripcion_item}</td>
            <td>{item.cantidad}</td>
            <td>{item.nPrecio:C2}</td>
            <td>{item.total:C2}</td>
        </tr>
    "));

            return $@"
<html>
<head>
    <style>
        * {{
            box-sizing: border-box;
            margin: 0;
            padding: 0;
            font-family: Arial, sans-serif;
        }}
        body {{
            line-height: 1.5;
        }}
        .container {{
            padding: 8mm;
            margin: 20px 0;
        }}
        header {{
            position: relative;
            text-align: left;
            margin: auto 0;
        }}
        .logoContainer {{
            width: 200px;
        }}
        .logoContainer .logoImg {{
            width: 100%;
        }}
        .main {{
            margin: 20px 0
        }}
        .dataCliente {{
            margin: 20px 0;
            font-size: 14px;
            width: 100%;
        }}
        .dataRow {{
            margin-bottom: 10px;
            display: block;
        }}
        .dataRow .field {{
            font-weight: 700;
        }}
        .dataRow > span {{
            display: inline-block;
            width: 48%; 
            text-align: left;
            margin-right: 1%; 
            vertical-align: top;
        }}
        table {{
            min-width: 100%;
            border-collapse: collapse;
        }}
        .container-table{{
            margin-top: 10px;
        }}
        th, tbody td, tfoot .footceld {{
            padding: 12px;
            border: 1px solid rgb(17, 17, 114);
            text-align: center;
        }}
        th {{
            background-color: #f2f2f2;
            font-size: 14px;
        }}
        td:first-child, td:nth-child(2), td:nth-child(4), td:nth-child(5), td:last-child {{
            text-align: center;
        }}
        td:last-child{{
            width: 100%;
        }}
        td:nth-child(3) {{
            text-align: left;
            width: 300px;
        }}
        th.imgHead {{
            min-width: 180px;
        }}
        th.descripHead {{
            min-width: 290px;
        }}
        .imgBody {{
            min-width: 180px;
        }}
        .imgBody img {{
            width: 100%;
        }}
        .descripBody {{
            font-size: 14px;
        }}
        tfoot {{
            font-weight: bold;
            text-align: right;
        }}
        tfoot td.footceld {{
            border-top: 2px solid rgb(17, 17, 114);
        }}
        .total-cell {{
            text-align: right;
            border-right: none;
        }}
        .adicional {{
            margin: 20px 0;
        }}
        .observaciones {{
            color: rgb(17, 17, 114);
            margin: 0;
        }}
        .subtitleContainer {{
            padding-left: 30px;
        }}
        .subtitle {{
            margin: 10px 0;
        }}
        .subtitleContainer li {{
            font-size: 14px;
            text-align: justify;
            margin: 5px 0;
        }}
        .adicional .secondDataClient > p {{
            margin: 0;
            font-size: 14px;
        }}
        .adicional .secondDataClient {{
            padding-left: 30px;
            margin: 6px 0;
        }}
        footer {{
            display: inline-block;
            padding-left: 30px;
        }}
        footer > div {{
             margin: 0 30px;
         }}
        .metodoPagoContainer {{
            margin: 10px 0;
        }}

        .metodoPagoContainer .pagoImg {{
            width: 80px;
            margin-right: 10px;
            display: inline-block;
        }}
        .metodoPagoContainer div img {{
            width: 100%;
        }}
        .infoPago {{

            display:inline-block;
        }}
        .infoPago > p {{
            font-size: 14px;
            margin: 2px 0;
        }}
        </style>
    </head>
    <body>
        <div class='container'>
            <header>
                <div class='logoContainer'>
                    <img src='https://i.postimg.cc/3xDyPn5J/logo-vidrieria.png' alt='Logo' class='logoImg'>
                </div>
            </header>
            
            <main class='main'>
                <section class='dataCliente'>

                <div class='dataRow'>
                    <span>Presupuesto número: 
                        <span class='field'>{numero}</span> 
                    </span>
                    <span>Nombre del cliente: 
                        <span class='field'>{presupuesto.tNombreCliente}</span>
                    </span>
                </div>
                <div class='dataRow'>
                    <span>R.U.C: 
                        <span class='field'>{presupuesto.rucCliente}</span>
                    </span>
                    <span>DNI: 
                        <span class='field'>{presupuesto.tDniCliente}</span>
                    </span>
                </div>
                <div class='dataRow'>
                    <span>Fecha: 
                        <span class='field'>{presupuesto.entrega}</span>
                    </span>
                    <span>Obra: 
                        <span class='field'>{presupuesto.obra}</span>
                    </span>
                </div>
                </section>
                
                <section class='container-table'>
                    <table>
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th class='imgHead'>IMAGEN</th>
                                <th class='descripHead'>DESCRIPCION</th>
                                <th>UND</th>
                                <th>PRC/UND</th>
                                <th>TOTAL</th>
                            </tr>
                        </thead>
                        <tbody>
                            {itemsHtml}
                        </tbody>
                        <tfoot>
                                <tr class='total-row'
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td colspan='2' class='total-cell footceld'>TOTAL</td>
                            <td class='footceld'>{presupuesto.totalPresupuesto:C2}</td>
                        </tr>
                            <tr class='total-row'
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td colspan='2' class='total-cell footceld'>IGV</td>
                            <td class='footceld'>{presupuesto.igvPresupuesto:C2}</td>
                        </tr>
                           <tr class='total-row'
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td colspan='2' class='total-cell footceld'>TOTAL IGV</td>
                            <td class='footceld'>{presupuesto.totalIgvPresupuesto:C2}</td>
                        </tr>
                        </tfoot>
                    </table>
                </section>
            </main>
            <section class='adicional'>
            <h2 class='observaciones'>Observaciones</h2>

            <div class='subtitleContainer'>
                <h5 class='subtitle'>PROVISIONES INCLUIDAS</h5>
                <li>Esta cotización ha sido elaborada en base a los bocetos enviados, cualquier diferencia de estas medidas se considerará como costos adicionales al presente. Se encuentra expresada en nuevos soles</li>
            </div>

            <div class='subtitleContainer'>
                <h5 class='subtitle'>EXCLUSIONES</h5>
                <li>Los trabajos complementarios de albañilería (picado y resane) serán por cuenta del cliente.</li>
                <li>El picado y resane de la cajuela en el piso para el empotramiento de las guías inferiores de las mamparas corredizas será a cuenta del cliente, según indicación de nuestro técnico en obra.</li>
                <li>No se incluyen trabajos de carpintería de madera, cerrajería, albañilería ni pintura</li>
                <li>La energía eléctrica deberá ser suministrada por el cliente para el proceso de instalación.</li>
                <li>No incluye el trámite de licencias ni cercos provisionales.</li>
                <li>No incluye medios auxiliares de elevación. En caso de ser necesarios los proporcionará el cliente.</li>
            </div>

            <div class='subtitleContainer'>
                <h5 class='subtitle'>CONSIDERACIONES</h5>
                <li>Los vanos deberán estar nivelados y aplomados, antes de proceder con la instalación.</li>
                <li> El Cliente deberá proporcionar los planos de las instalaciones eléctricas y sanitarias a fin de evitar roturas de tuberías empotradas.</li>
                <li> El cliente dará las facilidades para realizar el remetrado, izamiento de materiales e instalación. Asimismo proporcionará un espacio adecuado y seguro para el almacenamiento de materiales durante el proceso de instalación.</li>
                <li>La fabricación se puede realizar con medidas pactadas siempre con la previa aprobación de detalles por el cliente</li>
                <li> Cotización sujeta a stock disponible en el momento de la aceptación del presupuesto.</li>
                <li>Todo cambio debe ser coordinado con su vendedor y generará el adicional correspondiente</li>
            </div>

            <div class='subtitleContainer'>
                <h5 class='subtitle'>PLAZOS DE ENTREGA</h5>
                <li>PLAZOS DE ENSAMBLE E INSTALACIÓN: Se realizará el cronograma de obra acorde a los plazos habituales que se adapte al cronograma general requerido por el cliente final.
                </li>
            </div>

            <div class='secondDataClient'>
                <p>ENSAMBLE: <span>{presupuesto.ensamble}</span></p>
    
                <p>INSTALACION: <span>{presupuesto.instalacion}</span></p>
    
                <p>ENTREGA: <span>{presupuesto.entrega}</span></p>
    
                <p>FORMA DE PAGO: <span>{presupuesto.formaPago}</span> A CONFORMIDAD DE ENTREGA</p>
    
                <p>RETENCIONES: <span>{presupuesto.retenciones}</span></p>
    
                <p>VALIDEZ DE LA OFERTA: <span>{presupuesto.validez}</span></p>
            </div>

        </section>

        <footer>
            <div class='metodoPagoContainer'>
                <div class='pagoImg'>
                    <img src='https://joseolaya.com/wp-content/uploads/2022/01/bcp-logo.png' alt=''>
                </div>
                <div class='infoPago'>
                    <p>BCP CTA. CORRIENTE SOLES: 310-2309097-0-24</p>
                    <p>CCI: 002-310002309097024</p>
                </div>
            </div>

            <div class='metodoPagoContainer'>
                <div class='pagoImg'>
                    <img src='https://financefeeds.com/wp-content/uploads/2024/01/scotiabanke.png' alt=''>
                </div>
                <div class='infoPago'>
                    <p>SCOTIABANK CTA. AHORROS SOLES: 755-0341213</p>
                    <p>CCI: 009-755207550341213</p>
                </div>
            </div>

            <div class='metodoPagoContainer'>
                <div class='pagoImg'>
                    <img src='https://www.bn.com.pe/img/logobn-compartir.png' alt=''>
                </div>
                <div class='infoPago'>
                    <p>BANCO DE LA NACION CTA. DE DETRACCIONES: 00-785-020413</p>
                    <p>*Solo servicios</p>
                </div>
            </div>
        </footer>
        </div>
    </body>
    </html>";
        }

    }
}
