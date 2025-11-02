// namespace SignalRServer.Models
// {
//     public class Room
//     {
//         public string Name { get; set; }
//         public List<string> ConnectedUsers { get; set; }

//         public Room(string name)
//         {
//             Name = name;
//             ConnectedUsers = new List<string>();
//         }

//         public void AddUser(string userName)
//         {
//             if (!ConnectedUsers.Contains(userName))
//             {
//                 ConnectedUsers.Add(userName);
//             }
//         }
//         public void RemoveUser(string userName)
//         {
//             if (ConnectedUsers.Contains(userName))
//             {
//                 ConnectedUsers.Remove(userName);
//             }
//         }
//     }
// }

// This isn't even used yet, so I've commented it out for now.