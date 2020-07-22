using System;

namespace Warranty
{
    public class Vehicle
    {
        public string RegNumber { get; }
        public DateTime DateOfFirstReg { get; }
        public string Franchise { get; }

        public Vehicle(string regNumber, string franchise, DateTime dateOfFirstReg)
        {
            RegNumber = regNumber;
            Franchise = franchise;
            DateOfFirstReg = dateOfFirstReg;
        }
    }
}