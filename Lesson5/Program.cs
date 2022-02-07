using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args).Build();

IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

Console.WriteLine("Read credentials");
Console.WriteLine();

var accessKeyId = config.GetValue<string>("accessKeyId");
var secretAccessKey = config.GetValue<string>("secretAccessKey");

Console.WriteLine("Read or create credentials profile");

AWSCredentials awsCredentials;

var chain = new CredentialProfileStoreChain();

if (chain.TryGetAWSCredentials("test-profile", out awsCredentials))
{
    Console.WriteLine("Credentials profile found");
}
else
{
    Console.WriteLine("Could not find credentials profile. Creating");

    var options = new CredentialProfileOptions
    {
        AccessKey = accessKeyId,
        SecretKey = secretAccessKey
    };

    var profile = new CredentialProfile("test-profile", options);
    profile.Region = RegionEndpoint.USEast2;

    var sharedFile = new SharedCredentialsFile();
    sharedFile.RegisterProfile(profile);

    chain.TryGetAWSCredentials("test-profile", out awsCredentials);
}

Console.WriteLine();

var s3 = new AmazonS3Client(awsCredentials);

var request = new GetObjectRequest
{
    BucketName = "roletestz",
    Key = "file.txt"
};

Console.WriteLine($"Attempting to get {request.Key} from bucket: {request.BucketName}");

using var response = await s3.GetObjectAsync(request);
Console.WriteLine(response.HttpStatusCode);

using var reader = new StreamReader(response.ResponseStream);
string content = reader.ReadToEnd();

Console.WriteLine("File: " + response.Key);
Console.WriteLine("Content: " + content);
Console.WriteLine();

await host.RunAsync();
