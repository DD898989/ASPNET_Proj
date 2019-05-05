using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication00.Models;

namespace MvcApplication00.Controllers
{
    public class FilmsController : Controller
    {
        private FilmDBContext db = new FilmDBContext();

        //
        // GET: /Films/

        public ActionResult Index()
        {
            return View(db.Films.ToList());
        }

        //
        // GET: /Films/Details/5

        public ActionResult Details(int id = 0)
        {
            Class1_Film class1_film = db.Films.Find(id);
            if (class1_film == null)
            {
                return HttpNotFound();
            }
            return View(class1_film);
        }

        //
        // GET: /Films/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Films/Create

        [HttpPost]
        public ActionResult Create(Class1_Film class1_film)
        {
            if (ModelState.IsValid)
            {
                db.Films.Add(class1_film);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(class1_film);
        }

        //
        // GET: /Films/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Class1_Film class1_film = db.Films.Find(id);
            if (class1_film == null)
            {
                return HttpNotFound();
            }
            return View(class1_film);
        }

        //
        // POST: /Films/Edit/5

        [HttpPost]
        public ActionResult Edit(Class1_Film class1_film)
        {
            if (ModelState.IsValid)
            {
                db.Entry(class1_film).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(class1_film);
        }

        //
        // GET: /Films/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Class1_Film class1_film = db.Films.Find(id);
            if (class1_film == null)
            {
                return HttpNotFound();
            }
            return View(class1_film);
        }

        //
        // POST: /Films/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Class1_Film class1_film = db.Films.Find(id);
            db.Films.Remove(class1_film);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}