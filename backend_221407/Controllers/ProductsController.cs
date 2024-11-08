using backend_221407.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_221407.Controllers
{
    public class ProductsController : Controller
    {

        //private readonly ProductDBContext _context;

        //public ProductsController(ProductDBContext context)
        //{
        //    _context = context;
        //}


        /* Danh mục Categories  */
        [HttpGet("ListCategories")]
        public IEnumerable<Category> ListCategories()
        {
            ProductDBContext dtx = new ProductDBContext();
            return dtx.Categories;
        }

        /* Danh mục Supplier  */
        [HttpGet("ListSuppliers")]
        public IEnumerable<Supplier> ListSuppliers()
        {
            ProductDBContext dtx = new ProductDBContext();
            return dtx.Suppliers;
        }

        [HttpGet("ListProducts")]
        public IEnumerable<Product> ListProducts()
        {
            ProductDBContext dtx = new ProductDBContext();
            return dtx.Products.Take(100).ToList();
        }

        [HttpGet("SearchProducts")]
        public IEnumerable<object> SearchProducts(String? str_search, int? pagenumber, int? pagesize)
        {
            if (str_search == null)
                str_search = "";
            if (!pagesize.HasValue)
                pagesize = 20;
            if (!pagenumber.HasValue)
                pagenumber = 1;
            int begin = (pagenumber.Value - 1) * pagesize.Value;
            ProductDBContext dtx = new ProductDBContext();
            return (from x in dtx.Products
                    join cat in dtx.Categories on x.CategoryID equals cat.CategoryID
                    join sup in dtx.Suppliers on x.SupplierID equals sup.SupplierID
                    where x.ProductName.Contains(str_search) || cat.CategoryName.Contains(str_search) ||
                    sup.CompanyName.Contains(str_search) || sup.Country.Contains(str_search) || sup.Phone.Contains(str_search)
                    select new
                    {
                        ProductID = x.ProductID,
                        ProductName = x.ProductName,
                        CategoryID = x.CategoryID,
                        CategoryName = cat.CategoryName,
                        SupplierID = x.SupplierID,
                        CompanyName = sup.CompanyName,
                        QuantityPerUnit = x.QuantityPerUnit,
                        UnitPrice = x.UnitPrice
                    }

                    )
                .Skip(begin).Take(pagesize.Value).ToList();
        }



        [HttpGet("GetCount")]
        public int GetCount(String? str_search)
        {
            if (str_search == null)
                str_search = "";
            ProductDBContext dtx = new ProductDBContext();
            int count = (int)(from x in dtx.Products
                              join cat in dtx.Categories on x.CategoryID equals cat.CategoryID
                              join sup in dtx.Suppliers on x.SupplierID equals sup.SupplierID
                              where x.ProductName.Contains(str_search) || cat.CategoryName.Contains(str_search) || sup.CompanyName.Contains(str_search)
                              select x

                    ).Count();
            return count;
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public Product Get(int id)
        {
            ProductDBContext dbx = new ProductDBContext();
            Product product = (from x in dbx.Products where x.ProductID == id select x).FirstOrDefault();
            return product;
        }


        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                ProductDBContext dtx = new ProductDBContext();
                // Validate foreign keys
                var categoryExists = await dtx.Categories.AnyAsync(c => c.CategoryID == product.CategoryID);
                var supplierExists = await dtx.Suppliers.AnyAsync(s => s.SupplierID == product.SupplierID);

                if (!categoryExists || !supplierExists)
                {
                    return BadRequest("Invalid CategoryID or SupplierID");
                }

                // Ensure required fields
                if (string.IsNullOrEmpty(product.ProductName))
                {
                    return BadRequest("Product name is required");
                }

                // Add product
                await dtx.Products.AddAsync(product);
                await dtx.SaveChangesAsync();

                return CreatedAtAction(nameof(Get), new { id = product.ProductID }, product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: Update existing product
        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (id != product.ProductID)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                ProductDBContext dtx = new ProductDBContext();
                var existingProduct = await dtx.Products.FindAsync(id);
                if (existingProduct == null)
                {
                    return NotFound($"Product with ID {id} not found");
                }

                // Validate foreign keys
                var categoryExists = await dtx.Categories.AnyAsync(c => c.CategoryID == product.CategoryID);
                var supplierExists = await dtx.Suppliers.AnyAsync(s => s.SupplierID == product.SupplierID);

                if (!categoryExists || !supplierExists)
                {
                    return BadRequest("Invalid CategoryID or SupplierID");
                }

                // Update properties
                existingProduct.ProductName = product.ProductName;
                existingProduct.CategoryID = product.CategoryID;
                existingProduct.SupplierID = product.SupplierID;
                existingProduct.QuantityPerUnit = product.QuantityPerUnit;
                existingProduct.UnitPrice = product.UnitPrice;
                existingProduct.UnitsInStock = product.UnitsInStock;
                existingProduct.UnitsOnOrder = product.UnitsOnOrder;
                existingProduct.ReorderLevel = product.ReorderLevel;
                existingProduct.Discontinued = product.Discontinued;

                dtx.Entry(existingProduct).State = EntityState.Modified;
                await dtx.SaveChangesAsync();

                return Ok(existingProduct);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: Delete product
        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                ProductDBContext dtx = new ProductDBContext();
                var product = await dtx.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found");
                }

                dtx.Products.Remove(product);
                await dtx.SaveChangesAsync();

                return Ok($"Product with ID {id} was deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Helper method to check if product exists
        private bool ProductExists(int id)
        {
            ProductDBContext dtx = new ProductDBContext();
            return dtx.Products.Any(e => e.ProductID == id);
        }


    }
}
