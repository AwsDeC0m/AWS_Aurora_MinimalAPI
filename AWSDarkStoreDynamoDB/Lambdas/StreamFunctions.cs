using Newtonsoft.Json;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.DynamoDBv2.Model;
using DarkStoreCommonLib.DynamoDB;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.DynamoDBv2.DocumentModel;

namespace AWSDarkStoreDynamoDB.Lambdas
{
    public class StreamFunctions : BaseLambdaFunction
    {
        private readonly DynamoDBContext _dynamoDBContext;

        public StreamFunctions()
        {
            _dynamoDBContext = new DynamoDBContext(new AmazonDynamoDBClient());
        }

        public void StreamHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
        {
            context.Logger.LogInformation($"Beginning to process {dynamoEvent.Records.Count} records...");

            foreach (var record in dynamoEvent.Records)
            {
                context.Logger.LogInformation($"Event ID: {record.EventID}");
                context.Logger.LogInformation($"Event Name: {record.EventName}");


                var odlImage = GetImage<DarkStoreDynamoSimple>(record.Dynamodb.OldImage);
                context.Logger.LogInformation($"OldImage >> : {JsonConvert.SerializeObject(odlImage)}");

                var newImage = GetImage<DarkStoreDynamoSimple>(record.Dynamodb.NewImage);
                context.Logger.LogInformation($"NewImage >> : {JsonConvert.SerializeObject(newImage)}");


                if (newImage.ParkingSize == 222)
                {
                    context.Logger.LogInformation($"ERROR !!!");
                    throw new Exception($"Unexpectible ParkingSize value = {newImage.ParkingSize }");
                }

            }

            context.Logger.LogInformation("Stream processing complete.");
        }

        private T GetImage<T>(Dictionary<string, AttributeValue> dict)
        {
            var docImage = Document.FromAttributeMap(dict);
            return _dynamoDBContext.FromDocument<T>(docImage);
        }


    }
}
