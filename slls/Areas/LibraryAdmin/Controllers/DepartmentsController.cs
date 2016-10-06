using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class DepartmentsController : AdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly GenericRepository _repository;
        private readonly string _entityName = DbRes.T("Departments.Department", "FieldDisplayName");

        public DepartmentsController()
        {
            _repository = new GenericRepository(typeof(Department));
            ViewBag.Title = DbRes.T("Departments", "EntityType");
        }

        // GET: Departments
        public ActionResult Index()
        {
            var list = _repository.GetAll<Department>().Where(d => d.Deleted == false).OrderBy(x => x.ListPos).ThenBy(x => x.Department1);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(list);
        }

        
        // GET: Departments/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add New " + _entityName;
            var davm = new DepartmentsAddViewModel();
            return PartialView(davm);
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "Department")] DepartmentsAddViewModel davm)
        {
            if (ModelState.IsValid)
            {
                var department = new Department
                {
                    Department1 = davm.Department,
                    CanUpdate = true,
                    CanDelete = true,
                    InputDate = DateTime.Now
                };
                _repository.Insert(department);
                CacheProvider.RemoveCache("departments");
                //return RedirectToAction("Index");
                return Json(new { success = true });
            }

            return PartialView(davm);
        }

        public ActionResult _add()
        {
            return PartialView();
        }

        // POST: Departments/_add
        [HttpPost]
        public JsonResult _add(Department department)
        {
            if (ModelState.IsValid)
            {
                var newDepartment = _db.Departments.FirstOrDefault(x => x.Department1 == department.Department1);

                if (newDepartment == null)
                {
                    newDepartment = new Department()
                    {
                        Department1 = department.Department1,
                        CanUpdate = true,
                        CanDelete = true,
                        InputDate = DateTime.Now
                    };

                    _db.Departments.Add(newDepartment);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("departments");

                    return Json(new
                    {
                        success = true,
                        newData = newDepartment
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        errMsg = "This " + _entityName + " already exists!"
                    });
                }
            }
            return null;
        }


        // GET: Departments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var department = _repository.GetById<Department>(id.Value);
            if (department == null)
            {
                return HttpNotFound();
            }
            if (department.Deleted)
            {
                return HttpNotFound();
            }

            ViewBag.Title = "Edit " + _entityName;
            var devm = new DepartmentsEditViewModel
            {
                Department = department.Department1,
                DepartmentID = department.DepartmentID
            };
            return PartialView(devm);
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepartmentID,Department")] DepartmentsEditViewModel viewModel)
        {
            if (!ModelState.IsValid) return PartialView(viewModel);
            var department = _repository.GetById<Department>(viewModel.DepartmentID);
            if (department == null)
            {
                return HttpNotFound();
            }
            if (department.Deleted)
            {
                return HttpNotFound();
            }
            //department.DepartmentID = viewModel.DepartmentID;
            department.Department1 = viewModel.Department;
            department.LastModified = DateTime.Now;
            _repository.Update(department);
            //return RedirectToAction("Index");
            CacheProvider.RemoveCache("departments");
            return Json(new { success = true });
        }
        
        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var dept = _repository.GetById<Department>(id);
            if (dept == null)
            {
                return HttpNotFound();
            }
            if (dept.Deleted)
            {
                return HttpNotFound();
            }
            if (dept.CanDelete == false)
            {
                return RedirectToAction("Index");
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel dcvm = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "Departments",
                DetailsText = dept.Department1
            };
            return PartialView("_DeleteConfirmation", dcvm);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var item = _repository.GetById<Department>(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Delete(item);
                    CacheProvider.RemoveCache("departments");
                    return Json(new { success = true });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            return PartialView("_DeleteConfirmation", dcvm);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
                _repository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}