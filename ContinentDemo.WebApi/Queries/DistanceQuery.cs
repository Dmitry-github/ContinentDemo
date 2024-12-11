namespace ContinentDemo.WebApi.Queries
{
    using FluentValidation;

    public class DistanceQuery
    {
        public string Iata1 { get; set; }
        public string Iata2 { get; set; }
    }

    public class TransactionQueryValidator : AbstractValidator<DistanceQuery>
    {
        private const string RegExpIata = @"^[A-Z]{3}$";
        
        public TransactionQueryValidator()
        {
            RuleFor(x => x.Iata1).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("Iata code 1 required")
                .Matches(RegExpIata).WithMessage("Iata code 1 must be '^[A-Z]{3}$'");
            RuleFor(x => x.Iata2).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("Iata code 2 required")
                .Matches(RegExpIata).WithMessage("Iata code 2 must be '^[A-Z]{3}$'");

            RuleFor(x => x.Iata1).Cascade(CascadeMode.Stop).NotEqual(y => y.Iata2)
                .WithMessage("Iata1 and Iata2 must be different");
        }
    }
}
