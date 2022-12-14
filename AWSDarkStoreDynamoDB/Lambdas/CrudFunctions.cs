using System.Net;
using Amazon.Runtime;
using Newtonsoft.Json;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;
using DarkStoreCommonLib.DynamoDB;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.DynamoDBv2.DocumentModel;

/*
 https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DotNetSDKMidLevel.html
 */

namespace AWSDarkStoreDynamoDB.Lambdas;

public class CrudFunctions : BaseLambdaFunction
{
    private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();
    private DarkStoreDynamo storeBody = new();
    private Document document;

    public CrudFunctions()
    { }


    public async Task<APIGatewayProxyResponse> CreateItem(APIGatewayProxyRequest request, ILambdaContext context)
    {
        if (!RequestValid(request, context))
            return GenerealResponse(HttpStatusCode.BadRequest, "Check the request data.");

        try
        {
            switch (storeBody.RequestCommandMode)
            {
                case 1:
                    var storeInfo = new Document();
                    storeInfo["City"] = storeBody.City;
                    storeInfo["NrStore"] = storeBody.NrStore;
                    storeInfo["Square"] = storeBody.Options.Square;
                    storeInfo["Address"] = storeBody.Options.Address;
                    storeInfo["ParkingSize"] = storeBody.Options.ParkingSize;
                    storeInfo["HasGroceries"] = new DynamoDBBool(storeBody.Options.HasGroceries);
                    storeInfo["HasHouseholdGoods"] = new DynamoDBBool(storeBody.Options.HasHouseholdGoods);

                    Table darkStoreDB = Table.LoadTable(client, storeBody.TableName);
                    await darkStoreDB.PutItemAsync(storeInfo);

                    break;

                case 2:
                    var requestPut = new PutItemRequest
                    {
                        TableName = storeBody.TableName,
                        Item = new Dictionary<string, AttributeValue>
                    {
                        { "City", new AttributeValue { S = storeBody.City} },
                        { "NrStore", new AttributeValue { N = storeBody.NrStore.ToString()} },
                        {
                            "StoreOptions",
                            new AttributeValue
                            {
                                M = new Dictionary<string, AttributeValue>
                                {
                                    { "Address", new AttributeValue { S = storeBody.Options.Address } },
                                    { "Square", new AttributeValue { N = storeBody.Options.Square.ToString() } },
                                    { "ParkingSize", new AttributeValue{ N = storeBody.Options.ParkingSize.ToString() } },
                                    { "HasGroceries", new AttributeValue{ BOOL = storeBody.Options.HasGroceries } },
                                    { "HasHouseholdGoods", new AttributeValue { BOOL = storeBody.Options.HasHouseholdGoods } },
                                }
                            }
                        }
                    },
                        // this condition expression will not allow updates,
                        // it will only succeed if the record does not already exist
                        ConditionExpression = "attribute_not_exists(NrStore)",
                    };

                    context.Logger.LogInformation($"requestPut::{JsonConvert.SerializeObject(requestPut)}");

                    var response = await client.PutItemAsync(requestPut);

                    break;
                default:
                    break;
            }
        }
        catch (AmazonDynamoDBException e)
        { return GenerealResponse(HttpStatusCode.NotModified, $" #1 (by mode = {storeBody.RequestCommandMode}) Error: {e.Message}."); }
        catch (AmazonServiceException e)
        { return GenerealResponse(HttpStatusCode.NotModified, $" #2 (by mode = {storeBody.RequestCommandMode}) Error: {e.Message}."); }
        catch (Exception e)
        { return GenerealResponse(HttpStatusCode.NotModified, $" #3 (by mode = {storeBody.RequestCommandMode}) Error: {e.Message}. {e.InnerException}"); }


        return GenerealResponse(HttpStatusCode.OK, $"Item has been created (by mode = {storeBody.RequestCommandMode}).");
    }


