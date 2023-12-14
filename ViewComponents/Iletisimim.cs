using Microsoft.AspNetCore.Mvc;
using Storer.Data;
using System.Linq;

namespace Storer.ViewComponents
{
    public class Iletisimim : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public Iletisimim(ApplicationDbContext db)
        {
            _db = db;
        }
        public IViewComponentResult Invoke()
        {
            var iletisim = _db.Iletisimims.ToList();
            return View(iletisim);
        }
    }
}
