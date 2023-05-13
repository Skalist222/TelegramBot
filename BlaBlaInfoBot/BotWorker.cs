using aaa;
using BlaBlaInfoBot;
using Microsoft.Win32;
using System.Drawing;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;
using static UserWorker;

namespace BotWorkerSpace
{

    public class USL 
    {
        public long idUser;
        public string addressLastStih;
        public USL (long id,string address)
        {
            idUser = id;
            addressLastStih = address;
        }
    }
    public class UserStihList: List<USL>
    {
        public string LastAddress(long id)
        {
            for (int i = Count-1; i >= 0; i--)
            {
                if (this[i].idUser == id) return this[i].addressLastStih;
            }
            return "Ин 3:16";
        }
        public void Add(USL usl)
        {
            for (int i = 0; i < Count; i++)
            {
                if (usl.idUser == this[i].idUser)
                {
                    this[i].addressLastStih = usl.addressLastStih;
                    return;
                }
            }
            base.Add(usl);
        }
    }

    public class BotWorker
    {
        UserList userList;
        static UserStihList lastAdd = new UserStihList();


        static bool getMessage = false;
        static bool cleanerWork = true;

        static string defaultPath = "D:\\BlaBlaBot";
        static string defaultPathResend = defaultPath+"\\ResendPhotos\\";
        static string defaultPathMems = defaultPath+"\\Memases\\";
        static string pathGoldenStihArray = defaultPath+ "\\GoldenStihArray.txt";
        static string pathUserList = defaultPath + "\\Users.txt";

        static string mediaGroupId = "";
        static CancellationToken token;
        private readonly TelegramBotClient _botClient;
       

        public BotWorker(TelegramBotClient botClient)
        {
            //587617523|Artur
            userList = new UserList(pathUserList);
          
            _botClient = botClient;
          
            Thread cleaner = new Thread(CleanerWaiter);
            cleaner.Start();
        }
        public void CleanerWaiter()
        {
            Console.WriteLine("Запущен клинер ресендов");
            while (cleanerWork)
            {
                Thread.Sleep(10000);
                if (getMessage == false)
                {
                    string[] files = Directory.GetFiles(defaultPathResend);
                    foreach (string f in files)
                    {
                        try 
                        { 
                            System.IO.File.Delete(f);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Не удалось полностью очиститиь папку ресенда:"+e.Message);
                        }
                       
                    }
                }
            }
        }

        public async Task ListenForMessagesAsync()
        {
            using var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };
            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await _botClient.GetMeAsync();

            Console.WriteLine($"Сервер запущене @{me.Username}");
            Console.ReadLine();
        }
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
           
