namespace Genesis;

public class IPAddressConverter
{
    public static int ConvertToInt(string ipAddress)
    {
        string[] octets = ipAddress.Split('.');
        
        if (octets.Length != 4)
            throw new ArgumentException("Invalid IP address format");

        int result = (int.Parse(octets[0]) << 24) |
                     (int.Parse(octets[1]) << 16) |
                     (int.Parse(octets[2]) << 8)  |
                     int.Parse(octets[3]);

        return result;
    }
}