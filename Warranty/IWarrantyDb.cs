using System;
using System.Collections.Generic;

namespace Warranty
{
    public interface IWarrantyDb
    {
        IEnumerable<IWarrantyInfoRow> WarrantyInfo{ get; }
    }

    public interface IWarrantyInfoRow
    {
        public string Franchise { get; }
        public string AppliesTo { get; set; }

        public byte? MonthOfLifeGreaterThan { get; }
        public byte? MonthOfLifeLessThan { get; }

        public DateTime? RegisteredAfter { get; }
        public DateTime? RegisteredBefore { get; }

        public int? MileageLessThan { get; }
    }
}