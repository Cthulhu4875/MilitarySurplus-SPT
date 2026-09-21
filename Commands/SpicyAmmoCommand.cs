using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services.Commerce;

namespace MilitarySurplus;

[Injectable]
public class SpicyAmmoCommand(MailSendService mailSendService)
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
            "5ba26835d4351e0035628ff5", // 4.6x30mm AP SX
            "5cc80f38e4a949001152b560", // 5.7x28mm SS190
            "5c0d5e4486f77478390952fe", // 5.45x39mm PPBS Igolnik
            "601949593ae8f707c4608daa", // 5.56x45mm SSA AP
            "573603562459776430731618", // 7.62x25mm TT Pst
            "573719df2459775a626ccbc2", // 9x18mm PBM
            "5efb0da7a29a85116f6ea05f", // 9x19mm PBP
            "6576f4708ca9c4381d16cd9d", // 9x21mm 7N42 Zubilo
            "5efb0cabfb3e451d70735af5", // .45 ACP AP
            "668fe62ac62660a5d8071446", // .50 AE AE FMJ
            "5fd20ff893a8961fc660a954", // .300 BLK AP
            "601aa3d2b2bcb34913271e6d", // 7.62x39mm MAI AP
            "5c0d688c86f77413ae3407b2", // 9x39mm BP
            "5f0596629e22f464da6bbdd9", // .366 TKM AP-M
            "62330b3ed4dc74626d570b95", // .357 FMJ
            "6529243824cbe3c74a05e5c1", // 6.8x51mm SIG HYBRID
            "6768c25aa7b238f14a08d3f6", // 7.62x51mm M80A1
            "5e023d48186a883be655e551", // 7.62x54R BS
            "5fc382a9d724d907e2077dab", // .388 Lapua AP
            "5cadf6eeae921500134b2799", // 12.7x55mm PS12B
            "67dc2648ba5b79876906a166", // .50 BMG M903 SLAP
            "660137d8481cc6907a0c5cda", // 20/70 TSS AP
            "5d6e6911a4b9361bd5780d52", // 12/70 Flechette
            "5d6e68a8a4b9360b6c0d54e2", // 12/70 AP-20 Slug
            "5e85aa1a988a8701445df1f5"  // 23x75mm Barrikada Slug
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
            "Your spicy package is here.",
            items,
            null,
            null
        );

        return ValueTask.FromResult("That's Spicy!!");
    }
}
