using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services.Commerce;

namespace MilitarySurplus;

[Injectable]
public class SpicyBoxesCommand(MailSendService mailSendService)
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

        // All 25 ammo box templates
        var ammoBoxTemplates = new[]
        {
            "657025ebc5d7d4cb4d078588", // 5.45x39 PPBS Box 120pc - 2x1
            "65702474bfc87b3a34093226", // 12/70 Flechette Box 25pc
            "64898838d5b4df6140000a20", // 12/17 AP-20 Slug Box 25pc
            "67657764c832f8c59c016d45", // 20/70 TSS AP Box 25pc
            "657024b8bfc87b3a34093232", // 23x75mm Barrikada Pack 5pc - 2x1
            "65702610cfc010a0f5006a41", // 9x18mm PBM Box 50pc
            "65702546cfc010a0f5006a1f", // 7.62x25mm Pst Box 25pc
            "648987d673c462723909a151", // 9x19mm PBP Box 50pc
            "6489879db5a2df1c815a04ef", // .45 ACP AP Box 50pc
            "676009ed8f1fee08740f9479", // .50 AE AE FMJ Box 20pc
            "6579847c5a0e5879d12f2873", // 9x21mm 7N42 Zubilo Box 30pc
            "648986bbc827d4637f01791e", // 5.7x28mm SS190 Box 50pc
            "6489870774a806211e4fb685", // 4.6x30mm AP SX Box 40pc
            "6489854673c462723909a14e", // 9x39mm BP Box 20pc
            "657023f81419851aef03e6f1", // .366 TKM AP-M Box 20pc
            "65702681bfc87b3a3409325f", // 5.56x45mm SSA AP Box 100pc - 2x1
            "6489851fc827d4637f01791b", // 7.62x39 MAI AP Box 20pc
            "648985c074a806211e4fb682", // .300 BLK AP Box 50pc
            "67600a516f01341c9106ab4c", // 6.8x51mm SIG HYBRID Box 20pc
            "6769b8e3c1a1466c850658a8", // 7.62x51mm M80A1 Box 20pc
            "648984b8d5b4df6140000a1a", // 7.62x54R BS Box 20pc
            "648983d6b5a2df1c815a04ec", // 12.7x55mm PS12B Box 10pc
            "6489848173c462723909a14b", // .388 Lapua AP Box 20pc - 2x1
            "657023decfc010a0f50069ec", // .357 FMJ Box 25pc
            "68e915dfd996b7754e0f25c9"  // .50 BMG M903 SLAP Box 10pc
        };

        // These four boxes occupy 2 horizontal slots.
        var twoWideBoxes = new HashSet<string>
        {
            "657025ebc5d7d4cb4d078588", // 5.45x39 PPBS
            "657024b8bfc87b3a34093232", // 23x75mm Barrikada
            "65702681bfc87b3a3409325f", // 5.56x45mm SSA AP
            "6489848173c462723909a14b"  // .388 Lapua AP
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
                    $"[MilitarySurplus] WARNING: Could not find space for ammo box {templateId}."
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
            "Your spicy boxes have arrived.",
            items,
            null,
            null
        );

        return ValueTask.FromResult("That's a lot of Spicy Boxes!!");
    }
}
