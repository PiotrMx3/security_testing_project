using DungeonGame.Infrastructure;
using DungeonGame.Interfaces;
using DungeonGame.Security;
using DungeonGame.Services;
using DungeonGame.UI;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DungeonGame
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // ===================================================================
            // 1. INITIALISEER DE CONFIGURATIE (Nu als eerste stap!)
            // ===================================================================
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // ===================================================================
            // SEC-13: AUTOMATISCHE KAMER INITIALISATIE (SILENT SEEDER)
            // ===================================================================
            string roomsFolder = "Rooms";
            string room1Path = Path.Combine(roomsFolder, "room1.enc");
            string room2Path = Path.Combine(roomsFolder, "room2.enc");

            if (!Directory.Exists(roomsFolder) || !File.Exists(room1Path) || !File.Exists(room2Path))
            {
                Directory.CreateDirectory(roomsFolder);

                // Veilig de keys en passphrases uitlezen uit de gitignored appsettings.json
                string key1 = configuration["SeedSettings:Room1:Key"] ?? "";
                string pass1 = configuration["SeedSettings:Room1:Passphrase"] ?? "";
                string key2 = configuration["SeedSettings:Room2:Key"] ?? "";
                string pass2 = configuration["SeedSettings:Room2:Passphrase"] ?? "";

                // Alleen genereren als de configuratie ook daadwerkelijk is ingevuld
                if (!string.IsNullOrEmpty(key1) && !string.IsNullOrEmpty(pass1))
                {
                    string text1 = "You have entered the legendary treasure room! A massive golden chest sparkles in the torchlight, overflowing with gems and ancient artifacts. You made it!";
                    AesEncryptionService.EncryptRoomFile("room1", key1, pass1, text1);
                }

                if (!string.IsNullOrEmpty(key2) && !string.IsNullOrEmpty(pass2))
                {
                    string text2 = "You enter a terrifying, pitch-black cave. The air is thick with smoke, and a colossal red Dragon stands in the center, guarding the path south!";
                    AesEncryptionService.EncryptRoomFile("room2", key2, pass2, text2);
                }
            }
            // ===================================================================
            // START VAN DE ECHTE GAME LOGICA & DEPENDENCY INJECTION PIPELINE
            // ===================================================================

            // 2. Verkrijg de API URL uit de lokale configuratie.
            // We gebruiken een fallback naar poort 7100 (HTTP) om te matchen met de lokale API-instellingen van het team.
            string apiUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7100/";

            // 3. Instantieer de AuthService.
            // OPGELET: We maken deze service bewust leeg aan zonder HttpClient om een circulaire 
            // dependency te voorkomen (AuthService -> HttpClient -> AuthHandler -> AuthService).
            var authService = new AuthService();

            // 4. Bouw de HTTP Message Handler Pipeline.
            // De 'AuthHandler' acteert als onze custom HTTP middleware op de client. Hij vangt elk uitgaand 
            // verzoek op en injecteert automatisch de JWT Bearer token zodra de speler is ingelogd (SEC-12).
            var authHandler = new AuthHandler(authService)
            {
                InnerHandler = new HttpClientHandler()
                {
                    // SSL-bypass guard: Voorkomt dat zelfondertekende certificaten op localhost de HttpClient direct laten crashen.
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                }
            };

            // 5. Configureer de centrale HttpClient.
            // Door de 'authHandler' hier als root-handler mee te geven, forceren we dat áLLE uitgaande API-calls 
            // (inclusief keyshare opvragingen) automatisch geauthenticeerd zijn via de middleware pipeline.
            var httpClient = new HttpClient(authHandler)
            {
                BaseAddress = new Uri(apiUrl)
            };

            // 6. Los de circulaire dependency handmatig op (Property Injection).
            // Nu de HttpClient volledig is geconfigureerd met de middleware handler, koppelen we hem terug aan de AuthService.
            authService.SetClient(httpClient);

            // 7. Render de ASCII Art & Introductie
            Console.WriteLine(
                "═══════════════════════════════════════════\n" +
                "        DUNGEON OF NO RETURN\n" +
                "═══════════════════════════════════════════\n" +
                " You wake up in the dark. Cold stone beneath\n" +
                " your hands. Somewhere ahead — a growl.\n" +
                "\n" +
                " Find the treasure. Don't die trying.\n" +
                "═══════════════════════════════════════════\n" +
                " Commands: help | look | inventory\n" +
                "           go n|e|s|w | take <item>\n" +
                "           fight | quit\n" +
                "═══════════════════════════════════════════");

            // SEC-11: Toegangspoort & Identity Management.
            // We blokkeren de start van de game-loop volledig totdat de gebruiker succesvol is ingelogd 
            // of een geldig nieuw account heeft geregistreerd via de interactive UI flow.
            IAuthService finalAuth = await LoginFlow.Execute(authService);

            // SEC-12: Instantieer de RoomUnlockService.
            // We geven de beveiligde httpClient mee zodat de service de keyshares kan opvragen bij de API.
            var roomUnlockService = new RoomUnlockService(
                configuration,
                httpClient);

            // 8. Initialiseer de core Game State.
            Game game = new Game(finalAuth.Username ?? "Player", finalAuth);

            Console.WriteLine($"\nWelcome, {finalAuth.Username}! Type 'help' for commands.\n");

            // ===================================================================
            // CORE GAME LOOP
            // ===================================================================
            while (!game.IsGameOver())
            {
                Console.Write("> ");
                string input = Console.ReadLine() ?? "";
                Console.WriteLine();

                // Verwerk het commando asynchroon. Alle sub-logica omtrent beweging,
                // gevechten en cryptografie is gedelegeerd naar de Command Handler.
                await HandleCommand(input, game, roomUnlockService);

                // Controleer na elke speleractie of de winconditie is getriggerd.
                game.CheckWin();
            }

            // 9. Afhandeling van de Game-Over State
            if (game.Player.IsWinner)
            {
                Console.WriteLine($"\n{game.Player.Name}, you escaped the dungeon! You win!");
            }
            else
            {
                Console.WriteLine("\nYou died. Game over!");
            }
        }

        /// <summary>
        /// Gecentraliseerde Command Handler. Verwerkt console-invoer en koppelt deze 
        /// direct aan de bijbehorende business- en securitylogica.
        /// </summary>
        private static async Task HandleCommand(string input, Game game, RoomUnlockService roomUnlockService)
        {
            // Splits de invoer op in een 'command' (actie) en 'parts' (argument/richting/item)
            string[] parts = input.Trim().Split(" ", 2);
            string command = parts[0].ToLower();

            if (command == "help")
            {
                Console.WriteLine(game.Help());
            }
            else if (command == "look")
            {
                Console.WriteLine(game.Look());
            }
            else if (command == "inventory")
            {
                Console.WriteLine(game.ShowInventory());
            }
            else if (command == "about" && parts.Length == 2 && parts[1].ToLower() == "me")
            {
                Console.WriteLine(game.AboutMe());
            }
            else if (command == "go" && parts.Length == 2)
            {
                Direction? direction = DirectionHelper.Parse(parts[1]);

                // Validatie-guard: Bestaat de richting of de exit überhaupt?
                if (direction == null || !game.Rooms.CurrentRoom.HasExit(direction.Value))
                {
                    Console.WriteLine("You can't go that way!");
                    return;
                }

                IRoom nextRoom = game.Rooms.CurrentRoom.Exits[direction.Value];

                // Cryptografische Barrière Controle (SEC-13 / SEC-14)
                if (nextRoom.IsEncrypted)
                {
                    // SEC-14: Admin Privilege Escalation / Noclip Bypass.
                    // Als de ingelogde gebruiker de rol 'Admin' heeft, mag hij de fysieke barrière negeren.
                    // De content in de kamer blijft echter cryptografisch intact totdat de sleutel matcht.
                    if (game.AuthService?.UserRole == "Admin")
                    {
                        Console.WriteLine("[Admin Noclip] As an admin, you bypass the encryption barrier and enter the room. Content remains encrypted.");
                    }
                    else
                    {
                        Console.WriteLine("This room is encrypted.");
                        Console.Write("Enter passphrase: ");

                        string passphrase = Console.ReadLine() ?? "";

                        // SEC-13 & SEC-15: Symmetrische Ontgrendeling & Foutafhandeling.
                        // We schieten de roomId en de ingetypte passphrase naar de service. Deze haalt de keyshare 
                        // op via de API, berekent de SHA256-controlehash, vergelijkt deze met de verwachte hash 
                        // uit de appsettings en voert de AES-256 decryptie uit op het lokale .enc bestand.
                        string? decryptedText = await roomUnlockService.UnlockAndDecryptRoomAsync(
                            nextRoom.EncryptionRoomId!,
                            passphrase);

                        // Veilige afbreking (SEC-15): Bij een foute passphrase, netwerkfout of corrupte padding 
                        // geeft de service null/fout terug. We blokkeren de speler direct zonder te crashen.
                        if (decryptedText == null || decryptedText.StartsWith("[Fout]") || decryptedText.StartsWith("[Security]"))
                        {
                            Console.WriteLine("Access denied. Wrong passphrase or server error.");
                            return;
                        }

                        // SEC-13: Decryptie Geslaagd!
                        // De ontsleutelde tekst overschrijft de cryptische bytes en de vlag gaat permanent uit.
                        nextRoom.Description = decryptedText;
                        nextRoom.IsEncrypted = false;
                        Console.WriteLine("Room successfully decrypted!");
                    }
                }

                // Voer de daadwerkelijke fysieke verplaatsing van de speler uit in de engine
                bool moved = game.Move(parts[1]);

                if (!game.Player.IsAlive)
                {
                    Console.WriteLine("You entered a deadly room...");
                }
                else if (moved)
                {
                    Console.WriteLine("You moved to " + game.Rooms.CurrentRoom.Name);
                }
                else
                {
                    Console.WriteLine("You can't go that way!");
                }
            }
            else if (command == "take" && parts.Length == 2)
            {
                bool picked = game.Take(parts[1]);

                if (picked)
                {
                    Console.WriteLine("You picked up " + parts[1]);
                }
                else
                {
                    Console.WriteLine("You can't take that!");
                }
            }
            else if (command == "fight")
            {
                bool hasMonster = game.Rooms.CurrentRoom.Monster != null
                                  && game.Rooms.CurrentRoom.Monster.IsAlive;

                if (!hasMonster)
                {
                    Console.WriteLine("There is nothing to fight here.");
                }
                else
                {
                    bool won = game.Fight();
                    Console.WriteLine(won ? "You defeated the monster!" : "You died fighting!");
                }
            }
            else if (command == "quit")
            {
                game.Player.Health = 0;
                Console.WriteLine("You gave up...");
            }
            else
            {
                Console.WriteLine("Unknown command. Type 'help' for a list of commands.");
            }
        }
    }
}