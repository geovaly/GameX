# GameX
Solution to a technical exercise for a job application. 

# Senior Server Developer exercise

Create a Net8.0 Solution with the relevant projects for the below requirements.
- Create a basic Game Server which accepts WebSockets (use Dotnet’s System.Net only, do not use signalR or mediatR or any other lib)
  - Implement handlers for the following socket messages (all messages from client should be sent through the socket only):
    - Login
      - Accept DeviceId(UDID) and response with PlayerId
      - Make sure the player is not connected already, If so respond accordingly.
    - UpdateResources
      - Accept ResourceType(coins, rolls), ResourceValue and Response with new balance.
    - SendGift
      - Accept FriendPlayerId, ResourceType and ResourceValue.
      - Update the sending player’s player state and the friend player state.
      - If the friend is online then send a GiftEvent with the relevant information to him.
  - Make sure that the socket messages routing can be easily extended with more handlers in a clean way.
  - Player state can be saved in Ram or Sqlite.
- Create a client console Application to test the GameServer APIs.
- Use the Serilog library to effectively log both Server and Client.
- Make sure to use a clean and highly professional attitude when preparing this Solution.
  
Good luck!

# Conclusion

Software development is inherently subjective, with multiple valid approaches to solving the same problems. The solution presented reflects my best judgment in the absence of predefined team agreements.
