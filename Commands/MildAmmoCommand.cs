using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services.Commerce;

namespace MilitarySurplus;

[Injectable]
public class MildAmmoCommand(MailSendService mailSendService)
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

        // All 25 mild ammunition types
        var ammoTemplates = new[]
        {
            "5ba26844d4351e00334c9475", // 4.6x30mm Subsonic SX
            "5cc80f53e4a949000e1ea4f8", // 5.7x28mm L191
            "56dff2ced2720bb4668b4567", // 5.45x39mm PP gs
            "59e690b686f7746c9f75e848", // 5.56x45mm M995
            "573603c924597764442bd9cb", // 7.62x25mm TT PT gzh
            "573719762459775a626ccbc1", // 9x18mm PM P gzh
            "5c925fa22e221601da359b7b", // 9x19mm AP 6.3
            "5a26ac0ec4a28200741e1e18", // 9x21mm BT gzh
            "5efb0d4f4bc50b58e81710f3", // .45 ACP Lasermatch FMJ
            "66a0d1e0ed648d72fe064d06", // .50 AE Copper Solid
            "619636be6db0f2477964e710", // .300 BLK M62 Tracer
            "5656d7c34bdc2d9d198b4587", // 7.62x39mm PS gzh
            "5c0d668f86f7747ccb7f13b2", // 9x39mm SPP gs
            "59e655cb86f77411dc52a77b", // .366 TKM EKO
            "62330c18744e5e31df12f516", // .357 Magnum JHP
            "6529302b8c26af6326029fb7", // 6.8x51mm SIG FMJ
            "5a608bf24f39f98ffc77720e", // 7.62x51mm M62 Tracer
            "5887431f2459777e1612938f", // 7.62x54R LPS gzh
            "5fc382c1016cce60e8341b20", // .338 Lapua UCW
            "5cadf6ddae9215051e1c23b2", // 12.7x55mm PS12
            "67d41936f378a36c4706eeb9", // .50 BMG HP
            "660137ef76c1b56143052be8", // 20/70 Dangerous Game Slug
            "5d6e68c4a4b9361b93413f79", // 12/70 makeshift .50 BMG slug
            "5d6e67fba4b9361bc73bc779", // 12/70 6.5mm Express buckshot
            "5f647f31b6238e5dd066e196"  // 23x75mm Shrapnel-25 buckshot
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

        // Put the case and all ammo into the player's mail
        var items = new List<Item>
        {
            ammoCase
        };

        items.AddRange(ammoItems);

        mailSendService.SendSystemMessageToPlayer(
            sessionId,
            "Your mild package is here.",
            items,
            null,
            null
        );

        return ValueTask.FromResult("That's Mild!!");
    }
}