            token = cancellationToken;
            // Only process Message updates
            if (update.Message is not { } message)
            {
                return;
            }
            getMessage = true;
            User user = message.From;
            userList.SaveUserInFile(pathUserList,new UserI(user.Id,user.Username,user.FirstName,user.LastName));
            CreatorMessage.CreateAnsvere(botClient, update);
            getMessage = false;
        }
        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        private static async Task SendMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken, string info,long idChat = -1)
        {
            
            long id = idChat == -1 ? message.Chat.Id : idChat;
            try
            {
                Message sendArtwork = await botClient.SendTextMessageAsync(
                chatId: id,
                text: info,
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine("Какой то дурак вызвал ексепшан: "+e.Message);
            }
        }
        private static async Task SendAdminMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken,MessageType type,string text ="")
        {
            try
            {
                long id = 1094316046;// кому отправлять
                if (message.Chat.Id != id)
                {
                    if (text == "")
                    {
                        switch (type)
                        {
                            case MessageType.Text:
                                {
                                    await botClient.SendTextMessageAsync(
                                    chatId: id,
                                    text: message.Text,
                                    entities: message.Entities,
                                    parseMode: ParseMode.Html,
                                    cancellationToken: token
                                    );
                                }
                                break;
                            case MessageType.Photo:
                                {
                                    var file = await botClient.GetFileAsync(message.Photo.LastOrDefault().FileId);
                                    var fileName = defaultPathResend+"\\resendphoto" + file.FileId + "." + file.FilePath.Split('.').Last();
                                    using (FileStream imageSaver = new FileStream(fileName, FileMode.Create))
                                    {
                                        await botClient.DownloadFileAsync(file.FilePath, imageSaver);
                                    }
                                    SendImage(botClient, message, token, fileName);
                                }
                                break;
                            case MessageType.Sticker:
                                {
                                    await botClient.SendStickerAsync(
                                    chatId: id,
                                          sticker: message.Sticker.FileId,
                                          cancellationToken: token
                                        );
                                }
                                break;
                            case MessageType.Video:
                                {
                                    await botClient.SendTextMessageAsync(
                                    chatId: id,
                                    text: "Видео",
                                    entities: message.Entities,
                                    parseMode: ParseMode.Html,
                                    cancellationToken: token
                                    );
                                }
                                break;
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                                  chatId: id,
                                  text: text,
                                  entities: message.Entities,
                                  parseMode: ParseMode.Html,
                                  cancellationToken: token
                                  );
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Какой то дурак вызвал ексепшан: " + e.Message);
            }
        }
        private static async Task SendScreen(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken,  long idChat = -1)
        {
            long id = idChat == -1 ? message.Chat.Id : idChat;
            var bm = Bitmap.FromFile(defaultPath+"\\ScreenShot.png");
            var ms = new MemoryStream();
            bm.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;

            Message sendArtwork = await botClient.SendPhotoAsync(
            chatId: id,
                photo: new InputOnlineFile(ms, "ScreenShot.png"),
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
        }
        private static async Task SendImage(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken, string pathImage, long idChat = -1,string caption ="")
        {
            long id = idChat == -1 ? message.Chat.Id : idChat;
            var cap = caption == "" ? message.Caption : caption;
            var bm = Bitmap.FromFile(pathImage);
            var ms = new MemoryStream();
            bm.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            ms.Position = 0;

            await botClient.SendPhotoAsync(
            chatId: id,
                photo: new InputOnlineFile(ms, "ScreenShot.png"),
                caption: cap,
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
        }




        public class CreatorMessage
        {
            public async static void CreateAnsvere(ITelegramBotClient botClient, Update up) 
            {
                Message mes = up.Message;
                if (mes is not null)
                {
                    MessageType type = up.Message.Type;
                    Telegram.Bot.Types.User user = up.Message.From;
                    string text = (up.Message.Text ?? up.Message.Caption) ?? "";
                    TextMessageWorker tmW = new TextMessageWorker(text);
                    Commands commands = Commands.Selected(tmW.GetCommand());
                    string responceText = "";


                    if (mediaGroupId == mes.MediaGroupId && mes.MediaGroupId != "")
                    {
                        AddMem(botClient, up, token);
                    }
                    else
                    {
                        mediaGroupId = "";
                    if (commands.IsEmpty)
                        {
                            responceText = tmW.EmptC.GetRandomAnswer();
                            SendMessageAsync(botClient, mes, token, responceText);
                        }
                        else
                    if (commands.IsStart)
                        {
                            responceText = tmW.StartC.GetRandomAnswer();
                            Start(botClient, up, token, responceText);
                        }
                        else
                    if (commands.IsInfo)
                        {
                            responceText = GetBotInfo();
                            SendMessageAsync(botClient, mes, token, responceText);
                        }
                        else
                    if (commands.IsMem)
                        {
                            responceText = tmW.MemC.GetRandomAnswer();
                            SendMessageAsync(botClient, mes, token, responceText);
                            SendImage(botClient, mes, token, CommandMem());
                        }
                        else
                    if (commands.IsAddMem)
                        {
                            if (type == MessageType.Photo)
                            {
                                responceText = tmW.Add.GetRandomAnswer() + " спасибо!)";

                                AddMem(botClient, up, token);
                                SendMessageAsync(botClient, mes, token, responceText);
                            }
                            else
                            {
                                responceText = "Я конечно добавлю мем, но ты пришли картинку и в подписи напиши что это новый мем";
                                SendMessageAsync(botClient, mes, token, responceText);
                            }
                        }
                        else
                    if (commands.IsScreen)
                        {
                            responceText = tmW.ScrinC.GetRandomAnswer();
                            SendMessageAsync(botClient, mes, token, responceText);
                            CommandScreen();
                            SendScreen(botClient, mes, token);
                        }
                        else
                    if (commands.IsVP)
                        {
                            MemWorker mw = new MemWorker(defaultPathMems);
                            string nameImage = mw.GetRandomVPPathMem(text);
                            if (nameImage=="")
                            {
                                responceText = "Не нашел такого мема";
                                SendMessageAsync(botClient, mes, token, responceText);
                            }
                            else
                            {
                                string pathImage = defaultPathMems + "\\" + nameImage;
                                responceText = pathImage;
                                SendImage(botClient, mes, token, pathImage);
                            }
                        }

                       
                        SendAdminMessageAsync(botClient, mes, token, type, "От " + user.FirstName + "(" + user.Id + ")");
                        SendAdminMessageAsync(botClient, mes, token, type);
                        WriteTextInFile(text, up, responceText);
                    }

                }
                else
                {
                    Console.WriteLine("Получено пустое сообщение");
                }
            }
            public static void WriteTextInFile(string message, Update up, string responce)
            {
                string textInFile = "----------------Start Message---------------" + Environment.NewLine;
                textInFile += "("+up.Message.From.Username + " " + up.Message.From.FirstName +" "+ up.Message.From.LastName+") ";
                textInFile += up.Message.Date.ToLongDateString() + Environment.NewLine;
                textInFile += "mesText: "+message + Environment.NewLine;
                textInFile += "responce: "+responce + Environment.NewLine;
                textInFile += "____________________________________________" + Environment.NewLine;
                string pathFile = Path.Combine(defaultPath, "Messages.txt");
                Console.WriteLine(textInFile);
                System.IO.File.AppendAllText(pathFile, textInFile);
            }
            public static string GetBotInfo()
            {
                string newLine = Environment.NewLine;
                string info = System.IO.File.ReadAllText(Directory.GetCurrentDirectory()+"\\BotInfo.txt");
                return info;
            }
            public static void SetImAwait(bool isAwait = true)
            {
                try
                {
                    RegistryKey waiter = Registry.CurrentUser.OpenSubKey("Waiter", true);
                    waiter.SetValue("ImAwait", isAwait);
                    waiter.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            public static bool WaitScreenShot()
            {
                int time = 100000;
                RegistryKey waiter = Registry.CurrentUser.OpenSubKey("Waiter", true);
                while (true)
                {
                    string wait = (string)(waiter.GetValue("ImAwait"));
                    time--;
                    if (wait == "False")
                    {
                        waiter.Close();
                        Console.WriteLine(time + "");
                        return true;
                    }
                    if (time <= 0)
                    {
                        Console.WriteLine("Не дождался скриншот, видимо программа сканирования закрыта");
                        return false;
                    }
                }
            }
            public static string CommandMem()
            {
                try
                {
                    MemWorker mw = new MemWorker(defaultPathMems);
                    string ImagePath = mw.GetRandomPathMem();
                    return ImagePath;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return "";
                }
            }
            public static string CommandMemVp()
            {
                try
                {
                    MemWorker mw = new MemWorker(defaultPathMems);
                    string ImagePath = mw.GetRandomPathMem();
                    return ImagePath;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return "";
                }
            }


            // Разветвление команды get
    
            public static bool CommandScreen()
            {
                SetImAwait();
                return WaitScreenShot();

            }
            public static  async void AddMem(ITelegramBotClient botClient, Update up, CancellationToken token)
            {
                try
                {
                    int countPic  = 0;
                    mediaGroupId = up.Message.MediaGroupId ?? "";
                        var file = await botClient.GetFileAsync(up.Message.Photo.LastOrDefault().FileId);
                        var fileName = defaultPathMems+"mem " + file.FileUniqueId + "." + file.FilePath.Split('.').Last();
                        using (FileStream imageSaver = new FileStream(fileName, FileMode.Create))
                        {
                            await botClient.DownloadFileAsync(file.FilePath, imageSaver);
                        }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка добавления мема!");
                    Console.WriteLine(ex);
                }
            }
           
            public static async void Start(ITelegramBotClient botClient, Update up, CancellationToken token,string answer,bool startAdd=false)
            {
                
                KeyboardButton screen = new KeyboardButton("Скрин");
                KeyboardButton mem = new KeyboardButton("Мем");
                KeyboardButton start = new KeyboardButton("Старт");

                KeyboardButton addMemStart = new KeyboardButton("");
                KeyboardButton addMemStop  = new KeyboardButton("");

                List<List<Telegram.Bot.Types.ReplyMarkups.KeyboardButton>> board = new List<List<Telegram.Bot.Types.ReplyMarkups.KeyboardButton>>()
                { new List<KeyboardButton>(){  screen, mem, start }  };
                ReplyKeyboardMarkup mrkp = new ReplyKeyboardMarkup(keyboard:board);
                mrkp.ResizeKeyboard = true;
                await botClient.SendTextMessageAsync(
                    chatId: up.Message.Chat.Id,
                    text: answer,
                    replyMarkup:mrkp
                    ) ;
            }
        }
        
    }
}
