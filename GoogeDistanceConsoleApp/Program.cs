using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace GoogeDistanceConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*** Google Maps API - Finding Distances Between Places ***\n");
            Console.WriteLine("This app finds the distance between two places with an API from Google Maps\n");
            Console.Write("Would you like to find the distance between two places? (y/n) >> ");
            char response = Convert.ToChar(Console.ReadLine());

            while (response == 'y')
            {
                FindDistance(); // continues to rest of program
                // proceeds to update LCV because of async
                Console.Write("\nWould you like to find the distance between another two locations? (y/n) >> ");
                response = Convert.ToChar(Console.ReadLine());
            }

            Console.ReadLine();
        }

        static async void FindDistance()
        {
            string[] locationUrls = { BuildUrlForLocationId(), BuildUrlForLocationId() },
                idLocations = new string[2];
            HttpClient http = new HttpClient();

            for (int i = 0; i < idLocations.Length; i++)
            {
                var responseId = await http.GetAsync(locationUrls[i]);

                if (responseId.IsSuccessStatusCode)
                {
                    var result = await responseId.Content.ReadAsStringAsync();
                    RootLocationBase root = JsonConvert.DeserializeObject<RootLocationBase>(result);
                    idLocations[i] = root.results[0].place_id;
                }
                else
                    Console.WriteLine("Connection to Google Places API unsuccessful...");
            }


            var responseDistance = await http.GetAsync(BuildUrlForDistance(idLocations[0], idLocations[1]));

            if (responseDistance.IsSuccessStatusCode)
            {
                var result = await responseDistance.Content.ReadAsStringAsync();
                RootDistanceBase root = JsonConvert.DeserializeObject<RootDistanceBase>(result);
                Console.WriteLine("Distance: " + root.rows[0].elements[0].distance.text + ".");
                Console.WriteLine("Duration: " + root.rows[0].elements[0].duration.text + ".\n");
            }
            else
                Console.WriteLine("Connection to Google Distances Matrix API unsuccessful...");

        }

        static string BuildUrlForLocationId()
        {
            string location = "";
            string[] locationAsArray;
            Console.Write("Enter the address of a location here >> ");
            locationAsArray = (Console.ReadLine()).Split();

            for (int i = 0; i < locationAsArray.Length; i++)
            {
                if (i < locationAsArray.Length - 1)
                    location += locationAsArray[i] + "+";
                else
                    location += locationAsArray[i];
            }

            return "https://maps.googleapis.com/maps/api/place/textsearch/json?key=AIzaSyAe19SG-FjgoyLwiZFXo3V5cWFXO_KKsrg&query=" + location;
        }

        static string BuildUrlForDistance(string place1, string place2)
        {
            string url = "https://maps.googleapis.com/maps/api/distancematrix/json?key=AIzaSyAMxfjuYn6x00PmtahL7skkas0vnQqZblA&units=imperial&origins=";
            return url + "place_id:" + place1 + "&destinations=place_id:" + place2;
        }
    }
}
