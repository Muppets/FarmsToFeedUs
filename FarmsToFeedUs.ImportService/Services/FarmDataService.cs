using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FarmsToFeedUs.ImportService.Services
{
    public class FarmDataService : IFarmDataService
    {
        public FarmDataService(IHttpClientFactory httpClientFactory)
        {
            HttpClient = httpClientFactory.CreateClient();
        }

        private HttpClient HttpClient { get; }

        public async Task<List<FarmData>> GetFarmDataAsync()
        {
            var farms = await GetRawDataFromGoogleAsync();

            foreach (var farm in farms.ToList())
            {
                if (string.IsNullOrWhiteSpace(farm.Name))
                {
                    farms.Remove(farm);
                    continue;
                }

                // Clean the data
                farm.Name = CleanField(farm.Name);
                farm.Postcode = CleanField(farm.Postcode);
                farm.Town = CleanField(farm.Town);
                farm.County = CleanField(farm.County);
                farm.SocialMedia = CleanField(farm.SocialMedia);
                farm.Product = CleanField(farm.Product);
                farm.Feature = CleanField(farm.Feature);
                farm.Website = CleanField(farm.Website);
                farm.OnlineOrdering = CleanField(farm.OnlineOrdering);
                farm.Delivery = CleanField(farm.Delivery);
                farm.AcceptingNewCustomers = CleanField(farm.AcceptingNewCustomers);
                farm.PickupHub = CleanField(farm.PickupHub);
                farm.PreferredAccess = CleanField(farm.PreferredAccess);
                farm.Contact = CleanField(farm.Contact);
                farm.NeedVolunteers = CleanField(farm.NeedVolunteers);
                farm.AmpleSupply = CleanField(farm.AmpleSupply);
            }

            MergeDuplicates(farms);

            return farms;
        }

        private async Task<List<FarmData>> GetRawDataFromGoogleAsync()
        {
            var responseMessage = await HttpClient.GetAsync("https://docs.google.com/spreadsheets/d/1gIuV4OztjUWlOV5sx4S9iD_gAUfUvlho5_0r4F1zYZo/export?format=csv&id=1gIuV4OztjUWlOV5sx4S9iD_gAUfUvlho5_0r4F1zYZo&gid=1829502908");
            responseMessage.EnsureSuccessStatusCode();

            using var csvStream = await responseMessage.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(csvStream);

            // The first row is an intro
            streamReader.ReadLine();

            // The second row is the headers
            var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
            using var csvReader = new CsvReader(streamReader, csvConfiguration);

            var records = csvReader.GetRecords<FarmData>();

            return records.ToList();
        }

        private string? CleanField(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim();

            if (value == "-")
                return null;

            return value;
        }

        private void MergeDuplicates(List<FarmData> farms)
        {
            // Some rows are duplicated, merge them together to 
            // get the most information into a single record

            var duplicateNames = farms.GroupBy(f => f.Name)
                                  .Where(g => g.Count() > 1)
                                  .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var duplicateName in duplicateNames)
            {
                FarmData merged = duplicateName.Value.First();

                foreach (var duplicate in duplicateName.Value.Skip(1))
                {
                    merged.Postcode = MostValuable(merged.Postcode, duplicate.Postcode);
                    merged.Town = MostValuable(merged.Town, duplicate.Town);
                    merged.County = MostValuable(merged.County, duplicate.County);
                    merged.SocialMedia = MostValuable(merged.SocialMedia, duplicate.SocialMedia);
                    merged.Product = MostValuable(merged.Product, duplicate.Product);
                    merged.Feature = MostValuable(merged.Feature, duplicate.Feature);
                    merged.Website = MostValuable(merged.Website, duplicate.Website);
                    merged.OnlineOrdering = MostValuable(merged.OnlineOrdering, duplicate.OnlineOrdering);
                    merged.Delivery = MostValuable(merged.Delivery, duplicate.Delivery);
                    merged.AcceptingNewCustomers = MostValuable(merged.AcceptingNewCustomers, duplicate.AcceptingNewCustomers);
                    merged.PickupHub = MostValuable(merged.PickupHub, duplicate.PickupHub);
                    merged.PreferredAccess = MostValuable(merged.PreferredAccess, duplicate.PreferredAccess);
                    merged.Contact = MostValuable(merged.Contact, duplicate.Contact);
                    merged.NeedVolunteers = MostValuable(merged.NeedVolunteers, duplicate.NeedVolunteers);
                    merged.AmpleSupply = MostValuable(merged.AmpleSupply, duplicate.AmpleSupply);

                    farms.Remove(duplicate);
                }
            }
        }

        private string? MostValuable(string? source, string? additional)
            => !string.IsNullOrWhiteSpace(source) ? source : additional;
    }
}
