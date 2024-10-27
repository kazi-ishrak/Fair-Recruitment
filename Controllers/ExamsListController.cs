using Fair_Recruitment_Web_Result.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq.Dynamic.Core;
namespace Fair_Recruitment_Web_Result.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ExamsListController : ControllerBase
    {
        private readonly DbHandler _dbHandler;
        public ExamsListController(DbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        [HttpPost]
        public async Task<IActionResult> GetExamList()
        {
            string draw = Request.Form["draw"];
            int start = Convert.ToInt32(Request.Form["start"]);
            int length = Convert.ToInt32(Request.Form["length"]);
            string search = Request.Form["search[value]"];
            string sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
            string sortDirection = Request.Form["order[0][dir]"];
            string id = Request.Form["id"].ToString();

            var ExamList = await _dbHandler.GetExamList(id);
            int recordsTotal = ExamList.Count();

            if (!string.IsNullOrEmpty(search))
            {
                ExamList = ExamList.Where(x => x.office_name != null && x.office_name.ToLower().Contains(search.ToLower()) ||
                (x.post_name != null && x.post_name.ToLower().Contains(search.ToLower())) ||
                 (x.exam_type_name != null && x.exam_type_name.ToLower().Contains(search.ToLower())) 
                
                ).ToList();
            }
            int recordsFiltered = ExamList.Count();

            //Sorting
            if (!string.IsNullOrEmpty(sortColumn))
            {
                ExamList = ExamList.AsQueryable().OrderBy(sortColumn + " " + sortDirection).ToList();
            }
            //Paging
            ExamList = ExamList.Skip(start).Take(length).ToList();
            return Ok(new { draw, recordsTotal, recordsFiltered, data = ExamList });

        }
    }

}
