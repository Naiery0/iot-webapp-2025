using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DbFirstWebApp.Models;

namespace DbFirstWebApp.Controllers
{
    public class BookController : Controller
    {
        private readonly BookrentalshopContext _context;

        public BookController(BookrentalshopContext context)
        {
            _context = context;
        }

        // GET: Book
        public async Task<IActionResult> Index()
        {
            var bookrentalshopContext = _context.Bookstbls.Include(b => b.DivisionNavigation)
                                                    .OrderBy(b => b.Idx);
            return View(await bookrentalshopContext.ToListAsync());
        }

        // GET: Book/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookstbl = await _context.Bookstbls
                .Include(b => b.DivisionNavigation)
                .FirstOrDefaultAsync(m => m.Idx == id);
            if (bookstbl == null)
            {
                return NotFound();
            }

            return View(bookstbl);
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            // HACK : 자동 생성 후 변경
            //ViewData["Division"] = new SelectList(_context.Divtbls, "Division", "Division");
            ViewData["Division"] = new SelectList(_context.Divtbls, "Division", "Names");
            return View();
        }

        // POST: Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idx,Author,Division,Names,ReleaseDate,Isbn,Price")] Bookstbl bookstbl)
        {
            ModelState.Remove("DivisionNavigation");
            if (ModelState.IsValid)
            {
                _context.Add(bookstbl);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // HACK : 자동 생성 후 확인
            // 오류 확인시 사용
            /*if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"Model error on {kv.Key}: {error.ErrorMessage}");
                    }
                }
            }*/
            ViewData["Division"] = new SelectList(_context.Divtbls, "Division", "Division", bookstbl.Division);
            return View(bookstbl);
        }

        // GET: Book/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookstbl = await _context.Bookstbls.FindAsync(id);
            if (bookstbl == null)
            {
                return NotFound();
            }
            // HACK : 자동 생성 후 변경
            //ViewData["Division"] = new SelectList(_context.Divtbls, "Division", "Division", bookstbl.Division);
            ViewData["Division"] = new SelectList(_context.Divtbls, "Division", "Names", bookstbl.Division);

            return View(bookstbl);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idx,Author,Division,Names,ReleaseDate,Isbn,Price")] Bookstbl bookstbl)
        {
            if (id != bookstbl.Idx)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookstbl);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookstblExists(bookstbl.Idx))
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
            // HACK : 자동 생성 후 변경
            //ViewData["Division"] = new SelectList(_context.Divtbls, "Division", "Division", bookstbl.Division);
            ViewData["Division"] = new SelectList(_context.Divtbls, "Division", "Names", bookstbl.Division);

            return View(bookstbl);
        }

        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookstbl = await _context.Bookstbls
                .Include(b => b.DivisionNavigation)
                .FirstOrDefaultAsync(m => m.Idx == id);
            if (bookstbl == null)
            {
                return NotFound();
            }

            return View(bookstbl);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookstbl = await _context.Bookstbls.FindAsync(id);
            if (bookstbl != null)
            {
                _context.Bookstbls.Remove(bookstbl);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookstblExists(int id)
        {
            return _context.Bookstbls.Any(e => e.Idx == id);
        }
    }
}
