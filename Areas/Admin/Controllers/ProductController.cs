using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyApp.Areas.Admin.Controllers
{
    [Area("Admin")] 
    public class ProductController : Controller
    {
       
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork , IWebHostEnvironment webHost)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHost;
        }


        public IActionResult Index()
        {
            List<Product> productList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
           
           
            return View(productList);
        }

        public IActionResult Upsert(int?id) /// update and insert
        {

            #region CodeCourse
            //ProductVM productVM = new()
            //{
            //    CategoryList = _unitOfWork.Category.GetAll()
            //    .Select
            //    (
            //        u => new SelectListItem
            //        {
            //            Text = u.Name,
            //            Value = u.Id.ToString()
            //        }
            //    ),
            //    Product = new Product()
            //}; 
            //if(id == null || id == 0)
            //{
            //    // insert / create new product 
            //    return View(productVM);
            //}
            //else
            //{
            //    // update 
            //    productVM.Product = _unitOfWork.Product.Get(p => p.Id == id);
            //}
            #endregion
            // My Code 
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll()
                .Select
                (
                    u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }
                ),
                Product = (id == null || id == 0) ? new Product() : _unitOfWork.Product.Get(p => p.Id == id)
            };

            return View(productVM);

        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM , IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);   
                    string productPath = Path.Combine(wwwRootPath, @"Images\Products");
                    
                    // If Image already exist and you want to update it 
                    // First delete the old one
                    if(!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        var oldImagePath =
                            Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    
                    using (var fileStream = new FileStream(Path.Combine(productPath , fileName) , FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                   productVM.Product.ImageUrl = @"\Images\Products\" + fileName;
                }
                if(productVM.Product.Id ==0) _unitOfWork.Product.Add(productVM.Product);
                else _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Save();

                return RedirectToAction("Index");
            }
            //return View();
            productVM.CategoryList = _unitOfWork.Category.GetAll()
                 .Select
                 (
                     u => new SelectListItem
                     {
                         Text = u.Name,
                         Value = u.Id.ToString()
                     }
                 );
               
            return View(productVM);
        }

        #region remove Edit 
        //public IActionResult Edit(int? Id)
        //{
        //    if (Id == null || Id == 0) return NotFound();
        //    // var category =_unitOfWork.Category.Categories.FirstOrDefault(c => c.Id == Id);    
        //    Product? product= _unitOfWork.Product.Get(c => c.Id == Id);
        //    if (product == null) return NotFound();


        //    return View(product);
        //}

        //[HttpPost]
        //public IActionResult Edit(Product product)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Update(product);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product Updated Successfully";
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        // }
        #endregion
        #region Remove Delete 
        //public IActionResult Delete(int? Id)
        //{
        //    if (Id == null || Id == 0) return NotFound();
        //    // var category =_unitOfWork.Category.Categories.FirstOrDefault(c => c.Id == Id);    
        //    Product? product = _unitOfWork.Product.Get(c => c.Id == Id);
        //    if (product == null) return NotFound();


        //    return View(product);
        //}

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePost(int? id)
        //{

        //    Product? product = _unitOfWork.Product.Get(c => c.Id == id);
        //    if (product == null) return NotFound();

        //    _unitOfWork.Product.Remove(product);
        //    _unitOfWork.Save();

        //    TempData["success"] = "Product Deleted Successfully";
        //    return RedirectToAction("Index");

        //}
        #endregion
        
        #region API CALL


        [HttpGet]
        public IActionResult GetAll()
        {

            List<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = productList });
        }
        [HttpDelete] // Why and when?

        public IActionResult Delete(int? id)
        {
            var productToDelete = _unitOfWork.Product.Get(p => p.Id == id);
            if(productToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            // delete the image 
            var imageProduct =
                    Path.Combine(_webHostEnvironment.WebRootPath,
                    productToDelete.ImageUrl.TrimStart('\\')
                    );
            if (System.IO.File.Exists(imageProduct))
            {
                System.IO.File.Delete(imageProduct);
            }

            _unitOfWork.Product.Remove(productToDelete);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Product Delete Successfully" });
        }
        #endregion
    }
}
