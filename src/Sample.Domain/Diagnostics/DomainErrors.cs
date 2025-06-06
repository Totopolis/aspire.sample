using ErrorOr;

namespace Sample.Domain.Diagnostics;

public static class DomainErrors
{
    public static readonly Error IncorrectDate = Error.Validation(
        code: "Sample.Domain.IncorrectDate",
        description: "Incorrect datetime");

    public static readonly Error IncorrectAmount = Error.Validation(
        code: "Sample.Domain.IncorrectAmount",
        description: "Incorrect amount of money");
}
