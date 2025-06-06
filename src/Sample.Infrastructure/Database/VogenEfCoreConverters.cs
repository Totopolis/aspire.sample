using Vogen;

namespace Sample.Infrastructure.Database;

[EfCoreConverter<Domain.Transactions.TransactionId>]
internal partial class VogenEfCoreConverters;
