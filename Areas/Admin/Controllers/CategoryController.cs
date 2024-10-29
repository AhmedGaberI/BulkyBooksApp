
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Microsoft.AspNetCore.Mvc;
namespace BulkyApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
        
    {
       
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



        public IActionResult Index()
        {
            List<Category> categoryList = _unitOfWork.Category.GetAll().ToList();
            return View(categoryList);
        }

        public IActionResult Create()
        {
            return View();   
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0) return NotFound();
            // var category =_unitOfWork.Category.Categories.FirstOrDefault(c => c.Id == Id);    
            Category? category = _unitOfWork.Category.Get(c => c.Id == Id);
            if (category == null) return NotFound();


            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Save();
                TempData["success"] = "Category Updated Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0) return NotFound();
            // var category =_unitOfWork.Category.Categories.FirstOrDefault(c => c.Id == Id);    
            Category? category = _unitOfWork.Category.Get(c => c.Id == Id);
            if (category == null) return NotFound();


            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {

            Category? category = _unitOfWork.Category.Get(c => c.Id == id);
            if (category == null) return NotFound();

            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();

            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index");

        }

    }
}
