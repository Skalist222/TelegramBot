// See https://aka.ms/new-console-template for more information
using BotWorkerSpace;
using Telegram.Bot;

var botClient = new TelegramBotClient(File.ReadAllText(Path.Combine("D:\\BlaBlaBot", "Token.txt")));


BotWorker bW = new BotWorker(botClient);
bW.ListenForMessagesAsync().GetAwaiter().GetResult();
//Console.WriteLine($"Hello, World! I am bot {me.Id} and my name is {me.FirstName}.");

