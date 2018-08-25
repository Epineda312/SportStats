using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace SoccerStats
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo directory = new DirectoryInfo(currentDirectory);

            //Specify .csv file
            var fileName = Path.Combine(directory.FullName, "SoccerGameResults.csv");
            var fileContents = ReadSoccerResults(fileName);
            
            //Specify .Json file
            fileName = Path.Combine(directory.FullName, "players.json");
            var players = DeserializePlayers(fileName);

            //Retrieve top ten players, write top players stats in seperate Json file
            var topTenPlayers = GetTopTenPlayers(players);
            foreach(var player in topTenPlayers)
            {
                Console.WriteLine("Name: " + player.FirstName + " PPG: " + player.PointsPerGame);
            }
            fileName = Path.Combine(directory.FullName, "topten.json");
            SerializePlayerToFile(topTenPlayers, fileName);
        }


        //Initializes stream reader with specified csv file
        public static string ReadFile(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                return reader.ReadToEnd();
            }
        }

        //Create list of GameResult objects
        public static List<GameResult> ReadSoccerResults(string fileName)
        {          
            var soccerResults = new List<GameResult>();
            using (var reader = new StreamReader(fileName))
            {
                string line = "";
                reader.ReadLine();               
                while ((line = reader.ReadLine()) != null)
                {
                    var gameResult = new GameResult();
                    string[] values = line.Split(',');                  

                    //Parse file to receive gameDate 
                    DateTime gameDate;
                    if (DateTime.TryParse(values[0], out gameDate))
                    {
                        gameResult.GameDate = gameDate;
                    }
                    gameResult.TeamName = values[1];

                    //Parse file to receive homeOrAway 
                    HomeOrAway homeOrAway;
                    if (Enum.TryParse(values[2], out homeOrAway))
                    {
                        gameResult.HomeOrAway = homeOrAway;
                    }

                    //Parse file to receive all int values
                    int parseInt;
                    if (int.TryParse(values[3], out parseInt))
                    {
                        gameResult.Goals = parseInt;
                    }
                    if (int.TryParse(values[4], out parseInt))
                    {
                        gameResult.GoalAttempts = parseInt;
                    }
                    if (int.TryParse(values[5], out parseInt))
                    {
                        gameResult.ShotsOnGoal = parseInt;
                    }
                    if (int.TryParse(values[6], out parseInt))
                    {
                        gameResult.ShotsOffGoal = parseInt;
                    }

                    //Parse file to receive PossessionPercent value
                    double possessionPercent;
                    if(double.TryParse(values[7], out possessionPercent))
                    {
                        gameResult.PossesionPercent = possessionPercent;
                    }
                    
                    soccerResults.Add(gameResult);
                }
            }
            return soccerResults;
        }

        //Deserialize Json file
        public static List<Player> DeserializePlayers(string fileName)
        {
            var players = new List<Player>();
            var serializer = new JsonSerializer();
            using (var reader = new StreamReader(fileName))
            using(var jsonReader = new JsonTextReader(reader))
            {
                players = serializer.Deserialize<List<Player>>(jsonReader);
            }
                
            return players;
        }

        //Sort top ten players from json file
        public static List<Player> GetTopTenPlayers(List<Player> players)
        {
            var topTenPlayers = new List<Player>();
            players.Sort(new PlayerComparer());
            int counter = 0;
            foreach(var player in players)
            {
                topTenPlayers.Add(player);
                counter++;
                if (counter == 10)
                    break;
            }
            return topTenPlayers;
        }

        //Write sorted top ten players list to a json file of its own
        public static void SerializePlayerToFile(List<Player> players, string fileName)
        {          
            var serializer = new JsonSerializer();
            using (var writer = new StreamWriter(fileName))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                serializer.Serialize(jsonWriter, players);
            }

        }

    }
}

