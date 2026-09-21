using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services.Commerce;

namespace MilitarySurplus;

[Injectable]
public class PlainAmmoCommand(MailSendService mailSendService)
{
    public ValueTask<string> Handle(MongoId sessionId)
    {
        // Create the Ammo Case
        var ammoCase = new Item
        {
            Id = new MongoId(Guid.NewGuid().ToString("N")[..24]),
            Template = new MongoId("5aafbde786f774389d0cbc0f"),
            Upd = new Upd()
        };

        // All ammunition to put inside the case
        var ammoTemplates = new[]
        {
            "5d6e6806a4b936088465b17e", // 12/70 8.5mm Magnum
			"5d6e68d1a4b93622fe60e845", // 12/70 SuperFormance HP slug
			"5d6e695fa4b936359b35d852", // 20/70 5.6mm buckshot
			"5e85a9f4add9fe03027d9bf1", // 23x75mm Zvezda flashbang round
			"57372140245977611f70ee91", // 9x18mm PM SP7 gzh
			"5736026a245977644601dc61", // 7.62x25mm TT P gl
			"5c0d56a986f774449d5de529", // 9x19mm RIP
			"5ea2a8e200685063ec28c05a", // .45 ACP RIP
			"66a0d1c87d0d369e270bb9de", // .50 AE JHP
			"5a26ac06c4a282000c5a90a8", // 9x21mm PE gzh
			"62330c40bdd19b369e1e53d1", // .357 Magnum SP
			"5cc86832d7f00c000d3a6e6c", // 5.7x28mm R37.F
			"5ba26812d4351e003201fef1", // 4.6x30mm Action SX
			"6576f96220d53a5b8f3e395e", // 9x39mm FMJ
			"59e6658b86f77411d949b250", // .366 TKM Geksa
			"56dff216d2720bbd668b4568", // 5.45x39mm HP
			"5c0d5ae286f7741e46554302", // 5.56x45mm Warmageddon
			"59e4d3d286f774176a36250a", // 7.62x39mm HP
			"6196365d58ef8c428c287da1", // .300 Whisper
			"5e023e88277cce2b522ff2b1", // 7.62x51mm Ultra Nosler
			"64b8f7c241772715af0f9c3d", // 7.62x54mm R HP BT
			"5cadf6e5ae921500113bb973", // 12.7x55mm PS12A
			"67d41936f378a36c4706eeb9", // .50 BMG HP
			"5fc382b6d6fa9c00c571bbc3"  // .338 Lapua Magnum TAC-X
        };

        var ammoItems = new List<Item>();

        for (var i = 0; i < ammoTemplates.Length; i++)
        {
            ammoItems.Add(new Item
            {
                Id = new MongoId(Guid.NewGuid().ToString("N")[..24]),
                Template = new MongoId(ammoTemplates[i]),
                ParentId = ammoCase.Id.ToString(),
                SlotId = "main",
                Location = new Dictionary<string, object>
                {
                    ["x"] = i % 7,
                    ["y"] = i / 7,
                    ["r"] = 0
                },
                Upd = new Upd
                {
                    StackObjectsCount = 99999
                }
            });
        }

        var items = new List<Item>
        {
            ammoCase
        };

        items.AddRange(ammoItems);

        mailSendService.SendSystemMessageToPlayer(
            sessionId,
            "Your flavorless package is here.",
            items,
            null,
            null
        );

        return ValueTask.FromResult("That's so Plain!!");
    }
}