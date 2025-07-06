namespace SuperPlay.GameX.Shared.DomainLayer.Data
{
    public record PlayerData(PlayerId PlayerId, ResourceValue Coins, ResourceValue Rolls)
    {
        public ResourceValue GetResourceValue(ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.Coin => Coins,
                ResourceType.Roll => Rolls,
                _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
            };
        }
    }
}
