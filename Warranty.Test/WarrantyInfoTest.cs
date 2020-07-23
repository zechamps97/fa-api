using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

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
        public void ReturnsWarrantyInfo_BasedOnFranchise()
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

            Assert.AreEqual("Ford", fordResult.Franchise);
            Assert.AreEqual("Toyota", toyotaResult.Franchise);
        }

        [TestMethod]
        public void ReturnsWarrantyInfo_WhenWithinMonthOfLifeLimit()
        {
            var vehicleLookup = new Mock<IVehicleLookup>(MockBehavior.Strict);
            var older = new Vehicle(regNumber: "FO20RD1", "Ford", dateOfFirstReg: new DateTime(2006, 03, 1));
            var newer = new Vehicle(regNumber: "TO20YOT", "Toyota", dateOfFirstReg: new DateTime(2020, 03, 1));

            vehicleLookup.Setup(s => s.Vehicle("FO20RD1")).Returns(older);
            vehicleLookup.Setup(s => s.Vehicle("TO20YOT")).Returns(newer);

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
                MonthOfLifeLessThan = 36
            });

            var warrantyInfo = new WarrantyInfo(vehicleLookup.Object, _db.Object);

            var olderResult = warrantyInfo.Query("FO20RD1", default);
            var newerResult = warrantyInfo.Query("TO20YOT", default);

            Assert.IsNull(olderResult);
            Assert.IsNotNull(newerResult);
        }

        [TestMethod]
        public void ReturnsWarrantyInfo_WhenWithinMonthOfLife_AndMileageLimit()
        {
            var vehicle = new Vehicle(regNumber: "FO20RD1", "Ford", dateOfFirstReg: new DateTime(2020, 03, 1));

            var vehicleLookupService = new Mock<IVehicleLookup>(MockBehavior.Strict);
            vehicleLookupService.Setup(s => s.Vehicle("FO20RD1")).Returns(vehicle);

            _warrantyInfoRows.Add(new WarrantyInfoRowStub()
            {
                Franchise = "Ford",
                AppliesTo = "All Models",
                MonthOfLifeGreaterThan = 0,
                MonthOfLifeLessThan = 24,
                MileageLessThan = 100000
            });

            var warrantyInfo = new WarrantyInfo(vehicleLookupService.Object, _db.Object);

            var result = warrantyInfo.Query("FO20RD1", 50000);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReturnsWarrantyInfo_AlwaysFail()
        {
            Assert.Fail("STOP THE DEPLOYMENT!");
        }
    }

    public class WarrantyInfoRowStub : IWarrantyInfoRow
    {
        public string Franchise { get; set; }
        public string AppliesTo { get; set; }
        public short? MonthOfLifeGreaterThan { get; set; }
        public short? MonthOfLifeLessThan { get; set; }
        public DateTime? RegisteredAfter { get; set; }
        public DateTime? RegisteredBefore { get; set; }
        public int? MileageLessThan { get; set; }
    }
}
