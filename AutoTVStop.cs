using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using System;
using System.Threading.Tasks;

namespace AutoTVStop;

public class AutoTVStopPlugin : BasePlugin
{
    public override string ModuleName => "Auto TV Stop";
    public override string ModuleVersion => "1.0.8";

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventCsWinPanelMatch>(OnMatchEnd);
        RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
        RegisterListener<Listeners.OnMapStart>(OnMapStart);
    }

    [GameEventHandler]
    public HookResult OnMatchEnd(EventCsWinPanelMatch @event, GameEventInfo info)
    {
        Console.WriteLine("Match end detected. Stopping SourceTV recording.");
        StopSourceTVRecording();
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if (@event.Winner == 3) 
        {
            Console.WriteLine("Possible match end detected on round end. Stopping SourceTV recording.");
            StopSourceTVRecording();
        }
        return HookResult.Continue;
    }

    private void OnMapStart(string mapName)
    {
        Console.WriteLine($"New map started: {mapName}. Ensuring SourceTV recording is stopped.");
        StopSourceTVRecording();
    }

private void StopSourceTVRecording()
{
    AddTimer(5.0f, () =>
    {
        ExecuteCommand("tv_stoprecord");
        Console.WriteLine("SourceTV stop recording command executed.");

        // Additional safety measure
        AddTimer(2.0f, () =>
        {
            ExecuteCommand("tv_stoprecord");
            Console.WriteLine("SourceTV stop recording command executed again for safety.");
        });
    });
}

    private void ExecuteCommand(string command)
    {
        try
        {
            Server.ExecuteCommand(command);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to execute command '{command}'. Error: {ex.Message}");
        }
    }
}
