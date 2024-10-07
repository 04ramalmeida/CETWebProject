namespace CETWebProject.Data
{
    public class UserTempRepository : GenericRepository<UserTemp>, IUserTempRepository
    {
        private readonly DataContext _context;

        public UserTempRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
