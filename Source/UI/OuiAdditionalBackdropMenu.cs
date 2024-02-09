using Celeste.Mod.UI;
using System.Linq;
using System.Text.RegularExpressions;

namespace Celeste.Mod.FewerVisualDistractions.UI;

class OuiAdditionalBackdropMenu : OuiGenericMenu, OuiModOptions.ISubmenu
{
    public override string MenuName => "BACKDROPS FROM MODS";

    public override void addOptionsToMenu(TextMenu menu)
    {
        if (FewerVisualDistractionsModule.Settings.AdditionalBackdrops.Count == 0)
        {
            menu.Add(new TextMenu.Button("No modded backdrops encountered yet!"));
            menu.Add(new TextMenu.Button("Make sure you've loaded a level with custom"));
            menu.Add(new TextMenu.Button("backdrops, and then return here."));
            return;
        }

        var modRegex = new Regex(@"^Celeste\.Mod\.([^.]+)");
        var backdropRegex = new Regex(@"\.([^.]+)$");

        foreach (var (fullName, (assembly, value)) in FewerVisualDistractionsModule.Settings.AdditionalBackdrops)
        {
            Match modMatch = modRegex.Match(fullName);
            Match backdropMatch = backdropRegex.Match(fullName);
            string displayName;

            // "Mod name: backdrop name" if possible, otherwise use the full name including namespace
            if (modMatch.Success && backdropMatch.Success)
                displayName = $"{modMatch.Groups[1].Value}: {backdropMatch.Groups[1].Value}";
            else
                displayName = fullName;

            // Hide entries from mods not currently active.
            // Save the settings though, in case it's reenabled later.
            if (!Everest.Modules.Any(m => m.GetType().Assembly.GetName().Name == assembly))
                continue;

            menu.Add(new TextMenu.OnOff(displayName, value).Change(newValue =>
                FewerVisualDistractionsModule.Settings.AdditionalBackdrops[fullName] = (assembly, newValue)));
        }
    }
}
