﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NToastNotify;
using Storer.Data;
using Storer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Storer.Areas.Musteri.Controllers // namespace düzenleme ile homecontroller yolunu düzenledik
{
    [Area("Musteri")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IToastNotification _toast;
        private readonly IWebHostEnvironment _whe;
        private object iletisim;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, IToastNotification toast, IWebHostEnvironment whe)
        {
            _logger = logger;
            _db = db;
            _toast = toast;
            _whe = whe;
        }

        public IActionResult Index()
        {
            //özel menüleri ana sayfada getirme işlemi
            var menu = _db.Menus.Where(i => i.OzelMenu).ToList();
            return View(menu);
        }
        public IActionResult CategoryDetails(int? id)
        {
            //kategorilere ait menüleri getirme
            var menu = _db.Menus.Where(i => i.CategoryId == id).ToList();
            ViewBag.KategoriId = id;
            return View(menu);
        }
        public IActionResult Iletisim()
        {
            return View();
        }
        // POST: Yonetici/Iletisim/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Iletisim([Bind("Id,Name,Email,Telefon,Mesaj")] Iletisim iletisim)
        {
            if (ModelState.IsValid)
            {
                iletisim.Tarih = DateTime.Now;
                _db.Add(iletisim);
                await _db.SaveChangesAsync();
                _toast.AddSuccessToastMessage("Mesajınız başarıyla iletildi.");
                return RedirectToAction(nameof(Index));
            }
            return View(iletisim);
        }

        public IActionResult Yorumlar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Yorumlar(Yorumlar yorumlar)
        {
            if (!ModelState.IsValid)
            {
				yorumlar.Tarih = DateTime.Now;
                //yorum tarihi müşteri girmeyecek, sistemden biz çekeceğiz. şuan için normal yapalım.
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
                    if (yorumlar.Image != null)
                    {
                        var ImagePath = Path.Combine(_whe.WebRootPath, yorumlar.Image.TrimStart('\\'));
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
                    yorumlar.Image = @"\WebSite\menu\" + fileName + extn;
                }
                _db.Add(yorumlar);
                await _db.SaveChangesAsync();
                _toast.AddSuccessToastMessage("Yorumunuz iletildi, onaylanan yorumları sayfamızdan görebilirsiniz.Teşekkür ederiz.");
                return RedirectToAction(nameof(Index));
            }
            return View(yorumlar);
        }
        public IActionResult Hakkında()
        {
            var hakkında = _db.Hakkındas.ToList();
            return View(hakkında);
        }
        public IActionResult Galeri()
        {
            var galeri = _db.Galeris.ToList();
            return View(galeri);
        }


        public IActionResult Menu()
        {
            //menu sayfasına tüm meüleri getirme işlemi
            var menu = _db.Menus.ToList();
            return View(menu);
        }
        private static List<CartItem> cartItems = new List<CartItem>();
        public IActionResult Cart()
        {
            var cartItems = _db.Carts.ToList();
            return View(cartItems);
        }

        public IActionResult ProductDetails(int productId)
        {
            var menu = GetMenuById(productId);

            return View(menu);
        }
        
  
        //ürün detayları sayfasında eklenebilen yorumlar
        public IActionResult Yorum()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddToCart(int productId)
        {
            try
            {
                var menu = _db.Menus.Find(productId);

                if (menu != null)
                {
                    var existingItem = _db.Carts.FirstOrDefault(item => item.ProductId == productId);

                    if (existingItem != null)
                    {
                        existingItem.Quantity++;
                    }
                    else
                    {
                        _db.Carts.Add(new CartItem
                        {
                            ProductId = menu.Id,
                            ProductName = menu.Title,
                            Description = menu.Description,
                            Image = menu.Image,
                            Price = menu.Price,
                            Quantity = 1
                        });
                    }

                    _db.SaveChanges();

                    return Json(new { success = true, message = "Ürün sepete eklendi!" });
                }
                else
                {
                    return Json(new { success = false, message = "Ürün bulunamadı." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ürün eklenirken bir hata oluştu: " + ex.Message });
            }
        }


        public ActionResult RemoveFromCart(int productId)
        {
            var itemToRemove = _db.Carts.Find(productId);

            if (itemToRemove != null)
            {
                _db.Carts.Remove(itemToRemove);
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult ClearCart()
        {
            try
            {
                var cartItems = _db.Carts.ToList();

                foreach (var cartItem in cartItems)
                {
                    _db.Carts.Remove(cartItem);
                }

                _db.SaveChanges();

                return Json(new { success = true, message = "Sepet temizlendi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Sepet temizlenirken bir hata oluştu: " + ex.ToString() });
            }
        }


        private Menu GetMenuById(int productId)
        {
            return new Menu { Id = productId, Title = "Ürün Adı", Description = "Ürün Açıklaması", Image = "path/to/image.jpg", Price = 19.99 };
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
