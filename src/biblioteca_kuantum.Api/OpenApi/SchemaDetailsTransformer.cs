using System.Globalization;
using System.Text.Json;
using biblioteca_kuantum.BusinessLayer.DTO;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;          // .NET 10 (Microsoft.OpenApi 2.x)

namespace biblioteca_kuantum.Api.OpenApi;

/// <summary>
/// Documentación centralizada de los esquemas (DTOs) de la API Biblioteca Kuantum.
/// Cada DTO se identifica por su tipo y cada propiedad por su nombre JSON
/// (camelCase; la búsqueda no distingue mayúsculas).
/// Los límites que ya están en Data Annotations ([MaxLength], [EmailAddress]) se repiten
/// aquí solo para que la documentación quede completa y coherente.
/// </summary>
public sealed class SchemaDetailsTransformer : IOpenApiSchemaTransformer
{
    private sealed record PropDoc(
        string Description,
        object? Example = null,
        int? MinLength = null,
        int? MaxLength = null,
        decimal? Minimum = null,
        string? Format = null,
        bool Integer = false);   // fuerza type: integer / format: int32

    private sealed record SchemaDoc(
        string Description,
        Dictionary<string, PropDoc> Properties);

    private static Dictionary<string, PropDoc> Props(params (string Name, PropDoc Doc)[] items) =>
        items.ToDictionary(i => i.Name, i => i.Doc, StringComparer.OrdinalIgnoreCase);

    // ------------------------------------------------------------------
    //  Propiedades comunes (reutilizadas en varios DTOs)
    // ------------------------------------------------------------------
    private const string ExampleDate = "2026-10-31T18:00:00-03:00";

    private static readonly PropDoc DocId = new(
        "Identificador único del documento.", Example: 1, Minimum: 1, Integer: true);

    private static readonly PropDoc DocName = new(
        "Nombre o título del documento.",
        Example: "Introducción a la computación cuántica", MaxLength: 100);

    private static readonly PropDoc DocDescription = new(
        "Descripción o resumen del contenido del documento.",
        Example: "Texto introductorio sobre qubits, superposición y entrelazamiento.", MaxLength: 1000);

    private static readonly PropDoc AuthorFullName = new(
        "Nombre completo del autor del documento.", Example: "María González");

    private static readonly PropDoc AuthorEmail = new(
        "Correo electrónico de contacto del autor.",
        Example: "maria.gonzalez@example.com", MaxLength: 100, Format: "email");

    private static readonly PropDoc SerialCode = new(
        "Código de serie único del documento.", Example: "SN-2026-000123", MinLength: 1, MaxLength: 16);

    private static readonly PropDoc PublicationCode = new(
        "Código de publicación del documento.", Example: "PUB-2026-0042", MinLength: 1, MaxLength: 100);

    private static readonly PropDoc CreatedAt = new(
        "Fecha y hora de creación del registro (ISO 8601 con zona horaria).",
        Example: ExampleDate, Format: "date-time");

    private static readonly PropDoc UpdatedAt = new(
        "Fecha y hora de la última actualización (ISO 8601 con zona horaria). " +
        "Es nulo si el documento nunca se ha modificado.",
        Example: ExampleDate, Format: "date-time");

    private static readonly PropDoc PageName = new(
        "Nombre de la entrada del índice (capítulo, sección, etc.).", Example: "Capítulo 1: Qubits");

    private static readonly PropDoc PageNumber = new(
        "Número de página donde comienza la entrada.", Example: 1, Minimum: 1, Integer: true);

