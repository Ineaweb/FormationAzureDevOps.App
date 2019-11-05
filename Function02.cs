using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Onepoint.Function
{  
    public static class Function02
    {
        private static readonly string accountName = Environment.GetEnvironmentVariable("AppStorageName", EnvironmentVariableTarget.Process);
        private static readonly string key = Environment.GetEnvironmentVariable("AppStorageKey", EnvironmentVariableTarget.Process);
        private static readonly string tableName = "Table01";  

        [FunctionName("Function02")]
        public static async Task Run([ServiceBusTrigger("queue01", Connection = "ServiceBusConnectionString02")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            log.LogTrace($"Parse message into BreaknewsData");
            var data = JsonConvert.DeserializeObject<BreaknewsData>(myQueueItem);
            data.ETag = "*";
            log.LogTrace($"Message correctly parsed");

            log.LogTrace($"Open connection into Storage Account");
            var storageCredentials = new StorageCredentials(accountName, key);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, useHttps: true);
            var tableClient = cloudStorageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);

            log.LogTrace($"Insert data into table {tableName}");
            var result = await table.ExecuteAsync(TableOperation.InsertOrReplace(data));
            log.LogTrace($"Data correctly inserted into table {tableName}");
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.11.1.0 (Newtonsoft.Json v12.0.0.0)")]
    public partial class BreaknewsData : TableEntity, System.ComponentModel.INotifyPropertyChanged
    {
        private int? _id;
        private string _partitionKey;
        private string _title;
        private string _shortText;
        private string _messageText;
        private DateTimeOffset _originDateTime;
        private bool _confirmed;

        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? Id
        {
            get
            {
                return _id;
            }

            set
            {
                if (_id != value)
                {
                    _id = value;
                    RaisePropertyChanged();
                }

                RowKey = (value != null) ? value.ToString() : default;
            }
        }

        /// <summary>Clé de partition (par défaut: rules)</summary>
        [Newtonsoft.Json.JsonProperty("partitionKey", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public new string PartitionKey
        {
            get
            {
                return _partitionKey;
            }

            set
            {
                if (_partitionKey != value)
                {
                    _partitionKey = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Newtonsoft.Json.JsonProperty("title", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Newtonsoft.Json.JsonProperty("shortText", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ShortText
        {
            get
            {
                return _shortText;
            }

            set
            {
                if (_shortText != value)
                {
                    _shortText = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Newtonsoft.Json.JsonProperty("messageText", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string MessageText
        {
            get
            {
                return _messageText;
            }

            set
            {
                if (_messageText != value)
                {
                    _messageText = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Newtonsoft.Json.JsonProperty("originDateTime", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public DateTimeOffset OriginDateTime
        {
            get
            {
                return _originDateTime;
            }

            set
            {
                if (_originDateTime != value)
                {
                    _originDateTime = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Newtonsoft.Json.JsonProperty("confirmed", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Confirmed
        {
            get
            {
                return _confirmed;
            }

            set
            {
                if (_confirmed != value)
                {
                    _confirmed = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static BreaknewsData FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<BreaknewsData>(data);
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }    
}
