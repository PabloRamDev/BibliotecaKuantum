using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;          // .NET 10 (Microsoft.OpenApi 2.x)

namespace biblioteca_kuantum.Api.OpenApi;

/// <summary>
/// Define la información general de la API (título, versión, descripción, contacto)
/// y la descripción de cada grupo de operaciones (tags) que se muestra en Swagger UI.
/// </summary>
public sealed class DocumentInfoTransformer : IOpenApiDocumentTransformer
{
    // Descripción de cada tag (por defecto, el nombre del controlador sin "Controller")
    private static readonly Dictionary<string, string> TagDescriptions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Documents"] = "Gestión de documentos de la biblioteca: alta, consulta, búsqueda paginada, " +
                        "actualización y eliminación."
    };

    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "Biblioteca Kuantum API",
            Version = "v1",
            // Swagger UI interpreta Markdown en la descripción
            Description =
                """
                API REST de Biblioteca Kuantum, (prueba técnica octubre 2026) por Pablo Ramírez. 
                Proporciona endpoints para crear, obtener, actualizar y eliminar documentos, 
                además de un endpoint de búsqueda dedicado para consultar el catálogo de documentos.

                Permite:
                - Registrar documentos con su índice de páginas.
                - Consultar un documento por su identificador.
                - Buscar documentos por código de serie, código de publicación o texto libre, con resultados paginados.
                - Actualizar y eliminar documentos existentes.
                
                Primeros pasos
                 1. Agregar la variable apiKey con una API key registrada en el archivo .ENV en el botón Authorize.
                 3. Se puede probar cada endpoint. Se incluye ejemplos guardados que muestran respuestas exitosas y de error.
                """,
            Contact = new OpenApiContact
            {
                Name = "Pablo Ramírez",
                Email = "pabloram.dev@gmail.com",
            },
            // License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
        };

        // Descripción de los grupos de operaciones (tags)
        document.Tags ??= new HashSet<OpenApiTag>();
        foreach (var (name, description) in TagDescriptions)
        {
            var tag = document.Tags.FirstOrDefault(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
            if (tag is null)
                document.Tags.Add(new OpenApiTag { Name = name, Description = description });
            else
                tag.Description = description;
        }

        return Task.CompletedTask;
    }
}