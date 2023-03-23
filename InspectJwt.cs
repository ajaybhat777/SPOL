using System.IdentityModel.Tokens.Jwt;

public void InspectToken(string token)
{
    // Create a new JWT token handler
    var tokenHandler = new JwtSecurityTokenHandler();

    // Decode the token and parse the JWT header and payload
    var jwtToken = tokenHandler.ReadJwtToken(token);

    // Print the token header and payload to the console
    Console.WriteLine("Header:");
    Console.WriteLine(jwtToken.Header);

    Console.WriteLine("Payload:");
    foreach (var claim in jwtToken.Claims)
    {
        Console.WriteLine($"{claim.Type}: {claim.Value}");
    }
}
