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
    public class BaiVietBaoCaosController : Controller
    {
        private ForumTroChuyenEntities db = new ForumTroChuyenEntities();

        // GET: Admin/BaiVietBaoCaos
        public ActionResult Index()
        {
            var baiVietBaoCaos = db.BaiVietBaoCaos.Include(b => b.BaiViet).Include(b => b.NguoiDung);
            return View(baiVietBaoCaos.ToList());
        }

        // GET: Admin/BaiVietBaoCaos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BaiVietBaoCao baiVietBaoCao = db.BaiVietBaoCaos.Find(id);
            if (baiVietBaoCao == null)
            {
                return HttpNotFound();
            }
            return View(baiVietBaoCao);
        }

        // GET: Admin/BaiVietBaoCaos/Create
        public ActionResult Create()
        {
            ViewBag.BaiVietID = new SelectList(db.BaiViets, "BaiVietID", "TieuDe");
            ViewBag.UserID = new SelectList(db.NguoiDungs, "UserID", "TenNguoiDung");
            return View();
        }

        // POST: Admin/BaiVietBaoCaos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,BaiVietID,UserID,LyDoBaoCao,NgayBaoCao,TrangThai")] BaiVietBaoCao baiVietBaoCao)
        {
            if (ModelState.IsValid)
            {
                db.BaiVietBaoCaos.Add(baiVietBaoCao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BaiVietID = new SelectList(db.BaiViets, "BaiVietID", "TieuDe", baiVietBaoCao.BaiVietID);
            ViewBag.UserID = new SelectList(db.NguoiDungs, "UserID", "TenNguoiDung", baiVietBaoCao.UserID);
            return View(baiVietBaoCao);
        }

        // GET: Admin/BaiVietBaoCaos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BaiVietBaoCao baiVietBaoCao = db.BaiVietBaoCaos.Find(id);
            if (baiVietBaoCao == null)
            {
                return HttpNotFound();
            }
            ViewBag.BaiVietID = new SelectList(db.BaiViets, "BaiVietID", "TieuDe", baiVietBaoCao.BaiVietID);
            ViewBag.UserID = new SelectList(db.NguoiDungs, "UserID", "TenNguoiDung", baiVietBaoCao.UserID);
            return View(baiVietBaoCao);
        }

        // POST: Admin/BaiVietBaoCaos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,BaiVietID,UserID,LyDoBaoCao,NgayBaoCao,TrangThai")] BaiVietBaoCao baiVietBaoCao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(baiVietBaoCao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BaiVietID = new SelectList(db.BaiViets, "BaiVietID", "TieuDe", baiVietBaoCao.BaiVietID);
            ViewBag.UserID = new SelectList(db.NguoiDungs, "UserID", "TenNguoiDung", baiVietBaoCao.UserID);
            return View(baiVietBaoCao);
        }

        // GET: Admin/BaiVietBaoCaos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BaiVietBaoCao baiVietBaoCao = db.BaiVietBaoCaos.Find(id);
            if (baiVietBaoCao == null)
            {
                return HttpNotFound();
            }
            return View(baiVietBaoCao);
        }

        // POST: Admin/BaiVietBaoCaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BaiVietBaoCao baiVietBaoCao = db.BaiVietBaoCaos.Find(id);
            db.BaiVietBaoCaos.Remove(baiVietBaoCao);
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

        public ActionResult ChiTietBaiViet(int? baiVietID)
        {
            if (baiVietID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BaiViet baiViet = db.BaiViets.Find(baiVietID);
            if (baiViet == null)
            {
                return HttpNotFound();
            }

            return View(baiViet);
        }
    }
}