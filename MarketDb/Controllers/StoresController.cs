using MarketDb.Abstraction;
using MarketDb.DTO;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace MarketDb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Stores : ControllerBase
    {
        private readonly IStoreRepository _storeRepository;
        public Stores(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        [HttpPost(template: "addstore")]
        public ActionResult AddStore([FromBody] StoreDto storeDto)
        {
            var result = _storeRepository.AddStore(storeDto);
            return Ok(result);
        }
        [HttpGet(template: "getstores")]
        public ActionResult<IEnumerable<StoreDto>> GetStores()
        {
            var list = _storeRepository.GetStores();
            return Ok(list);
        }

        [HttpGet("getstoresexcel")]
        public IActionResult GetStoresExcel()
        {
            var stores = _storeRepository.GetStores().ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Stores");

                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Description";
                worksheet.Cells[1, 4].Value = "Adress";

                for (int i = 0; i < stores.Count(); i++)
                {
                    worksheet.Cells[i + 2, 1].Value = stores[i].Id;
                    worksheet.Cells[i + 2, 2].Value = stores[i].Name;
                    worksheet.Cells[i + 2, 3].Value = stores[i].Description;
                    worksheet.Cells[i + 2, 4].Value = stores[i].Adress;
                }

                var excelBytes = package.GetAsByteArray();

                var fileName = $"stores_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        [HttpDelete("deletestore/{id}")]
        public IActionResult DeleteStore(int id)
        {
            var result = _storeRepository.DeleteStore(id);

            return Ok();
        }
    }
}
