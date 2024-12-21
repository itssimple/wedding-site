using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace WeddingSite.Pages
{
    public class ProfileModel(WeddingContext db) : PageModel
    {
        [BindProperty]
        public Guest? Guest { get; set; }

        public List<string> SelectedSoloOptions { get; set; } = [];
        public List<string> SelectedPlusOptions { get; set; } = [];
        public List<(string text, string value)> AvailableSoloOptions { get; set; } = [
            ("Vegan", "vegan"),
            ("Gluten (allergi eller intolerans)", "gluten"),
            ("Laktos (allergi eller intolerans)", "lactose"),
            ("Nötallergi", "nuts"),
            ("Soja (allergi eller intolerans)", "soy"),
            ("Baljväxter (t.ex. ärtor, linser)", "baljvaxt"),
            ("Citrus (allergi eller intolerans)", "citrus"),
            ("Annat (ange i textfältet ovan)", "other")
        ];

        public List<(string text, string value)> AvailablePlusOptions { get; set; } = [
            ("Vegan", "plus_vegan"),
            ("Gluten (allergi eller intolerans)", "plus_gluten"),
            ("Laktos (allergi eller intolerans)", "plus_lactose"),
            ("Nötallergi", "plus_nuts"),
            ("Soja (allergi eller intolerans)", "plus_soy"),
            ("Baljväxter (t.ex. ärtor, linser)", "plus_baljvaxt"),
            ("Citrus (allergi eller intolerans)", "plus_citrus"),
            ("Annat (ange i textfältet ovan)", "plus_other")
        ];

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

            var dietaryOptions = Guest.RSVPData?.DietaryOptions?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [];

            foreach (var option in dietaryOptions)
            {
                if (AvailableSoloOptions.Any(o => o.value == option))
                {
                    SelectedSoloOptions.Add(option);
                }
                else if (AvailablePlusOptions.Any(o => o.value == option))
                {
                    SelectedPlusOptions.Add(option);
                }
            }

            return Page();
        }

        public string FilterValidDiets(StringValues value)
        {
            var validDiets = new List<string> {
                "vegan", "gluten", "lactose", "nuts", "soy", "baljvaxt", "citrus", "other",
                "plus_vegan", "plus_gluten", "plus_lactose", "plus_nuts", "plus_soy", "plus_baljvaxt", "plus_citrus", "plus_other"
            };

            var selectedDiets = new HashSet<string>();
            foreach (var item in validDiets)
            {
                if (value.Contains(item))
                {
                    selectedDiets.Add(item);
                }
            }

            return string.Join(",", selectedDiets);
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

            if (Guest?.RSVPData != null)
            {
                if (guest.RSVPData == null)
                {
                    guest.RSVPData = Guest.RSVPData;
                    guest.RSVPData.GuestId = guest.GuestId;
                }
                else
                {
                    guest.RSVPData.Attending = Guest.RSVPData.Attending;
                    guest.RSVPData.PlusOneAttending = Guest.RSVPData.PlusOneAttending;
                    guest.RSVPData.NumberOfGuests = Guest.RSVPData.NumberOfGuests;
                    guest.RSVPData.DietaryRequirements = Guest.RSVPData.DietaryRequirements;
                    guest.RSVPData.Message = Guest.RSVPData.Message;
                }

                SelectedSoloOptions = [.. FilterValidDiets(Request.Form["SelectedSoloOptions"]).Split(',', StringSplitOptions.RemoveEmptyEntries)];
                SelectedPlusOptions = [.. FilterValidDiets(Request.Form["SelectedPlusOptions"]).Split(',', StringSplitOptions.RemoveEmptyEntries)];

                HashSet<string> selectedOptions = [.. SelectedSoloOptions, .. SelectedPlusOptions];

                guest.RSVPData.DietaryOptions = string.Join(",", selectedOptions);
            }

            db.Guests.Update(guest);
            await db.SaveChangesAsync();

            Guest = guest;

            return Page();
        }
    }
}
