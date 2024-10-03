using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ShopSiloAppFSD.Services;

namespace ShopSiloAppFSD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceService _invoiceService;

        public InvoiceController(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> DownloadInvoice(int orderId)
        {
            try
            {
                // Generate the invoice PDF
                byte[] pdfBytes = await _invoiceService.GenerateInvoicePdfAsync(orderId);

                // Return the PDF as a file download
                return File(pdfBytes, "application/pdf", $"Invoice_{orderId}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
