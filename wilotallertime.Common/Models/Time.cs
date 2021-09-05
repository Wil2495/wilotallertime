using System;

namespace wilotallertime.Common.Models
{
    public class Time
    {
        public int? IdEmployee { get; set; }

        public DateTime? Date { get; set; }

        public int? Type { get; set; }

        public bool Consolidated { get; set; }
    }
}
