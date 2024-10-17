using Microsoft.Extensions.Diagnostics.HealthChecks;
using UserService.Interfaces;

public class ReadinessHealthCheck : IHealthCheck
{
    private readonly IUserService _userService;
    private int failChance;


    public ReadinessHealthCheck(IUserService userService)
    {
        _userService = userService;
        failChance = int.Parse(Environment.GetEnvironmentVariable("DatabaseFailChance") ?? "0");
    }

    /// <summary>
    /// Simulates a failed connection to an external database.
    /// </summary>
    /// <returns></returns>
    private bool ExternalDataBaseConnection()
    {
        int random = Random.Shared.Next(0, 100);
        if (random < failChance)
        {
            return false;
        }
        return true;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ExternalDataBaseConnection())
            {
                throw new Exception($"Connection to database failed: {failChance}% Fail Chance");
            }

            // Get all users from the database
            var users = await _userService.GetAllUsers();

            // Return healthy if it succeds
            string successMessage = "User Service Readiness success";
            Console.WriteLine(successMessage);
            return HealthCheckResult.Healthy(successMessage);
        }
        catch (Exception exception)
        {
            // If there was an exception. Database down? return error message
            string errorMessage = $"User Service Readiness failed: {exception.Message}";
            Console.WriteLine(errorMessage);
            return HealthCheckResult.Unhealthy(errorMessage);
        }
    }
}