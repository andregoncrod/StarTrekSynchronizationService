using StarTrekSynchronizationService.Models;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace StarTrekSynchronizationService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly StarTrekContext _dbContext;
        private HttpHelper _httpHelper;

        public Worker(ILogger<Worker> logger, StarTrekContext dbContext)
        {
            _logger = logger;
            _httpHelper = new HttpHelper();
            _dbContext = dbContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {


                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async  void GetAllSpacecraftsFromAPI()
        {
            var response = GetSpacecraftsByPage(1);
            if(response != null)
            {

            }
        }

        private async Task<SpacecraftsPagedAPIDto?> GetSpacecraftsByPage(int pageNumber)
        {
            string url = $"{AppSettings.StarTrekAPIUrl}/spacecraft/search?pageNumber={pageNumber}&pageSize=100";
            string responseJson = await _httpHelper.Get(url);
            if(!string.IsNullOrWhiteSpace(responseJson))
                return JsonSerializer.Deserialize<SpacecraftsPagedAPIDto>(responseJson);
            return null;
        }

        private void InsertResponsesOnDatabase(SpacecraftsPagedAPIDto response)
        {
            if (response == null || response.spacecrafts == null)
                return;

            foreach (var item in response.spacecrafts)
            {
                var newSpacecraft = new Spacecraft()
                {
                    UID = item.uid,
                    Name = item.name,
                    Registry = item.registry,
                    DateStatus = item.dateStatus,
                    Status = item.status,

                };

                _dbContext.Spacecrafts.Add(newSpacecraft);
                _dbContext.SaveChanges();
            }
        }
    }
}