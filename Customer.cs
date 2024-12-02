namespace JsonCompare;

public class Customer{
    public string Name { get; set; }
    public int Orders { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string EmailAddress { get; set; }
    public string IpAddress { get; set; }
    public Address[] Addresses { get; set; }
}

public class Address {
    public string Street { get; set; }
    public string City { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
}