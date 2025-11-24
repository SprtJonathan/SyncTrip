using Microsoft.EntityFrameworkCore;
using SyncTrip.Core.Entities;

namespace SyncTrip.Infrastructure.Persistence.Seed;

/// <summary>
/// Seed data pour les marques de véhicules.
/// Contient les marques principales pour motos et voitures.
/// </summary>
public static class BrandSeed
{
    /// <summary>
    /// Applique le seed data des marques dans le ModelBuilder.
    /// </summary>
    public static void SeedBrands(this ModelBuilder modelBuilder)
    {
        var brands = new List<Brand>
        {
            // Marques de motos
            Brand.Create(1, "Yamaha", "https://example.com/logos/yamaha.png"),
            Brand.Create(2, "Honda", "https://example.com/logos/honda.png"),
            Brand.Create(3, "Kawasaki", "https://example.com/logos/kawasaki.png"),
            Brand.Create(4, "Suzuki", "https://example.com/logos/suzuki.png"),
            Brand.Create(5, "BMW Motorrad", "https://example.com/logos/bmw-motorrad.png"),
            Brand.Create(6, "Ducati", "https://example.com/logos/ducati.png"),
            Brand.Create(7, "KTM", "https://example.com/logos/ktm.png"),
            Brand.Create(8, "Harley-Davidson", "https://example.com/logos/harley-davidson.png"),
            Brand.Create(9, "Triumph", "https://example.com/logos/triumph.png"),
            Brand.Create(10, "Aprilia", "https://example.com/logos/aprilia.png"),

            // Marques de voitures françaises
            Brand.Create(11, "Renault", "https://example.com/logos/renault.png"),
            Brand.Create(12, "Peugeot", "https://example.com/logos/peugeot.png"),
            Brand.Create(13, "Citroën", "https://example.com/logos/citroen.png"),
            Brand.Create(14, "DS Automobiles", "https://example.com/logos/ds.png"),
            Brand.Create(15, "Alpine", "https://example.com/logos/alpine.png"),

            // Marques de voitures allemandes
            Brand.Create(16, "BMW", "https://example.com/logos/bmw.png"),
            Brand.Create(17, "Mercedes-Benz", "https://example.com/logos/mercedes.png"),
            Brand.Create(18, "Audi", "https://example.com/logos/audi.png"),
            Brand.Create(19, "Volkswagen", "https://example.com/logos/volkswagen.png"),
            Brand.Create(20, "Porsche", "https://example.com/logos/porsche.png"),

            // Marques de voitures italiennes
            Brand.Create(21, "Ferrari", "https://example.com/logos/ferrari.png"),
            Brand.Create(22, "Lamborghini", "https://example.com/logos/lamborghini.png"),
            Brand.Create(23, "Fiat", "https://example.com/logos/fiat.png"),
            Brand.Create(24, "Alfa Romeo", "https://example.com/logos/alfa-romeo.png"),

            // Marques de voitures japonaises
            Brand.Create(25, "Toyota", "https://example.com/logos/toyota.png"),
            Brand.Create(26, "Nissan", "https://example.com/logos/nissan.png"),
            Brand.Create(27, "Mazda", "https://example.com/logos/mazda.png"),
            Brand.Create(28, "Subaru", "https://example.com/logos/subaru.png"),
            Brand.Create(29, "Mitsubishi", "https://example.com/logos/mitsubishi.png"),

            // Marques de voitures américaines
            Brand.Create(30, "Ford", "https://example.com/logos/ford.png"),
            Brand.Create(31, "Chevrolet", "https://example.com/logos/chevrolet.png"),
            Brand.Create(32, "Tesla", "https://example.com/logos/tesla.png"),
            Brand.Create(33, "Jeep", "https://example.com/logos/jeep.png"),

            // Marques de voitures coréennes
            Brand.Create(34, "Hyundai", "https://example.com/logos/hyundai.png"),
            Brand.Create(35, "Kia", "https://example.com/logos/kia.png"),

            // Marques de véhicules utilitaires / camping-cars
            Brand.Create(36, "Fiat Professional", "https://example.com/logos/fiat-professional.png"),
            Brand.Create(37, "Iveco", "https://example.com/logos/iveco.png"),
            Brand.Create(38, "MAN", "https://example.com/logos/man.png"),
            Brand.Create(39, "Scania", "https://example.com/logos/scania.png"),
            Brand.Create(40, "Volvo Trucks", "https://example.com/logos/volvo-trucks.png")
        };

        modelBuilder.Entity<Brand>().HasData(brands);
    }
}
