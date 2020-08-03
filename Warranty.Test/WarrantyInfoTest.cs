using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Warranty.Test
{
    [TestClass]
    public class WarrantyInfoTest
    {
        private Mock<IWarrantyDb> _db;
        private IList<WarrantyInfoRowStub> _warrantyInfoRows;

        [TestInitialize]
        public void Initialize()
        {
            _db = new Mock<IWarrantyDb>();
            _warrantyInfoRows = new List<WarrantyInfoRowStub>();
            _db.SetupGet(db => db.WarrantyInfo).Returns(_warrantyInfoRows);
        }

        [TestMethod]
        public void ReturnsWarrantyInfo_WhenFranchiseMatches()
        {
            var vehicleLookup = new Mock<IVehicleLookup>(MockBehavior.Strict);
            var ford = new Vehicle(regNumber: "FO20RD1", "Ford", dateOfFirstReg: new DateTime(2020, 03, 1));
            var toyota = new Vehicle(regNumber: "TO20YOT", "Toyota", dateOfFirstReg: new DateTime(2020, 03, 1));

            vehicleLookup.Setup(s => s.Vehicle("FO20RD1")).Returns(ford);
            vehicleLookup.Setup(s => s.Vehicle("TO20YOT")).Returns(toyota);

            _warrantyInfoRows.Add(new WarrantyInfoRowStub()
            {
                Franchise = "Ford",
                AppliesTo = "All Models",
                MonthOfLifeGreaterThan = 0,
                MonthOfLifeLessThan = 24
            });
            _warrantyInfoRows.Add(new WarrantyInfoRowStub()
            {
                Franchise = "Toyota",
                AppliesTo = "All Models",
                MonthOfLifeGreaterThan = 0,
                MonthOfLifeLessThan = 24
            });

            var warrantyInfo = new WarrantyInfo(vehicleLookup.Object, _db.Object);

            var fordResult = warrantyInfo.Query("FO20RD1", default);
            var toyotaResult = warrantyInfo.Query("TO20YOT", default);

            Assert.AreEqual("Ford", fordResult.First().Franchise);
            Assert.AreEqual("Toyota", toyotaResult.First().Franchise);
        }

        [TestMethod]
        public void ReturnsWarrantyInfo_WhenWithinMonthOfLifeLimit()
        {
            var vehicleLookup = new Mock<IVehicleLookup>(MockBehavior.Strict);
            var older = new Vehicle(regNumber: "FO10RD1", "Ford", dateOfFirstReg: new DateTime(2010, 03, 01));
            var newer = new Vehicle(regNumber: "FO20RD1", "Ford", dateOfFirstReg: new DateTime(2020, 03, 01));

            vehicleLookup.Setup(s => s.Vehicle("FO10RD1")).Returns(older);
            vehicleLookup.Setup(s => s.Vehicle("FO20RD1")).Returns(newer);

            _warrantyInfoRows.Add(new WarrantyInfoRowStub()
            {
                Franchise = "Ford",
                AppliesTo = "All Models",
                MonthOfLifeGreaterThan = 0,
                MonthOfLifeLessThan = 24
            });

            var warrantyInfo = new WarrantyInfo(vehicleLookup.Object, _db.Object);

            var olderResult = warrantyInfo.Query("FO10RD1", default);
            var newerResult = warrantyInfo.Query("FO20RD1", default);

            Assert.IsFalse(olderResult.Any());
            Assert.IsTrue(newerResult.Any());
        }

        [DataTestMethod]
        [DataRow(0, 24, 100000)]
        [DataRow(null, 24, 100000)]
        [DataRow(null, null, 100000)]
        public void ReturnsWarrantyInfo_WhenWithinMonthOfLife_AndMileageLimit(int? monthOfLifeGreaterThan, int? monthOfLifeLessThan, int? mileageLessThan)
        {
            var vehicle = new Vehicle(regNumber: "FO20RD1", "Ford", dateOfFirstReg: new DateTime(2020, 03, 1));

            var vehicleLookupService = new Mock<IVehicleLookup>(MockBehavior.Strict);
            vehicleLookupService.Setup(s => s.Vehicle("FO20RD1")).Returns(vehicle);

            _warrantyInfoRows.Add(new WarrantyInfoRowStub()
            {
                Franchise = "Ford",
                AppliesTo = "All Models",
                MonthOfLifeGreaterThan = (byte?)monthOfLifeGreaterThan,
                MonthOfLifeLessThan = (byte?)monthOfLifeLessThan,
                MileageLessThan = mileageLessThan
            });

            var warrantyInfo = new WarrantyInfo(vehicleLookupService.Object, _db.Object);

            var result = warrantyInfo.Query("FO20RD1", 50000);

            Assert.IsNotNull(result.First());
        }

        [TestMethod]
        public void ReturnsWarrantyInfo_WhenWithinRegistrationPeriod()
        {
            var vehicle = new Vehicle(regNumber: "FO20RD1", "Ford", dateOfFirstReg: new DateTime(2020, 03, 1));

            var vehicleLookupService = new Mock<IVehicleLookup>(MockBehavior.Strict);
            vehicleLookupService.Setup(s => s.Vehicle("FO20RD1")).Returns(vehicle);

            _warrantyInfoRows.Add(new WarrantyInfoRowStub()
            {
                Franchise = "Ford",
                AppliesTo = "All Models",
                RegisteredAfter = new DateTime(2020, 01, 01),
                RegisteredBefore = new DateTime(2022, 12, 31),
                MileageLessThan = default
            });

            var warrantyInfo = new WarrantyInfo(vehicleLookupService.Object, _db.Object);

            var result = warrantyInfo.Query("FO20RD1", 50000);

            Assert.IsNotNull(result.First());
        }
    }

    public class WarrantyInfoRowStub : IWarrantyInfoRow
    {
        public string Franchise { get; set; }
        public string AppliesTo { get; set; }
        public byte? MonthOfLifeGreaterThan { get; set; }
        public byte? MonthOfLifeLessThan { get; set; }
        public DateTime? RegisteredAfter { get; set; }
        public DateTime? RegisteredBefore { get; set; }
        public int? MileageLessThan { get; set; }
    }
}
