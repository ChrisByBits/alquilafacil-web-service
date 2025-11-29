using AlquilaFacilPlatform.Locals.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Locals.Domain.Model.Commands;
using AlquilaFacilPlatform.Locals.Domain.Model.Entities;
using AlquilaFacilPlatform.Locals.Domain.Repositories;
using AlquilaFacilPlatform.Locals.Domain.Services;
using AlquilaFacilPlatform.IAM.Domain.Repositories;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.Locals.Application.Internal.CommandServices;

public class SeedLocalsCommandService(
    ILocalRepository localRepository,
    ILocalCategoryRepository localCategoryRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : ISeedLocalsCommandService
{
    public async Task Handle(SeedLocalsCommand command)
    {
        // Check if locals already exist
        var existingLocals = await localRepository.GetAllAsync();
        if (existingLocals.Any()) return;

        // Get admin user (created by SeedAdminCommandService)
        var adminUser = await userRepository.FindByUsername("Admin");
        if (adminUser == null) return;

        // Get categories
        var categories = (await localCategoryRepository.GetAllLocalCategories()).ToList();
        if (!categories.Any()) return;

        var localsData = new List<(string name, string description, string city, string district, string street, int price, int capacity, string features, int categoryIndex, double lat, double lng, string[] photos)>
        {
            (
                "Salon de Eventos Luna Azul",
                "Hermoso salon para eventos con capacidad para 150 personas. Cuenta con aire acondicionado, sistema de sonido profesional, iluminacion LED programable, cocina equipada y estacionamiento privado para 30 vehiculos. Ideal para bodas, quinceañeros, eventos corporativos y celebraciones especiales. Incluye mesas, sillas y manteleria.",
                "Lima",
                "Miraflores",
                "Av. Pardo 456",
                120,
                150,
                "WiFi gratis,Aire acondicionado,Estacionamiento,Sistema de audio,Proyector,Cocina equipada",
                3, // ElegantRoom
                -12.1199,
                -77.0292,
                new[] {
                    "https://images.unsplash.com/photo-1519167758481-83f550bb49b3?w=800",
                    "https://images.unsplash.com/photo-1464366400600-7168b8af9bc3?w=800",
                    "https://images.unsplash.com/photo-1478146896981-b80fe463b330?w=800"
                }
            ),
            (
                "Casa de Playa Paraiso",
                "Espectacular casa frente al mar en Asia. 4 habitaciones, 3 baños, piscina privada, terraza con vista al oceano, parrilla y jardin. Perfecta para familias o grupos de amigos que buscan disfrutar del verano. A solo 5 minutos de Boulevard de Asia. Capacidad maxima 12 personas.",
                "Lima",
                "Asia",
                "Playa Las Palmas Km 97.5",
                250,
                12,
                "Piscina,Vista al mar,Parrilla,WiFi gratis,Estacionamiento,Aire acondicionado",
                0, // BeachHouse
                -12.7833,
                -76.6167,
                new[] {
                    "https://images.unsplash.com/photo-1499793983690-e29da59ef1c2?w=800",
                    "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800",
                    "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=800"
                }
            ),
            (
                "Finca El Refugio",
                "Hermosa casa de campo en Cieneguilla, rodeada de naturaleza y tranquilidad. Cuenta con 3 hectareas de terreno, piscina, cancha de futbol, zona de camping, huerto organico y animales de granja. Ideal para retiros empresariales, cumpleaños o escapadas familiares. Incluye desayuno.",
                "Lima",
                "Cieneguilla",
                "Av. Nueva Toledo Km 25",
                180,
                30,
                "Piscina,Cancha de futbol,Zona de camping,Estacionamiento,Desayuno incluido,Areas verdes",
                1, // LandscapeHouse
                -12.0833,
                -76.7667,
                new[] {
                    "https://images.unsplash.com/photo-1564013799919-ab600027ffc6?w=800",
                    "https://images.unsplash.com/photo-1523217582562-09d0def993a6?w=800",
                    "https://images.unsplash.com/photo-1600596542815-ffad4c1539a9?w=800"
                }
            ),
            (
                "Loft Moderno San Isidro",
                "Exclusivo loft en el corazon financiero de Lima. Diseño minimalista con acabados de primera, cocina americana totalmente equipada, smart TV 65 pulgadas, Netflix incluido. Ubicacion privilegiada cerca de centros comerciales, restaurantes y parques. Ideal para ejecutivos y turistas.",
                "Lima",
                "San Isidro",
                "Calle Los Libertadores 234",
                85,
                4,
                "WiFi gratis,Smart TV,Netflix,Aire acondicionado,Cocina equipada,Seguridad 24/7",
                2, // CityHouse
                -12.0977,
                -77.0365,
                new[] {
                    "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=800",
                    "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=800",
                    "https://images.unsplash.com/photo-1554995207-c18c203602cb?w=800"
                }
            )
        };

        foreach (var data in localsData)
        {
            var category = categories.ElementAtOrDefault(data.categoryIndex);
            if (category == null) continue;

            var local = new Local(
                data.name,
                data.description,
                "Peru",
                data.city,
                data.district,
                data.street,
                data.price,
                data.capacity,
                data.features,
                category.Id,
                adminUser.Id
            );

            local.UpdateCoordinates(data.lat, data.lng);

            // Add photos
            foreach (var photoUrl in data.photos)
            {
                local.LocalPhotos.Add(new LocalPhoto(photoUrl));
            }

            await localRepository.AddAsync(local);
        }

        await unitOfWork.CompleteAsync();
    }
}
