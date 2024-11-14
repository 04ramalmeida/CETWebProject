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

        public async Task AddInvoiceAsync(string userId, double usageAmount, Reading reading)
        {
            List<decimal> results = await ComputeValue(usageAmount);
            var user = await _userHelper.GetUserById(userId);
            var newInvoice = new Invoice
            {
                TotalValue = results.Last(),
                Date = DateTime.Now,
                User = user,
                reading = reading,
                FirstDecimalValue = results.First(),
                SecondDecimalValue = results.ElementAt(1),
                ThirdDecimalValue = results.ElementAt(2)
            };
            _context.Add(newInvoice);
            await _context.SaveChangesAsync();
        }

        private async Task<List<decimal>> ComputeValue(double usageAmount)
        {
            List<decimal> results = new List<decimal>(new decimal[4]);
            var firstEchelon = await _echelonRepository.GetByIdAsync(1);
            var secondEchelon = await _echelonRepository.GetByIdAsync(2);
            var thirdEchelon = await _echelonRepository.GetByIdAsync(3);
            double aux = usageAmount;
            decimal total = 0;
            while (aux > 0)
            {
                if (usageAmount <= 5)
                {
                    var firstpass = Convert.ToDecimal((usageAmount * firstEchelon.Value));
                    total = firstpass;
                    results[0] = firstpass;
                    aux = 0;
                }
                else
                {
                    var secondpass = Convert.ToDecimal((5 * firstEchelon.Value));
                    total += secondpass;
                    results[0] = secondpass;
                    aux -= 5;
                    if (usageAmount > 5)
                    {
                        if (aux > 10)
                        {
                            var thirdpass = Convert.ToDecimal(10 * secondEchelon.Value);
                            total += thirdpass;
                            results[1] = thirdpass;
                            aux -= 10;
                        }
                        else
                        {
                            var fourthpass = Convert.ToDecimal(aux * secondEchelon.Value);
                            total += fourthpass;
                            results[1] = fourthpass;
                            aux = 0;
                        }
                        if (usageAmount > 15)
                        {
                            if (aux > 20)
                            {
                                var fifthpass = Convert.ToDecimal(20 * secondEchelon.Value);
                                total += fifthpass;
                                results[1] = fifthpass;
                                aux -= 20;
                            }
                            else
                            {
                                var sixthpass = Convert.ToDecimal(aux * thirdEchelon.Value);
                                total += sixthpass;
                                results[2] = sixthpass;
                                aux = 0;
                            }
                            if (usageAmount > 25)
                            {
                                var seventhpass = Convert.ToDecimal(aux * thirdEchelon.Value);
                                total += seventhpass;
                                results[2] = seventhpass;
                                aux = 0;
                            }
                        }
                    }
                }
            }
            results[3] = total;
            return results;
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
