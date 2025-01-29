using MarketDb.Abstraction;
using MarketDb.Data;
using MarketDb.DTO;
using MarketDb.Models;
using MarketDb.Repo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OfficeOpenXml;
using System;

namespace MarketDb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductGroups : ControllerBase
    {
        private IGroupRepository _groupRepository;
        public ProductGroups(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        [HttpPost(template: "addgroup")]
        public ActionResult AddGroup([FromBody] ProductGroupDto productGroupDto)
        {
            var result = _groupRepository.AddGroup(productGroupDto);
            return Ok(result);
        }

        [HttpGet(template: "getgroups")]
        public ActionResult<IEnumerable<ProductGroupDto>> GetGroups()
        {
            var list = _groupRepository.GetGroups();
            return Ok(list);
        }
        [HttpGet("getgroupsexcel")]
        public IActionResult GetStoresExcel()
        {
            var groups = _groupRepository.GetGroups().ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Groups");

                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Description";

                for (int i = 0; i < groups.Count(); i++)
                {
                    worksheet.Cells[i + 2, 1].Value = groups[i].Id;
                    worksheet.Cells[i + 2, 2].Value = groups[i].Name;
                    worksheet.Cells[i + 2, 3].Value = groups[i].Description;
                }

                var excelBytes = package.GetAsByteArray();

                var fileName = $"groups_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        [HttpDelete("deletegroup/{id}")]
        public IActionResult DeleteGroup(int id)
        {
            var result = _groupRepository.DeleteGroup(id);

            return Ok();

        }
    }
}