using Microsoft.AspNetCore.Mvc;
using MarketDb.Abstraction;
using MarketDb.DTO;
using OfficeOpenXml;

namespace MarketDb.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class Products : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        public Products(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpPost(template: "addproduct")]
        public ActionResult AddProduct([FromBody] ProductDto productDto)
        {
            var result = _productRepository.AddProduct(productDto);
            return Ok(result);
        }
        [HttpGet(template: "getproducts")]
        public ActionResult<IEnumerable<ProductDto>> GetProducts()
        {
            var list = _productRepository.GetProducts();
            return Ok(list);
        }
        [HttpGet(template: "getproductsbygroup/{groupid}")]
        public ActionResult<IEnumerable<ProductDto>> GetProductsByGroup(int groupid)
        {
            var list = _productRepository.GetProductsByGroup(groupid);
            return Ok(list);
        }

        [HttpGet(template: "getproductsbystore/{storeid}")]
        public ActionResult<IEnumerable<ProductDto>> GetProductsByStore(int storeid)
        {
            var list = _productRepository.GetProductsByStore(storeid);
            return Ok(list);
        }

        [HttpGet("getproductsexcel")]
        public IActionResult GetProductsExcel()
        {
            var products = _productRepository.GetProducts().ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Products");

                // Добавление 
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Price";
                worksheet.Cells[1, 4].Value = "ProductGroupId";
                worksheet.Cells[1, 5].Value = "Count";
                worksheet.Cells[1, 6].Value = "Description";
                worksheet.Cells[1, 7].Value = "StoreID";

                for (int i = 0; i < products.Count(); i++)
                {
                    worksheet.Cells[i + 2, 1].Value = products[i].Id;
                    worksheet.Cells[i + 2, 2].Value = products[i].Name;
                    worksheet.Cells[i + 2, 3].Value = products[i].Price;
                    worksheet.Cells[i + 2, 4].Value = products[i].ProductGroupId;
                    worksheet.Cells[i + 2, 5].Value = products[i].Count;
                    worksheet.Cells[i + 2, 6].Value = products[i].Description;
                    worksheet.Cells[i + 2, 7].Value = products[i].StoreID;
                }

                var excelBytes = package.GetAsByteArray();

                var fileName = $"products_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        [HttpPut("updateproduct/{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            var result = _productRepository.UpdateProduct(id, updateProductDto);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }


        [HttpDelete("deleteproduct/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var result = _productRepository.DeleteProduct(id);
            return Ok();
        }
    }
}