    // ------------------------------------------------------------------
    //  DTOs
    // ------------------------------------------------------------------
    private static readonly Dictionary<Type, SchemaDoc> Schemas = new()
    {
        // ---------- Peticiones ----------
        [typeof(CreateDocument)] = new(
            "Datos necesarios para registrar un nuevo documento en la biblioteca.",
            Props(
                ("name", DocName),
                ("description", DocDescription),
                ("authorFullName", AuthorFullName),
                ("authorEmail", AuthorEmail),
                ("serialCode", SerialCode),
                ("publicationCode", PublicationCode),
                ("documentPages", new(
                    "Índice de páginas del documento. Se require agregar una página o más"))
            )),

        [typeof(UpdateDocument)] = new(
            "Datos para actualizar un documento existente.",
            Props(
                ("name", DocName),
                ("description", DocDescription),
                ("authorFullName", AuthorFullName),
                ("authorEmail", AuthorEmail),
                ("serialCode", SerialCode with { MinLength = null }),
                ("publicationCode", PublicationCode with { MinLength = null }),
                ("documentPages", new(
                    "Índice de páginas del documento."))
            )),

        [typeof(CreateDocumentPageIndex)] = new(
            "Entrada del índice de páginas de un documento (al crear o actualizar).",
            Props(
                ("name", PageName),
                ("page", PageNumber)
            )),

        // ---------- Respuestas ----------
        [typeof(DocumentResponse)] = new(
            "Documento de la biblioteca con su índice de páginas.",
            Props(
                ("id", DocId),
                ("name", DocName),
                ("description", DocDescription),
                ("authorFullName", AuthorFullName),
                ("authorEmail", AuthorEmail),
                ("serialCode", SerialCode),
                ("publicationCode", PublicationCode),
                ("createdAt", CreatedAt),
                ("updatedAt", UpdatedAt),
                ("documentPages", new(
                    "Índice de páginas del documento."))
            )),

        [typeof(DocumentSearchResponse)] = new(
            "Documento devuelto en resultados de búsqueda (sin índice de páginas).",
            Props(
                ("id", DocId),
                ("name", DocName),
                ("description", DocDescription),
                ("authorFullName", AuthorFullName),
                ("authorEmail", AuthorEmail),
                ("serialCode", SerialCode),
                ("publicationCode", PublicationCode),
                ("createdAt", CreatedAt),
                ("updatedAt", UpdatedAt)
            )),

        [typeof(DocumentPageIndexResponse)] = new(
            "Entrada registrada del índice de páginas de un documento.",
            Props(
                ("id", new("Identificador único de la entrada del índice.", Example: 1, Minimum: 1, Integer: true)),
                ("name", PageName),
                ("page", PageNumber),
                ("createdAt", CreatedAt)
            )),
    };

    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        // Solo se procesa el esquema del tipo completo, no cada propiedad por separado.
        if (context.JsonPropertyInfo is not null)
            return Task.CompletedTask;

        if (!Schemas.TryGetValue(context.JsonTypeInfo.Type, out var doc))
            return Task.CompletedTask;

        schema.Description = doc.Description;

        if (schema.Properties is null)
            return Task.CompletedTask;

        foreach (var (name, propSchema) in schema.Properties)
        {
            if (propSchema is not OpenApiSchema prop
                || !doc.Properties.TryGetValue(name, out var pd))
                continue;

            prop.Description = pd.Description;

            if (pd.Integer)
            {
                // .NET 10 genera los enteros como "integer | string" con un pattern regex;
                // aquí se dejan como entero puro.
                var nullable = prop.Type?.HasFlag(JsonSchemaType.Null) == true;
                prop.Type = JsonSchemaType.Integer | (nullable ? JsonSchemaType.Null : 0);
                prop.Format = "int32";
                prop.Pattern = null;
            }

            if (pd.Example is not null)
                prop.Example = JsonSerializer.SerializeToNode(pd.Example);

            if (pd.MinLength is not null)
                prop.MinLength = pd.MinLength;

            if (pd.MaxLength is not null)
                prop.MaxLength = pd.MaxLength;

            if (pd.Minimum is not null)
                prop.Minimum = pd.Minimum.Value.ToString(CultureInfo.InvariantCulture);

            if (pd.Format is not null)
                prop.Format = pd.Format;
        }

        return Task.CompletedTask;
    }
}