using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace $safeprojectname$.IdentityServer
{
    public class ConfigurationDbSeedData
    {
        private readonly ConfigurationDbContext context;
        private readonly ILogger<ConfigurationDbSeedData> logger;

        public ConfigurationDbSeedData(ConfigurationDbContext context, ILogger<ConfigurationDbSeedData> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task SeedData()
        {
            logger.LogInformation("Seeding identityserver database ...");

            await SeedClients();
            await SeedIdentityResources();
            await SeedApiResources();

            logger.LogInformation("Done seeding identityserver database.");
        }

        private async Task SeedClients()
        {
            logger.LogInformation("Update Clients ...");

            IEnumerable<Client> configClients = IdentityConfiguration.Clients.Select(item => item.ToEntity());
            IQueryable<Client> currentClients = context.Clients.AsQueryable();

            IEnumerable<string> configClientIds = configClients.Select(item => item.ClientName);
            IEnumerable<string> currentClientIds = currentClients.Select(item => item.ClientName);

            // delete non-existent in configuration
            IEnumerable<Client> deleteClients = currentClients.Where(item => !configClientIds.Contains(item.ClientName));
            context.Clients.RemoveRange(deleteClients);

            // add new clients
            IEnumerable<string> newClientIds = configClientIds.Except(currentClientIds);
            IEnumerable<Client> newClients = configClients.Where(item => newClientIds.Contains(item.ClientName));
            context.Clients.AddRange(newClients);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error : {ex.Message}");
                throw;
            }

            await SeedCientClaims();

            logger.LogInformation("Clients updated.");
        }


        private async Task SeedCientClaims()
        {
            logger.LogInformation("Update ClientClaims ...");

            IEnumerable<Client> configClients = IdentityConfiguration.Clients.Select(item => item.ToEntity());
            IQueryable<Client> currentClients = context.Clients.Include($"{nameof(Client.Claims)}").AsQueryable();

            foreach (var configClient in configClients)
            {
                Client currentClient = currentClients.First(item => item.ClientName == configClient.ClientName);

                IEnumerable<string> configClientTypes = configClient.Claims.Select(item => item.Type);
                IEnumerable<string> currentClientTypes = currentClients.First(item => item.ClientName == configClient.ClientName)
                    .Claims.Select(item => item.Type);

                // delete non-existent in configuration      
                foreach (var item in currentClient.Claims.Where(item => !configClientTypes.Contains(item.Type)).ToList())
                {
                    currentClient.Claims.Remove(item);
                }

                // add new ClientClaims
                IEnumerable<string> newClientIds = configClientTypes.Except(currentClientTypes);
                IEnumerable<ClientClaim> newClaims = configClient.Claims.Where(item => newClientIds.Contains(item.Type));
                currentClient.Claims.AddRange(newClaims);
            }

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error : {ex.Message}");
                throw;
            }

            logger.LogInformation("Client claims updated.");
        }

        private async Task SeedIdentityResources()
        {
            var existingIdentityResources = await context.IdentityResources.Select(i => i.Name).ToListAsync();
            var missingIdentityResources = IdentityConfiguration.IdentityResources.Where(c => existingIdentityResources.All(name => name != c.Name)).ToList();
            if (missingIdentityResources.Any())
            {
                logger.LogInformation("Update IdentityResources ...");
                foreach (var resource in missingIdentityResources)
                {
                    await context.IdentityResources.AddAsync(resource.ToEntity());
                }
                await context.SaveChangesAsync();
            }
            else
            {
                logger.LogInformation("IdentityResources updated.");
            }
        }

        private async Task SeedApiResources()
        {
            logger.LogInformation("Update ApiResources ...");

            IEnumerable<ApiResource> configApiResources = IdentityConfiguration.ApiResources.Select(item => item.ToEntity());
            IQueryable<ApiResource> currentApiResources = context.ApiResources.AsQueryable();

            IEnumerable<string> configApiResourceNames = configApiResources.Select(item => item.Name);
            IEnumerable<string> currentApiResourceNames = currentApiResources.Select(item => item.Name);

            // delete non-existent in configuration          
            IEnumerable<ApiResource> deleteApiResources = currentApiResources.Where(item => !configApiResourceNames.Contains(item.Name));
            context.ApiResources.RemoveRange(deleteApiResources);

            // add new ApiResources
            IEnumerable<string> newApiResourceNames = configApiResourceNames.Except(currentApiResourceNames);
            IEnumerable<ApiResource> newApiResources = configApiResources.Where(item => newApiResourceNames.Contains(item.Name));
            context.ApiResources.AddRange(newApiResources);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error : {ex.Message}");
                throw;
            }

            await SeedApiResourcesUserClaims();

            logger.LogInformation("ApiResources updated.");
        }

        private async Task SeedApiResourcesUserClaims()
        {
            logger.LogInformation("Update ApiResourcesUserClaims");

            IEnumerable<ApiResource> configApiResources = IdentityConfiguration.ApiResources.Select(item => item.ToEntity());
            IQueryable<ApiResource> currentApiResources = context.ApiResources.Include($"{nameof(ApiResource.UserClaims)}").AsQueryable();

            foreach (var configApiResource in configApiResources)
            {
                ApiResource currentApiResource = currentApiResources.First(item => item.Name == configApiResource.Name);

                IEnumerable<string> configApiClaimsNames = configApiResource.UserClaims.Select(item => item.Type);
                IEnumerable<string> currentApiClaimsNames = currentApiResources.First(item => item.Name == configApiResource.Name)
                    .UserClaims.Select(item => item.Type);

                // delete non-existent in configuration       
                foreach (var item in currentApiResource.UserClaims.Where(item => !configApiClaimsNames.Contains(item.Type)).ToList())
                    currentApiResource.UserClaims.Remove(item);

                // add new ApiResourcesUserClaims
                IEnumerable<string> newApiClaimsNames = configApiClaimsNames.Except(currentApiClaimsNames);
                IEnumerable<ApiResourceClaim> newClaims = configApiResource.UserClaims.Where(item => newApiClaimsNames.Contains(item.Type));
                currentApiResource.UserClaims.AddRange(newClaims);
            }

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error : {ex.Message}");
                throw;
            }

            logger.LogInformation("ApiResourcesUserClaims updated");
        }
    }
}
