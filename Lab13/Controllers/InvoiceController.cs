using Lab13.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab13.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InvoiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Invoice
        [HttpPost]
        public async Task<ActionResult<Invoice>> InsertInvoice(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.IdInvoices }, invoice);
        }

        // POST: api/Invoice/InsertInvoiceDetail
        [HttpPost("InsertInvoiceDetail")]
        public async Task<IActionResult> InsertInvoiceDetail([FromBody] InsertInvoiceDetailRequest request)
        {
            var invoice = await _context.Invoices.FindAsync(request.IdInvoice);
            if (invoice == null)
            {
                return NotFound();
            }

            foreach (var detail in request.ListDetails)
            {
                detail.Invoices_IdInvoices = request.IdInvoice;
                _context.Details.Add(detail);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET: api/Invoice/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }

            return invoice;
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.IdInvoices == id);
        }

        public class InsertInvoiceDetailRequest
        {
            public int IdInvoice { get; set; }
            public List<Detail> ListDetails { get; set; }
        }
    }
}
