using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Storer.Data;
using Storer.Models;

namespace Storer.Areas.Yonetici.Controllers
{
    [Area("Yonetici")]
    [Authorize]
    public class YorumlarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _whe;

        public YorumlarController(ApplicationDbContext context, IWebHostEnvironment whe)
        {
            _context = context;
            _whe = whe;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Yorumlars.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yorum = await _context.Yorumlars
				.FirstOrDefaultAsync(m => m.Id == id);
            if (yorum == null)
            {
                return NotFound();
            }

            return View(yorum);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Yorumlar yorum)
        {
            if (!ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                //if dosya kontrolü yaptım
                if (files.Count > 0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    //resim eklemek için path metodu kullanılır
                    //resim kaydetmek istediğim dosya yolunu belirttim
                    var uploads = Path.Combine(_whe.WebRootPath, @"WebSite\menu");
                    var extn = Path.GetExtension(files[0].FileName);
                    //menü resmini if ile kontrol ettim
                    //menü alanı boş değil ise resimleri ekler
                    if (yorum.Image != null)
                    {
                        var ImagePath = Path.Combine(_whe.WebRootPath, yorum.Image.TrimStart('\\'));
                        //menü silinirse menüye ait resmi de menü dosyasından silmesini sağladım
                        if (System.IO.File.Exists(ImagePath))
                        {
                            System.IO.File.Delete(ImagePath);
                        }
                    }
                    using (var filesStreams = new FileStream(Path.Combine(uploads, fileName + extn), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    yorum.Image = @"\WebSite\menu\" + fileName + extn;
                }
                _context.Add(yorum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(yorum);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Yorumlars.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Name,Email,Image,Onay,Mesaj,Tarih")] Yorumlar yorum)
        {
            if (id != yorum.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(yorum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(yorum.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(yorum);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Yorumlars
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var yorum = await _context.Yorumlars.FindAsync(id);
            _context.Yorumlars.Remove(yorum);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Yorumlars.Any(e => e.Id == id);
        }
    }
}
