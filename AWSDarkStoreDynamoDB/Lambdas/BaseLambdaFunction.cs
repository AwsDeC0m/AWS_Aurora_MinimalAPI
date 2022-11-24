using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSDarkStoreDynamoDB.Lambdas;

public class BaseLambdaFunction
{ }
