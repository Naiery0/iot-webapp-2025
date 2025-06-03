using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DbFirstWebApp.Models;

public partial class Bookstbl
{
    [DisplayName("순번")]
    public int Idx { get; set; }

    [DisplayName("저자")]

    public string? Author { get; set; }

    [DisplayName("장르")]

    public string Division { get; set; } = null!;

    [DisplayName("책제목")]

    public string? Names { get; set; }

    [DisplayName("출판일")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = false)]

    public DateTime? ReleaseDate { get; set; }

    [DisplayName("ISBN")]

    public string? Isbn { get; set; }

    [DisplayName("책가격")]
    [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = false)]
    public decimal? Price { get; set; }

    // 부모로 Divtbl을 가지고 있다
    // EF Core의 탐색 속성(Navigation Property)
    // = null!로 정의되어 있어서 EF Core는 내부적으로 필수(NOT NULL) 관계라고 간주
    // HACK : 자동 생성 후 변경
    //public virtual Divtbl DivisionNavigation { get; set; } = null!;
    [DisplayName("장르")]
    public virtual Divtbl? DivisionNavigation { get; set; }

    // 자식으로 RentalTbl을 가지고 있다
    public virtual ICollection<Rentaltbl> Rentaltbls { get; set; } = new List<Rentaltbl>();
}
