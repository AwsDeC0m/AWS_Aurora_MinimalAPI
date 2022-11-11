using System.Net;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using AwsDarkStoreCloudFormation.Db;
using Amazon.Lambda.APIGatewayEvents;
using DarkStoreAWServerless.Services;
using DarkStoreCommonLib.Contracts.Requests;

namespace AwsDarkStoreLambdas.Lambdas;

public class PostStoreFunction : BaseLambdaFunction
{
    public APIGatewayProxyResponse PostStore(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"\n PostStore Request:{JsonConvert.SerializeObject(request)} \n");

        var storeInfo = JsonConvert.DeserializeObject<StoreInfoRequest>(request.Body);

        DarkStoreService service = new DarkStoreService(new AppDbContext());

        var storesID = service.PostStore(storeInfo);

        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = $"Store has been created with id = {storesID}",
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };

        return response;
    }
}
