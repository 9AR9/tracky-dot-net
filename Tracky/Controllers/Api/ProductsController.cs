using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Tracky.Models;

namespace Tracky.Controllers.Api
{
    /* This API controller represents a made-from-scratch, beginner's controller that one might
     * create when following a tutorial on WebAPI, to become familiar with the basics of the
     * framwork, without having to create much architecture.
     * 
     * The Product domain object itself has been created in the Models folder of the web
     * application. No database is employed; rather, a local array of Products is created to
     * and used for two GET functions to return the full set of Products, or a single Product
     * given an Id.
     * 
     * This small effort demonstrates some of the out-of-the-box routing functionality that
     * is provided by creating a Controller for a domain object, and using conventional method
     * names. The routes to /api/products and /api/product/5 are now enabled, without having to
     * set them up elsewhere.
     */

    public class ProductsController : ApiController
    {
        readonly Product[] products =      // in lieu of querying a database, an array of products is stored locally
        {
            new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },
            new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M },
            new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M }
        };

        /// <summary>
        /// This method enables GET requests for the full collection of product objects, via its "GetAll{domain}s"
        /// naming convention with Web API default routing.
        /// GET: /api/products
        /// </summary>
        /// <returns>Collection of all product objects</returns>
        public IEnumerable<Product> GetAllProducts()
        {
            //products[1].Name += _authorService.ReturnThree();
            return products;
        }

        /// <summary>
        /// This method enables GET requests for a specific product using a product ID, via its "Get{domain}"
        /// naming convention and input parameter with Web API default routing.
        /// GET: /api/products/9
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult GetProduct(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
    }
}
