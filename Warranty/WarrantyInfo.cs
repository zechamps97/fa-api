using FleetAssist.Common.Date.MonthOfLife;
using System;
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

        public IWarrantyInfoRow? Query(string regNumber, int mileage)
        {
            var vehicle = _vehicleLookup.Vehicle(regNumber);

            var monthOfLife = new MonthOfLifeConverter().ToMonthOfLife(vehicle.DateOfFirstReg);

            var franchiseWarranty = _warrantyDb.WarrantyInfo.Where(db => db.Franchise == vehicle.Franchise);

            return franchiseWarranty.FirstOrDefault(w => 
                AgeIsWithinWarranty(w, monthOfLife) &&
                MileageIsWithinWarranty(w, mileage)
            );

        }

        /// <summary>
        /// Checks if a vehicle's mileage is within a warranty limit.
        /// </summary>
        /// <param name="row">A warranty.</param>
        /// <param name="mileage">The vehicle's mileage.</param>
        /// <returns>True if within warranty limit, false if not.</returns>
        private bool MileageIsWithinWarranty(IWarrantyInfoRow row, int mileage)
        {
            if (row.MileageLessThan.HasValue)
            {
                return mileage <= row.MileageLessThan;
            }
            else
            {
                return true; //Unlimited mileage?
            }
        }

        /// <summary>
        /// Checks if the vehicle's month of life is within a warranty period.
        /// </summary>
        /// <param name="row">A warranty.</param>
        /// <param name="monthOfLife">The vehicle's month of life.</param>
        /// <returns>True if within warranty period, false if not.</returns>
        private bool AgeIsWithinWarranty(IWarrantyInfoRow row, int monthOfLife)
        {
            return
                monthOfLife >= row.MonthOfLifeGreaterThan &&
                monthOfLife <= row.MonthOfLifeLessThan
            ;
        }
    }
}