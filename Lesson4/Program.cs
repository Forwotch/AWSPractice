using Amazon.EC2;

Console.WriteLine("aws ec2 describe-instances");

var ec2 = new AmazonEC2Client();

var response = await ec2.DescribeInstancesAsync();
Console.WriteLine($"Response status: {response.HttpStatusCode}");

var numberOfReservations = response.Reservations.Count;
Console.WriteLine($"Number of reservations: {numberOfReservations}");
Console.WriteLine();

Console.WriteLine("Instances description:");

if (numberOfReservations > 0)
{
    foreach (var reservation in response.Reservations)
    {
        foreach (var instance in reservation.Instances)
        {
            var name = string.Empty;
            foreach (var tag in instance.Tags)
            {
                if (tag.Key == "Name")
                {
                    name = tag.Value;
                }
            }

            Console.WriteLine($"Name: {name}, IP: {instance.PublicIpAddress}");
        }
    }
}
