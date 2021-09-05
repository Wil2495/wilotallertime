using System;

namespace wilotallertime.Common.Models
{
    internal class Time
    {
        public int? IdEmployee { get; set; }

        public DateTime? Date { get; set; }

        public int? Type { get; set; }

        public bool Consolidated { get; set; }
    }
}
