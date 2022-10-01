﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

internal static class CharacterFilteringGroupPatcher
{
    [HarmonyPatch(typeof(CharacterFilteringGroup), "Compare")]
    internal static class Compare_Patch
    {
        //PATCH: correctly offers on adventures with min/max caps on character level (MULTICLASS)
        private static int MyLevels(IEnumerable<int> levels)
        {
            return levels.Sum();
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var bypass = 0;
            var myLevelMethod = new Func<IEnumerable<int>, int>(MyLevels).Method;
            var levelsField = typeof(RulesetCharacterHero.Snapshot).GetField("Levels");

            foreach (var instruction in instructions)
            {
                if (bypass-- > 0)
                {
                    continue;
                }

                yield return instruction;

                if (!instruction.LoadsField(levelsField))
                {
                    continue;
                }

                yield return new CodeInstruction(OpCodes.Call, myLevelMethod);

                bypass = 2;
            }
        }
    }
}
