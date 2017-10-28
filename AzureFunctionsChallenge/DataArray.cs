using Microsoft.WindowsAzure.Storage.Table;

namespace AzureFunctionsChallenge
{
    public class DataTable : TableEntity
    {
        public int Value { get; set; }
    }
}
