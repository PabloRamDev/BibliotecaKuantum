using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;          // .NET 10 (Microsoft.OpenApi 2.x)
// using Microsoft.OpenApi.Models; // .NET 9 (Microsoft.OpenApi 1.x) – use this line instead

namespace biblioteca_kuantum.Api.OpenApi;

/// <summary>
/// Documentación centralizada de todas las operaciones de la API Biblioteca Kuantum.
/// Cada entrada se identifica por "MÉTODO ruta" (sin barra inicial y sin restricciones
/// de ruta, tal como la reporta ASP.NET Core; p. ej. "api/Documents/{id}").
/// </summary>
public sealed class OperationDetailsTransformer : IOpenApiOperationTransformer
{
    private sealed record OperationDoc(
        string Summary,
        string Description,
        Dictionary<string, string>? Parameters = null,
        Dictionary<string, string>? Responses = null,
        string? RequestBody = null,
        bool Deprecated = false);

    // ------------------------------------------------------------------
    //  DocumentsController
    // ------------------------------------------------------------------
    private static readonly Dictionary<string, OperationDoc> Operations =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // POST api/Documents
            ["POST api/Documents"] = new(
                Summary: "Crear documento",
                Description: "Registra un nuevo documento en la biblioteca y devuelve el documento creado " +
                             "con su identificador asignado. La cabecera Location apunta al recurso creado " +
                             "(GET api/Documents/{id}).",
                RequestBody: "Datos del documento a crear.",
                Responses: new()
                {
                    ["201"] = "Documento creado correctamente. Devuelve el documento con su Id.",
                    ["400"] = "Los datos enviados no son válidos o el documento no pudo crearse."
                }),

            // GET api/Documents/{id}
            ["GET api/Documents/{id}"] = new(
                Summary: "Obtener documento por Id",
                Description: "Devuelve un único documento a partir de su identificador numérico.",
                Parameters: new()
                {
                    ["id"] = "Identificador numérico del documento."
                },
                Responses: new()
                {
                    ["200"] = "Documento encontrado.",
                    ["404"] = "No existe un documento con el Id indicado."
                }),

            // GET api/Documents/search
            ["GET api/Documents/search"] = new(
                Summary: "Buscar documentos",
                Description: "Busca documentos aplicando filtros opcionales y devuelve los resultados paginados. " +
                             "Todos los filtros son opcionales y se pueden combinar; si no se envía ninguno, " +
                             "se devuelven todos los documentos de forma paginada.",
                Parameters: new()
                {
                    ["id"] = "Filtra por identificador exacto del documento.",
                    ["serialCode"] = "Filtra por código de serie del documento.",
                    ["publicationCode"] = "Filtra por código de publicación del documento.",
                    ["searchText"] = "Texto libre para buscar coincidencias en los datos del documento.",
                    ["pageNumber"] = "Número de página a devolver, comenzando en 1. Valor por defecto: 1.",
                    ["pageSize"] = "Cantidad de documentos por página. Valor por defecto: 10."
                },
                Responses: new()
                {
                    ["200"] = "Resultado paginado con los documentos que cumplen los filtros.",
                    ["404"] = "La búsqueda no pudo realizarse o no devolvió resultados."
                }),

            // PUT api/Documents/{id}
            ["PUT api/Documents/{id}"] = new(
                Summary: "Actualizar documento",
                Description: "Actualiza los datos de un documento existente y devuelve el documento actualizado.",
                Parameters: new()
                {
                    ["id"] = "Identificador numérico del documento a actualizar."
                },
                RequestBody: "Nuevos datos del documento.",
                Responses: new()
                {
                    ["200"] = "Documento actualizado correctamente.",
                    ["400"] = "Los datos enviados no son válidos o el documento no existe."
                }),

            // DELETE api/Documents/{id}
            ["DELETE api/Documents/{id}"] = new(
                Summary: "Eliminar documento",
                Description: "Elimina un documento de la biblioteca a partir de su identificador.",
                Parameters: new()
                {
                    ["id"] = "Identificador numérico del documento a eliminar."
                },
                Responses: new()
                {
                    ["200"] = "Documento eliminado correctamente.",
                    ["400"] = "El documento no pudo eliminarse (por ejemplo, porque no existe)."
                }),
        };

    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var method = context.Description.HttpMethod ?? string.Empty;
        var route = context.Description.RelativePath?.TrimStart('/') ?? string.Empty;

        if (!Operations.TryGetValue($"{method} {route}", out var doc))
            return Task.CompletedTask;

        operation.Summary = doc.Summary;
        operation.Description = doc.Description;
        operation.Deprecated = doc.Deprecated;

        // Descripción de parámetros (ruta y query)
        if (doc.Parameters is not null && operation.Parameters is not null)
        {
            foreach (var p in operation.Parameters.OfType<OpenApiParameter>())
            {
                if (p.Name is not null && doc.Parameters.TryGetValue(p.Name, out var desc))
                    p.Description = desc;
            }
        }

        // Descripción del cuerpo de la petición
        if (doc.RequestBody is not null && operation.RequestBody is OpenApiRequestBody body)
        {
            body.Description = doc.RequestBody;
        }

        // Descripción de respuestas (actualiza las existentes y agrega las que falten)
        if (doc.Responses is not null)
        {
            operation.Responses ??= new OpenApiResponses();
            foreach (var (code, desc) in doc.Responses)
            {
                if (operation.Responses.TryGetValue(code, out var existing)
                    && existing is OpenApiResponse concrete)
                {
                    concrete.Description = desc;
                }
                else if (!operation.Responses.ContainsKey(code))
                {
                    operation.Responses[code] = new OpenApiResponse { Description = desc };
                }
            }
        }

        return Task.CompletedTask;
    }
}