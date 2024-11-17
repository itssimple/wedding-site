using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WeddingSite.Pages
{
    public class ProfileModel(WeddingContext db) : PageModel
    {
        [BindProperty]
        public Guest? Guest { get; set; }

        [BindProperty]
        public bool IsVegan { get; set; }

        [BindProperty]
        public string SoloDietary {  get; set; }

        public async Task<IActionResult> OnGet()
        {
            var guest = HttpContext.Session.Get<Guest>("guest_loggedin");

            if (guest == null)
            {
                TempData["ErrorMessage"] = "Du måste fylla i din inbjudningskod för att se din profil";
                return RedirectToPage("/Register");
            }

            Guest = await db.Guests.FirstOrDefaultAsync(g => g.GuestId == guest.GuestId);

            if (Guest == null)
            {
                TempData["ErrorMessage"] = "Du måste fylla i din inbjudningskod för att se din profil";
                return RedirectToPage("/Register");
            }

            if(string.IsNullOrWhiteSpace(Guest.PlusOne))
            {
                if(Guest.RSVPData?.VegetarianCount > 0)
                {
                    SoloDietary = "vegetarian";
                }
                else if(Guest.RSVPData?.VeganCount > 0)
                {
                    SoloDietary = "vegan";
                }
                else
                {
                    SoloDietary = "other";
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var guest = HttpContext.Session.Get<Guest>("guest_loggedin");

            if (guest == null)
            {
                TempData["ErrorMessage"] = "Du måste fylla i din inbjudningskod för att se din profil";
                return RedirectToPage("/Register");
            }

            // To get updated data, since we don't update the session after login
            guest = await db.Guests.FirstAsync(g => g.GuestId == guest.GuestId);

            if(Guest?.RSVPData != null)
            {
                if(guest.RSVPData == null)
                {
                    guest.RSVPData = Guest.RSVPData;
                    guest.RSVPData.GuestId = guest.GuestId;

                    switch(SoloDietary)
                    {
                        case "vegan":
                            guest.RSVPData.VeganCount = 1;
                            guest.RSVPData.VegetarianCount = 0;
                            break;
                        case "vegetarian":
                            guest.RSVPData.VegetarianCount = 1;
                            guest.RSVPData.VeganCount = 0;
                            break;
                        case "other":
                            guest.RSVPData.VegetarianCount = 0;
                            guest.RSVPData.VeganCount = 0;
                            break;
                    }
                }
                else
                {
                    guest.RSVPData.Attending = Guest.RSVPData.Attending;
                    guest.RSVPData.PlusOneAttending = Guest.RSVPData.PlusOneAttending;
                    guest.RSVPData.VeganCount = Guest.RSVPData.VeganCount;
                    guest.RSVPData.VegetarianCount = Guest.RSVPData.VegetarianCount;
                    guest.RSVPData.NumberOfGuests = Guest.RSVPData.NumberOfGuests;
                    guest.RSVPData.DietaryRequirements = Guest.RSVPData.DietaryRequirements;
                    guest.RSVPData.Message = Guest.RSVPData.Message;
                }
            }

            db.Guests.Update(guest);
            await db.SaveChangesAsync();

            Guest = guest;

            return Page();
        }
    }
}
