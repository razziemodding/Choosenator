using Newtonsoft.Json.Linq;
// ReSharper disable All

class choosenator {
    
    private static IList<string> names = new List<string>();
     
    static async Task Main() {
        Console.WriteLine("Welcome to the CHOOSENATOR setup process.");
        Console.WriteLine("INSTRUCTIONS:");
        Console.WriteLine("Log into steam on your primary browser.");
        Console.WriteLine("Once complete, go to this website: https://store.steampowered.com/dynamicstore/userdata/");
        Console.WriteLine("In the top left, select \"Raw Data\"");
        Console.WriteLine("Below where you've just selected, push the \"Copy\" button.");
        Console.WriteLine("Once copied, paste all data below.");
        Console.WriteLine("Input Steam user-data .json (copy/paste from website):");
        string data = Console.ReadLine();

        IList<int> ids = parseData(data);

        await getAppIdNames(ids);
        beginChoosenator(names, ids);
    }

    static IList<int> parseData(String input) {
        JObject json =  JObject.Parse(input);
        
        IList<JToken> appIds = json["rgWishlist"].Children().ToList();
        IList<int> ids = new List<int>();

        foreach (JToken appId in appIds) {
            int temp = appId.ToObject<int>();
            ids.Add(temp);
            //Console.WriteLine(appId.ToString());
        }

        return ids;
    }

    static async Task getAppIdNames(IList<int> appIds) {
        Console.WriteLine("Converting appIds to game names...");
        
        HttpClient client = new HttpClient();
        IList<string> appNames = new List<string>();

        foreach (int appId in appIds) {
            HttpResponseMessage appData =
                await client.GetAsync("https://store.steampowered.com/api/appdetails/?appids=" + appId);
            string temp = await appData.Content.ReadAsStringAsync();
            try {
                JObject appDataJson = JObject.Parse(temp);

                JToken appName = appDataJson[appId.ToString()]["data"].Children().ElementAt(1);
                Console.WriteLine(appName.ToObject<string>());
                names.Add(appName.ToObject<string>());
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine("This is typically caused by the Steam API failing to provide data...");
                Console.WriteLine("...i think.");
            }
        }
        
        Console.WriteLine("Found " + names.Count + " game names.");
        Console.WriteLine("Initiating CHOOSENATOR.");
        Console.WriteLine();
    }

    static void beginChoosenator(IList<string> names, IList<int> ids) {
        IList<string> gamePass = new List<string>();
        
        Console.WriteLine(" --- CHOOSENATOR INITIATED --- ");
        Console.WriteLine("You will now be presented with two games at a time.");
        Console.WriteLine("Type in '1' or '2', depending on which one you like more.");
        Console.WriteLine("The one you select will STAY in the pool.");
        Console.WriteLine("The one you do NOT select will be REMOVED from the pool.");
        Console.WriteLine("This will repeat until you are left with one game.");
        Console.WriteLine("Are you ready? Press the 'enter' key to begin the CHOOSENATOR.");
        Console.ReadLine();
        
        while (gamePass.Count != 1) {
            //Console.WriteLine("while start (count): " + names.Count);
            int limit = names.Count - 2;
            for (int i = 0; i <= limit; i += i + 2 > limit ? 1 : 2) {
                
                //Console.WriteLine("Count: " + i + " / " + limit);
                Console.WriteLine("1:");
                Console.WriteLine("Name: " + names[i]);
                Console.WriteLine("Store page: https://store.steampowered.com/app/" +  ids[i]);
                Console.WriteLine("2:");
                Console.WriteLine("Name: " + names[i + 1]);
                Console.WriteLine("Store page: https://store.steampowered.com/app/" +  ids[i + 1]);
                string choice = Console.ReadLine();

                switch (choice) {
                    case "1":
                        //Console.WriteLine("debug 1");
                        gamePass.Add(names[i]);
                        break;
                    case "2":
                        //Console.WriteLine("debug 2");
                        gamePass.Add(names[i + 1]);
                        break;
                }
                
               //Console.WriteLine("end for");
            }
            //Console.WriteLine("gamePass length: " +  gamePass.Count);
            names = gamePass.ToList();
            if (gamePass.Count != 1) {
                gamePass.Clear();
            }
            
            /*
            if (gamePass.Count == 1) {
                Console.WriteLine("finished");
                game = gamePass.First();
                Console.WriteLine("game set");
                running = false;
                Console.WriteLine("while loop stop");
            } else {
                Console.WriteLine("cont");
                names = gamePass;
                gamePass.Clear();
                Console.WriteLine("cont2");
            }
            */
        }
        
        Console.WriteLine(ids.Count);
        
        Console.WriteLine("The CHOOSENATOR has finished instruction.");
        Console.WriteLine("Congratulations on your selected game:");
        Console.WriteLine("Name: " + gamePass.First());
        Console.WriteLine("Store link: WIP");
        
        Console.WriteLine("Push enter to close the program.");
        Console.ReadLine();
        /*
        create a class where the name and ids of a game are both stored in said class
        to make retrieval of either easier.
        */
    }
}