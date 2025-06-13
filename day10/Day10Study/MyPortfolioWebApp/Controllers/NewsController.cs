using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPortfolioWebApp.Models;
using System.Diagnostics;

namespace MyPortfolioWebApp.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: News . http://locahost:5234/News/Index 요청했음
        // param int page는 처음 게시판은 무조건 1페이지부터 시작
        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            // 뷰쪽에 보내고 싶은 데이터
            //ViewData["Key"] //ViewBag.Title //TempData["Key"]
            ViewData["Title"] = "서버에서 변경가능";
            // _context News
            // SELECT * FROM News.
            //return View(await _context.News
            //                       .OrderByDescending(o => o.PostDate)
            //                       .ToListAsync()); // 뷰화면에 데이터를 가지고감
            // 쿼리로 처리가능
            //var news = await _context.News.FromSql($@"SELECT Id, Writer, Title, Description, PostDate, ReadCount
            //                                            FROM News")
            //                        .OrderByDescending(o => o.PostDate)
            //                        .ToListAsync();
            //return View(news);

            // 최종단계
            // 페이지 개수
            //var totalCount = _context.News.FromSql($@"SELECT * FROM News WHERE Title LIKE '%{search}%'").Count(); // 현재 문제발생!!
            var totalCount = _context.News.Where(n => EF.Functions.Like(n.Title, $"%{search}%")).Count(); // HACK! 위 구문이 오류발생
            var countList = 10; // 한페이지에 기본 뉴스갯수 10개
            var totalPage = totalCount / countList; // 한페이지당 개수로 나누면 전체페이지 수
            // HACK : 게시판페이지 중요로직. 남는 데이터도 한페이지를 차지해야 함
            if (totalCount % countList > 0) totalPage++;  // 남은 게시글이 있으면 페이지수 증가            
            if (totalPage < page) page = totalPage;
            // 마지막 페이지 구하기
            var countPage = 10; // 페이지를 표시할 최대페이지개수, 10개
            var startPage = ((page - 1) / countPage) * countPage + 1;
            var endPage = startPage + countPage - 1;
            // HACK : 나타낼 페이수가 10이 안되면 페이지수 조정.
            // 마지막페이지까지 글이 12개면  1, 2 페이지만 표시
            if (totalPage < endPage) endPage = totalPage;

            // 저장프로시저에 보낼 rowNum값, 시작번호랑 끝번호
            var startCount = ((page - 1) * countPage) + 1; // 2페이지의 경우 11
            var endCount = startCount + countList - 1; // 2페이지의 경우 20

            // View로 넘기는 데이터, 페이징 숫자컨트롤 사용
            ViewBag.StartPage = startPage;
            ViewBag.EndPage = endPage;
            ViewBag.Page = page;
            ViewBag.TotalPage = totalPage;
            ViewBag.Search = search; // 검색어

            // 저장프로시저 호출
            var news = await _context.News.FromSql($"CALL New_PagingBoard({startCount}, {endCount}, {search})").ToListAsync();
            return View(news);
        }

        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // SELECT * FROM News WHERE id = @id
            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            // 조회수 증가 로직
            news.ReadCount++;
            _context.News.Update(news);
            await _context.SaveChangesAsync();

            return View(news);
        }

        // GET: http://localhost:5234/News/Create GET method로 호출!!
        //[HttpGet] 가 default
        public IActionResult Create()
        {
            var news = new News
            {
                Writer = "관리자",
                PostDate = DateTime.Now,
                ReadCount = 0,
            };
            return View(news);  // View로 데이터를 가져갈게 아무것도 없음
        }

        // POST: News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // <form asp-controller="News" asp-action="Create"> 이 http://localhost:5234/News/Create 포스트메서드 호출
        // News 모델에 저장하는 것은 파일경로
        // IFormFile? UploadFile에는 파일자체 바이너리 데이터
        public async Task<IActionResult> Create([Bind("Id,Title,Description")] News news, IFormFile? UploadFile)
        {
            const long MaxFileSize = 10 * 1024 * 1024;

            if (ModelState.IsValid)
            {
                // 파일이 존재하면
                if (UploadFile != null && UploadFile.Length > 0)
                {
                    // 서버에 파일저장, 모델에 파일경로 저장
                    Debug.WriteLine(UploadFile.Length);

                    if (UploadFile.Length > MaxFileSize)
                    {
                        ModelState.AddModelError("UploadFile", "파일크기는 10MB 이하로 제한합니다.");
                        return View(news);
                    }

                    string upFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload");
                    Directory.CreateDirectory(upFolder); // 폴더가 없으면 생성

                    // example.jpg파일이 여러번올라가면 파일이 겹쳐짐
                    // 파일명을 변경
                    // Guid.NewGuid() = 랜덤아이디 생성
                    // Path.GetExetension() = 파일의 확장자만 가져옴 .jpg
                    string newFileName = Guid.NewGuid() + Path.GetExtension(UploadFile.FileName); // bcc5206a-1718-4885-929c-8c9060ba9374.jpg
                    string filePath = Path.Combine(upFolder, newFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await UploadFile.CopyToAsync(stream); // 파일저장
                    }

                    // 모델에 파일명 할당
                    news.UploadFile = newFileName;
                }
                news.Writer = "관리자"; // 작성자는 자동으로 관리자
                news.PostDate = DateTime.Now; // 게시일자는 현재
                news.ReadCount = 0;

                // INSERT INTO...
                _context.Add(news);
                // COMMIT
                await _context.SaveChangesAsync();

                TempData["success"] = "뉴스 저장 성공!";
                return RedirectToAction(nameof(Index));
            }
            return View(news);
        }

        // GET: News/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // SELECT * FROM News WHERE id = @id
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description")] News news, IFormFile? NewFile)
        {
            if (id != news.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 방식2 원본을 찾아서 수정해주는 방식
                    var existingNews = await _context.News.FindAsync(id);
                    if (existingNews == null)
                    {
                        return NotFound();
                    }

                    existingNews.Title = news.Title;
                    existingNews.Description = news.Description;

                    // 파일이 변경되었으면
                    if (NewFile != null && NewFile.Length > 0)
                    {
                        string upFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload");
                        Directory.CreateDirectory(upFolder); // 폴더가 없으면 생성

                        // example.jpg파일이 여러번올라가면 파일이 겹쳐짐
                        // 파일명을 변경
                        // Guid.NewGuid() = 랜덤아이디 생성
                        // Path.GetExetension() = 파일의 확장자만 가져옴 .jpg
                        string newFileName = Guid.NewGuid() + Path.GetExtension(NewFile.FileName); // bcc5206a-1718-4885-929c-8c9060ba9374.jpg
                        string filePath = Path.Combine(upFolder, newFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await NewFile.CopyToAsync(stream); // 파일저장
                        }

                        // 이전뉴스 파일명 할당
                        existingNews.UploadFile = newFileName;
                    }

                    // UPDATE News SET ...
                    //_context.Update(news); // 방식1 ID가 같은 새글을 UPDATE하면 수정                    
                    // COMMIT
                    await _context.SaveChangesAsync();
                    TempData["success"] = "뉴스 수정 성공!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(news);
        }

        // GET: News/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                // DELETE FROM News WHERE id = @id
                _context.News.Remove(news);
            }
            // COMMIT
            await _context.SaveChangesAsync();

            TempData["success"] = "뉴스 삭제 성공!";
            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }
    }
}