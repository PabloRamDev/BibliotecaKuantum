using biblioteca_kuantum.BusinessLayer.DTO;
using biblioteca_kuantum.BusinessLayer.Lib;
using FluentValidation;

namespace biblioteca_kuantum.BusinessLayer.Validators
{
    public class CreateDocumentValidator : AbstractValidator<CreateDocument>
    {

        public CreateDocumentValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("La longitud del Nombre no puede superar los 100 caracteres");
            RuleFor(x => x.Description).NotEmpty().MaximumLength(1000).WithMessage("La longitud de la descripción no puede superar los 1000 caracteres");
            RuleFor(x => x.AuthorFullName).NotEmpty().MaximumLength(300).WithMessage("La longitud del nombre completo del autor no puede superar los 300 caracteres");
            RuleFor(x => x.AuthorEmail).NotEmpty().EmailAddress().WithMessage("La dirección de email debe ser válida").MaximumLength(100).WithMessage("La longitud del email no debe superar los 100 caracteres");
            RuleFor(x => x.PublicationCode).NotEmpty().Must(BeValidCode).WithMessage("Formato ISO-<número> (ISO-27001), Ley N° <número> (Ley N° 19.628) ó P-XX.AAAAMMDD (P-01.20250614).");
            RuleFor(x => x.SerialCode)
            .NotEmpty().WithMessage("El código de serie es obligatorio.")
            .MaximumLength(16).WithMessage("El código de serie no debe superar los 16 caracteres.")
            .Matches("^[0-9a-fA-F]+$").WithMessage("El código de serie solo puede llevar caracteres hexadecimales (0-9, A-F).");
        }

        /*Claude Prompt:need this validations with regex for FluentValidation:

Prefijo “ISO-” seguido de un número.EJ: “ISO-27001”
- Prefijo “Ley N° ” seguido de un número con separador de miles. EJ: “Ley
N° 19.628”
- Prefijo "P-” seguido de un texto “XX.[AAAAMMDD]”, donde XX es el
número de la versión (EJ: 01), AAAA un año, MM, meses del 01 al 12 y
DD días del 01 al 31. EJ: (P-01.20250614) */
        private static bool BeValidCode(string code) =>
            DocumentCodePatterns.Iso().IsMatch(code)
            || DocumentCodePatterns.Ley().IsMatch(code)
            || (DocumentCodePatterns.Prefix().IsMatch(code) && DocumentCodePatterns.HasValidPrefixDate(code));
    }
}
