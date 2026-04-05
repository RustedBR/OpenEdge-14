using Content.Shared._OE14.Input;
using Robust.Shared.Input;

namespace Content.Client._OE14.Input
{
    public static class OE14ContentContexts
    {
        public static void SetupContexts(IInputContextContainer contexts)
        {
            var human = contexts.GetContext("human");
            human.AddFunction(OE14ContentKeyFunctions.OpenBelt2);
            human.AddFunction(OE14ContentKeyFunctions.SmartEquipBelt2);
            human.AddFunction(OE14ContentKeyFunctions.OE14OpenSkillMenu);
        }
    }
}
