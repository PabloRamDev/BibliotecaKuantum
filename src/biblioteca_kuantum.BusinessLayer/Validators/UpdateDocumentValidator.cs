using biblioteca_kuantum.BusinessLayer.DTO;
using biblioteca_kuantum.BusinessLayer.Lib;
using FluentValidation;


namespace biblioteca_kuantum.BusinessLayer.Validators
{
    public class UpdateDocumentValidator : AbstractValidator<UpdateDocument>
    {
        public UpdateDocumentValidator()
        {
            RuleFor(x => x.Name).MaximumLength(100).WithMessage("La longitud del Nombre no puede superar los 100 caracteres");

            RuleFor(x => x.Description).MaximumLength(1000).WithMessage("La longitud de la descripción no puede superar los 1000 caracteres");

            RuleFor(x => x.AuthorFullName).MaximumLength(300).WithMessage("La longitud del nombre completo del autor no puede superar los 300 caracteres");

            //Custom rule, based on CreateDocumentValidator AuthorEmailRule
            RuleFor(x => x.AuthorEmail)
                .MaximumLength(100).WithMessage("La longitud del email no debe superar los 100 caracteres")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$").WithMessage("La dirección de email debe ser válida");

            RuleFor(x => x.PublicationCode).Must(BeValidCode).WithMessage("Formato ISO-<número> (ISO-27001), Ley N° <número> (Ley N° 19.628) ó P-XX.AAAAMMDD (P-01.20250614).").When(x => !String.IsNullOrEmpty(x.PublicationCode));

            RuleFor(x => x.SerialCode)
            .MaximumLength(16).WithMessage("El código de serie no debe superar los 16 caracteres.")
            .Matches("^[0-9a-fA-F]+$").WithMessage("El código de serie solo puede llevar caracteres hexadecimales (0-9, A-F).").When(x => !String.IsNullOrEmpty(x.SerialCode));

            RuleFor(x => x.DocumentPages)
                .Must(pages => pages!.Select(p => p.Page).Distinct().Count() == pages!.Count)
                .WithMessage("Los números de página no deben duplicarse.")
                .When(x => x.DocumentPages is not null);

            // rules for each item
            RuleForEach(x => x.DocumentPages)
                .SetValidator(new DocumentPageIndexValidator());
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
