using System;
using System.Collections.Generic;
using System.Linq;

namespace LMS.Blazor.Client.Helpers;

public static class HolidayHelper
{
    public enum DayStatus { Normal, Overlap, OutOfBounds, Weekend }

    public static DayStatus GetDayStatus(DateTime date, DateTime? courseStart, IEnumerable<(DateTime Start, DateTime End)>? existingModules)
    {
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            return DayStatus.Weekend;

        if (courseStart.HasValue && date < courseStart.Value.Date)
            return DayStatus.OutOfBounds;

        if (existingModules != null)
        {
            foreach (var mod in existingModules)
            {
                if (date.Date >= mod.Start.Date && date.Date <= mod.End.Date)
                    return DayStatus.Overlap;
            }
        }

        return DayStatus.Normal;
    }

    public static bool IsDisabledDate(DateTime date, DateTime? courseStart, IEnumerable<(DateTime Start, DateTime End)>? existingModules)
    {
        var status = GetDayStatus(date, courseStart, existingModules);
        return status != DayStatus.Normal;
    }

    public static string GetDisabledReason(DateTime date, DateTime? courseStart, IEnumerable<(DateTime Start, DateTime End)>? existingModules)
    {
        var status = GetDayStatus(date, courseStart, existingModules);
        
        return status switch
        {
            DayStatus.Weekend => "Weekend",
            DayStatus.OutOfBounds => "Before course starts",
            DayStatus.Overlap => "Module Overlap",
            _ => string.Empty
        };
    }
}
