using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers.Dialogue.Commando;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Dialog;
using SPTarkov.Server.Core.Models.Eft.Profile;

namespace MilitarySurplus;

[Injectable]
public class MilitarySurplusCommand(
    SpicyAmmoCommand spicyAmmoCommand,
    SpicyBoxesCommand spicyBoxesCommand,
    MildAmmoCommand mildAmmoCommand,
    MildBoxesCommand mildBoxesCommand,
    SpecialCommand specialCommand,
	PlainAmmoCommand plainAmmoCommand,
	PlainBoxesCommand plainBoxesCommand) : ICommandoCommand
{
    static MilitarySurplusCommand()
    {
        Console.WriteLine("[MilitarySurplus] Got some \x1b[38;2;255;165;0mspicy\x1b[0m stuff here.");
    }

    public string CommandPrefix => "surplus";

    public List<string> Commands =>
    [
        "spicyammo",
        "spicyboxes",
        "mildammo",
        "mildboxes",
        "special",
		"plainammo",
		"plainboxes"
    ];

    public string GetCommandHelp(string command)
    {
        if (command == "spicyammo")
        {
            return "surplus spicyammo - Sends spicy ammo.";
        }

        if (command == "spicyboxes")
        {
            return "surplus spicyboxes - Sends spicy boxes.";
        }

        if (command == "mildammo")
        {
            return "surplus mildammo - Sends mild ammo.";
        }

        if (command == "mildboxes")
        {
            return "surplus mildboxes - Sends mild boxes.";
        }

        if (command == "special")
        {
            return "surplus special - Sends a Special package.";
        }
		
		if(command == "plainammo")
		{
			return "surplus plainammo - Sends plain ammo.";
		}
			
		if(command == "plainboxes")
		{
			return "surplus plainboxes - Sends plain boxes.";
		}		
        return null;
    }

    public ValueTask<string> Handle(
        string command,
        UserDialogInfo commandHandler,
        MongoId sessionId,
        SendMessageRequest request)
    {
        Console.WriteLine(
            $"[MilitarySurplus] Command='{command}' Text='{request.Text}'"
        );

        if (command == "spicyammo")
        {
            return spicyAmmoCommand.Handle(sessionId);
        }

        if (command == "spicyboxes")
        {
            return spicyBoxesCommand.Handle(sessionId);
        }

        if (command == "mildammo")
        {
            return mildAmmoCommand.Handle(sessionId);
        }

        if (command == "mildboxes")
        {
            return mildBoxesCommand.Handle(sessionId);
        }

        if (command == "special")
        {
            return specialCommand.Handle(sessionId);
        }
		
		if (command == "plainammo")
		{
			return plainAmmoCommand.Handle(sessionId);
		}

		if (command == "plainboxes")
		{	
			return plainBoxesCommand.Handle(sessionId);
		}

        return ValueTask.FromResult(string.Empty);
    }
}