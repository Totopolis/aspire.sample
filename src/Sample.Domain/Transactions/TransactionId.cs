using Vogen;

namespace Sample.Domain.Transactions;

[ValueObject<Guid>]
public partial struct TransactionId
{
    private static Validation Validate(Guid value)
    {
        if (value == Guid.Empty)
        {
            return Validation.Invalid("TransactionId can not be empty");
        }

        if (value == Guid.AllBitsSet)
        {
            return Validation.Invalid("TransactionId can not contains all bits");
        }

        if (value.Version != 7)
        {
            return Validation.Invalid("TransactionId must be 7 version");
        }

        return Validation.Ok;
    }
}
