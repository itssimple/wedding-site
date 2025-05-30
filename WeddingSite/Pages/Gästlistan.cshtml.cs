using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WeddingSite.Pages
{
    public class GästlistanModel(IConfiguration configuration, WeddingContext db) : PageModel
    {
        public List<GuestRsvpRecord> GuestsWithRsvp { get; set; } = new();

        public IActionResult OnGet()
        {
            var key = configuration["GuestList:Key"]!;
            if (!Request.Query.ContainsKey(key))
            {
                return NotFound();
            }

            var value = configuration["GuestList:Pass"];

            if (Request.Query[key] != value)
            {
                return NotFound();
            }

            var guestsWithRsvp = (from g in db.Guests
                                  join r in db.RSVPs on g.GuestId equals r.GuestId
                                  orderby r.RSVPDate descending
                                  select new GuestRsvpRecord(
                                      g.FirstName,
                                      g.LastName,
                                      g.PlusOne,
                                      r.Attending,
                                      r.PlusOneAttending,
                                      r.DietaryRequirements,
                                      r.DietaryOptions,
                                      r.Message,
                                      r.OwnTransport,
                                      r.RSVPDate
                                  )).ToList();

            GuestsWithRsvp = guestsWithRsvp;

            return Page();
        }
    }

    public record GuestRsvpRecord(string FirstName, string LastName, string? PlusOne, bool? Attending, bool? PlusOneAttending, string? DietaryRequirements, string? DietaryOptions, string? Message, bool OwnTransport, DateTimeOffset? RSVPDate);
}
