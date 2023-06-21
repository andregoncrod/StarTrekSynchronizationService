using Microsoft.Extensions.DependencyInjection;
using StarTrekSynchronizationService.Models;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace StarTrekSynchronizationService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HttpHelper _httpHelper;
        private readonly IServiceProvider _serviceProvider;


        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _httpHelper = new HttpHelper();
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<StarTrekContext>();
                    await GetAllSpacecraftsFromAPI(dbContext);
                }
                
                _logger.LogInformation("Worker stop running at: {time}", DateTimeOffset.Now);

                DateTime now = DateTime.Now;
                DateTime nextMidnight = now.Date.AddDays(1);
                await Task.Delay(nextMidnight - now, stoppingToken);
            }
        }

        private async Task<bool> GetAllSpacecraftsFromAPI(StarTrekContext dbContext)
        {
            var response = await GetSpacecraftsByPage(1);
            if (response != null)
            {
                InsertResponsesOnDatabase(dbContext, response);
                if (response.page != null && response.page.totalPages > 1)
                {
                    for (int i = 2; i <= response.page.totalPages; i++)
                    {
                        var responseNextPages = await GetSpacecraftsByPage(i);
                        if (responseNextPages != null)
                        {
                            InsertResponsesOnDatabase(dbContext, responseNextPages);
                        }
                    }
                }
            }
            return true;
        }

        private async Task<SpacecraftsPagedAPIDto?> GetSpacecraftsByPage(int pageNumber)
        {
            string url = $"{AppSettings.StarTrekAPIUrl}/spacecraft/search?pageNumber={pageNumber}&pageSize=100";
            string responseJson = await _httpHelper.Get(url);
            if (!string.IsNullOrWhiteSpace(responseJson))
                return JsonSerializer.Deserialize<SpacecraftsPagedAPIDto>(responseJson);
            return null;
        }

        private void InsertResponsesOnDatabase(StarTrekContext dbContext, SpacecraftsPagedAPIDto response)
        {
            bool newInserted = false;
            if (response == null || response.spacecrafts == null)
                return;

            foreach (var item in response.spacecrafts)
            {
                var existingSpacecraft = dbContext.Spacecrafts.FirstOrDefault(s => s.UID == item.uid);
                if (existingSpacecraft == null)
                {
                    string[] words = { "Apple", "Banana", "Carrot", "Donut", "Elephant", "Frog" };

                    Random random = new Random();
                    int index = random.Next(words.Length);
                    string randomWord = words[index];

                    var newSpacecraft = new Spacecraft()
                    {
                        UID = item.uid,
                        Name = $"{item.name} {randomWord}",
                        Registry = item.registry,
                        DateStatus = item.dateStatus,
                        Status = item.status,
                        Deleted = false,
                        SystemDate = DateTime.Now,
                        LastChange = null
                    };

                    dbContext.Spacecrafts.Add(newSpacecraft);
                    newInserted = true;
                }
            }

            if (newInserted)
                dbContext.SaveChanges();
        }
    }
}