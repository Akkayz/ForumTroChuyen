using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebTroChuyen.Models;

namespace WebTroChuyen.Areas.Admin.Controllers
{
    public class BaiVietsController : Controller
    {
        private ForumTroChuyenEntities db = new ForumTroChuyenEntities();

        // GET: Admin/BaiViets
        public ActionResult Index(string trangThai)
        {
            var baiViets = db.BaiViets.Include(b => b.DanhMuc).Include(b => b.NguoiDung);

            if (!string.IsNullOrEmpty(trangThai))
            {
                bool trangThaiValue = trangThai.ToLower() == "true";

                baiViets = baiViets.Where(b => b.TrangThai == trangThaiValue);
            }

            return View(baiViets.ToList());
        }

        // GET: Admin/BaiViets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BaiViet baiViet = db.BaiViets.Find(id);
            if (baiViet == null)
            {
                return HttpNotFound();
            }
            return View(baiViet);
        }

        // GET: Admin/BaiViets/Create
        public ActionResult Create()
        {
            ViewBag.DanhMucID = new SelectList(db.DanhMucs, "DanhMucID", "TenDanhMuc");
            ViewBag.UserID = new SelectList(db.NguoiDungs, "UserID", "TenNguoiDung");
            return View();
        }

        // POST: Admin/BaiViets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BaiVietID,TieuDe,NoiDung,NgayDang,UserID,DanhMucID,HinhAnh,TrangThai")] BaiViet baiViet)
        {
            if (ModelState.IsValid)
            {
                db.BaiViets.Add(baiViet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DanhMucID = new SelectList(db.DanhMucs, "DanhMucID", "TenDanhMuc", baiViet.DanhMucID);
            ViewBag.UserID = new SelectList(db.NguoiDungs, "UserID", "TenNguoiDung", baiViet.UserID);
            return View(baiViet);
        }

        // GET: Admin/BaiViets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BaiViet baiViet = db.BaiViets.Find(id);
            if (baiViet == null)
            {
                return HttpNotFound();
            }
            ViewBag.DanhMucID = new SelectList(db.DanhMucs, "DanhMucID", "TenDanhMuc", baiViet.DanhMucID);
            ViewBag.UserID = new SelectList(db.NguoiDungs, "UserID", "TenNguoiDung", baiViet.UserID);
            return View(baiViet);
        }

        // POST: Admin/BaiViets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BaiVietID,TieuDe,NoiDung,NgayDang,UserID,DanhMucID,HinhAnh,TrangThai")] BaiViet baiViet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(baiViet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DanhMucID = new SelectList(db.DanhMucs, "DanhMucID", "TenDanhMuc", baiViet.DanhMucID);
            ViewBag.UserID = new SelectList(db.NguoiDungs, "UserID", "TenNguoiDung", baiViet.UserID);
            return View(baiViet);
        }

        // GET: Admin/BaiViets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BaiViet baiViet = db.BaiViets.Find(id);
            if (baiViet == null)
            {
                return HttpNotFound();
            }
            return View(baiViet);
        }

        // POST: Admin/BaiViets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BaiViet baiViet = db.BaiViets.Find(id);
            db.BaiViets.Remove(baiViet);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}