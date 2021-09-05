using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace wilotallertime.Functions.Entities
{
    public class TimeEntity : TableEntity
    {
        public int IdEmployee { get; set; }

        public DateTime Date { get; set; }

        public int Type { get; set; }

        public bool Consolidated { get; set; }
    }
}
