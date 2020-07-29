using FleetAssist.Common.Date.MonthOfLife;
using System.Collections.Generic;
using System.Linq;

namespace Warranty
{
    public class WarrantyInfo
    {
        private IVehicleLookup _vehicleLookup { get; }
        private IWarrantyDb _warrantyDb { get; }

        public WarrantyInfo(IVehicleLookup vehicleLookup, IWarrantyDb warrantyDb)
        {
            _vehicleLookup = vehicleLookup;
            _warrantyDb = warrantyDb;
        }

        public IEnumerable<IWarrantyInfoRow> Query(string regNumber, int mileage)
        {
            var vehicle = _vehicleLookup.Vehicle(regNumber);

            var monthOfLife = new MonthOfLifeConverter().ToMonthOfLife(vehicle.DateOfFirstReg);

            var filteredWarranty = _warrantyDb.WarrantyInfo.Where(db => db.Franchise == vehicle.Franchise);

            filteredWarranty = FilterOutMileageTooHigh(filteredWarranty, mileage);
            filteredWarranty = FilterOutNotWithinMonthOfLife(filteredWarranty, monthOfLife);

            return filteredWarranty;
        }

        /// <summary>
        /// Removes warranties from the selection pool if a vehicle's mileage is outside the warranty limit.
        /// </summary>
        /// <param name="warranties">Warranties to check.</param>
        /// <param name="mileage">The vehicle's mileage.</param>
        /// <returns>Warranties where there is no mileage limit or the mileage is below the limit.</returns>
        private IEnumerable<IWarrantyInfoRow> FilterOutMileageTooHigh(IEnumerable<IWarrantyInfoRow> warranties, int mileage)
        {
            return warranties.Where(w => w.MileageLessThan == null || mileage <= w.MileageLessThan);
        }

        /// <summary>
        /// Removes warranties from the selection pool if the vehicle's month of life is outside a warranty period.
        /// </summary>
        /// <param name="warranties">Warranties to check.</param>
        /// <param name="monthOfLife">The vehicle's month of life.</param>
        /// <returns>Warranties where there is no month of life limit or the month of life is within the limit.</returns>
        private IEnumerable<IWarrantyInfoRow> FilterOutNotWithinMonthOfLife(IEnumerable<IWarrantyInfoRow> warranties, int monthOfLife)
        {
            var validWarranties = warranties;

            validWarranties = validWarranties.Where(w => w.MonthOfLifeLessThan == null || monthOfLife <= w.MonthOfLifeLessThan);

            validWarranties = validWarranties.Where(w => w.MonthOfLifeGreaterThan == null || monthOfLife >= w.MonthOfLifeGreaterThan);

            return validWarranties;
        }
    }
}