using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            List<PageVM> pageList;

         
            using (Db db = new Db())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            return View(pageList);
        }


        // GET: Admin/Pages/AddPages
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/Pages/AddPages
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {

                
                string slug;

                PagesDTO dto = new PagesDTO();

                dto.Title = model.Title.ToUpper();

                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "This title already exist");
                    return View(model);
                }
                else if (db.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "This slug already exist");
                    return View(model);
                }

                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                db.Pages.Add(dto);
                db.SaveChanges();

            }

            TempData["SM"] = "You have added a new page!";

            return RedirectToAction("Index");
        }

        // GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {

            PageVM model;

            using (Db db = new Db())
            {

                PagesDTO dto = db.Pages.Find(id);


                if (dto == null)
                {
                    return Content("The page doesn't exist");
                }


                model = new PageVM(dto);
            }
            return View(model);
        }

        // POST: Admin/Pages/AddPage
        public ActionResult EditPage(PageVM model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {

                int id = model.Id;


                string slug = "home";


                PagesDTO dto = db.Pages.Find(id);


                dto.Title = model.Title;


                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }


                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "This title already exists.");
                    return View(model);
                }

                else if (db.Pages.Where(x=>x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "This slug already exists.");
                    return View(model);
                }


                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;


                db.SaveChanges();

            }


            TempData["SM"] = "You have edited the page!";

            return RedirectToAction("EditPage");
        }

        // GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {

            PageVM model;

            using (Db db = new Db())
            {

                PagesDTO dto = db.Pages.Find(id);    


                if (dto == null)
                {
                    return Content("The page doesn't exist.");
                }


                model = new PageVM(dto);
            }

                return View(model);
        }

        // GET: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {


                PagesDTO dto = db.Pages.Find(id);


                db.Pages.Remove(dto);


                db.SaveChanges();

            }


            TempData["SM"] = "You have deleted the page!";

            return RedirectToAction("Index");
        }

        // GET: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {
                int count = 1;

                PagesDTO dto;

                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }
            }
        }

        
        // GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            SidebarVM model;

            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebars.Find(1); //Говнокод: Жёсткие значения

                model = new SidebarVM(dto);
            }
            return View(model);
        }

        // POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebars.Find(1); //Конкретное значение. Лучше исправить

                dto.Body = model.Body;

                db.SaveChanges();
            }

            TempData["SM"] = "You have edited the sidebar!";

            return RedirectToAction("EditSidebar");
        }
    }
}