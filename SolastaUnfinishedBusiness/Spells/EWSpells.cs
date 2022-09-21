﻿using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Utils;
using UnityEngine;
using static ConditionOperationDescription;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;
using static SolastaUnfinishedBusiness.Models.SpellsContext;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Spells;

internal static class EwSpells
{
    internal static void Register()
    {
        RegisterSpell(BuildSunlightBlade(), 0, SpellListWarlock, SpellListWizard, SpellListSorcerer);
        RegisterSpell(BuildResonatingStrike(), 0, SpellListWarlock, SpellListWizard, SpellListSorcerer);
    }

    private static SpellDefinition BuildSunlightBlade()
    {
        var highlight = new ConditionOperationDescription
        {
            hasSavingThrow = false,
            operation = ConditionOperation.Add,
            conditionDefinition = ConditionDefinitionBuilder
                .Create(DatabaseHelper.ConditionDefinitions.ConditionHighlighted, "ConditionSunlightBladeHighlighted",
                    DefinitionBuilder.CENamespaceGuid)
                .SetSpecialInterruptions(RuleDefinitions.ConditionInterruption.Attacked)
                .SetDuration(RuleDefinitions.DurationType.Round, 1)
                .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn)
                .SetSpecialDuration(true)
                .AddToDB()
        };

        var dimLight = new LightSourceForm
        {
            brightRange = 0,
            dimAdditionalRange = 2,
            lightSourceType = RuleDefinitions.LightSourceType.Basic,
            color = new Color(0.9f, 0.8f, 0.4f),
            graphicsPrefabReference = DatabaseHelper.FeatureDefinitionAdditionalDamages
                .AdditionalDamageBrandingSmite.LightSourceForm.graphicsPrefabReference
        };

