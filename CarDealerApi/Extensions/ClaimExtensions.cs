namespace Extensions;

public static class ClaimExtensions
{
    public static int GetDealerId(this HttpContext context)
    {
        var claim = context.User.Claims.First(c => c.Type == "dealerId")?.Value;
        if (claim == null) throw new UnauthorizedAccessException();
        return int.Parse(claim);
    }
}