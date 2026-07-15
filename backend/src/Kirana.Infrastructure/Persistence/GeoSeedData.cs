using Kirana.Domain.Geo;
using Microsoft.EntityFrameworkCore;

namespace Kirana.Infrastructure.Persistence;

/// <summary>
/// Seeds the country → state → city reference data used by the registration
/// cascade. Ships India (all 28 states + 8 UTs) with major cities as a starter
/// set; extend the arrays or import a fuller dataset as needed.
/// </summary>
public static class GeoSeedData
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        if (await db.Countries.AnyAsync(ct)) return;

        var india = new Country { Name = "India", Iso2 = "IN", PhoneCode = "+91" };
        db.Countries.Add(india);

        foreach (var (code, name, cities) in IndiaStates)
        {
            var state = new State { CountryId = india.Id, Name = name, Code = code };
            db.States.Add(state);

            foreach (var cityName in cities)
                db.Cities.Add(new City { StateId = state.Id, Name = cityName });
        }

        await db.SaveChangesAsync(ct);
    }

    // (state code, state name, major cities)
    private static readonly (string Code, string Name, string[] Cities)[] IndiaStates =
    {
        ("AP", "Andhra Pradesh", new[] { "Visakhapatnam", "Vijayawada", "Guntur", "Nellore", "Tirupati" }),
        ("AR", "Arunachal Pradesh", new[] { "Itanagar", "Naharlagun" }),
        ("AS", "Assam", new[] { "Guwahati", "Silchar", "Dibrugarh", "Jorhat" }),
        ("BR", "Bihar", new[] { "Patna", "Gaya", "Bhagalpur", "Muzaffarpur" }),
        ("CG", "Chhattisgarh", new[] { "Raipur", "Bhilai", "Bilaspur", "Durg" }),
        ("GA", "Goa", new[] { "Panaji", "Margao", "Vasco da Gama" }),
        ("GJ", "Gujarat", new[] { "Ahmedabad", "Surat", "Vadodara", "Rajkot", "Gandhinagar" }),
        ("HR", "Haryana", new[] { "Gurugram", "Faridabad", "Panipat", "Ambala", "Hisar" }),
        ("HP", "Himachal Pradesh", new[] { "Shimla", "Solan", "Dharamshala", "Mandi" }),
        ("JH", "Jharkhand", new[] { "Ranchi", "Jamshedpur", "Dhanbad", "Bokaro" }),
        ("KA", "Karnataka", new[] { "Bengaluru", "Mysuru", "Hubballi", "Mangaluru", "Belagavi" }),
        ("KL", "Kerala", new[] { "Thiruvananthapuram", "Kochi", "Kozhikode", "Thrissur" }),
        ("MP", "Madhya Pradesh", new[] { "Bhopal", "Indore", "Jabalpur", "Gwalior", "Ujjain" }),
        ("MH", "Maharashtra", new[] { "Mumbai", "Pune", "Nagpur", "Nashik", "Thane", "Aurangabad" }),
        ("MN", "Manipur", new[] { "Imphal" }),
        ("ML", "Meghalaya", new[] { "Shillong", "Tura" }),
        ("MZ", "Mizoram", new[] { "Aizawl" }),
        ("NL", "Nagaland", new[] { "Kohima", "Dimapur" }),
        ("OD", "Odisha", new[] { "Bhubaneswar", "Cuttack", "Rourkela", "Berhampur" }),
        ("PB", "Punjab", new[] { "Ludhiana", "Amritsar", "Jalandhar", "Patiala", "Mohali" }),
        ("RJ", "Rajasthan", new[] { "Jaipur", "Jodhpur", "Udaipur", "Kota", "Ajmer" }),
        ("SK", "Sikkim", new[] { "Gangtok" }),
        ("TN", "Tamil Nadu", new[] { "Chennai", "Coimbatore", "Madurai", "Tiruchirappalli", "Salem" }),
        ("TS", "Telangana", new[] { "Hyderabad", "Warangal", "Nizamabad", "Karimnagar" }),
        ("TR", "Tripura", new[] { "Agartala" }),
        ("UP", "Uttar Pradesh", new[] { "Lucknow", "Kanpur", "Ghaziabad", "Agra", "Varanasi", "Noida" }),
        ("UK", "Uttarakhand", new[] { "Dehradun", "Haridwar", "Roorkee", "Haldwani" }),
        ("WB", "West Bengal", new[] { "Kolkata", "Howrah", "Durgapur", "Siliguri", "Asansol" }),
        // Union Territories
        ("AN", "Andaman and Nicobar Islands", new[] { "Port Blair" }),
        ("CH", "Chandigarh", new[] { "Chandigarh" }),
        ("DH", "Dadra and Nagar Haveli and Daman and Diu", new[] { "Daman", "Silvassa" }),
        ("DL", "Delhi", new[] { "New Delhi", "Delhi" }),
        ("JK", "Jammu and Kashmir", new[] { "Srinagar", "Jammu" }),
        ("LA", "Ladakh", new[] { "Leh", "Kargil" }),
        ("LD", "Lakshadweep", new[] { "Kavaratti" }),
        ("PY", "Puducherry", new[] { "Puducherry", "Karaikal" })
    };
}
