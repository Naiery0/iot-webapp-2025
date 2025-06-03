using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyPortfolioWebApp.Models
{
    // 실제 DB의 News 테이블로 만들어짐
    public class News
    {
        [Key] // Id == Primary Key
        [DisplayName("번호")]
        public int Id { get; set; }

        [DisplayName("작성자")]
        [BindNever]  // ★ 폼 입력 무시, 서버에서 설정
        public string? Writer { get; set; }

        [Required] // Not Null
        [DisplayName("뉴스제목")]
        public string Title { get; set; } = null!;

        [DisplayName("뉴스내용")]
        public string Description { get; set; } = null!;

        [DisplayName("작성일")]
        [BindNever]  // ★ 폼 입력 무시, 서버에서 설정
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = false)]
        public DateTime? PostDate { get; set; }

        [DisplayName("조회수")]
        [BindNever]  // ★ 폼 입력 무시, 서버에서 설정
        public int ReadCount { get; set; }
    }
}
