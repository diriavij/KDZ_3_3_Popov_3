using BotClientLibrary;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static UserInteractionBot.BotMethods;
using static UserInteractionBot.DataProcessing;
using static UserInteractionBot.LoggingMethods;

namespace UserInteractionBot.DataMethods;

public static class SortingMethods
{
    /// <summary>
    /// Does sorting in chosen by user way.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="botClient"></param>
    /// <param name="currentClient"></param>
    /// <param name="token"></param>
    internal static async void SortData(Message message, ITelegramBotClient botClient, Client currentClient,
        CancellationToken token)
    {
        var correctInput = new [] { "1", "2", "1. In direct order", "2. In reverse order"}; // Possible variants of
                                                                                            // correct user's input.
        if (correctInput.Contains(message.Text))
        {
            bool flag = message.Text == "1" || message.Text == "1. In direct order";
            Sort(currentClient, flag);
            await botClient.SendTextMessageAsync(message.Chat.Id,
                "Sorting has been successfully done!", cancellationToken: token);
            Log("BotMethods", "Sorting has been done.", LogLevel.Information);
            SendMainMenu(botClient, message, token);
            currentClient.State = ClientState.Menu;
        }
        else
        {
            await botClient.SendTextMessageAsync(message.Chat.Id,
                "Incorrect input, try again!", cancellationToken: token);
        }
    }
    
    /// <summary>
    /// Gives user a choice between two ways of sorting.
    /// </summary>
    /// <param name="currentClient"></param>
    /// <param name="botClient"></param>
    /// <param name="message"></param>
    /// <param name="token"></param>
    internal static async void SortDataOpenMenu(Client currentClient, ITelegramBotClient botClient,
        Message message, CancellationToken token)
    {
        var keyboard = new ReplyKeyboardMarkup
        (
            new[]
            {
                new[]
                {
                    new KeyboardButton("1. In direct order"),
                    new KeyboardButton("2. In reverse order")
                }
            }
        )
        {
            ResizeKeyboard = true
        };
        await botClient.SendTextMessageAsync(message.Chat.Id,
            "Choose one of the sorting methods (send me the number of the option or press the button):\n" +
            "1. AdmArea alphabetically in direct order\n2. AdmArea alphabetically in reverse order", 0, ParseMode.Html,
            null, false, false, false,
            null, false, keyboard, token);
        currentClient.State = ClientState.ChoosingSortingOrder;
    }
}