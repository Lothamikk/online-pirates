using System.ComponentModel.DataAnnotations;

namespace DarkPortal.Models;

public class UserDataProfile
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string Name { get; set; } = "Аноним";
    public int Age { get; set; }
    public bool HasDog { get; set; }
    public bool HasCar { get; set; }
    public bool UsesVPN { get; set; }
    public bool HasSmartHome { get; set; }

    public AppUser? User { get; set; }

    public double EstimatedPricePerAd
    {
        get
        {
            double price = 2.0;
            if (Age >= 18 && Age <= 25) price += 8.0;
            if (Age >= 26 && Age <= 40) price += 5.0;
            if (HasDog) price += 3.0;
            if (HasCar) price += 4.5;
            if (HasSmartHome) price += 6.0;
            if (UsesVPN) price -= 2.0;
            return Math.Max(price, 0.5);
        }
    }
}