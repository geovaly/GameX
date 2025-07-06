using RequestResponseFramework.Shared;
using RequestResponseFramework.Shared.Requests;
using SuperPlay.GameX.Shared.ApiLayer;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Frontend.ConsoleGameClient
{
    public class GameProgram(IGameClient client)
    {
        private PlayerLoggedInContext? _loggedInContext;

        public async Task Run()
        {
            client.EventsReceived += EventsReceived;
            while (client.IsRunning)
            {
                try
                {
                    await ExecuteNextAction();
                }
                catch (NetworkSystemException)
                {
                    Console.WriteLine("Network Error. Press any key to exit.");
                    Console.ReadKey();
                    return;
                }
            }
        }

        private async Task ExecuteNextAction()
        {
            Console.WriteLine();
            Console.WriteLine(
                "Choose action: [1]Login  [2]GetMyPlayer  [3]UpdateResources  [4]SendGift  [5]Logout  [6]Exit");
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    await Login();
                    break;
                case "2":
                    await GetMyPlayer();
                    break;
                case "3":
                    await UpdateResources();
                    break;
                case "4":
                    await SendGift();
                    break;
                case "5":
                    await Logout();
                    break;
                case "6":
                    await client.DisposeAsync();
                    break;
                default:
                    Console.WriteLine("Unknown choice.");
                    break;
            }
        }

        private async Task Login()
        {
            var deviceId = ReadDeviceId();
            var result = await client.TryExecuteAsync(new LoginCommand(deviceId));
            WriteLineRequestResult(result);
            SetLoggedInContext(result);
        }

        private async Task Logout()
        {
            if (EnsurePlayerIsLoggedIn()) return;
            var result = await client.TryExecuteAsync(new LogoutCommand(GetLoggedInContext()));
            WriteLineRequestResult(result);
            SetLoggedInContext(result);
        }

        private async Task GetMyPlayer()
        {
            if (EnsurePlayerIsLoggedIn()) return;
            var result = await client.TryExecuteAsync(new GetMyPlayerQuery(GetLoggedInContext()));
            WriteLineRequestResult(result);
        }

        private async Task UpdateResources()
        {
            if (EnsurePlayerIsLoggedIn()) return;
            var resourceType = ReadResourceType();
            var resourceValue = ReadResourceValue();
            var result = await client.TryExecuteAsync(new UpdateResourcesCommand(GetLoggedInContext(), resourceType, resourceValue));
            WriteLineRequestResult(result);
        }

        private async Task SendGift()
        {
            if (EnsurePlayerIsLoggedIn()) return;
            var friendPlayerId = ReadFriendPlayerId();
            var resourceType = ReadResourceType();
            var resourceValue = ReadResourceValue();
            var result = await client.TryExecuteAsync(new SendGiftCommand(GetLoggedInContext(), friendPlayerId, resourceType, resourceValue));
            WriteLineRequestResult(result);
        }

        private PlayerLoggedInContext GetLoggedInContext() => _loggedInContext ?? throw new InvalidOperationException();

        private void EventsReceived(object? sender, Event e)
        {
            Console.WriteLine($"Event Received: {e}");
        }

        private void SetLoggedInContext(Response result)
        {
            if (result is Ok<LoginResult> okLogin)
            {
                _loggedInContext = new PlayerLoggedInContext(okLogin.Value.PlayerId);
            }
            else if (result is Ok<LogoutResult>)
            {
                _loggedInContext = null;
            }

        }

        private bool EnsurePlayerIsLoggedIn()
        {
            if (_loggedInContext == null)
            {
                Console.WriteLine("Error: Please login first");
                return true;
            }

            return false;
        }

        private static PlayerId ReadPlayerId(String msg = "PlayerId: ")
        {
            Console.WriteLine(msg);
            while (true)
            {
                var line = Console.ReadLine()!.Trim();
                if (int.TryParse(line, out var value))
                {
                    return new PlayerId(value);
                }
                else
                {
                    Console.WriteLine("Invalid PlayerId. Try again. ");
                }
            }

        }

        private static PlayerId ReadFriendPlayerId() => ReadPlayerId("Friend PlayerId: ");

        private static ResourceValue ReadResourceValue()
        {
            Console.WriteLine("DeltaResourceValue (int): ");
            while (true)
            {
                if (int.TryParse(Console.ReadLine()!.Trim(), out var value))
                {
                    return new ResourceValue(value);
                }
                else
                {
                    Console.WriteLine("Invalid Int. Try again. ");
                }
            }
        }

        private static ResourceType ReadResourceType()
        {
            Console.WriteLine("ResourceType (int): [1] Coins, [2] Rolls");
            while (true)
            {
                if (int.TryParse(Console.ReadLine()!.Trim(), out var value) && Enum.IsDefined(typeof(ResourceType), value))
                {
                    return (ResourceType)value;
                }
                else
                {
                    Console.WriteLine("Invalid Enum. Try again. ");
                }
            }
        }


        private static DeviceId ReadDeviceId()
        {
            Console.WriteLine("Enter DeviceId: ");
            while (true)
            {
                var line = Console.ReadLine()!.Trim();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    return new DeviceId(line);
                }
                else
                {
                    Console.WriteLine("Invalid DeviceId. Try again. ");
                }
            }
        }

        private static void WriteLineRequestResult(Response result)
        {
            Console.WriteLine($"Response: {result}");
        }
    }
}
