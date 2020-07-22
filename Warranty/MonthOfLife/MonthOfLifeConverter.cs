using System;

namespace FleetAssist.Common.Date.MonthOfLife
{
    /// <summary>
    /// Vehicles are often described as being in a certain month of their life.
    /// This converts dates to the month a vehicle is in its life.
    /// </summary>
    public class MonthOfLifeConverter
    {
        /// <summary>
        /// Takes the date and returns how many months of life a vehicle has from that date.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>The month of life.</returns>
        public int ToMonthOfLife(DateTime startDate, DateTime endDate)
        {
            var monthOfLife = 12 * (endDate.Year - startDate.Year) + (endDate.Month - startDate.Month) + 1;
            if (startDate.Day > endDate.Day)
            {
                return monthOfLife - 1;
            }
            else
            {
                return monthOfLife;
            }
        }

        /// <summary>
        /// Takes the date and returns how many months of life a vehicle has from that date.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <returns>The month of life.</returns>
        public int ToMonthOfLife(DateTime startDate)
        {
            return ToMonthOfLife(startDate, DateTime.Now);
        }

        /// <summary>
        /// Takes the date and returns how many months of life a vehicle has from that date.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <returns>The month of life.</returns>
        public int? ToMonthOfLife(DateTime? startDate)
        {
            if (startDate.HasValue)
            {
                return ToMonthOfLife(startDate.Value);
            }
            else
            {
                return null;
            }
        }
    }
}
