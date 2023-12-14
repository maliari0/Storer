using Microsoft.AspNetCore.Mvc;
using Storer.Data;
using System.Linq;

namespace Storer.ViewComponents
{
    public class Comments : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public Comments(ApplicationDbContext db)
        {
            _db = db;
        }
        public IViewComponentResult Invoke()
        {
            /*var comment=_db.Bloga.ToList();*///yönetici onayı olmadan tüm yorumları blog sayfasında gösterir//
            var comment = _db.Blogs.Where(i=>i.Onay).ToList(); //sadece onaylı yorumları gösterir
            return View(comment);
        }
    }
}
