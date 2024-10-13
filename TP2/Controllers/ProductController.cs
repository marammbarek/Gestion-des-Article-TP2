using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TP2.Models.Repositories;
using TP2.Models;
using TP2.ViewModels;

namespace TP2.Controllers
{
    public class ProductController : Controller
    {
        private readonly IRepository<Product> productRepository;
        private readonly IWebHostEnvironment hostingEnvironment;

        // Injection de dépendance pour le repository de produit
        public ProductController(IRepository<Product> prodRepository , IWebHostEnvironment hostingEnvironment)
        {
            productRepository = prodRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: ProductController
        public ActionResult Index()
        {
            var products = productRepository.GetAll(); // Récupérer tous les produits
            return View(products);
        }

        // GET: ProductController/Details/5
        public ActionResult Details(int id)
        {
            var product = productRepository.Get(id); // Utiliser Get au lieu de FindByID
            if (product == null)
            {
                return NotFound(); // Retourner NotFound si le produit n'est pas trouvé
            }
            return View(product); // Retourner la vue avec les détails du produit
        }

        // GET: ProductController/Create
        public ActionResult Create()
        {
            return View(); // Retourner la vue de création
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                // Si l'image a été sélectionnée par l'utilisateur
                if (model.ImagePath != null)
                {
                    // Chemin vers le dossier images dans wwwroot
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");

                    // Générer un nom de fichier unique
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Use CopyTo() method provided by IFormFile interface to
                    // copy the file to wwwroot/images folder
                    model.ImagePath.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                // Créer un nouvel objet Product
                Product newProduct = new Product
                {
                    Désignation = model.Désignation,
                    Prix = model.Prix,
                    Quantite = model.Quantite,
                    Image = uniqueFileName // Stocker le nom de fichier unique
                };

                // Ajouter le produit à la base de données
                productRepository.Add(newProduct);
                return RedirectToAction("Details", new { id = newProduct.Id });
            }

            return View();
        }

        // GET: ProductController/Edit/5
        public ActionResult Edit(int id)
        {
            Product product = productRepository.Get(id);
            EditViewModel productEditViewModel = new EditViewModel
            {
                Id = product.Id,
                Désignation = product.Désignation,
                Prix = product.Prix,
                Quantite = product.Quantite,
                ExistingImagePath = product.Image
            };
            return View(productEditViewModel);
        }



        //------------------------------------------------------------------

        // POST: ProductController/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditViewModel model)
        {
            // Check if the provided data is valid, if not rerender the edit view
            // so the user can correct and resubmit the edit form
            if (ModelState.IsValid)
            {
                // Retrieve the product being edited from the database
                Product product = productRepository.Get(model.Id);
                // Update the product object with the data in the model object
                product.Désignation = model.Désignation;
                product.Prix = model.Prix;
                product.Quantite = model.Quantite;
                // If the user wants to change the photo, a new photo will be
                // uploaded and the Photo property on the model object receives
                // the uploaded photo. If the Photo property is null, user did
                // not upload a new photo and keeps his existing photo
                if (model.ImagePath != null)
                {
                    // If a new photo is uploaded, the existing photo must be
                    // deleted. So check if there is an existing photo and delete
                    if (model.ExistingImagePath != null)
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingImagePath);
                        System.IO.File.Delete(filePath);
                    }
                    // Save the new photo in wwwroot/images folder and update
                    // PhotoPath property of the product object which will be
                    // eventually saved in the database
                    product.Image = ProcessUploadedFile(model);
                }
                // Call update method on the repository service passing it the

                // product object to update the data in the database table
                Product updatedProduct = productRepository.Update(product);
                if (updatedProduct != null)
                    return RedirectToAction("Index");
                else
                    return NotFound();

            }
            return View(model);
        }
        [NonAction]
        private string ProcessUploadedFile(EditViewModel model)
        {
            string uniqueFileName = null;
            if (model.ImagePath != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImagePath.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }



        //----------------------------------------------------------------------------------------

        // GET: ProductController/Delete/5
        public ActionResult Delete(int id)
        {
            var product = productRepository.Get(id); // Utiliser Get au lieu de FindByID
            if (product == null)
            {
                return NotFound(); // Retourner NotFound si le produit n'est pas trouvé
            }
            return View(product); // Retourner la vue de confirmation de suppression
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                productRepository.Delete(id); // Supprimer le produit
                return RedirectToAction(nameof(Index)); // Rediriger vers l'index après suppression
            }
            catch
            {
                return View(); // Retourner la vue de suppression en cas d'erreur
            }
        }

       // public ActionResult Search(string term)
        //{
         //   var result = productRepository.Search(term); // Ajouter une méthode de recherche si disponible
          //
          //return View("Index", result); // Afficher les résultats de la recherche
        //}
    }
}
