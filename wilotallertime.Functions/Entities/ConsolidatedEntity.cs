using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace wilotallertime.Functions.Entities
{
    public class ConsolidatedEntity : TableEntity
    {
        public int IdEmployee { get; set;}

        public DateTime Date { get; set;}

        public int MinutesWork { get; set;}
    }
}
