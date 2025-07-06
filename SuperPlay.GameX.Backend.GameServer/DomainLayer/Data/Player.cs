using SuperPlay.GameX.Shared.DomainLayer.Data;
using System.ComponentModel.DataAnnotations;

namespace SuperPlay.GameX.Backend.GameServer.DomainLayer.Data
{

    public class Player
    {
        private static readonly ResourceValue DefaultCoins = new(0);
        private static readonly ResourceValue DefaultRolls = new(0);

        public required PlayerId PlayerId { get; set; }
        public required DeviceId DeviceId { get; set; }
        public required ResourceValue Coins { get; set; }
        public required ResourceValue Rolls { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }


        public static Player CreateNewPlayer(DeviceId deviceId)
        {
            return new Player
            {
                PlayerId = PlayerId.Empty,
                DeviceId = deviceId,
                Coins = DefaultCoins,
                Rolls = DefaultRolls,
            };
        }
        public ResourceValue GetResourceValue(ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.Coin => Coins,
                ResourceType.Roll => Rolls,
                _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
            };
        }

        public void UpdateResourceValue(ResourceType resourceType, ResourceValue deltaResourceValue)
        {
            switch (resourceType)
            {
                case ResourceType.Coin:
                    UpdateCoins(deltaResourceValue);
                    break;
                case ResourceType.Roll:
                    UpdateRolls(deltaResourceValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null);
            }
        }

        public void UpdateCoins(ResourceValue deltaCoins)
        {
            Coins = Coins.Update(deltaCoins);
        }

        public void UpdateRolls(ResourceValue deltaRolls)
        {
            Rolls = Rolls.Update(deltaRolls);
        }
    }
}
