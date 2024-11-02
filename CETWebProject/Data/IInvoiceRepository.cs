using CETWebProject.Data.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CETWebProject.Data
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        Task<ICollection<Invoice>> GetInvoicesByUserAsync(string userId);

        Task AddInvoiceAsync(string userId, double usageAmount);

        Invoice GetInvoiceWithUser(int id);

        Task PayInvoice(Invoice invoice);
    }
}
