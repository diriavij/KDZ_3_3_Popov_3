using AttractionLibrary;
using BotClientLibrary;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static UserInteractionBot.LoggingMethods;

namespace UserInteractionBot.DataMethods;

public static class SavingMethods
{
    /// <summary>
    /// Provides user a choice between two file extensions: CSV and JSON.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="botClient"></param>
    /// <param name="currentClient"></param>
    /// <param name="token"></param>
    internal static async void SaveDataOpenMenu(Message message, ITelegramBotClient botClient, Client currentClient,
        CancellationToken token)
    {
        var keyboard = new ReplyKeyboardMarkup
        (
            new[]
            {
                new[]
                {
                    new KeyboardButton("1. CSV"),
                    new KeyboardButton("2. JSON")
                }
            }
        )
        {
            ResizeKeyboard = true
        };
        await botClient.SendTextMessageAsync(message.Chat.Id,
            "Now choose one of the file extensions (send me the number of the option or press the button):\n" +
            "1. CSV\n2. JSON", 0, ParseMode.Html, null, false,
            false, false, null, false, keyboard, token);
        currentClient.State = ClientState.ChoosingSavingMethod;
    }
    
    /// <summary>
    /// Sends user a file with chosen extension.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="botClient"></param>
    /// <param name="currentClient"></param>
    internal static async void SaveData(Message message, ITelegramBotClient botClient, Client currentClient)
    {
        var correctInput = new [] { "1", "2", "1. CSV", "2. JSON"}; // Possible variants of correct user's input.
        if (correctInput.Contains(message.Text))
        {
            bool flag = message.Text == "1" || message.Text == "1. CSV";
            if (flag)
            {
                var processor = new CsvProcessing();
                await botClient.SendDocumentAsync(message.Chat.Id,
                    InputFile.FromStream(processor.Write(currentClient.Attractions!), "result.csv"));
                processor.OutputStream!.Close();
                Log("BotMethods", "CSV file sent to client.", LogLevel.Information);
            }
            else
            {
                var processor = new JsonProcessing();
                await botClient.SendDocumentAsync(message.Chat.Id,
                    InputFile.FromStream(processor.Write(currentClient.Attractions!), "result.json"));
                processor.OutputStream!.Close();
                Log("BotMethods", "JSON file sent to client.", LogLevel.Information);
            }
            await botClient.SendTextMessageAsync(message.Chat.Id,
                "File sent! Send me another file to start working with it (CSV/JSON)");
            currentClient.State = ClientState.Introduction;
        }
        else
        {
            await botClient.SendTextMessageAsync(message.Chat.Id,
                "Incorrect input, try again!");
        }
    }
}