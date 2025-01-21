using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WeddingSite.Pages
{
    public class RegisterModel(WeddingContext db) : PageModel
    {
        [BindProperty]
        public string InviteCode { get; set; }

        public void OnGet()
        {
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ModelState.AddModelError("InviteCode", TempData["ErrorMessage"].ToString());
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(InviteCode == null || InviteCode.Length < 6)
            {
                ModelState.AddModelError("InviteCode", "Koden måste vara minst 6 tecken lång");
                return Page();
            }

            var guest = await db.Guests.FirstOrDefaultAsync(g => g.InviteCode == InviteCode.ToUpper().Trim());
            if (guest == null)
            {
                ModelState.AddModelError("InviteCode", "Koden är inte giltig");
                return Page();
            }

            if(guest.FirstLogin == null)
            {
                guest.FirstLogin = DateTimeOffset.UtcNow;
            }

            guest.HasLoggedIn = true;
            guest.LastLogin = DateTimeOffset.UtcNow;

            await db.SaveChangesAsync();

            HttpContext.Session.Set("guest_loggedin", guest);

            return RedirectToPage("/Profil");
        }
    }
}
