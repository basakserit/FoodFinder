﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Services;
using FoodService.Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace FoodService
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {
         
        
        public List<VenueModel> GetVenue(string latitude, string longitude)
        {
            String jsonData;
            List<VenueModel> venueList = new List<VenueModel>();
            List<VenueModel> venueJsonList = new List<VenueModel>();

            string clientID = ConfigurationSettings.AppSettings["client_id"];
            string clientSecret = ConfigurationSettings.AppSettings["client_secret"];
            string currentDate = DateTime.Now.ToString("yyyyMMdd").ToString();

            using (var client = new WebClient())
            {
                //jsonData = client.DownloadString("https://api.foursquare.com/v2/venues/search?v=&ll=38.42%2C27.13&intent=browse&radius=800&categoryId=4d4b7105d754a06374d81259&client_id=TOMWFBM0IH5Y3XQLE1L41JY22H5OIQO0DKUURDQ5JOKKK4Q3&client_secret=HCGTEWHVFD35NS5ABIKGI3FAXOUCGPUMC5Z0MSVCOYBBBKNX");
                client.Encoding = System.Text.Encoding.UTF8;
                jsonData = client.DownloadString("https://api.foursquare.com/v2/venues/search?v="+ currentDate + "&ll="+ latitude + ","+ longitude + "&intent=browse&radius=800&categoryId=4d4b7105d754a06374d81259&client_id="+ clientID + "&client_secret="+ clientSecret);
            }

            JObject responseData = JObject.Parse(jsonData);

            var secondElement = responseData["response"];
            var venuesElement = secondElement["venues"];
            int VenuesCount = venuesElement.Count();

            for (int i = 0; i < VenuesCount; i++)
            {
                VenueModel vm = new VenueModel();
                var element = venuesElement[i];
                vm.venueId =  element["id"].ToString();
                vm.venueName = element["name"].ToString();
                var locationElement = element["location"];
                vm.address = locationElement["address"].ToString();
                vm.lat = double.Parse(locationElement["lat"].ToString());
                vm.lng = double.Parse(locationElement["lng"].ToString());
                vm.distance = int.Parse(locationElement["distance"].ToString());
                var categories = element["categories"];
                var categoryElement = categories[0];
                vm.categoryId = categoryElement["id"].ToString();
                vm.categoryName = categoryElement["name"].ToString();
                //vm.url = element["url"].ToString() == null ? "No Website" : element["url"].ToString();
                if (element["url"] != null)
                {
                    vm.url = element["url"].ToString();
                }
                else {
                    vm.url = "No Website";
                }

                venueList.Add(vm);
            }
            int cnt = venueList.Count;

            return venueList;
        }

        [WebMethod]
        public string GetVenueJson(string latitude, string longitude)
        {
            List<VenueModel> venueList = new List<VenueModel>();

            venueList = GetVenue(latitude, longitude);
            string jsonVenueList = JsonConvert.SerializeObject(venueList);

            return jsonVenueList;
        }

        public int RandomVenueNumberChooser(List<VenueModel> venueList)
        {
            int listCount = venueList.Count;

            Random rand = new Random(DateTime.Now.Millisecond);

            int number = rand.Next();

            int randomNumber = number % listCount;

            return randomNumber;
        }

        [WebMethod]
        public string RandomVenueChooser(string latitude, string longitude)
        {

            List<VenueModel> venueList = new List<VenueModel>();
            venueList = GetVenue(latitude, longitude);

            int selectedVenueNumber = RandomVenueNumberChooser(venueList);

            VenueModel vm = new VenueModel();
            vm = venueList.ElementAt(selectedVenueNumber);

            string jsonSelectedVenue = JsonConvert.SerializeObject(vm);

            return jsonSelectedVenue;

        }


        }
}
