using ErrorOr;

namespace Sample.Application.Diagnostics;

public static class ApplicationErrors
{
    public static readonly Error TransactionNotFound = Error.NotFound(
        code: "Sample.Application.TransactionNotFound",
        description: "Transaction not found");

    public static readonly Error TooManyTransactions = Error.Failure(
        code: "Sample.Application.TooManyTransactions",
        description: "Too many transactions");

    public static readonly Error TransactionAlreadyProcessed = Error.Failure(
        code: "Sample.Application.TransactionAlreadyProcessed",
        description: "Transaction already processed");
}
