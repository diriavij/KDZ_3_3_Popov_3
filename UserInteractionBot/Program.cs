using Microsoft.Extensions.Logging;
using Telegram.Bot;
using static UserInteractionBot.LoggingMethods;
namespace UserInteractionBot;

//Telegram bot link: t.me/csv_json_data_manager_bot
//Username: @csv_json_data_manager_bot
//The bot has been uploaded to a Beget VPS, so it works automatically.

public static class Program
{
    public static void Main()
    {
        Log("Program", "Bot started.", LogLevel.Information);
        var client = new TelegramBotClient("7198748213:AAHj3AH7URbaXUjl1QrVz9YsuEHkhZpLUWY");
        client.StartReceiving(BotMethods.HandleUpdateAsync, BotMethods.HandlePollingErrorAsync);
        Console.ReadLine(); // Bot keeps running until you press Enter.
    }
}