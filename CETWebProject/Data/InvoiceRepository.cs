using CETWebProject.Data.Entities;
using CETWebProject.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CETWebProject.Data
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IEchelonRepository _echelonRepository;

        public InvoiceRepository(DataContext context, IUserHelper userHelper, IEchelonRepository echelonRepository) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
            _echelonRepository = echelonRepository;
        }

        public async Task AddInvoiceAsync(string userId, double usageAmount)
        {
            var user = await _userHelper.GetUserById(userId);
            var newInvoice = new Invoice
            {
                Value = await ComputeValue(usageAmount),
                Date = DateTime.Now,
                User = user
            };
            _context.Add(newInvoice);
            await _context.SaveChangesAsync();
        }

        private async Task<decimal> ComputeValue(double usageAmount)
        {
            var firstEchelon = await _echelonRepository.GetByIdAsync(1);
            var secondEchelon = await _echelonRepository.GetByIdAsync(2);
            var thirdEchelon = await _echelonRepository.GetByIdAsync(3);
            double aux = usageAmount;
            decimal result = 0;
            while (aux > 0)
            {
                if (usageAmount <= 5)
                {
                    result = Convert.ToDecimal((usageAmount * firstEchelon.Value));
                    aux = 0;
                }
                else
                {
                    result += Convert.ToDecimal((5 * firstEchelon.Value));
                    aux -= 5;
                    if (usageAmount > 5)
                    {
                        if (aux > 10)
                        {
                            result += Convert.ToDecimal(10 * secondEchelon.Value);
                            aux -= 10;
                        }
                        else
                        {
                            result += Convert.ToDecimal(aux * secondEchelon.Value);
                            aux = 0;
                        }
                        if (usageAmount > 15)
                        {
                            if (aux > 20)
                            {
                                result += Convert.ToDecimal(20 * secondEchelon.Value);
                                aux -= 20;
                            }
                            else
                            {
                                result += Convert.ToDecimal(aux * thirdEchelon.Value);
                                aux = 0;
                            }
                            if (usageAmount > 25)
                            {
                                result += Convert.ToDecimal(aux * thirdEchelon.Value);
                                aux = 0;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public async Task<ICollection<Invoice>> GetInvoicesByUserAsync(string userId)
        {
            var user = await _userHelper.GetUserById(userId);
            return _context.invoices.Where(i => i.User == user)
                .OrderBy(i => i.Id).ToList();
        }

        public Invoice GetInvoiceWithUser(int id)
        {
            return _context.invoices
                .Where(i => i.Id == id)
                .Include(i => i.User)
                .FirstOrDefault();
        }

        public async Task PayInvoice(Invoice invoice)
        {
            invoice.IsPaid = true;
            _context.Update(invoice);
            await _context.SaveChangesAsync();
        }

        public Invoice GetLastUnpaidInvoice()
        {
            return _context.invoices
                .Where(i => i.IsPaid == false)
                .OrderBy(i => i.Date)
                .FirstOrDefault();
        }
    }
}
