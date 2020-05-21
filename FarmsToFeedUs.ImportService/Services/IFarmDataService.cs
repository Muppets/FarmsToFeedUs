using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FarmsToFeedUs.ImportService.Services
{
    public interface IFarmDataService
    {
        Task<List<FarmData>> GetFarmDataAsync();
    }

    public class FarmData
    {
        // Farm or business name
        [Index(0)]
        public string? Name { get; set; }

        // Postcode
        [Index(1)]
        public string? Postcode { get; set; }

        // Nearest town
        [Index(2)]
        public string? Town { get; set; }

        // County
        [Index(3)]
        public string? County { get; set; }

        // Website / Instagram / Facebook
        [Index(4)]
        public string? SocialMedia { get; set; }

        // What are you currently offering that is available for sale?
        [Index(5)]
        public string? Product { get; set; }

        // Would you consider your farm / produce to be: choose one of these: Organic, Biodynamic, Regenerative, Other. 
        [Index(6)]
        public string? Feature { get; set; }

        // If you have a product list online, please provide the URL below. 
        [Index(7)]
        public string? Website { get; set; }

        // Are you offering online ordering?
        [Index(8)]
        public string? OnlineOrdering { get; set; }

        // Are you offering delivery? If yes, please state nationally or locally
        [Index(9)]
        public string? Delivery { get; set; }

        // Are you still accepting new customers for online order during the COVID-19 outbreak?
        [Index(10)]
        public string? AcceptingNewCustomers { get; set; }

        // If you are using a local hub for pickup, please provide the details.
        [Index(11)]
        public string? PickupHub { get; set; }

        // Please specify how customers can best access your products during the COVID-19 outbreak.
        [Index(12)]
        public string? PreferredAccess { get; set; }

        // Please provide the email address or telephone number you would like customers to contact you with.
        [Index(13)]
        public string? Contact { get; set; }

        // Do you envisage needing volunteer workers on your land or in your business?
        [Index(14)]
        public string? NeedVolunteers { get; set; }

        // Can you tell us what produce you envisage being in ample supply and in shorter supply than usual in the next weeks and months?   
        [Index(15)]
        public string? AmpleSupply { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
