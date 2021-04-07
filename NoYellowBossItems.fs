namespace NoYellowBossItems
open BepInEx
open RoR2
open System.Security
open System.Security.Permissions

[<assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)>]
[<UnverifiableCode>]
do ()

[<BepInPlugin("com.xoxfaby.NoYellowBossDrops", "NoYellowBossDrops", "1.0.4.1")>]
type NoYellowBossItems() = 
    inherit BaseUnityPlugin()

    [<DefaultValue>] val mutable ItemConfig : List<string>

    member this.hook_BossGroup_DropRewards = On.RoR2.BossGroup.hook_DropRewards (fun (orig:  On.RoR2.BossGroup.orig_DropRewards) (self : RoR2.BossGroup) ->
        self.bossDrops <- System.Collections.Generic.List<PickupIndex> (seq {
            for pickupIndex in self.bossDrops do
                if not (List.contains (ItemCatalog.GetItemDef pickupIndex.pickupDef.itemIndex).nameToken this.ItemConfig) then
                    pickupIndex
        })
        orig.Invoke self)


    member this.Awake () =
        On.RoR2.BossGroup.add_DropRewards this.hook_BossGroup_DropRewards
        ItemCatalog.availability.CallWhenAvailable (fun () ->
        this.ItemConfig <- [
            for itemDef in ItemCatalog.itemDefs do 
                if itemDef.tier = ItemTier.Boss then 
                    let configEntry = this.Config.Bind("Disabled Boss Items", itemDef.nameToken, true, "Set true to stop this item from dropping.")
                    if configEntry.Value then
                        configEntry.Definition.Key
        ])

    

        
