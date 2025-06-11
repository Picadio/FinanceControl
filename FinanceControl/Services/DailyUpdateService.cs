using FinanceControl.Database;
using FinanceControl.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Services;

// TODO make this by Dedicated Online CRON Job
public class DailyUpdateService : BackgroundService
{
    private readonly ILogger<DailyUpdateService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    
    public DailyUpdateService(ILogger<DailyUpdateService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRunTime = now.Date.AddDays(1);

            if (now > nextRunTime)
            {
                nextRunTime = now.Date.AddDays(1);
            }

            var delay = nextRunTime - now;

            _logger.LogInformation("Daily update service waiting for: {Delay}", delay);
            await Task.Delay(delay, stoppingToken);

            if (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Executing daily update...");
                await PerformDailyUpdate(); 
            }
        }
    }

    private async Task PerformDailyUpdate()
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var now = DateTime.Now;
            var receipts = await dbContext.Receipts
                .Include(receipt => receipt.User)
                .Where(receipt => receipt.Day == now.Day)
                .ToListAsync();
        
            var users = new HashSet<User>();
            foreach (var receipt in receipts)
            {
                var user = receipt.User;
                user.Balance += receipt.Sum;
                users.Add(user);
            }
        
            dbContext.Users.UpdateRange(users);
            await dbContext.SaveChangesAsync();
        }
        
        _logger.LogInformation("Daily update logic executed successfully!");
    }
}