    public async Task<APIGatewayProxyResponse> ReadItem(APIGatewayProxyRequest request, ILambdaContext context)
    {
        if (!RequestValid(request, context))
            return GenerealResponse(HttpStatusCode.BadRequest, "Check the request data.");

        try
        {
            switch (storeBody.RequestCommandMode)
            {
                case 1:

                    context.Logger.LogInformation($"Mode #1 storeBody = {storeBody}");

                    var requestGet = new GetItemRequest
                    {
                        TableName = storeBody.TableName,
                        Key = new Dictionary<string, AttributeValue>
                    {
                        {"City",  new AttributeValue {S = storeBody.City} },
                        {"NrStore",  new AttributeValue {N = storeBody.NrStore.ToString()} }
                    }
                    };

                    var response = await client.GetItemAsync(requestGet);

                    document = Document.FromAttributeMap(response.Item);

                    return GenerealResponse(HttpStatusCode.OK, $"Items has been read. >>> Doc: {JsonConvert.SerializeObject(document)}");

                    break;

                case 2:

                    context.Logger.LogInformation($"Mode #2 storeBody = {storeBody}");

                    var requestBatch = new BatchGetItemRequest
                    {
                        RequestItems = new Dictionary<string, KeysAndAttributes> {
                        {
                            storeBody.TableName,    //Table to read 
                            new KeysAndAttributes   //Definition of the keys to retrieve
                            {
                                Keys = new List<Dictionary<string, AttributeValue>>
                                {
                                    new Dictionary<string, AttributeValue> {
                                        { "City", new AttributeValue { S = "London" }},
                                        { "NrStore", new AttributeValue { N = "1"}}
                                     },
                                    new Dictionary<string, AttributeValue> {
                                        { "City", new AttributeValue { S = "London" }},
                                        { "NrStore", new AttributeValue { N = "3"}}
                                     },
                                    new Dictionary<string, AttributeValue> {
                                        { "City", new AttributeValue { S = "Rome" }},
                                        { "NrStore", new AttributeValue { N = "13" }}
                                     },
                                    new Dictionary<string, AttributeValue> {
                                        { "City", new AttributeValue { S = "Paris" }},
                                        { "NrStore", new AttributeValue { N = "34" }}
                                     },
                                    new Dictionary<string, AttributeValue> {
                                        { "City", new AttributeValue { S = "Amsterdam" }},
                                        { "NrStore", new AttributeValue { N = "33" }}
                                    },
                                },
                                //If you don't need consistent reads use "false" instead to have cheaper retrieve prices
                                ConsistentRead = false,
                            }
                        }   //You can add more tables to read here.
                    },
                        ReturnConsumedCapacity = "TOTAL"
                    };

                    var responseBatch = await client.BatchGetItemAsync(requestBatch);

                    List<Document> docResult = new();
                    foreach (var item in responseBatch.Responses[storeBody.TableName])
                    {
                        docResult.Add(Document.FromAttributeMap(item));
                    }

                    return GenerealResponse(HttpStatusCode.OK, $"Items has been read. >>> Doc: {JsonConvert.SerializeObject(docResult)} ");

                    break;

                case 3:

                    context.Logger.LogInformation($"Mode #3 storeBody = {storeBody}");

                    DynamoDBContext dynamoDbContext = new DynamoDBContext(client,
                                  new DynamoDBContextConfig
                                  {
                                      ConsistentRead = false,
                                      IgnoreNullValues = true
                                  });

                    var storeInfo = await dynamoDbContext.LoadAsync<DarkStoreDynamo>(storeBody.City, storeBody.NrStore);

                    if (storeInfo != null)
                        return GenerealResponse(HttpStatusCode.OK, $"Item: {JsonConvert.SerializeObject(storeInfo)}");

                    return GenerealResponse(HttpStatusCode.BadRequest, $"Bad Request");

                    break;

                default:
                    break;
            }
        }
        catch (AmazonDynamoDBException e)
        { return GenerealResponse(HttpStatusCode.NotModified, $" #1 (by mode = {storeBody.RequestCommandMode}) Error: {e.Message}."); }
        catch (AmazonServiceException e)
        { return GenerealResponse(HttpStatusCode.NotModified, $" #2 (by mode = {storeBody.RequestCommandMode}) Error: {e.Message}."); }
        catch (Exception e)
        { return GenerealResponse(HttpStatusCode.NotModified, $" #3 (by mode = {storeBody.RequestCommandMode}) Error: {e.Message}. {e.InnerException}"); }


        return GenerealResponse(HttpStatusCode.OK, $"Items has been read.");
    }


