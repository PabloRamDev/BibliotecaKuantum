using biblioteca_kuantum.DataAccessLayer.Data;
using biblioteca_kuantum.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;


namespace biblioteca_kuantum.BusinessLayer.Tests.Helpers
{
    public class TestDbContextFactory
    {
        public static AppDbContext CreateInMemoryDbContext(bool seedData = true)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            var dbContext = new AppDbContext(options);

            if (seedData)
            {
                SeedTestData(dbContext);
            }

            return dbContext;

        }

        private static DateTimeOffset Utc(int y, int m, int d) =>
    new(y, m, d, 9, 0, 0, TimeSpan.Zero);

        //Seed data created with Claude
        private static void SeedTestData(AppDbContext dbContext)
        {
            dbContext.Documents.AddRange(
                new Document
                {
                    Name = "Política de Seguridad de la Información",
                    Description = "Lineamientos de seguridad basados en la norma ISO 27001.",
                    AuthorFullName = "María José Fernández",
                    AuthorEmail = "mj.fernandez@example.com",
                    SerialCode = "A1B2C3D4E5F60718",
                    PublicationCode = "ISO-27001",
                    CreatedAt = Utc(2025, 3, 10),
                    UpdatedAt = Utc(2025, 3, 10),
                    DocumentPages =
                    [
                        new DocumentPageIndex { Name = "Introducción",        Page = 1, CreatedAt = Utc(2025, 3, 10) },
                new DocumentPageIndex { Name = "Alcance",             Page = 3, CreatedAt = Utc(2025, 3, 10) },
                new DocumentPageIndex { Name = "Controles de Acceso", Page = 7, CreatedAt = Utc(2025, 3, 10) },
                    ]
                },
                new Document
                {
                    Name = "Manual de Gestión de Calidad",
                    Description = "Procesos y controles del sistema de gestión de calidad.",
                    AuthorFullName = "Juan Pablo Soto",
                    AuthorEmail = "j_soto@example.com",
                    SerialCode = "0F1E2D3C4B5A6978",
                    PublicationCode = "ISO-9001",
                    CreatedAt = Utc(2025, 4, 2),
                    UpdatedAt = Utc(2025, 5, 15),
                    DocumentPages =
                    [
                        new DocumentPageIndex { Name = "Objetivos de Calidad", Page = 1,  CreatedAt = Utc(2025, 4, 2) },
                new DocumentPageIndex { Name = "Auditorías Internas",  Page = 12, CreatedAt = Utc(2025, 4, 2) },
                    ]
                },
                new Document
                {
                    Name = "Protección de la Vida Privada",
                    Description = "Resumen y aplicación de la ley sobre protección de datos personales.",
                    AuthorFullName = "Camila Rojas",
                    AuthorEmail = "camila.rojas@example.com",
                    SerialCode = "DEADBEEF00112233",
                    PublicationCode = "Ley N\u00B0 19.628",
                    CreatedAt = Utc(2025, 6, 1),
                    UpdatedAt = Utc(2025, 6, 1),
                    DocumentPages =
                    [
                        new DocumentPageIndex { Name = "Definiciones",         Page = 2,  CreatedAt = Utc(2025, 6, 1) },
                new DocumentPageIndex { Name = "Derechos del Titular", Page = 5,  CreatedAt = Utc(2025, 6, 1) },
                new DocumentPageIndex { Name = "Sanciones",            Page = 11, CreatedAt = Utc(2025, 6, 1) },
                    ]
                },
                new Document
                {
                    Name = "Procedimiento de Préstamo de Libros",
                    Description = "Pasos para el préstamo y devolución de material bibliográfico.",
                    AuthorFullName = "Camila Rojas",
                    AuthorEmail = "camila.rojas@example.com",
                    SerialCode = "1234567890ABCDEF",
                    PublicationCode = "P-01.20250614",
                    CreatedAt = Utc(2025, 6, 14),
                    UpdatedAt = Utc(2025, 6, 14),
                    DocumentPages =
                    [
                        new DocumentPageIndex { Name = "Requisitos del Usuario", Page = 1, CreatedAt = Utc(2025, 6, 14) },
                new DocumentPageIndex { Name = "Plazos y Renovaciones",  Page = 4, CreatedAt = Utc(2025, 6, 14) },
                    ]
                },
                new Document
                {
                    Name = "Procedimiento de Inventario Anual",
                    SerialCode = "CAFEBABE13572468",
                    PublicationCode = "P-02.20240131",
                    CreatedAt = Utc(2024, 1, 31),
                    UpdatedAt = Utc(2024, 2, 5),
                    DocumentPages =
                    [
                        new DocumentPageIndex { Name = "Calendario del Inventario", Page = 1, CreatedAt = Utc(2024, 1, 31) },
                    ]
                },
                new Document
                {
                    Name = "Procedimiento Obsoleto de Catalogación",
                    Description = "Documento reemplazado; se mantiene por trazabilidad.",
                    AuthorFullName = "Pedro Muñoz",
                    AuthorEmail = "pedro.munoz@example.com",
                    SerialCode = "00FF00FF00FF00FF",
                    PublicationCode = "P-01.20230815",
                    CreatedAt = Utc(2023, 8, 15),
                    UpdatedAt = Utc(2024, 9, 1),
                    DeletedAt = Utc(2024, 9, 1)   // soft-deleted
                });

            dbContext.SaveChanges();
        }
    }
}
