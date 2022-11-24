using System.Net;
using Newtonsoft.Json;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.DynamoDBv2.Model;
using DarkStoreCommonLib.Contracts;
using Amazon.Lambda.APIGatewayEvents;
using DarkStoreCommonLib.Contracts.Requests;

namespace AWSDarkStoreDynamoDB.Lambdas;

public class ActionTableFunctions : BaseLambdaFunction
{
    public ActionTableFunctions()
    { }


    /// <summary>
    /// A Lambda function to respond to HTTP Get methods from API Gateway
    /// </summary>
    /// <param name="request"></param>
    /// <returns>The API Gateway response.</returns>
    public async Task<APIGatewayProxyResponse> CreateTable(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"\n CreateTable Request:{JsonConvert.SerializeObject(request)} \n");

        var actionDb = JsonConvert.DeserializeObject<ActionDynamoDbRequest>(request.Body);

        context.Logger.LogInformation($"\n \n TableName = '{actionDb?.TableName}', Action = '{actionDb?.Action}'");

        if (string.IsNullOrWhiteSpace(actionDb?.TableName))
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = $"Check the request data. TableName = '{actionDb?.TableName}', Action = '{actionDb?.Action}'",
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };

        DescribeTableResponse tableResp = new DescribeTableResponse();

        var client = new AmazonDynamoDBClient();
        string tableName = actionDb.TableName;
        string partitionKey = actionDb.PartitionKey;
        string sortKey = actionDb.SortKey;

        #region Creating Table

        var response = await client.CreateTableAsync(new CreateTableRequest
        {
            TableName = tableName,
            AttributeDefinitions = new List<AttributeDefinition>()
                                              {
                                                new AttributeDefinition
                                                {
                                                  AttributeName = partitionKey,
                                                  AttributeType = "S",
                                                },
                                                new AttributeDefinition
                                                {
                                                    AttributeName = sortKey,
                                                    AttributeType = "N",
                                                },
                                              },
            KeySchema = new List<KeySchemaElement>()
                                              {
                                                new KeySchemaElement
                                                {
                                                  AttributeName = partitionKey,
                                                  KeyType = "HASH",
                                                },
                                                new KeySchemaElement
                                                {
                                                  AttributeName = sortKey,
                                                  KeyType = "RANGE",
                                                },
                                              },
            BillingMode = "PAY_PER_REQUEST"
            //,ProvisionedThroughput = new ProvisionedThroughput
            //{
            //    ReadCapacityUnits = 1,
            //    WriteCapacityUnits = 1,
            //}
        });

        tableResp = await WaitForTableToBeCreated(client, tableName, response.TableDescription.TableStatus);

        #endregion


        if (string.IsNullOrWhiteSpace(tableResp.Table.TableName))
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.NotModified,
                Body = $"Action '{actionDb.Action}' for table '{actionDb.TableName}' has not information about finishing",
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };

        return new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = $"Action '{actionDb.Action}' has been done on the table '{tableResp.Table.TableName}'. Status of the table is {tableResp.Table.TableStatus}",
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };
    }

    public async Task<APIGatewayProxyResponse> DeleteTable(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"\n DeleteTable Request:{JsonConvert.SerializeObject(request)} \n");

        var actionDb = JsonConvert.DeserializeObject<ActionDynamoDbRequest>(request.Body);

        context.Logger.LogInformation($"\n \n TableName = '{actionDb?.TableName}', Action = '{actionDb?.Action}'");

        if (string.IsNullOrWhiteSpace(actionDb?.TableName))
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = $"Check the request data. TableName = '{actionDb?.TableName}', Action = '{actionDb?.Action}'",
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };

        DescribeTableResponse tableResp = new DescribeTableResponse();

        var client = new AmazonDynamoDBClient();
        string tableName = actionDb.TableName;

        #region Deleting Table

        var requestDel = new DeleteTableRequest
        {
            TableName = tableName,
        };

        var response = await client.DeleteTableAsync(requestDel);

        tableResp = await WaitForTableToBeDeleted(client, tableName);

        #endregion

        return new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = $"Action '{actionDb.Action}' has been done on the table '{tableResp?.Table?.TableName}'. Status of the table is {tableResp?.Table?.TableStatus}",
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };
    }


    private static async Task<DescribeTableResponse> WaitForTableToBeCreated(
                                      IAmazonDynamoDB client,
                                      string tableName,
                                      string tableStatus)
    {
        DescribeTableResponse resp = new DescribeTableResponse();
        string waitingForStatus = "ACTIVE";

        int sleepDuration = 1000; // One second

        // Don't wait more than 10 seconds.
        while (tableStatus != waitingForStatus && sleepDuration < 10000)
        {
            resp = await client.DescribeTableAsync(new DescribeTableRequest
            {
                TableName = tableName,
            });

            tableStatus = resp.Table.TableStatus;

            Thread.Sleep(sleepDuration);
            sleepDuration *= 2;
        }

        return resp;
    }

    private static async Task<DescribeTableResponse> WaitForTableToBeDeleted(AmazonDynamoDBClient client, string tableName)
    {
        bool tablePresent = true;
        DescribeTableResponse resp = new DescribeTableResponse();

        while (tablePresent)
        {
            try
            {
                resp = await client.DescribeTableAsync(new DescribeTableRequest
                {
                    TableName = tableName
                });

                var tableStatus = resp.Table.TableStatus;
            }
            catch (ResourceNotFoundException)
            {
                tablePresent = false;
            }

            Thread.Sleep(5000); // Wait 5 seconds.
        }

        return resp;

    }

}