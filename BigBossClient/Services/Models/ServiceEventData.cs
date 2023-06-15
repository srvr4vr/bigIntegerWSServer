using System.Numerics;

namespace BigBossClient.Services.Models;

public struct ServiceEventData {
    public ServiceEventType Type;
    public string? Message;

    private ServiceEventData(ServiceEventType type, string? message = null) {
        Type = type;
        Message = message;
    }

    public static ServiceEventData CreateError(string message) => new(ServiceEventType.Error, message);
    
    public static ServiceEventData CreateResult(string message) => new(ServiceEventType.Ok, message);

    public override string ToString()
        => Type == ServiceEventType.Error
            ? Message ?? "[ERROR]: unknown error"
            : string.IsNullOrEmpty(Message)
                ? "[ERROR]: no number"
                : $"[OK] {Message}";
}