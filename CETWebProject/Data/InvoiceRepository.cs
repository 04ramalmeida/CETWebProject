using CETWebProject.Data.Entities;
using CETWebProject.Helpers;
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

        public InvoiceRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task AddInvoiceAsync(string userId, double usageAmount)
        {
            var user = await _userHelper.GetUserById(userId);
            var newInvoice = new Invoice
            {
                Value = ComputeValue(usageAmount),
                Date = DateTime.Now,
                User = user
            };
            _context.Add(newInvoice);
            await _context.SaveChangesAsync();
        }

        private decimal ComputeValue(double usageAmount)
        {
            double aux = usageAmount;
            decimal result = 0;
            while (aux > 0)
            {
                if (usageAmount <= 5)
                {
                    result = Convert.ToDecimal((usageAmount * 0.3));
                    aux = 0;
                }
                else
                {
                    result += Convert.ToDecimal((5 * 0.3));
                    aux -= 5;
                    if (usageAmount > 5)
                    {
                        if (aux > 10)
                        {
                            result += Convert.ToDecimal(10 * 0.8);
                            aux -= 10;
                        }
                        else
                        {
                            result += Convert.ToDecimal(aux * 0.8);
                            aux = 0;
                        }
                        if (usageAmount > 15)
                        {
                            if (aux > 20)
                            {
                                result += Convert.ToDecimal(20 * 0.8);
                                aux -= 20;
                            }
                            else
                            {
                                result += Convert.ToDecimal(aux * 1.20);
                                aux = 0;
                            }
                            if (usageAmount > 25)
                            {
                                result += Convert.ToDecimal(aux * 1.8);
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


    }
}
