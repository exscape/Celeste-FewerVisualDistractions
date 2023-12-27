using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Celeste.Mod.BackdropHider;
public static class BackdropBlacklist
{
    public static void Load()
    {
        IL.Celeste.BackdropRenderer.Render += BackdropRendererHook;
    }

    public static void Unload()
    {
        IL.Celeste.BackdropRenderer.Render -= BackdropRendererHook;
    }

    private static void BackdropRendererHook(ILContext il)
    {
        // foreach (Backdrop backdrop in this.Backdrops)
        // {
        //+  if (!IsBackdropEnabled(backdrop))
        //+    continue;    
        //   if (backdrop.Visible)

        ILCursor cursor = new(il);
        ILLabel loopEnd = default;
        if (!cursor.TryGotoNext(
          instr => instr.MatchLdloc(2),
          instr => instr.MatchLdfld<Backdrop>("Visible"),
          instr => instr.MatchBrfalse(out loopEnd)
        ))
        {
            Logger.Log(LogLevel.Error, "BackdropHider", "Couldn't find BackdropRenderer.Render CIL sequence to hook!");
            return;
        }
        cursor.Emit(OpCodes.Ldloc_2);
        cursor.EmitDelegate(IsBackdropEnabled);
        cursor.Emit(OpCodes.Brfalse, loopEnd);
    }

    // determine whether the backdrop should render
    private static bool IsBackdropEnabled(Backdrop backdrop) => backdrop switch
    {
        Snow => BackdropHiderModule.Settings.Snow,
        // ...
        _ => true
    };
}