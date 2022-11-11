using System.Net;
using Newtonsoft.Json;
using AwsDarkStoreCloudFormation.Db;
using Amazon.Lambda.APIGatewayEvents;
using DarkStoreAWServerless.Services;

namespace AwsDarkStoreLambdas.Lambdas;

public class GetStoresFunction : BaseLambdaFunction
{
    public APIGatewayProxyResponse GetStores(APIGatewayProxyRequest request)
    {
        DarkStoreService service = new DarkStoreService(new AppDbContext());

        var stores = service.GetStoresResponse();

        if (stores == null || stores.Count == 0)
            return new APIGatewayProxyResponse()
            {
                StatusCode = (int)HttpStatusCode.NotFound,
            };

        return new APIGatewayProxyResponse()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonConvert.SerializeObject(stores)
        };
    }

    public APIGatewayProxyResponse GetStore(APIGatewayProxyRequest request)
    {
        if (request.PathParameters == null || !request.PathParameters.ContainsKey("id"))
            throw new ArgumentException($"{nameof(APIGatewayProxyRequest)} has not parameter 'id'.");

        
        Guid.TryParse(request.PathParameters["id"], out var id);

        DarkStoreService service = new DarkStoreService(new AppDbContext());

        var store = service.GetStore(id);

        if (store == null)
            return new APIGatewayProxyResponse()
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Body = "Called GetStore"
            };

        return new APIGatewayProxyResponse()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonConvert.SerializeObject(store)
        };
    }

}