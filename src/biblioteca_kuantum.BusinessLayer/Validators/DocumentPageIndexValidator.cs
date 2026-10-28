using biblioteca_kuantum.BusinessLayer.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace biblioteca_kuantum.BusinessLayer.Validators
{
    public class DocumentPageIndexValidator : AbstractValidator<CreateDocumentPageIndex>
    {
        public DocumentPageIndexValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0).WithMessage("El número de página debe ser mayor a cero");

            RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("El nombre de la página no debe tener más de 100 caracteres");
        }
    }
}
