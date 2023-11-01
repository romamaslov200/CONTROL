using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using System.Threading;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Passport;
using static System.Net.Mime.MediaTypeNames;

using System.Drawing;
using System.Windows.Forms;

using SharpAvi;
using SharpAvi.Codecs;
using SharpAvi.Output;
using System.Reflection;

using System.Data;
using System.Data.SqlClient;
using NReco.VideoConverter;

using Captura;
using static System.Net.WebRequestMethods;
using System.Drawing.Imaging;


using Emgu.CV;
using Emgu.Util;

using System.Text.RegularExpressions;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Web.UI.WebControls;
using System.Web;


namespace CONTROLL
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (!System.IO.File.Exists("token.txt"))
            {
                System.IO.File.Create("token.txt").Dispose();
            }

            if (!System.IO.File.Exists("chatid.txt"))
            {
                System.IO.File.Create("chatid.txt").Dispose();
            }

            ITelegramBotClient bot = new TelegramBotClient(System.IO.File.ReadAllText("token.txt"));

            try
            {
                Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);
            }

            catch
            {
                MessageBox.Show("Введите token бота в файле token.txt!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            System.Threading.Thread.Sleep(-1);
        }
        public static int status;
        static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\DataBase\db.mdf;Integrated Security=True";
        static SqlConnection sql = new SqlConnection(connectionString);

        //static ITelegramBotClient bot = new TelegramBotClient(System.IO.File.ReadAllText("token.txt"));
        //static ITelegramBotClient bot = new TelegramBotClient("6120629335:AAF8ERXPC7rCzWccZbKwi1WxODAzqBPObx8");
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));

            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var keyboard = new ReplyKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        new KeyboardButton("Выключить ПК"),
                        new KeyboardButton("Перезагрузить ПК"),
                        new KeyboardButton("Спящий режим")
                    },
                    new[]
                    {
                        new KeyboardButton("📸Сделать скриншот📸"),
                        new KeyboardButton("🎬Записать видео🎬"),
                        new KeyboardButton("📸Скриншот вэбки📸")
                    },
                    new[]
                    {
                        new KeyboardButton("➡Далее➡")
                    }
                }
                );

                var keyboard2 = new ReplyKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        new KeyboardButton("🌐Открыть ссылку🌐"),
                        new KeyboardButton("📺Поиск в ютубе📺"),
                        new KeyboardButton("🌐Поиск в яндекс🌐")
                    },
                    new[]
                    {
                        new KeyboardButton("ALT🔘TAB"),
                        //new KeyboardButton("🔇"),
                        new KeyboardButton("✂Очистить папку файлов✂")
                    },
                    new[]
                    {
                        new KeyboardButton("⬅Назад⬅"),
                    }
                }
                );

                var keyboardExit = new ReplyKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        new KeyboardButton("⬇Назад⬇")
                    }
                }
                );
                
                var message = update.Message;
                long chatid = default;
                try
                {
                    chatid = Convert.ToInt64(System.IO.File.ReadAllText("chatid.txt"));
                }
                catch
                {
                    MessageBox.Show("Введите chatid в файле chatid.txt!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }

                if (message.Chat.Id == chatid)
                {
                    if (message.Type == MessageType.Document)
                    {
                        if (!Directory.Exists("files"))
                        {
                            Directory.CreateDirectory("files");
                        }

                        var file = await botClient.GetFileAsync(message.Document.FileId);
                        FileStream fs = new FileStream($@"files\{message.Document.FileName}", FileMode.Create);
                        try
                        {
                            await botClient.DownloadFileAsync(file.FilePath, fs);
                        }
                        catch { }

                        fs.Close();
                        fs.Dispose();

                        try
                        {
                            System.Diagnostics.Process.Start($@"files\{message.Document.FileName}");
                        }
                        catch { }
                        await botClient.SendTextMessageAsync(message.Chat, "Файл открыт");
                    }

                    if (message.Type == MessageType.Audio)
                    {
                        if (!Directory.Exists("files"))
                        {
                            Directory.CreateDirectory("files");
                        }

                        var file = await botClient.GetFileAsync(message.Audio.FileId);
                        FileStream fs = new FileStream($@"files\{message.Audio.FileName}", FileMode.Create);
                        try
                        {
                            await botClient.DownloadFileAsync(file.FilePath, fs);
                        }
                        catch { }

                        fs.Close();
                        fs.Dispose();

                        System.Diagnostics.Process.Start($@"files\{message.Audio.FileName}");

                        await botClient.SendTextMessageAsync(message.Chat, "Аудио открыто");
                    }

                    if (message.Type == MessageType.Voice)
                    {
                        if (!Directory.Exists("files"))
                        {
                            Directory.CreateDirectory("files");
                        }

                        var file = await botClient.GetFileAsync(message.Voice.FileId);
                        FileStream fs = new FileStream($@"files\{message.Voice.FileId}.ogg", FileMode.Create);
                        try
                        {
                            await botClient.DownloadFileAsync(file.FilePath, fs);
                        }
                        catch { }

                        fs.Close();
                        fs.Dispose();

                        System.Diagnostics.Process.Start($@"files\{message.Voice.FileId}.ogg");

                        await botClient.SendTextMessageAsync(message.Chat, "Голосовое сообщение открыто");
                    }

                    if (message.Type == MessageType.Photo)
                    {
                        if (!Directory.Exists("files"))
                        {
                            Directory.CreateDirectory("files");
                        }

                        var file = await botClient.GetFileAsync(message.Photo.LastOrDefault().FileId);
                        FileStream fs = new FileStream($@"files\{message.Photo.LastOrDefault().FileId}.jpg", FileMode.Create);
                        try
                        {
                            await botClient.DownloadFileAsync(file.FilePath, fs);
                        }
                        catch { }

                        fs.Close();
                        fs.Dispose();

                        System.Diagnostics.Process.Start($@"files\{message.Photo.LastOrDefault().FileId}.jpg");

                        await botClient.SendTextMessageAsync(message.Chat, "Фото открыто");
                    }

                    if (message.Type == MessageType.Video)
                    {
                        if (!Directory.Exists("files"))
                        {
                            Directory.CreateDirectory("files");
                        }

                        var file = await botClient.GetFileAsync(message.Video.FileId);
                        try
                        {
                            FileStream fs = new FileStream($@"files\{message.Video.FileName}", FileMode.Create);
                            await botClient.DownloadFileAsync(file.FilePath, fs);
                            fs.Close();
                            fs.Dispose();
                            System.Diagnostics.Process.Start($@"files\{message.Video.FileName}");
                        }
                        catch
                        {
                            FileStream fs = new FileStream($@"files\{message.Video.FileId}.mp4", FileMode.Create);
                            await botClient.DownloadFileAsync(file.FilePath, fs);
                            fs.Close();
                            fs.Dispose();
                            System.Diagnostics.Process.Start($@"files\{message.Video.FileId}.mp4");
                        }

                        await botClient.SendTextMessageAsync(message.Chat, "Image save");
                    }

                    if (message.Type == MessageType.VideoNote)
                    {
                        if (!Directory.Exists("files"))
                        {
                            Directory.CreateDirectory("files");
                        }

                        var file = await botClient.GetFileAsync(message.VideoNote.FileId);
                        FileStream fs = new FileStream($@"files\{message.VideoNote.FileId}.mp4", FileMode.Create);
                        try
                        {
                            await botClient.DownloadFileAsync(file.FilePath, fs);
                        }
                        catch { }

                        fs.Close();
                        fs.Dispose();

                        System.Diagnostics.Process.Start($@"files\{message.VideoNote.FileId}.mp4");

                        await botClient.SendTextMessageAsync(message.Chat, "Image save");
                    }

                    if (message.Text != null)
                    {

                        if (message.Text == "/start")
                        {
                            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
                            {
                                sql.Open(); //Открываем sql подключение
                                SqlCommand command = new SqlCommand($"insert into Users (chatid, status) values ('{message.Chat.Id}', '0')", sql); //Прописываем добавление данных в таблицу
                                await botClient.SendTextMessageAsync(message.Chat, $"sql {message.Chat.Id}", replyMarkup: keyboard);

                                try
                                {
                                    await command.ExecuteNonQueryAsync(); //Выполняем команду
                                }
                                catch
                                {

                                }

                                sql.Close();
                                await botClient.SendTextMessageAsync(message.Chat, "sql close", replyMarkup: keyboard);

                            }

                            await botClient.SendTextMessageAsync(message.Chat, $"Добро пожаловать {message.Chat.FirstName}!", replyMarkup: keyboard);
                            return;
                        }

                        if (message.Text == "⬅Назад⬅")
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Первая страница 📃", replyMarkup: keyboard);
                        }

                        if (message.Text == "➡Далее➡")
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Вторая страница 📃", replyMarkup: keyboard2);
                        }

                        if (sql.State == ConnectionState.Closed)
                        {
                            sql.Open();
                            SqlCommand command = new SqlCommand($"Select status From Users Where chatid = '{message.Chat.Id}'", sql);
                            SqlDataReader reader = command.ExecuteReader();
                            await reader.ReadAsync();
                            status = Convert.ToInt32(reader[0]); //count - обычная int переменная. Мы считали данные с таблицы и присвоили значение второй колонки этой переменной.
                            reader.Close();
                            sql.Close();
                        }

                        if (status == 1)
                        {
                            sql.Open();
                            SqlCommand command = new SqlCommand($"Update Users Set status = '0' where chatid = '{message.Chat.Id}'", sql);
                            command.ExecuteNonQuery();

                            string linck = message.Text.ToLower();

                            if (!linck.Contains("https://"))
                            {
                                linck = $"https://{linck}";
                            }


                            if (message.Text != "⬇Назад⬇")
                            {
                                try
                                {
                                    System.Diagnostics.Process.Start(linck);
                                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Перехожу по ссылке: {linck}", replyMarkup: keyboard2);
                                }
                                catch
                                {
                                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Ошибка: Это не ссылка\n📖Возврощаюсь в меню📖", replyMarkup: keyboard2);
                                }
                            }

                            if (message.Text == "⬇Назад⬇")
                            {
                                await botClient.SendTextMessageAsync(message.Chat, "📖Возврощаюсь в меню📖", replyMarkup: keyboard2);
                                sql.Close();
                                return;
                            }

                            sql.Close();
                            return;
                        }

                        if (status == 2)
                        {
                            sql.Open();
                            SqlCommand command = new SqlCommand($"Update Users Set status = '0' where chatid = '{message.Chat.Id}'", sql);
                            command.ExecuteNonQuery();
                            //Анулируем вторую колонку.
                            if (message.Text != "⬇Назад⬇")
                            {
                                try
                                {
                                    var rec = new Recorder(new RecorderParams("video.avi", 10, SharpAvi.KnownFourCCs.Codecs.MotionJpeg, 50));
                                    Console.WriteLine("Press any key to Stop...");

                                    await botClient.SendTextMessageAsync(message.Chat.Id, $"🎞️Записываю видео🎞️", replyMarkup: keyboard);

                                    System.Threading.Thread.Sleep(int.Parse($"{message.Text}000"));

                                    // Finish Writing
                                    rec.Dispose();

                                    var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                                    ffMpeg.ConvertMedia("video.avi", "video.mp4", Format.mp4);
                                    await botClient.SendVideoAsync(message.Chat, System.IO.File.Open("video.mp4", System.IO.FileMode.Open));
                                    await botClient.SendTextMessageAsync(message.Chat, "Видео готово🎥", replyMarkup: keyboard);
                                    //System.IO.File.Delete("video.avi");
                                }
                                catch
                                {
                                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Ошибка!\n📖Возврощаюсь в меню📖", replyMarkup: keyboard);
                                }
                            }

                            if (message.Text == "⬇Назад⬇")
                            {
                                await botClient.SendTextMessageAsync(message.Chat, "📖Возврощаюсь в меню📖", replyMarkup: keyboard);
                                sql.Close();
                                return;
                            }

                            sql.Close();
                            return;
                        }

                        if (status == 3)
                        {
                            sql.Open();
                            SqlCommand command = new SqlCommand($"Update Users Set status = '0' where chatid = '{message.Chat.Id}'", sql);
                            command.ExecuteNonQuery();
                            //Анулируем вторую колонку.
                            if (message.Text != "⬇Назад⬇")
                            {
                                try
                                {
                                    System.Diagnostics.Process.Start($"https://www.youtube.com/results?search_query={message.Text}");
                                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Ищю в ютубе: {message.Text}", replyMarkup: keyboard2);
                                }
                                catch
                                {
                                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Ошибка!\n📖Возврощаюсь в меню📖", replyMarkup: keyboard2);
                                }
                            }

                            if (message.Text == "⬇Назад⬇")
                            {
                                await botClient.SendTextMessageAsync(message.Chat, "📖Возврощаюсь в меню📖", replyMarkup: keyboard2);
                                sql.Close();
                                return;
                            }

                            sql.Close();
                            return;
                        }

                        if (status == 4)
                        {
                            sql.Open();
                            SqlCommand command = new SqlCommand($"Update Users Set status = '0' where chatid = '{message.Chat.Id}'", sql);
                            command.ExecuteNonQuery();
                            //Анулируем вторую колонку.
                            if (message.Text != "⬇Назад⬇")
                            {
                                try
                                {
                                    System.Diagnostics.Process.Start($"https://yandex.ru/search/?text={message.Text}");
                                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Ищю в яндекс: {message.Text}", replyMarkup: keyboard2);
                                }
                                catch
                                {
                                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Ошибка!\n📖Возврощаюсь в меню📖", replyMarkup: keyboard2);
                                }
                            }

                            if (message.Text == "⬇Назад⬇")
                            {
                                await botClient.SendTextMessageAsync(message.Chat, "📖Возврощаюсь в меню📖", replyMarkup: keyboard2);
                                sql.Close();
                                return;
                            }

                            sql.Close();
                            return;
                        }

                        if (message.Text == "Выключить ПК")
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Готово✅", replyMarkup: keyboard);
                            Process.Start("shutdown", "/s /t 0");
                            return;
                        }

                        if (message.Text == "Перезагрузить ПК")
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Готово✅", replyMarkup: keyboard);
                            Process.Start("shutdown", "/r /t 0");
                            return;
                        }

                        if (message.Text == "Спящий режим")
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Готово✅", replyMarkup: keyboard);
                            System.Windows.Forms.Application.SetSuspendState(PowerState.Suspend, true, true);
                            return;
                        }

                        if (message.Text == "📸Сделать скриншот📸")
                        {
                            Graphics graph = null;

                            var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                            Screen.PrimaryScreen.Bounds.Height);

                            graph = Graphics.FromImage(bmp);

                            graph.CopyFromScreen(0, 0, 0, 0, bmp.Size);
                            string ScreenShot = "ScreenShot.webm";
                            bmp.Save(ScreenShot);


                            await botClient.SendPhotoAsync(chatId: message.Chat.Id, photo: System.IO.File.Open(ScreenShot, System.IO.FileMode.Open));

                            //System.IO.File.Delete("ScreenShot.webm");

                            return;
                        }

                        if (message.Text == "📸Скриншот вэбки📸")
                        {
                            try
                            {

                                VideoCapture capture = new VideoCapture(); //create a camera capture

                                string WebScreenShotName = "WebScreenShot.png";
                                Bitmap image = null;
                                for (int i = 0; i < 10; i++)
                                {
                                    image = capture.QueryFrame().Bitmap;
                                }
                                capture.Dispose();
                                image.Save(WebScreenShotName, ImageFormat.Png);

                                await botClient.SendPhotoAsync(chatId: message.Chat.Id, photo: System.IO.File.Open(WebScreenShotName, System.IO.FileMode.Open));
                            }
                            catch 
                            { 
                                await botClient.SendTextMessageAsync(message.Chat, "Ошибка\nМожет быть у вас нет камеры");
                            }




                            /*
                            VideoCapture capture = new VideoCapture(); //create a camera capture

                            //Bitmap image = capture.QueryFrame().Bitmap; //take a picture
                            Bitmap image = capture.QueryFrame().Bitmap; //take a picture

                            string WebScreenShotName = "WebScreenShot.png";

                            var update_image = new SetBrightness(image, 1.300f, 1.0f, 1.280f, WebScreenShotName);
                            capture.Dispose();
                            await botClient.SendPhotoAsync(chatId: message.Chat.Id, photo: System.IO.File.Open(WebScreenShotName, System.IO.FileMode.Open));
                            */
                            return;
                        }

                        if (message.Text == "🎬Записать видео🎬")
                        {
                            if (sql.State == ConnectionState.Closed) //Проверяем состояние подключения
                            {
                                //Отправляем сообщение, что-бы ввели 2 числа
                                sql.Open();
                                SqlCommand command = new SqlCommand($"Update Users Set status = '2' where chatid = '{message.Chat.Id}'", sql);
                                //Так, тут мы установили второй колонке значение 1 (это может быть любое число)
                                await command.ExecuteNonQueryAsync();
                                sql.Close();
                            }

                            await botClient.SendTextMessageAsync(message.Chat, "✏Введите длину видео✏", replyMarkup: keyboardExit);
                            return;
                        }

                        if (message.Text == "🌐Открыть ссылку🌐")
                        {
                            if (sql.State == ConnectionState.Closed) //Проверяем состояние подключения
                            {
                                //Отправляем сообщение, что-бы ввели 2 числа
                                sql.Open();
                                SqlCommand command = new SqlCommand($"Update Users Set status = '1' where chatid = '{message.Chat.Id}'", sql);
                                //Так, тут мы установили второй колонке значение 1 (это может быть любое число)
                                await command.ExecuteNonQueryAsync();
                                sql.Close();
                            }

                            await botClient.SendTextMessageAsync(message.Chat, "✏Введите ссылку✏", replyMarkup: keyboardExit);
                            return;
                        }

                        if (message.Text == "ALT🔘TAB")
                        {
                            SendKeys.SendWait("%{TAB}");
                            await botClient.SendTextMessageAsync(message.Chat, "Готово✅", replyMarkup: keyboard);
                        }

                        if (message.Text == "📺Поиск в ютубе📺")
                        {
                            if (sql.State == ConnectionState.Closed) //Проверяем состояние подключения
                            {
                                //Отправляем сообщение, что-бы ввели 2 числа
                                sql.Open();
                                SqlCommand command = new SqlCommand($"Update Users Set status = '3' where chatid = '{message.Chat.Id}'", sql);
                                //Так, тут мы установили второй колонке значение 1 (это может быть любое число)
                                await command.ExecuteNonQueryAsync();
                                sql.Close();
                            }

                            await botClient.SendTextMessageAsync(message.Chat, "✏Введите запрос✏", replyMarkup: keyboardExit);
                            return;
                        }

                        if (message.Text == "🌐Поиск в яндекс🌐")
                        {
                            if (sql.State == ConnectionState.Closed) //Проверяем состояние подключения
                            {
                                //Отправляем сообщение, что-бы ввели 2 числа
                                sql.Open();
                                SqlCommand command = new SqlCommand($"Update Users Set status = '4' where chatid = '{message.Chat.Id}'", sql);
                                //Так, тут мы установили второй колонке значение 1 (это может быть любое число)
                                await command.ExecuteNonQueryAsync();
                                sql.Close();
                            }

                            await botClient.SendTextMessageAsync(message.Chat, "✏Введите запрос✏", replyMarkup: keyboardExit);
                            return;
                        }

                        if (message.Text == "✂Очистить папку файлов✂")
                        {
                            System.IO.DirectoryInfo di = new DirectoryInfo("files");

                            foreach (FileInfo file in di.GetFiles())
                            {
                                file.Delete();
                            }
                            foreach (DirectoryInfo dir in di.GetDirectories())
                            {
                                dir.Delete(true);
                            }   

                            await botClient.SendTextMessageAsync(message.Chat, "Все файлы удаленны✅");
                            return;
                        }
                    }
                }

            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
            MessageBox.Show(Convert.ToString(Newtonsoft.Json.JsonConvert.SerializeObject(exception)), "Error");
            //Console.WriteLine(exception);
        }
    }
}