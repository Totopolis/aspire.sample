using NodaTime;

namespace Sample.Application.Abstractions;

public interface ITimeZoneApplicator
{
    DateTime ToZonedDatetime(Instant stamp);
}
