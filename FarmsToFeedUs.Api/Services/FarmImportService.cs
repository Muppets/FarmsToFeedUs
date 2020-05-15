using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FarmsToFeedUs.Api.Services
{
    public class FarmImportService
    {
        public FarmImportService(IHttpClientFactory httpClientFactory)
        {
            HttpClient = httpClientFactory.CreateClient();
        }

        private HttpClient HttpClient { get; }

        public async Task<List<FarmData>> GetFarmData()
        {
            var farms = await GetRawDataFromGoogleAsync();

            foreach (var farm in farms)
            {
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

        private string CleanField(string value)
        {
            if (value == null)
                return null;

            value = value.Trim();

            if (value == "-")
                return null;

            return value;
        }
    }

    public class FarmData
    {
        // Farm or business name
        [Index(0)]
        public string Name { get; set; }

        // Postcode
        [Index(1)]
        public string Postcode { get; set; }

        // Nearest town
        [Index(2)]
        public string Town { get; set; }

        // County
        [Index(3)]
        public string County { get; set; }

        // Website / Instagram / Facebook
        [Index(4)]
        public string SocialMedia { get; set; }

        // What are you currently offering that is available for sale?
        [Index(5)]
        public string Product { get; set; }

        // Would you consider your farm / produce to be: choose one of these: Organic, Biodynamic, Regenerative, Other. 
        [Index(6)]
        public string Feature { get; set; }

        // If you have a product list online, please provide the URL below. 
        [Index(7)]
        public string Website { get; set; }

        // Are you offering online ordering?
        [Index(8)]
        public string OnlineOrdering { get; set; }

        // Are you offering delivery? If yes, please state nationally or locally
        [Index(9)]
        public string Delivery { get; set; }

        // Are you still accepting new customers for online order during the COVID-19 outbreak?
        [Index(10)]
        public string AcceptingNewCustomers { get; set; }

        // If you are using a local hub for pickup, please provide the details.
        [Index(11)]
        public string PickupHub { get; set; }

        // Please specify how customers can best access your products during the COVID-19 outbreak.
        [Index(12)]
        public string PreferredAccess { get; set; }

        // Please provide the email address or telephone number you would like customers to contact you with.
        [Index(13)]
        public string Contact { get; set; }

        // Do you envisage needing volunteer workers on your land or in your business?
        [Index(14)]
        public string NeedVolunteers { get; set; }

        // Can you tell us what produce you envisage being in ample supply and in shorter supply than usual in the next weeks and months?   
        [Index(15)]
        public string AmpleSupply { get; set; }
    }
}
