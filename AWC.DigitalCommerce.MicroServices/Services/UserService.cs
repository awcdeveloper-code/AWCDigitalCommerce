// Services/ProductService.cs
using AWC.DigitalCommerce.MicroServices.Data;
using AWC.DigitalCommerce.MicroServices.Models;
using System.Collections.Generic;
using System.Linq;

namespace AWC.DigitalCommerce.MicroServices.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAllProducts()
        {
            return _context.Users.ToList();
        }

        public User GetProductById(int id)
        {
            return _context.Users.Find(id);
        }

        public void AddProduct(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}
