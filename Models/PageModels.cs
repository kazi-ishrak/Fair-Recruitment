using System.ComponentModel.DataAnnotations;

namespace Fair_Recruitment_Web_Result.Models
{
    public class PageModels
    {
        public class ExamListWithTypeDTO
        {
            public long id { get; set; }
            public string exam_code {  get; set; } 
            public string office_name { get; set; }
            public string post_name { get; set; }
            public long exam_type_id { get; set; }
            public string exam_date { get; set; }
            public bool status { get; set; }     
            public string exam_type_name { get; set; }
        }
        public class ExamResultDTO
        {
            public long id { get; set; }
            public long exam_id { get; set; }
            public string roll { get; set; }
            public string name { get; set; }
            public int? position { get; set; }
            public decimal? marks { get; set; }
            public string? enrolled_image { get; set; }
            public string? room_captured_image { get; set; }
            public decimal? room_similarity { get; set; }
            public bool? room_decision { get; set; }
            public DateTime? room_timestamp { get; set; }
            public string? gate_captured_image { get; set; }
            public decimal? gate_similarity { get; set; }
            public bool? gate_decision { get; set; }
            public DateTime? gate_timestamp { get; set; }

        }
    }
}
