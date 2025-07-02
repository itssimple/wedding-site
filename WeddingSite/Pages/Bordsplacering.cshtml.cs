using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WeddingSite.Pages
{
    public class BordsplaceringModel(IConfiguration configuration, WeddingContext db) : PageModel
    {
        [BindProperty]
        public string SecretCode { get; set; }

        public List<SeatingInfoBearer> SeatingList { get; set; } = [];

        public async Task<IActionResult> OnGetAsync()
        {
            var viewTableSession = HttpContext.Session.Get<bool>("view_table");

            if (!viewTableSession)
            {
                return Page();
            }

            SeatingList = [.. GetSeatingInfo()];

            return Page();
        }

        public class SeatingInfoBearer
        {
            public int TableNumber { get; set; }
            public int SeatNumber { get; set; }
            public string PersonName { get; set; }
        }

        public IEnumerable<SeatingInfoBearer> GetSeatingInfo()
        {
            using var dbConn = db.Database.GetDbConnection();
            if (dbConn.State != System.Data.ConnectionState.Open)
            {
                dbConn.Open();
            }

            var cmd = dbConn.CreateCommand();
            cmd.CommandText = """
                SELECT si.TableNumber, si.SeatNumber,
                CASE WHEN si.PlusOne = 0 THEN CONCAT(g.FirstName, ' ', g.LastName) ELSE g.PlusOne END GuestName
                FROM SeatingInfo si
                INNER JOIN Guests g ON g.GuestId = si.GuestId
                INNER JOIN RSVPs r ON g.GuestId = r.GuestId
                ORDER BY si.TableNumber, si.SeatNumber
                """;

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                yield return new SeatingInfoBearer
                {
                    TableNumber = reader.GetInt32(0),
                    SeatNumber = reader.GetInt32(1),
                    PersonName = reader.GetString(2)
                };
            }

            yield return new SeatingInfoBearer
            {
                TableNumber = 0,
                SeatNumber = 3,
                PersonName = "Chris Åkerfeldt Wendel"
            };

            yield return new SeatingInfoBearer
            {
                TableNumber = 0,
                SeatNumber = 4,
                PersonName = "Sol Åkerfeldt Wendel"
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(SecretCode) || SecretCode.Length < 6)
            {
                ModelState.AddModelError("SecretCode", "Koden måste vara minst 6 tecken lång");
                return Page();
            }

            if (SecretCode != configuration["SeatingList:Code"])
            {
                ModelState.AddModelError("SecretCode", "Koden är inte giltig");
                return Page();
            }

            HttpContext.Session.Set("view_table", true);

            SeatingList = [.. GetSeatingInfo()];

            return Page();
        }
    }
}
