using System.Text.Json;
using AttractionLibrary;
using BotClientLibrary;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static UserInteractionBot.DataMethods.SelectionMethods;
using static UserInteractionBot.DataMethods.SortingMethods;
using static UserInteractionBot.DataMethods.SavingMethods;
using static UserInteractionBot.LoggingMethods;

namespace UserInteractionBot;

public static class BotMethods
{
    /// <summary>
    /// List of users who started bot.
    /// </summary>
    private static List<Client> _botClients = new ();
    
    /// <summary>
    /// Starts action according to user's choice of option.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="botClient"></param>
    /// <param name="currentClient"></param>
    /// <param name="token"></param>
    private static async void SelectOption(Message? message, ITelegramBotClient botClient, Client currentClient,
        CancellationToken token)
    {
        if (message?.Text != null)
        {
            switch (message.Text)
            {
                case "1" or "1. Selection": currentClient.State = ClientState.Selecting;
                    SelectDataOpenMenu(botClient, message, token, currentClient);
                    break;
                case "2" or "2. Sorting": currentClient.State = ClientState.SortingMenu;
                    SortDataOpenMenu(currentClient, botClient, message, token);
                    break;
                case "3" or "3. Download file": currentClient.State = ClientState.SavingMenu;
                    SaveDataOpenMenu(message, botClient, currentClient, token);
                    break;
                default: await botClient.SendTextMessageAsync(message.Chat.Id, 
                    "Incorrect input, try again!", cancellationToken: token); break;
            }
        }
    }
    
    /// <summary>
    /// Sends main menu to user.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="message"></param>
    /// <param name="token"></param>
    internal static async void SendMainMenu(ITelegramBotClient botClient, Message message, CancellationToken token)
    {
        var keyboard = new ReplyKeyboardMarkup
        (
            new[]
            {
                new[]
                {
                    new KeyboardButton("1. Selection"),
                    new KeyboardButton("2. Sorting"),
                    new KeyboardButton("3. Download file")
                }
            }
        )
        {
            ResizeKeyboard = true
        };
        await botClient.SendTextMessageAsync(message.Chat.Id,
            "Now choose one of the options (send me the number of the option or press the button):\n" +
            "1. Selection\n2. Sorting\n3. Download CSV or JSON file", 0, ParseMode.Html,
            null, false, false, false,
            null, false, keyboard, token);
    }
    
    /// <summary>
    /// Receives a document from a user and deserializes data in it.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="botClient"></param>
    /// <param name="token"></param>
    /// <param name="currentClient"></param>
    private static async void ReceiveDocument(Message? message, ITelegramBotClient botClient,
        CancellationToken token, Client currentClient)
    {
        if (message?.Document != null)
        {
            Log("BotMethods", "File uploaded.", LogLevel.Information);
            var fileId = message.Document.FileId;
            var fileInfo = await botClient.GetFileAsync(fileId, token);
            var filePath = fileInfo.FilePath;
            var info = new FileInfo(filePath!);
            if (info.Extension == ".csv" || info.Extension == ".json") // Checks file's extension.
            {
                var destinationFilePath = "../../../../../DownloadedFiles/botFile" + info.Extension;
                await using (Stream fileStream = System.IO.File.Create(destinationFilePath))
                {
                    await botClient.DownloadFileAsync(
                        filePath: filePath!,
                        destination: fileStream,
                        cancellationToken: token);
                }
                await botClient.SendTextMessageAsync(message.Chat.Id, "File has been successfully uploaded!",
                    cancellationToken: token);
                try // Deserealizes data in user's file.
                {
                    using var file = new StreamReader("../../../../../DownloadedFiles/botFile" + info.Extension);
                    if (info.Extension == ".csv")
                    {
                        var processor = new CsvProcessing();
                        currentClient.Attractions = processor.Read(file);
                    }
                    else
                    {
                        var processor = new JsonProcessing();
                        currentClient.Attractions = processor.Read(file);
                    }
                    file.Close();
                }
                catch (FormatException)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Incorrect data in the CSV file, try again!",
                        cancellationToken: token);
                    Log("BotMethods", "Uploaded CSV file has invalid data.", LogLevel.Error);
                }
                catch (JsonException)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Incorrect data in the JSON file, try again!",
                        cancellationToken: token);
                    Log("BotMethods", "Uploaded JSON file has invalid data.", LogLevel.Error);
                }
                catch (Exception)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Unknown error occured, try again!",
                        cancellationToken: token);
                    Log("BotMethods", "Unknown error occured while reading uploaded file.", LogLevel.Error);
                }
                SendMainMenu(botClient, message, token);
                currentClient.State = ClientState.Menu;
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, 
                    "Incorrect file extension, try again!", cancellationToken: token);
                Log("BotMethods", "Uploaded file has an incorrect extension.", LogLevel.Error);
            }
        }
    }
    
    /// <summary>
    /// Handles updates received from a user.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    /// <param name="token"></param>
    internal static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        var message = update.Message;
        if (message is { Text: "/start" })
        {
            // Reregister user if they entered /start
            _botClients = (from client in _botClients where client.Id != message.Chat.Id select client).ToList();
            await botClient.SendStickerAsync(
                chatId: message.Chat.Id,
                sticker: InputFile.FromFileId("CAACAgIAAxkBAAELvQJl-Dym8LkUUtoqFEmng8jduyD7VQACHicAAnF3AAFIX-YjQ8eXm8Y0BA"),
                cancellationToken: token);
            await botClient.SendTextMessageAsync(message.Chat.Id,
                "Greetings!\nSend me a file to start working with it (CSV/JSON)", 
                cancellationToken: token);
            _botClients.Add(new Client(message.Chat.Id));
            Log("BotMethods", $"New client registered. Client's ID: {message.Chat.Id}",
                LogLevel.Information);
        }
        else if (_botClients.ConvertAll(x => x.Id).Contains(message!.Chat.Id))
        {
            var currentUser = _botClients.Find(x => x.Id == message.Chat.Id);
            switch (currentUser!.State) // Does action according to user's state.
            {
                case ClientState.Introduction: ReceiveDocument(message, botClient, token, currentUser); break;
                case ClientState.Menu: SelectOption(message, botClient, currentUser, token); break;
                case ClientState.ChoosingSelectionField: ChooseSelectionField(message, botClient, currentUser); break;
                case ClientState.Selecting: SelectDataOpenMenu(botClient, message, token, currentUser); break;
                case ClientState.InputtingSelectionValue: SelectData(currentUser, message, botClient, token); break;
                case ClientState.Saving: SaveDataOpenMenu(message, botClient, currentUser, token); break;
                case ClientState.ChoosingSavingMethod: SaveData(message, botClient, currentUser); break;
                case ClientState.ChoosingSortingOrder: SortData(message, botClient, currentUser, token); break;
            }
        }
        else
        {
            await botClient.SendTextMessageAsync(message.Chat.Id,
                "Unrecognised command, to start bot enter /start", cancellationToken: token);
        }
    }
    
    /// <summary>
    /// Handles occured errors.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="exception"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    internal static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken token)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };
        Log("BotMethods", $"Error occured: {errorMessage}", LogLevel.Critical);
        return Task.CompletedTask;
    }
}