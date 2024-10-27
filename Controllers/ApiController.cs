using Fair_Recruitment_Web_Result.Data;
using Fair_Recruitment_Web_Result.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fair_Recruitment_Web_Result.Controllers
{
    [Route("api/v1")]
    public class ApiController : Controller
    {
        private readonly DbHandler _dbHandler;
        public ApiController(DbHandler dbHandler) 
        {
            _dbHandler = dbHandler;
        }

        public class ExamPaperQrCodesApiResponseDto
        {
            public string? qr_name { get; set; }
            public string qr_decoded { get; set; }
            public string? qr_hashed { get; set; }
        }

        [Authorize]
        [HttpGet("exam_paper_qr_codes", Name = "exam_paper_qr_codes")]
        public async Task<IActionResult> GetAttendanceLogs(string exam_code)
        {
            if (string.IsNullOrWhiteSpace(exam_code))
            {
                return BadRequest();
            }

            var exam = await _dbHandler.GetExamByExamCode(exam_code);

            if (exam == null)
            {
                return NotFound();
            }

            var qr_codes = await _dbHandler.GetExamPaperQrCodesByExamId(exam.id);

            if (qr_codes == null)
            {
                return NotFound();
            }

            var response = qr_codes.Select(x => new ExamPaperQrCodesApiResponseDto { qr_name = x.qr_name, qr_decoded = x.qr_decoded, qr_hashed = x.qr_hashed }).ToList();
            return Ok(response);
        }
    }
}
