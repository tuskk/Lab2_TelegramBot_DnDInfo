using System;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json.Linq;

namespace DnDTelegramBot
{
    class Program
    {
        private static ITelegramBotClient botClient;
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly Random random = new Random();

        static async Task Main(string[] args)
        {
            string botToken = "BOT TOKEN";
            botClient = new TelegramBotClient(botToken);

            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");

            using var cts = new CancellationTokenSource();
            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions
                {
                    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
                },
                cancellationToken: cts.Token
            );

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            cts.Cancel();
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is Message message && message.Text != null)
            {
                Console.WriteLine($"Received a text message in chat {message.Chat.Id}.");

                string responseMessage = await HandleMessage(message);
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: responseMessage,
                    cancellationToken: cancellationToken
                );
            }
        }

        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception);
            return Task.CompletedTask;
        }

        private static async Task<string> HandleMessage(Message message)
        {
            string messageText = message.Text.ToLower();
            long chatId = message.Chat.Id;

            switch (messageText.Split(' ')[0])
            {
                case "/classes":
                    return "Available classes:\n/barbarian - Barbarian\n/bard - Bard\n/cleric - Cleric\n/druid - Druid\n/fighter - Fighter\n/monk - Monk\n/paladin - Paladin\n/ranger - Ranger\n/rogue - Rogue\n/sorcerer - Sorcerer\n/warlock - Warlock\n/wizard - Wizard";
                case "/start":
                    return "Welcome to the D&D Bot! Use /help to see available commands.";
                case "/help":
                    return "Available commands:\n/start - Welcome message\n/help - List of commands\n/spell [spell name] - Get details of a spell\n/classes - List all classes\n/{class} - Get details and picture of a class\n/roll [dice] - Roll the specified dice (e.g. /roll 2d6)";
                case "/roll":
                    if (messageText.Length > 6)
                    {
                        string diceExpression = messageText.Substring(6); // Extract the dice expression
                        return RollDice(diceExpression);
                    }
                    else
                    {
                        return "Please provide a dice expression. Usage: /roll [dice] (e.g. /roll 2d6)";
                    }

                // Case statements for classes
                case "/barbarian":
                    await SendClassImage(chatId, "https://www.dndbeyond.com/attachments/thumbnails/0/679/400/417/c3barbarianintro.png");
                    return "Barbarian is a fierce warrior of primitive background who can enter a battle rage.";
                case "/bard":
                    await SendClassImage(chatId, "https://www.dndbeyond.com/attachments/thumbnails/0/684/400/406/c3bardintro.png");
                    return "Bard is a master of song, speech, and the magic they contain.";
                case "/cleric":
                    await SendClassImage(chatId, "https://www.dndbeyond.com/attachments/thumbnails/0/687/380/437/c3clericintro.png");
                    return "Cleric is a priestly champion who wields divine magic in service of a higher power.";
                case "/druid":
                    await SendClassImage(chatId, "https://www.dndbeyond.com/attachments/thumbnails/0/693/400/399/c3druidintro.png");
                    return "Druid is a priest of the Old Faith, wielding the powers of nature and adopting animal forms.";
                case "/fighter":
                    await SendClassImage(chatId, "https://www.dndbeyond.com/attachments/thumbnails/0/697/400/475/c3fighterintro.png");
                    return "Fighter is a master of martial combat, skilled with a variety of weapons and armor.";
                case "/monk":
                    await SendClassImage(chatId, "https://www.dndbeyond.com/attachments/thumbnails/0/700/400/490/c3monkintro.png");
                    return "Monk is a master of martial arts, harnessing the power of the body in pursuit of physical and spiritual perfection.";
                case "/paladin":
                    await SendClassImage(chatId, "https://www.dndbeyond.com/attachments/thumbnails/0/701/400/473/c3paladinintro.png");
                    return "Paladin is a holy warrior bound to a sacred oath, wielding divine magic in service of righteousness.";
                case "/ranger":
                    await SendClassImage(chatId, "https://www.dndbeyond.com/attachments/thumbnails/0/707/400/444/c3rangerintro.png");
                    return "Ranger is a warrior who uses martial prowess and nature magic to combat threats on the edges of civilization.";
                case "/rogue":
                    await SendClassImage(chatId, "https://www.dndbeyond.com/attachments/thumbnails/0/709/375/480/c3rogueintro.png");
                    return "Rogue is a scoundrel who uses stealth and trickery to overcome obstacles and enemies.";
                case "/sorcerer":
                    await SendClassImage(chatId, "https://www.dndbeyond.com/attachments/thumbnails/0/712/400/517/c3sorcererintro.png");
                    return "Sorcerer is a spellcaster who draws on inherent magic from a gift or bloodline.";
                case "/warlock":
                    await SendClassImage(chatId, "https://www.dndbeyond.com/attachments/thumbnails/0/716/400/512/c3warlockintro.png");
                    return "Warlock is a wielder of magic that is derived from a bargain with an extraplanar entity.";
                case "/wizard":
                    await SendClassImage(chatId, "https://www.dndbeyond.com/attachments/thumbnails/0/717/400/484/c3wizardintro.png");
                    return "Wizard is a scholarly magic-user capable of manipulating the structures of reality.";

                // Case for spells
                case "/spell":
                    if (messageText.Length > 7)
                    {
                        string spellName = messageText.Substring(7); // Extract the spell name
                        string apiEndpoint = $"https://www.dnd5eapi.co/api/spells/{spellName.ToLower().Replace(" ", "-")}";
                        return await GetDndApiResponse(apiEndpoint);
                    }
                    else
                    {
                        return "Please provide a spell name. Usage: /spell [spell name]";
                    }
                default:
                    return "Unknown command. Try /help for a list of available commands.";
            }
        }

        private static async Task SendClassImage(long chatId, string imageUrl)
        {
            await botClient.SendPhotoAsync(
                chatId: chatId,
                photo: InputFile.FromUri(imageUrl),
                cancellationToken: CancellationToken.None
            );
        }

        private static async Task<string> GetDndApiResponse(string apiEndpoint)
        {
            try
            {
                var response = await httpClient.GetStringAsync(apiEndpoint);
                var data = JObject.Parse(response);
                return data["desc"]?.ToString() ?? "No description available.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private static string RollDice(string diceExpression)
        {
            try
            {
                string[] parts = diceExpression.Split('d');
                int count = int.Parse(parts[0]);
                int sides = int.Parse(parts[1]);

                if (count <= 0 || sides <= 0)
                {
                    return "Invalid dice expression. Please provide a valid dice expression (e.g. 2d6).";
                }

                int total = 0;
                string result = "";

                for (int i = 0; i < count; i++)
                {
                    int roll = random.Next(1, sides + 1);
                    total += roll;
                    result += $"{roll} ";
                }

                if (count == 1)
                {
                    return $"Rolling {count}d{sides}:\nResult = {total}";
                }
                else
                {
                    return $"Rolling {count}d{sides}:\n{result}= {total}";
                }
            }
            catch
            {
                return "Invalid dice expression. Please provide a valid dice expression (e.g. 2d6).";
            }
        }
    }
}
