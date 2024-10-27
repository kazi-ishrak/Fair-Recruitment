using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fair_Recruitment_Web_Result.Models
{
    public class Database
    {
        [Table(name: "exams")]
        public class Exam
        {
            [Key]
            public long id { get; set; }
            public string exam_code { get; set; }
            public string office_name { get; set; }
            public string post_name { get; set; }
            public long exam_type_id { get; set; }
            public string exam_date { get; set; }
            public bool status { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
        }

        [Table(name:"exam_results")]
        public class ExamResult
        {
            [Key]
            public long id { get; set; }
            public long exam_code { get; set; }
            public string roll { get; set; }
            public string? name { get; set; }
            public string? evaluator_user_id { get; set; }
            public int? position { get; set; }
            public decimal? marks { get; set; }
            public byte[]? enrolled_image { get; set; }
            public string? room_user_id { get; set; }
            public byte[]? room_captured_image { get; set; }
            public decimal? room_similarity { get; set; }
            public bool? room_decision { get; set; }
            public DateTime? room_timestamp { get; set; }
            public string? gate_user_id { get; set; }
            public byte[]? gate_captured_image { get; set; }
            public decimal? gate_similarity { get; set; }
            public bool? gate_decision { get; set; }
            public DateTime? gate_timestamp { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public DateTime? deleted_at { get; set; }
        }

        [Table(name:"exam_types")]
        public class ExamType
        {
            [Key]
            public long  id { get; set; }
            public string  name { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
        }

        [Table(name: "users")]
        public class User
        {
            [Key]
            public long  id { get; set; }
            public string? user_id { get; set; }
            public string name { get; set; }
            public string  email { get; set; }
            public string  password { get; set; }
            public bool status { get; set; }
            public int role { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public DateTime? deleted_at { get; set; }
        }
        [Table(name: "exam_paper_qr_codes")]
        public class ExamPaperQrCode
        {
            [Key]
            public long id { get; set; }
            public long exam_id { get; set; }
            public string? qr_name { get; set; }
            public string qr_decoded { get; set; }
            public string? qr_hashed { get; set; }
            public bool status { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
        }

        [Table(name: "login_attempts")]
        public class LoginAttempt
        {
            [Key]
            public long id { get; set; }
            public long request_time { get; set; }
            public string? email { get; set; }
            public string? password { get; set; }
            public string? remarks { get; set; }
            public bool status { get; set; }
            public DateTime created_at { get; set; }
            public DateTime? deleted_at { get; set; }
        }
    }
}
