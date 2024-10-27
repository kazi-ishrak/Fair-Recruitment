
using Fair_Recruitment_Web_Result.Data;
using Microsoft.EntityFrameworkCore;
using static Fair_Recruitment_Web_Result.Models.Database;
using static Fair_Recruitment_Web_Result.Models.PageModels;
namespace Fair_Recruitment_Web_Result.Handlers
{
    public class DbHandler
    {
        private readonly ApplicationDbContext _db;
        public DbHandler(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<List<ExamListWithTypeDTO>?> GetExamList(string id)
        {
            try
            {
               var Response= await _db.Exams.Where(x => x.status == true).
               Join(_db.ExamTypes, e => e.exam_type_id, t => t.id, (e, t) => new ExamListWithTypeDTO
               {
                   id = e.id,
                   exam_code = e.exam_code,
                   office_name = e.office_name,
                   post_name = e.post_name,
                   exam_type_id = e.exam_type_id,
                   exam_date = e.exam_date,
                   status = e.status,
                   exam_type_name = t.name
               }).
               ToListAsync();
                List<ExamListWithTypeDTO> ExamList = new List<ExamListWithTypeDTO>();
                ExamList=Response;
                if (!string.IsNullOrWhiteSpace(id))
                {
                     ExamList =  Response.Where(x => x.exam_code == id).ToList();
                }
                return ExamList;

            }
            catch (Exception ex)
            {
                LogHandler.WriteErrorLog(ex);
                return null;
            }

        }

        public async Task<List<ExamResultDTO>?> GetExamResultByExamId(string ExamId)
        {
            try
            {
                var Result = await _db.ExamResults.Where(x => x.exam_code == long.Parse(ExamId))
                    .Select(e=> new ExamResultDTO
                    {
                        id = e.id,
                        exam_id = e.exam_code,
                        position = e.position,
                        roll = e.roll,
                        name = e.name ?? "",
                        marks = e.marks,
                        room_similarity = e.room_similarity,
                        room_decision = e.room_decision,
                        room_timestamp = e.room_timestamp,
                        enrolled_image = e.enrolled_image != null ? Convert.ToBase64String(e.enrolled_image) : null,
                        room_captured_image = e.room_captured_image != null ? Convert.ToBase64String(e.room_captured_image) : null,
                        gate_similarity = e.gate_similarity,
                        gate_decision = e.gate_decision,
                        gate_timestamp = e.gate_timestamp,
                        gate_captured_image = e.gate_captured_image != null ? Convert.ToBase64String(e.gate_captured_image) : null
                    })
                    .ToListAsync();
                return Result;
            }
            catch (Exception ex)
            {

                LogHandler.WriteErrorLog(ex);
                return null;
            }
        }

        public async Task<User?> GetUserByEmailOrUserId(string emailOrUserId)
        {
            try
            {
                var user = await _db.Users.Where(x => x.email == emailOrUserId && x.status == true).FirstOrDefaultAsync();
                return user;
            }
            catch (Exception ex)
            {
                LogHandler.WriteErrorLog(ex);
                return null;
            }
        }
        public async Task<Exam?> GetExamByExamCode(string exam_code)
        {
            try
            {
                var exam = await _db.Exams.Where(x => x.exam_code == exam_code && x.status == true).FirstOrDefaultAsync();
                return exam;
            }
            catch (Exception ex)
            {
                LogHandler.WriteErrorLog(ex);
                return null;
            }
        }

        public async Task<List<ExamPaperQrCode>?> GetExamPaperQrCodesByExamId(long exam_id)
        {
            try
            {
                var qr_codes = await _db.ExamPaperQrCodes.Where(x => x.exam_id == exam_id && x.status == true).ToListAsync();
                return qr_codes;
            }
            catch (Exception ex)
            {
                LogHandler.WriteErrorLog(ex);
                return null;
            }
        }
    }
}