    public async Task<APIGatewayProxyResponse> UpdateItem(APIGatewayProxyRequest request, ILambdaContext context)
    {
        if (!RequestValid(request, context))
            return GenerealResponse(HttpStatusCode.BadRequest, "Check the request data.");

        try
        {
            // Define the name of a user account to update.
            // Note that in this example, we have to alias "name"
            // using ExpressionAttributeNames as name is a reserved word in DynamoDB.
            var requestUpd = new UpdateItemRequest
            {
                TableName = storeBody.TableName,
                Key = new Dictionary<string, AttributeValue>
                    {
                        { "City", new AttributeValue { S = storeBody.City} },
                        { "NrStore", new AttributeValue { N = storeBody.NrStore.ToString()} }
                    },
                UpdateExpression = "set #bool = :boolvalue, #addr = :addrvalue",
                ConditionExpression = "#parking > :minsize",
                ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        { "#bool", nameof(storeBody.Options.HasGroceries) },
                        { "#addr", nameof(storeBody.Options.Address) },
                        { "#parking", nameof(storeBody.Options.ParkingSize) }
                    },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        { ":boolvalue", new AttributeValue {BOOL = storeBody.Options.HasGroceries} },
                        { ":addrvalue", new AttributeValue {S = storeBody.Options.Address} },
                        { ":minsize", new AttributeValue {N = "100"} }
                    },
                ReturnValues = ReturnValue.ALL_NEW
            };

            var response = await client.UpdateItemAsync(requestUpd);
            Document docUpdated = Document.FromAttributeMap(response.Attributes);


            return GenerealResponse(HttpStatusCode.OK, $"Item has been updated. Response: {JsonConvert.SerializeObject(docUpdated)}");

        }
        catch (AmazonDynamoDBException e)
        { return GenerealResponse(HttpStatusCode.NotModified, $" #1 (by mode = {storeBody.RequestCommandMode}) Error: {e.Message}. {e.InnerException}"); }
        catch (AmazonServiceException e)
        { return GenerealResponse(HttpStatusCode.NotModified, $" #2 (by mode = {storeBody.RequestCommandMode}) Error: {e.Message}. {e.InnerException}"); }
        catch (Exception e)
        { return GenerealResponse(HttpStatusCode.NotModified, $" #3 (by mode = {storeBody.RequestCommandMode}) Error: {e.Message}. {e.InnerException}"); }


        return GenerealResponse(HttpStatusCode.OK, "Item has been updated.");
    }


    public async Task<APIGatewayProxyResponse> DeleteItem(APIGatewayProxyRequest request, ILambdaContext context)
    {
        if (!RequestValid(request, context))
            return GenerealResponse(HttpStatusCode.BadRequest, "Check the request data.");

        try
        {
            var requestDel = new DeleteItemRequest
            {
                TableName = storeBody.TableName,
                Key = new Dictionary<string, AttributeValue>
                    {
                        { "City", new AttributeValue { S = storeBody.City} },
                        { "NrStore", new AttributeValue { N = storeBody.NrStore.ToString()} }
                    },
                ConditionExpression = "#parking = :minsize",
                ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        { "#parking", nameof(storeBody.Options.ParkingSize) }
                    },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        { ":minsize", new AttributeValue {N = "100"} }
                    }

            };

            var response = await client.DeleteItemAsync(requestDel);

            return GenerealResponse(HttpStatusCode.OK, $"Item has been deleted. Response: {JsonConvert.SerializeObject(response)}");

        }
        catch (AmazonDynamoDBException e)
        { return GenerealResponse(HttpStatusCode.NotModified, $" #1  Error: {e.Message}. {e.InnerException}"); }
        catch (AmazonServiceException e)
        { return GenerealResponse(HttpStatusCode.NotModified, $" #2  Error: {e.Message}. {e.InnerException}"); }
        catch (Exception e)
        { return GenerealResponse(HttpStatusCode.NotModified, $" #3  Error: {e.Message}. {e.InnerException}"); }


        return GenerealResponse(HttpStatusCode.OK, "Item has been deleted.");
    }




    private APIGatewayProxyResponse GenerealResponse(HttpStatusCode httpStatus = HttpStatusCode.OK, string body = "")
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = (int)httpStatus,
            Body = body,
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };
    }

    private bool RequestValid(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"CreateTable Request:{JsonConvert.SerializeObject(request)}");

        storeBody = JsonConvert.DeserializeObject<DarkStoreDynamo>(request.Body);

        context.Logger.LogInformation($"table = '{storeBody?.TableName}', pk = '{storeBody?.City}', sk = '{storeBody?.NrStore}'");

        if (string.IsNullOrWhiteSpace(storeBody?.TableName) || string.IsNullOrWhiteSpace(storeBody?.City) || storeBody?.NrStore <= 0)
            return false;

        return true;
    }






}





