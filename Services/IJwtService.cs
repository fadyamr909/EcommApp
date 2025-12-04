namespace ECommerceApp.Services
{
    public interface IJwtService
    {
        string GenerateToken(string username, string userId);
    }
}