        var sunlitMark = ConditionDefinitionBuilder
            .Create("ConditionSunlightBladeMarked")
            .SetGuiPresentationNoContent()
            .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttacked)
            .AddSpecialInterruptions(RuleDefinitions.ConditionInterruption.AnyBattleTurnEnd)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .SetDuration(RuleDefinitions.DurationType.Round, 1)
            .AddToDB();

        return SpellDefinitionBuilder
            .Create("SunlightBlade", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Spell,
                CustomIcons.CreateAssetReferenceSprite("SunlightBlade", Resources.SunlightBlade, 128,
                    128))
            .SetSpellLevel(0)
            .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSomaticComponent(true)
            .SetVerboseComponent(false)
            .SetMaterialComponent(RuleDefinitions.MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 0, false)
            .SetCustomSubFeatures(
                PerformAttackAfterMagicEffectUse.MeleeAttack,
                new UpgradeRangeBasedOnWeaponReach(),
                CustomSpellEffectLevel.ByCasterLevel
            )
            .SetCastingTime(RuleDefinitions.ActivationTime.Action)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.ScorchingRay)
                .SetTargetingData(
                    RuleDefinitions.Side.Enemy,
                    RuleDefinitions.RangeType.Distance,
                    1,
                    RuleDefinitions.TargetType.IndividualsUnique
                )
                .SetSavingThrowData(
                    false,
                    false,
                    AttributeDefinitions.Dexterity,
                    true,
                    RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature,
                    AttributeDefinitions.Charisma
                )
                .SetEffectAdvancement(
                    RuleDefinitions.EffectIncrementMethod.CasterLevelTable,
                    additionalDicePerIncrement: 1,
                    incrementMultiplier: 1
                )
                .SetDurationData(RuleDefinitions.DurationType.Round, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.None)
                    .SetConditionForm(ConditionDefinitionBuilder
                            .Create("ConditionSunlightBlade", DefinitionBuilder.CENamespaceGuid)
                            .SetGuiPresentation(Category.Condition)
                            .SetSpecialInterruptions(RuleDefinitions.ConditionInterruption.AnyBattleTurnEnd)
                            .SetSilent(Silent.WhenAddedOrRemoved)
                            .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn)
                            .SetDuration(RuleDefinitions.DurationType.Round, 1)
                            .SetFeatures(FeatureDefinitionAdditionalDamageBuilder
                                .Create("AdditionalDamageSunlightBlade", DefinitionBuilder.CENamespaceGuid)
                                .Configure(
                                    "SunlightBlade",
                                    RuleDefinitions.FeatureLimitedUsage.None,
                                    RuleDefinitions.AdditionalDamageValueDetermination.Die,
                                    RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive,
                                    RuleDefinitions.RestrictedContextRequiredProperty.MeleeWeapon,
                                    true,
                                    RuleDefinitions.DieType.D8,
                                    1,
                                    RuleDefinitions.AdditionalDamageType.Specific,
                                    RuleDefinitions.DamageTypeRadiant,
                                    RuleDefinitions.AdditionalDamageAdvancement.SlotLevel,
                                    DiceByRankMaker.MakeBySteps(0, step: 5, increment: 1)
                                )
                                .SetTargetCondition(sunlitMark, RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasCondition)
                                .SetConditionOperations(highlight)
                                .SetAddLightSource(true)
                                .SetLightSourceForm(dimLight)
                                .AddToDB()
                            )
                            .AddToDB(),
                        ConditionForm.ConditionOperation.Add,
                        true,
                        false
                    ).Build(),
                    EffectFormBuilder.Create()
                        .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.None)
                        .SetConditionForm(sunlitMark, ConditionForm.ConditionOperation.Add)
                        .Build()
                )
                .Build()
            )
            .AddToDB();
    }

    private static SpellDefinition BuildResonatingStrike()
    {
        var resonanceHighLevel = new EffectDescriptionBuilder()
            .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.AcidSplash)
            .SetTargetFiltering(RuleDefinitions.TargetFilteringMethod.CharacterOnly)
            .SetTargetingData(RuleDefinitions.Side.Enemy, RuleDefinitions.RangeType.Touch, 1,
                RuleDefinitions.TargetType.Individuals)
            .SetEffectForms(new EffectFormBuilder()
                .SetBonusMode(RuleDefinitions.AddBonusMode.AbilityBonus)
                .SetDamageForm(
                    dieType: RuleDefinitions.DieType.D8,
                    diceNumber: 0,
                    bonusDamage: 0,
                    damageType: RuleDefinitions.DamageTypeThunder
                )
                .Build()
            )
            .SetEffectAdvancement(
                RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel,
                5, additionalDicePerIncrement: 1)
            .Build();

        var resonanceLeap = SpellDefinitionBuilder
            .Create("ResonatingStrikeLeap", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentationNoContent()
            .SetSpellLevel(1)
            .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSomaticComponent(false)
            .SetVerboseComponent(false)
            .SetCustomSubFeatures(
                new BonusSlotLevelsByClassLevel(),
                new UpgradeEffectFromLevel(resonanceHighLevel, 5)
            )
            .SetCastingTime(RuleDefinitions.ActivationTime.Action)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.AcidSplash)
                .SetTargetFiltering(RuleDefinitions.TargetFilteringMethod.CharacterOnly)
                .SetTargetingData(RuleDefinitions.Side.Enemy, RuleDefinitions.RangeType.Touch, 1,
                    RuleDefinitions.TargetType.Individuals)
                .SetEffectForms(new EffectFormBuilder()
                    .SetBonusMode(RuleDefinitions.AddBonusMode.AbilityBonus)
                    .SetDamageForm(
                        dieType: RuleDefinitions.DieType.D1,
                        diceNumber: 0,
                        bonusDamage: 0,
                        damageType: RuleDefinitions.DamageTypeThunder
                    )
                    .Build()
                )
                .Build()
            )
            .AddToDB();


        return SpellDefinitionBuilder
            .Create("ResonatingStrike", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Spell,
                CustomIcons.CreateAssetReferenceSprite("ResonatingStrike", Resources.ResonatingStrike,
                    128, 128)) //TODO: replace sprite with actual image
            .SetSpellLevel(0)
            .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSomaticComponent(true)
            .SetVerboseComponent(false)
            .SetMaterialComponent(RuleDefinitions.MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 0, false)
            .SetCustomSubFeatures(
                PerformAttackAfterMagicEffectUse.MeleeAttack,
                CustomSpellEffectLevel.ByCasterLevel,
                new ChainSpellEffectOnAttackHit(resonanceLeap, "ResonatingStrike")
            )
            .SetCastingTime(RuleDefinitions.ActivationTime.Action)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.ScorchingRay)
                .SetTargetingData(
                    RuleDefinitions.Side.Enemy,
                    RuleDefinitions.RangeType.Distance,
                    5,
                    RuleDefinitions.TargetType.IndividualsUnique,
                    2
                )
                .SetTargetProximityData(true, 1)
                .SetSavingThrowData(
                    false,
                    false,
                    AttributeDefinitions.Dexterity,
                    true,
                    RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature,
                    AttributeDefinitions.Charisma
                )
                .SetEffectAdvancement(
                    RuleDefinitions.EffectIncrementMethod.CasterLevelTable,
                    additionalDicePerIncrement: 1,
                    incrementMultiplier: 1
                )
                .SetDurationData(RuleDefinitions.DurationType.Round, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.None)
                    .SetConditionForm(ConditionDefinitionBuilder
                            .Create("ConditionResonatingStrike", DefinitionBuilder.CENamespaceGuid)
                            .SetGuiPresentation(Category.Condition)
                            .SetSpecialInterruptions(RuleDefinitions.ConditionInterruption.Attacks)
                            .SetSilent(Silent.WhenAddedOrRemoved)
                            .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn)
                            .SetDuration(RuleDefinitions.DurationType.Round, 1)
                            .SetFeatures(FeatureDefinitionAdditionalDamageBuilder
                                .Create("AdditionalDamageResonatingStrike", DefinitionBuilder.CENamespaceGuid)
                                .Configure(
                                    "ResonatingStrike",
                                    RuleDefinitions.FeatureLimitedUsage.None,
                                    RuleDefinitions.AdditionalDamageValueDetermination.Die,
                                    RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive,
                                    RuleDefinitions.RestrictedContextRequiredProperty.MeleeWeapon,
                                    true,
                                    RuleDefinitions.DieType.D8,
                                    1,
                                    RuleDefinitions.AdditionalDamageType.Specific,
                                    RuleDefinitions.DamageTypeThunder,
                                    RuleDefinitions.AdditionalDamageAdvancement.SlotLevel,
                                    DiceByRankMaker.MakeBySteps(0, step: 5, increment: 1),
                                    true
                                )
                                .AddToDB()
                            )
                            .AddToDB(),
                        ConditionForm.ConditionOperation.Add,
                        true,
                        false
                    )
                    .Build()
                )
                .Build()
            )
            .AddToDB();
    }
}

