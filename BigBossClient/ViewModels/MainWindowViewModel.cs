using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using BigBossClient.Services;
using ReactiveUI;

namespace BigBossClient.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    private string _host;
    private string _userId;

    public string Host
    {
        get => _host;
        set => this.RaiseAndSetIfChanged(ref _host, value);
    }
    
    public string UserId 
    {
        get => _userId;
        set => this.RaiseAndSetIfChanged(ref _userId, value);
    }
    
    public ObservableCollection<string> Messages { get; } = new();
    
    public BigBossService Service { get; }

    public ReactiveCommand<Unit, Unit> ConnectCommand { get; }
    public ReactiveCommand<Unit, Unit> RequestNextNumberCommand { get; }


    public MainWindowViewModel() {
        ConnectCommand = ReactiveCommand.CreateFromTask(ConnectFunc);
        RequestNextNumberCommand = ReactiveCommand.CreateFromTask(RequestNextNumberFunc);
        Service = new BigBossService();
        Service.OnReceiveMessage += data => Messages.Add(data.ToString());
        
        Host = "localhost:5083";
        UserId = Random.Shared.Next(1, 9999).ToString();
        Messages.Add("Greetings, I'm BigBossClient");
        Messages.Add("Another message");
    }

    private async Task ConnectFunc() {
        await Service.Connect(UserId, Host);
    }
    
    private async Task RequestNextNumberFunc() {
        await Service.RequestNextNumber();
    }

}