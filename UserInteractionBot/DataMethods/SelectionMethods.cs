using BotClientLibrary;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static UserInteractionBot.BotMethods;
using static UserInteractionBot.DataProcessing;
namespace UserInteractionBot.DataMethods;
using static LoggingMethods;

public static class SelectionMethods
{
    /// <summary>
    /// Does chosen by user selection.
    /// </summary>
    /// <param name="currentClient"></param>
    /// <param name="message"></param>
    /// <param name="botClient"></param>
    /// <param name="token"></param>
    internal static async void SelectData(Client currentClient, Message message, ITelegramBotClient botClient,
        CancellationToken token)
    {
        if (message.Text != null)
        {
            if (currentClient is { SelectionField: "AdmArea Location", ExtraValue: null }) // Doesn't change user's
                                                                                           // state, so they can enter another value.
            {
                currentClient.ExtraValue = message.Text;
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "Enter a value for selection (Location):", cancellationToken: token);
            }
            else
            {
                if (currentClient.SelectionField == "AdmArea Location")
                {
                    Filter(currentClient, currentClient.SelectionField!, currentClient.ExtraValue!,
                        message.Text);
                }
                else
                {
                    Filter(currentClient, currentClient.SelectionField!, message.Text);
                }
                if (currentClient.Attractions!.Count == 0)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        "The result of processing was an empty list of objects", cancellationToken: token);
                }
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "Selection has been successfully done!", cancellationToken: token);
                Log("BotMethods", "Selection has been done.", LogLevel.Information);
                SendMainMenu(botClient, message, token);
                currentClient.State = ClientState.Menu;
            }
        }
        else
        {
            await botClient.SendTextMessageAsync(message.Chat.Id,
                "Incorrect input, try again!", cancellationToken: token);
        }
    }
    
    /// <summary>
    /// Proceeds user's selection field choice.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="botClient"></param>
    /// <param name="currentClient"></param>
    /// <exception cref="ArgumentException"></exception>
    internal static async void ChooseSelectionField(Message message, ITelegramBotClient botClient, Client currentClient)
    {
        // Possible variants of correct user's input.
        var correctInput = new [] { "1", "2", "3", "1. District", "2. LocationType", "3. AdmArea and Location"};
        if (correctInput.Contains(message.Text))
        {
            currentClient.SelectionField = message.Text switch
            {
                "1" or "1. District" => "District",
                "2" or "2. LocationType" => "LocationType",
                "3" or "3. AdmArea and Location" => "AdmArea Location",
                _ => throw new ArgumentException()
            };
            string extraMessage = currentClient.SelectionField == "AdmArea Location" ? "(AdmArea)" : "";
            await botClient.SendTextMessageAsync(message.Chat.Id,
                $"Your choice has been processed! Enter a value for selection {extraMessage}");
            currentClient.State = ClientState.InputtingSelectionValue;
        }
        else
        {
            await botClient.SendTextMessageAsync(message.Chat.Id,
                "Incorrect input, try again!");
        }
    }
    
    /// <summary>
    /// Gives user a choice between different selection fields.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="message"></param>
    /// <param name="token"></param>
    /// <param name="currentClient"></param>
    internal static async void SelectDataOpenMenu(ITelegramBotClient botClient, Message message, CancellationToken token,
        Client currentClient)
    {
        var keyboard = new ReplyKeyboardMarkup
        (
            new[]
            {
                new[]
                {
                    new KeyboardButton("1. District"),
                    new KeyboardButton("2. LocationType"),
                    new KeyboardButton("3. AdmArea and Location")
                }
            }
        )
        {
            ResizeKeyboard = true
        };
        await botClient.SendTextMessageAsync(message.Chat.Id,
            "Choose one of the fields for selection (send me the number of the option or press the button):\n" +
            "1. District\n2. LocationType\n3. AdmArea and Location", 0, ParseMode.Html,
            null, false, false, false,
            null, false, keyboard, token);
        currentClient.State = ClientState.ChoosingSelectionField;
    }
}