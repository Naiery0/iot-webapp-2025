using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPortfolioWebApp.Models;

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
        public async Task<IActionResult> Index()
        {
            // _context News
            // SELECT * FROM News.
            return View(await _context.News.ToListAsync()); // 뷰화면에 데이터를 가지고감
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

            return View(news);
        }

        // GET: http://localhost:5234/News/Create GET method로 호출!!
        public IActionResult Create()
        {
            var news = new News
            {
                Writer = "관리자", // 작성자 자동으로 관리자 지정
                PostDate = DateTime.Now, // 작성일 자동으로 현재시간 지정
                ReadCount = 0 // 조회수 자동으로 0 지정
            };
            return View(news);  // View로 데이터를 가져갈게 아무것도 없음
        }

        // POST: News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // <form asp-controller="News" asp-action="Create"> 이 http://localhost:5234/News/Create 포스트메서드 호출
        public async Task<IActionResult> Create([Bind("Id,Title,Description")] News news)
        {
            if (ModelState.IsValid)
            {
                news.Writer = "관리자"; // 작성자 자동으로 관리자 지정
                news.PostDate = DateTime.Now; // 작성일 자동으로 현재시간 지정
                news.ReadCount = 0; // 조회수 자동으로 0 지정

                // INSERT INTO...
                _context.Add(news);
                // COMMIT
                await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description")] News news)
        {
            if (id != news.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 기존값 유지용
                    var existingNews = await _context.News.FindAsync(id);
                    if (existingNews == null)
                    {
                        return NotFound();
                    }

                    existingNews.Title = news.Title; // 제목 업데이트
                    existingNews.Description = news.Description; // 내용 업데이트

                    //// UPDATE News SET ...
                    //_context.Update(news);
                    // COMMIT
                    await _context.SaveChangesAsync();
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
            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }
    }
}
