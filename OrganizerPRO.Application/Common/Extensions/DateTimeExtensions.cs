public static class DateTimeExtensions
{
    public static (DateTime Start, DateTime End) GetDateRange(this DateTime dateTime, string keyword, TimeSpan timeZoneOffset = default)
    {
        DateTime today = dateTime.Date;

        // Adjust for timezone offset if the DateTime is in UTC
        if (dateTime.Kind == DateTimeKind.Utc && timeZoneOffset != default)
        {
            today = today.Subtract(timeZoneOffset);
        }

        switch (keyword.ToUpper())
        {
            case "TODAY":
                return (today, today.AddDays(1).AddTicks(-1));
            case "YESTERDAY":
                return (today.AddDays(-1), today.AddTicks(-1));
            case "TOMORROW":
                return (today.AddDays(1), today.AddDays(2).AddTicks(-1));
            case "THIS_WEEK":
                return (today.StartOfWeek(timeZoneOffset), today.EndOfWeek(timeZoneOffset).AddTicks(-1));
            case "LAST_WEEK":
                return (today.AddDays(-7).StartOfWeek(timeZoneOffset), today.AddDays(-7).EndOfWeek(timeZoneOffset).AddTicks(-1));
            case "NEXT_WEEK":
                return (today.AddDays(7).StartOfWeek(timeZoneOffset), today.AddDays(7).EndOfWeek(timeZoneOffset).AddTicks(-1));
            case "THIS_MONTH":
                return (new DateTime(today.Year, today.Month, 1), today.EndOfMonth(timeZoneOffset).AddTicks(-1));
            case "LAST_MONTH":
                var lastMonth = today.AddMonths(-1);
                return (new DateTime(lastMonth.Year, lastMonth.Month, 1), lastMonth.EndOfMonth(timeZoneOffset).AddTicks(-1));
            case "NEXT_MONTH":
                var nextMonth = today.AddMonths(1);
                return (new DateTime(nextMonth.Year, nextMonth.Month, 1), nextMonth.EndOfMonth(timeZoneOffset).AddTicks(-1));
            case "THIS_YEAR":
                return (new DateTime(today.Year, 1, 1), new DateTime(today.Year, 12, 31).AddTicks(-1));
            case "LAST_YEAR":
                return (new DateTime(today.Year - 1, 1, 1), new DateTime(today.Year - 1, 12, 31).AddTicks(-1));
            case "NEXT_YEAR":
                return (new DateTime(today.Year + 1, 1, 1), new DateTime(today.Year + 1, 12, 31).AddTicks(-1));
            case var kw when kw.StartsWith("LAST_") && kw.EndsWith("_DAYS"):
                if (int.TryParse(kw.Split('_')[1], out int days))
                {
                    return (today.AddDays(-days), today.AddTicks(-1));
                }
                break;
            case var kw when kw.StartsWith("NEXT_") && kw.EndsWith("_DAYS"):
                if (int.TryParse(kw.Split('_')[1], out int nextDays))
                {
                    return (today, today.AddDays(nextDays).AddTicks(-1));
                }
                break;
            case var kw when kw.StartsWith("LAST_") && kw.EndsWith("_MONTHS"):
                if (int.TryParse(kw.Split('_')[1], out int months))
                {
                    var start = today.AddMonths(-months).StartOfMonth();
                    return (start, today.EndOfMonth(timeZoneOffset).AddTicks(-1));
                }
                break;
            case var kw when kw.StartsWith("NEXT_") && kw.EndsWith("_MONTHS"):
                if (int.TryParse(kw.Split('_')[1], out int nextMonths))
                {
                    var start = today.StartOfMonth();
                    var end = today.AddMonths(nextMonths).EndOfMonth(timeZoneOffset).AddTicks(-1);
                    return (start, end);
                }
                break;
        }
        throw new ArgumentException("Invalid keyword", nameof(keyword));
    }

    /// <summary>
    /// Gets the start of the week for the provided date, assuming Monday as the first day of the week.
    /// </summary>
    /// <param name="dt">The date to find the start of the week for.</param>
    /// <param name="timeZoneOffset">The timezone offset to adjust for if needed.</param>
    /// <returns>The start date of the week, adjusted for the provided time zone offset if needed.</returns>
    public static DateTime StartOfWeek(this DateTime dt, TimeSpan timeZoneOffset = default)
    {
        int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
        DateTime startOfWeek = dt.AddDays(-1 * diff).Date;
        if (timeZoneOffset != default && dt.Kind == DateTimeKind.Utc)
        {
            startOfWeek = startOfWeek.Subtract(timeZoneOffset);
        }
        return dt.Kind == DateTimeKind.Utc ? startOfWeek.Subtract(timeZoneOffset) : startOfWeek;
    }

    public static DateTime EndOfWeek(this DateTime dt, TimeSpan timeZoneOffset = default)
    {
        DateTime endOfWeek = dt.StartOfWeek(timeZoneOffset).AddDays(6);
        if (timeZoneOffset != default && dt.Kind == DateTimeKind.Utc)
        {
            endOfWeek = endOfWeek.Subtract(timeZoneOffset);
        }
        return dt.Kind == DateTimeKind.Utc ? endOfWeek.Subtract(timeZoneOffset) : endOfWeek;
    }

    public static DateTime EndOfMonth(this DateTime dt, TimeSpan timeZoneOffset = default)
    {
        DateTime endOfMonth = new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
        if (timeZoneOffset != default && dt.Kind == DateTimeKind.Utc)
        {
            endOfMonth = endOfMonth.Subtract(timeZoneOffset);
        }
        return dt.Kind == DateTimeKind.Utc ? endOfMonth.Subtract(timeZoneOffset) : endOfMonth;
    }


    public static DateTime StartOfMonth(this DateTime dt, TimeSpan timeZoneOffset = default)
    {
        DateTime startOfMonth = new DateTime(dt.Year, dt.Month, 1);
        if (timeZoneOffset != default && dt.Kind == DateTimeKind.Utc)
        {
            startOfMonth = startOfMonth.Subtract(timeZoneOffset);
        }
        return startOfMonth;
    }


    public static string? ToLocalTime(this DateTime? utcTime, TimeSpan? localTimeOffset, string formatter = "yyyy-MM-dd HH:mm:ss")
    {
        if (utcTime == null || localTimeOffset==null)
            return null;

        // Add the local time offset to the UTC time to get the local time
        var localTime = utcTime.Value + localTimeOffset;

        // Format the local time according to the given formatter
        return localTime.Value.ToString(formatter);
    }
}