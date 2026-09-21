using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services.Commerce;

namespace MilitarySurplus;

[Injectable]
public class PlainBoxesCommand(MailSendService mailSendService)
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

        // All 24 plain ammo box templates
        var ammoBoxTemplates = new[]
        {
            "6570243bbfc87b3a3409321f", // 12/70 8.5mm Magnum buckshot ammo pack (25 pcs)
			"6570247ebfc87b3a34093229", // 12/70 SuperFormance HP slug ammo pack (25 pcs)
			"657024831419851aef03e703", // 20/70 5.6mm buckshot ammo pack (25 pcs)
			"657024bdc5d7d4cb4d078564", // 23x75mm Zvezda flashbang round ammo pack (5 pcs) -
			"657026341419851aef03e730", // 9x18mm PM SP7 gzh ammo pack (50 pcs)
			"657025421419851aef03e71e",  // 7.62x25mm TT P gl ammo pack (25 pcs)
			"5c1127bdd174af44217ab8b9", // 9x19mm RIP ammo pack (20 pcs)
			"65702414c5d7d4cb4d078555", // .45 ACP RIP ammo pack (50 pcs)
			"676009fe8f1fee08740f947c", // .50 AE JHP ammo pack (20 pcs)
			"657025c9cfc010a0f5006a38", // 9x21mm PE gzh ammo pack (30 pcs)
			"657023eccfc010a0f50069ef", // .357 Magnum SP ammo pack (25 pcs)
			"6570251ccfc010a0f5006a13", // 5.7x28mm R37.F ammo pack (50 pcs)
			"657024c81419851aef03e712", // 4.6x30mm Action SX ammo pack (40 pcs)
			"657984a50fbff513dd435765", // 9x39mm FMJ ammo pack (20 pcs)
			"657023fcbfc87b3a34093213", // .366 TKM Geksa ammo pack (20 pcs)
			"5737339e2459776af261abeb", // 5.45x39mm HP ammo pack (30 pcs)
			"5c11279ad174af029d64592b", // 5.56x45mm Warmageddon ammo pack (20 pcs)
			"64acea2c03378853630da53e", // 7.62x39mm HP ammo pack (20 pcs)
			"657023c61419851aef03e6eb", // .300 Whisper ammo pack (50 pcs)
			"6570255dbfc87b3a3409324a", // 7.62x51mm Ultra Nosler ammo pack (20 pcs)
			"64acee6903378853630da544", // 7.62x54mm R HP BT ammo pack (20 pcs)
			"65702420bfc87b3a34093219", // 12.7x55mm PS12A ammo pack (10 pcs)
			"68e9156326fbff63b30106a0", // .50 BMG HP ammo pack (10 pcs)
			"657023d6cfc010a0f50069e9"  // .338 Lapua Magnum TAC-X ammo pack (20 pcs) -
        };

        // These two boxes occupy 2 horizontal slots.
        var twoWideBoxes = new HashSet<string>
        {
             "657023d6cfc010a0f50069e9",  // .338 Lapua Magnum TAC-X ammo pack (20 pcs) -
			 "657024bdc5d7d4cb4d078564"   // 23x75mm Zvezda flashbang round ammo pack (5 pcs) -
        };
        // Ammo case is 7 slots wide.
        const int caseWidth = 7;

        // Track which grid cells are already occupied.
        var occupied = new HashSet<(int X, int Y)>();

        var ammoBoxItems = new List<Item>();

        foreach (var templateId in ammoBoxTemplates)
        {
            var width = twoWideBoxes.Contains(templateId) ? 2 : 1;
            var height = 1;

            var placed = false;

            // Search the case from top-left to bottom-right
            // until we find a location where the box fits.
            for (var y = 0; y < 20 && !placed; y++)
            {
                for (var x = 0; x <= caseWidth - width; x++)
                {
                    var canFit = true;

                    // Check every grid cell this box would occupy.
                    for (var checkX = 0; checkX < width; checkX++)
                    {
                        for (var checkY = 0; checkY < height; checkY++)
                        {
                            if (occupied.Contains((x + checkX, y + checkY)))
                            {
                                canFit = false;
                                break;
                            }
                        }

                        if (!canFit)
                        {
                            break;
                        }
                    }

                    if (!canFit)
                    {
                        continue;
                    }

                    // Mark the cells as occupied.
                    for (var checkX = 0; checkX < width; checkX++)
                    {
                        for (var checkY = 0; checkY < height; checkY++)
                        {
                            occupied.Add((x + checkX, y + checkY));
                        }
                    }

                    // Create the box.
                    ammoBoxItems.Add(new Item
                    {
                        Id = new MongoId(Guid.NewGuid().ToString("N")[..24]),
                        Template = new MongoId(templateId),
                        ParentId = ammoCase.Id.ToString(),
                        SlotId = "main",
                        Location = new Dictionary<string, object>
                        {
                            ["x"] = x,
                            ["y"] = y,
                            ["r"] = 0
                        },
                        Upd = new Upd
                        {
                            StackObjectsCount = 99999
                        }
                    });

                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                Console.WriteLine(
                    $"[MilitarySurplus] WARNING: Could not find space for plain ammo box {templateId}."
                );
            }
        }

        var items = new List<Item>
        {
            ammoCase
        };

        items.AddRange(ammoBoxItems);

        mailSendService.SendSystemMessageToPlayer(
            sessionId,
            "Your plain boxes have arrived.",
            items,
            null,
            null
        );

        return ValueTask.FromResult("That's a lot of Plain Boxes!!");
    }
}