using System;
using System.Threading.Tasks;
using BigBossClient.Services.Models;

namespace BigBossClient.Services; 

public interface IBigBossService {
    event Action<ServiceEventData> OnReceiveMessage; 
    Task Connect(string userId, string host);
    Task RequestNextNumber();
    Task Disconect();
}