internal sealed class ChainSpellEffectOnAttackHit : IChainMagicEffect
{
    private readonly string _notificationTag;
    private readonly SpellDefinition _spell;


    public ChainSpellEffectOnAttackHit(SpellDefinition spell, [CanBeNull] string notificationTag = null)
    {
        _spell = spell;
        _notificationTag = notificationTag;
    }

    [CanBeNull]
    public CharacterActionMagicEffect GetNextMagicEffect([CanBeNull] CharacterActionMagicEffect baseEffect,
        CharacterActionAttack triggeredAttack, RuleDefinitions.RollOutcome attackOutcome)
    {
        if (baseEffect == null) { return null; }

        var spellEffect = baseEffect as CharacterActionCastSpell;

        var repertoire = spellEffect?.ActiveSpell.SpellRepertoire;

        var actionParams = baseEffect.actionParams;
        if (actionParams == null) { return null; }

        if (baseEffect.Countered || baseEffect.ExecutionFailed)
        {
            return null;
        }

        if (attackOutcome != RuleDefinitions.RollOutcome.Success
            && attackOutcome != RuleDefinitions.RollOutcome.CriticalSuccess)
        {
            return null;
        }

        var caster = actionParams.ActingCharacter;
        var targets = actionParams.TargetCharacters;

        if (caster == null || targets.Count < 2) { return null; }

        var rulesetCaster = caster.RulesetCharacter;
        var rules = ServiceRepository.GetService<IRulesetImplementationService>();
        var bonusLevelProvider = _spell.GetFirstSubFeatureOfType<IBonusSlotLevels>();
        var slotLevel = _spell.SpellLevel;
        if (bonusLevelProvider != null)
        {
            slotLevel += bonusLevelProvider.GetBonusSlotLevels(rulesetCaster);
        }

        var effectSpell = rules.InstantiateEffectSpell(rulesetCaster, repertoire, _spell, slotLevel, false);

        for (var i = 1; i < targets.Count; i++)
        {
            var rulesetTarget = targets[i].RulesetCharacter;
            if (!string.IsNullOrEmpty(_notificationTag))
            {
                GameConsoleHelper.LogCharacterAffectsTarget(rulesetCaster, rulesetTarget, _notificationTag, true);
            }

            effectSpell.ApplyEffectOnCharacter(rulesetTarget, true, targets[i].LocationPosition);
        }

        effectSpell.Terminate(true);

        return null;
    }
}

internal interface IBonusSlotLevels
{
    public int GetBonusSlotLevels(RulesetCharacter caster);
}

internal sealed class BonusSlotLevelsByClassLevel : IBonusSlotLevels
{
    public int GetBonusSlotLevels([NotNull] RulesetCharacter caster)
    {
        return caster.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
    }
}
