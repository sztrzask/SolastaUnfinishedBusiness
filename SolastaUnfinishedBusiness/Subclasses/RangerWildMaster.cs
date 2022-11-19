﻿using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;
using static ActionDefinitions;
using static FeatureDefinitionAttributeModifier;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RangerWildMaster : AbstractSubclass
{
    private const string SpiritBeastTag = "SpiritBeast";
    private const string CommandSpiritBeastCondition = "ConditionWildMasterSpiritBeastCommand";
    internal const string SummonSpiritBeastPower = "PowerWildMasterSummonSpiritBeast";

    internal RangerWildMaster()
    {
        var actionAffinitySpiritBeast =
            FeatureDefinitionActionAffinityBuilder
                .Create("ActionAffinityWildMasterSpiritBeast")
                .SetGuiPresentationNoContent()
                .SetDefaultAllowedActionTypes()
                .SetForbiddenActions(Id.AttackMain, Id.AttackOff, Id.AttackReadied, Id.AttackOpportunity, Id.Ready,
                    Id.PowerMain, Id.PowerBonus, Id.PowerReaction, Id.SpendPower)
                .SetCustomSubFeatures(new SummonerHasConditionOrKOd())
                .AddToDB();

        var conditionAffinityWildMasterSpiritBeastInitiative =
            FeatureDefinitionConditionAffinityBuilder
                .Create("ConditionAffinityWildMasterSpiritBeastInitiative")
                .SetGuiPresentationNoContent()
                .SetConditionAffinityType(ConditionAffinityType.Immunity)
                .SetConditionType(ConditionDefinitions.ConditionSurprised)
                .SetCustomSubFeatures(ForceInitiativeToSummoner.Mark)
                .AddToDB();

        var perceptionAffinitySpiritBeast =
            FeatureDefinitionPerceptionAffinityBuilder
                .Create("PerceptionAffinityWildMasterSpiritBeast")
                .SetGuiPresentationNoContent()
                .CannotBeSurprised()
                .AddToDB();

        var powerWildMasterSummonSpiritBeastPool = FeatureDefinitionPowerBuilder
            .Create("PowerWildMasterSummonSpiritBeastPool")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .AddToDB();

        var spellEffectLevelFromSummonerLevel = new SpellEffectLevelFromSummonerLevel();

        var powerWildMasterInvisibility = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerGreen_Hag_Invisibility, "PowerWildMasterInvisibility")
            .SetCustomSubFeatures(spellEffectLevelFromSummonerLevel)
            .AddToDB();

        var combatAffinityWildMasterBeastIsNextToSummoner = FeatureDefinitionCombatAffinityBuilder
            .Create(FeatureDefinitionCombatAffinitys.CombatAffinityPackTactics,
                "CombatAffinityWildMasterBeastIsNextToSummoner")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetSituationalContext(ExtraSituationalContext.BeastIsNextToSummoner)
            .AddToDB();

        var combatAffinityWildMasterSummonerIsNextToBeast = FeatureDefinitionCombatAffinityBuilder
            .Create(FeatureDefinitionCombatAffinitys.CombatAffinityPackTactics,
                "CombatAffinityWildMasterSummonerIsNextToBeast")
            .SetSituationalContext(ExtraSituationalContext.SummonerIsNextToBeast)
            .AddToDB();

        #region EAGLE

        var powerWildMasterBreathWeaponLightning = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerDragonbornBreathWeaponBlue, "PowerWildMasterBreathWeaponLightning")
            .SetCustomSubFeatures(spellEffectLevelFromSummonerLevel)
            .AddToDB();

        var powerWildMasterResilienceLightning = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerFiendishResilienceLightning, "PowerWildMasterResilienceLightning")
            .SetCustomSubFeatures(spellEffectLevelFromSummonerLevel)
            .AddToDB();

        var powerWildMasterEyebiteLightning = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerEyebitePanicked, "PowerWildMasterEyebiteLightning")
            .SetCustomSubFeatures(spellEffectLevelFromSummonerLevel)
            .AddToDB();

        var powerKindredSpiritEagle03 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool,
            null, MonsterDefinitions.KindredSpiritEagle, 3, false,
            powerWildMasterBreathWeaponLightning,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            actionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        var powerKindredSpiritEagle07 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool,
            powerKindredSpiritEagle03, MonsterDefinitions.KindredSpiritEagle, 7, false,
            powerWildMasterBreathWeaponLightning,
            powerWildMasterResilienceLightning,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            actionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        var powerKindredSpiritEagle11 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool,
            powerKindredSpiritEagle07, MonsterDefinitions.KindredSpiritEagle, 11, true,
            powerWildMasterBreathWeaponLightning,
            powerWildMasterResilienceLightning,
            powerWildMasterInvisibility,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            actionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        var powerKindredSpiritEagle15 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool,
            powerKindredSpiritEagle11, MonsterDefinitions.KindredSpiritEagle, 15, true,
            powerWildMasterBreathWeaponLightning,
            powerWildMasterResilienceLightning,
            powerWildMasterInvisibility,
            powerWildMasterEyebiteLightning,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            perceptionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        #endregion

        #region BEAR

        var powerWildMasterBreathWeaponFire = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerDragonbornBreathWeaponGold, "PowerWildMasterBreathWeaponFire")
            .SetCustomSubFeatures(spellEffectLevelFromSummonerLevel)
            .AddToDB();

        var powerWildMasterResilienceFire = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerFiendishResilienceFire, "PowerWildMasterResilienceFire")
            .SetCustomSubFeatures(spellEffectLevelFromSummonerLevel)
            .AddToDB();

        var powerWildMasterEyebiteFire = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerEyebitePanicked, "PowerWildMasterEyebiteFire")
            .SetCustomSubFeatures(spellEffectLevelFromSummonerLevel)
            .AddToDB();

        var powerKindredSpiritBear03 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool,
            null, MonsterDefinitions.KindredSpiritBear, 3, false,
            powerWildMasterBreathWeaponFire,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            actionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        var powerKindredSpiritBear07 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool,
            powerKindredSpiritEagle03, MonsterDefinitions.KindredSpiritBear, 7, false,
            powerWildMasterBreathWeaponFire,
            powerWildMasterResilienceFire,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            actionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        var powerKindredSpiritBear11 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool,
            powerKindredSpiritEagle07, MonsterDefinitions.KindredSpiritBear, 11, true,
            powerWildMasterBreathWeaponFire,
            powerWildMasterResilienceFire,
            powerWildMasterInvisibility,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            actionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        var powerKindredSpiritBear15 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool,
            powerKindredSpiritEagle11, MonsterDefinitions.KindredSpiritBear, 15, true,
            powerWildMasterBreathWeaponFire,
            powerWildMasterResilienceFire,
            powerWildMasterInvisibility,
            powerWildMasterEyebiteFire,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            perceptionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        #endregion

        #region WOLF

        var powerWildMasterBreathWeaponCold = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerDragonbornBreathWeaponSilver, "PowerWildMasterBreathWeaponCold")
            .SetCustomSubFeatures(spellEffectLevelFromSummonerLevel)
            .AddToDB();

        var powerWildMasterResilienceCold = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerFiendishResilienceCold, "PowerWildMasterResilienceCold")
            .SetCustomSubFeatures(spellEffectLevelFromSummonerLevel)
            .AddToDB();

        var powerWildMasterEyebiteCold = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerEyebiteAsleep, "PowerWildMasterEyebiteCold")
            .SetCustomSubFeatures(spellEffectLevelFromSummonerLevel)
            .AddToDB();

        var powerKindredSpiritWolf03 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool,
            null, MonsterDefinitions.KindredSpiritWolf, 3, false,
            powerWildMasterBreathWeaponCold,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            actionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        var powerKindredSpiritWolf07 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool,
            powerKindredSpiritWolf03, MonsterDefinitions.KindredSpiritWolf, 7, false,
            powerWildMasterBreathWeaponCold,
            powerWildMasterResilienceCold,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            actionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        var powerKindredSpiritWolf11 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool,
            powerKindredSpiritWolf07, MonsterDefinitions.KindredSpiritWolf, 11, true,
            powerWildMasterBreathWeaponCold,
            powerWildMasterResilienceCold,
            powerWildMasterInvisibility,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            actionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        var powerKindredSpiritWolf15 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool,
            powerKindredSpiritWolf11, MonsterDefinitions.KindredSpiritWolf, 15, true,
            powerWildMasterBreathWeaponCold,
            powerWildMasterResilienceCold,
            powerWildMasterInvisibility,
            powerWildMasterEyebiteCold,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            perceptionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        #endregion

        var featureSetWildMaster03 = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetWildMaster03")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                BuildCommandSpiritBeast(),
                BuildPowerWildMasterSpiritBeastRecuperate(),
                BuildSpiritBeastAffinityLevel03(),
                powerKindredSpiritBear03,
                powerKindredSpiritEagle03,
                powerKindredSpiritWolf03)
            .AddToDB();

        var featureSetWildMaster07 = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetWildMaster07")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                BuildSpiritBeastAffinityLevel07(),
                powerKindredSpiritBear07,
                powerKindredSpiritEagle07,
                powerKindredSpiritWolf07)
            .AddToDB();

        var featureSetWildMaster11 = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetWildMaster11")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                BuildSpiritBeastAffinityLevel11(),
                powerKindredSpiritBear11,
                powerKindredSpiritEagle11,
                powerKindredSpiritWolf11)
            .AddToDB();

        var featureSetWildMaster15 = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetWildMaster15")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerKindredSpiritBear15,
                powerKindredSpiritEagle15,
                powerKindredSpiritWolf15)
            .AddToDB();

        FeatureDefinitionPowers.PowerEyebiteAsleep.guiPresentation.spriteReference =
            EyebiteAsleep.guiPresentation.spriteReference;

        FeatureDefinitionPowers.PowerEyebitePanicked.guiPresentation.spriteReference =
            EyebitePanicked.guiPresentation.spriteReference;

        FeatureDefinitionPowers.PowerEyebiteSickened.guiPresentation.spriteReference =
            EyebiteSickened.guiPresentation.spriteReference;

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("RangerWildMaster")
            .SetGuiPresentation(Category.Subclass, PatronFiend)
            .AddFeaturesAtLevel(3, powerWildMasterSummonSpiritBeastPool, featureSetWildMaster03,
                combatAffinityWildMasterBeastIsNextToSummoner)
            .AddFeaturesAtLevel(7, featureSetWildMaster07)
            .AddFeaturesAtLevel(11, featureSetWildMaster11)
            .AddFeaturesAtLevel(15, featureSetWildMaster15)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;

    private static FeatureDefinition BuildPowerWildMasterSpiritBeastRecuperate()
    {
        const string NAME = "PowerWildMasterSpiritBeastRecuperate";

        RestActivityDefinitionBuilder
            .Create("RestActivityWildMasterSpiritBeastRecuperate")
            .SetGuiPresentation(NAME, Category.Feature)
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RestType.ShortRest,
                RestActivityDefinition.ActivityCondition.CanUsePower,
                PowerBundleContext.UseCustomRestPowerFunctorName,
                NAME)
            .AddToDB();

        var power = FeatureDefinitionPowerBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(
                PowerVisibilityModifier.Hidden,
                HasModifiedUses.Marker,
                new ValidatorsPowerUse(HasInjuredBeast),
                new ModifyRestPowerTitleHandler(GetRestPowerTitle),
                new RetargetSpiritBeast())
            .SetUsesFixed(ActivationTime.Rest, RechargeRate.LongRest, 1, 0)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetHealingForm(HealingComputation.Dice, 0, DieType.D8, 1, false, HealingCap.MaximumHitPoints)
                    .Build())
                .Build())
            .AddToDB();

        power.AddCustomSubFeatures(new PowerUseModifier
        {
            PowerPool = power, Type = PowerPoolBonusCalculationType.ClassLevel, Attribute = RangerClass
        });

        return power;
    }

    private static RulesetCharacter GetSpiritBeast(RulesetCharacter character)
    {
        var spiritBeastEffect =
            character.powersUsedByMe.Find(p => p.sourceDefinition.Name.StartsWith(SummonSpiritBeastPower));
        var summons = EffectHelpers.GetSummonedCreatures(spiritBeastEffect);

        return summons.Empty() ? null : summons[0];
    }

    private static bool HasInjuredBeast(RulesetCharacter character)
    {
        var spiritBeast = GetSpiritBeast(character);

        return spiritBeast is { IsMissingHitPoints: true };
    }

    private static string GetRestPowerTitle(RulesetCharacter character)
    {
        var spiritBeast = GetSpiritBeast(character);

        if (spiritBeast == null)
        {
            return string.Empty;
        }

        return Gui.Format("Feature/&PowerWildMasterSpiritBeastRecuperateFormat",
            spiritBeast.CurrentHitPoints.ToString(),
            spiritBeast.TryGetAttributeValue(AttributeDefinitions.HitPoints).ToString());
    }

    private static FeatureDefinitionPower BuildSpiritBeastPower(
        FeatureDefinitionPower sharedPoolPower,
        FeatureDefinitionPower powerToReplace,
        MonsterDefinition monsterDefinition,
        int level,
        bool groupAttacks,
        params FeatureDefinition[] monsterAdditionalFeatures)
    {
        var spiritBeastMonster =
            BuildSpiritBeastMonster(monsterDefinition, level, groupAttacks, monsterAdditionalFeatures);
        var name = SummonSpiritBeastPower + monsterDefinition.name + level;

        return FeatureDefinitionPowerSharedPoolBuilder
            .Create(name)
            .SetGuiPresentation(
                Gui.Format("Feature/&PowerWildMasterSummonSpiritBeastTitle",
                    spiritBeastMonster.FormatTitle()),
                Gui.Format("Feature/&PowerWildMasterSummonSpiritBeastDescription",
                    spiritBeastMonster.FormatTitle()),
                monsterDefinition)
            .SetSharedPool(ActivationTime.Action, sharedPoolPower)
            .SetOverriddenPower(powerToReplace)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Permanent)
                .SetTargetingData(Side.Ally, RangeType.Distance, 3, TargetType.Position)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetSummonCreatureForm(1, spiritBeastMonster.Name)
                    .Build())
                .SetParticleEffectParameters(ConjureElementalAir)
                .Build())
            .SetUniqueInstance()
            .SetCustomSubFeatures(SkipEffectRemovalOnLocationChange.Always, ValidatorsPowerUse.NotInCombat)
            .AddToDB();
    }

    private static MonsterDefinition BuildSpiritBeastMonster(
        MonsterDefinition monsterDefinition,
        int level,
        bool groupAttacks,
        params FeatureDefinition[] monsterAdditionalFeatures)
    {
        return MonsterDefinitionBuilder
            .Create(monsterDefinition, "WildMasterSpiritBeast" + monsterDefinition.name + level)
            .AddFeatures(monsterAdditionalFeatures)
            .SetCreatureTags(SpiritBeastTag)
            .SetChallengeRating(0)
            .SetFullyControlledWhenAllied(true)
            .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
            .SetDungeonMakerPresence(MonsterDefinition.DungeonMaker.None)
            .NoExperienceGain()
            .SetGroupAttacks(groupAttacks)
            .AddToDB();
    }

    private static FeatureDefinition BuildSpiritBeastAffinityLevel03()
    {
        var acBonus = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierWildMasterSummonSpiritBeastAC")
            .SetGuiPresentationNoContent()
            .SetModifier(AttributeModifierOperation.AddConditionAmount, AttributeDefinitions.ArmorClass)
            .AddToDB();

        var hpBonus = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierWildMasterSummonSpiritBeastHP")
            .SetGuiPresentationNoContent()
            .SetModifier(AttributeModifierOperation.AddConditionAmount, AttributeDefinitions.HitPoints)
            .AddToDB();

        var toHit = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierWildMasterSummonSpiritBeastToHit")
            .SetGuiPresentationNoContent()
            .SetAttackRollModifier(1, AttackModifierMethod.SourceConditionAmount)
            .AddToDB();

        var toDamage = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierWildMasterSummonSpiritBeastDamage")
            .SetGuiPresentationNoContent()
            .SetDamageRollModifier(1, AttackModifierMethod.SourceConditionAmount)
            .AddToDB();

        return FeatureDefinitionSummoningAffinityBuilder
            .Create("SummoningAffinityWildMasterSummonSpiritBeast03")
            .SetGuiPresentationNoContent()
            .SetRequiredMonsterTag(SpiritBeastTag)
            .SetAddedConditions(
                //Bonuses from Ranger's Proficiency Bonus
                ConditionDefinitionBuilder
                    .Create("ConditionWildMasterSummonSpiritBeastAcBonus")
                    .SetGuiPresentation("Condition/&ConditionWildMasterSummonSpiritBeastBonusTitle", Gui.NoLocalization)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetPossessive()
                    .SetAmountOrigin(ConditionDefinition.OriginOfAmount.SourceSpellCastingAbility)
                    .SetFeatures(acBonus)
                    .AddToDB(),
                //Bonuses from Ranger's Spell Casting Ability
                ConditionDefinitionBuilder
                    .Create("ConditionWildMasterSummonSpiritBeastToHitBonus")
                    .SetGuiPresentation("Condition/&ConditionWildMasterSummonSpiritBeastBonusTitle", Gui.NoLocalization)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetPossessive()
                    .SetAmountOrigin(ConditionDefinition.OriginOfAmount.SourceSpellCastingAbility)
                    .SetFeatures(toHit)
                    .AddToDB(),
                //Bonuses from Ranger's Proficiency Bonus
                ConditionDefinitionBuilder
                    .Create("ConditionWildMasterSummonSpiritBeastDamageBonus")
                    .SetGuiPresentation("Condition/&ConditionWildMasterSummonSpiritBeastBonusTitle", Gui.NoLocalization)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ConditionDefinition.OriginOfAmount.SourceSpellCastingAbility)
                    .SetFeatures(toDamage)
                    .AddToDB(),
                //Bonuses from Ranger's level
                ConditionDefinitionBuilder
                    .Create("ConditionWildMasterSummonSpiritBeastLevel")
                    .SetGuiPresentationNoContent()
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceClassLevel)
                    //Set damage type to class name so `ExtraOriginOfAmount.SourceClassLevel` would know what class to use
                    .SetAdditionalDamageWhenHit(damageType: RangerClass, active: false)
                    .SetFeatures(hpBonus, hpBonus, hpBonus, hpBonus) // 4 HP per level
                    .AddToDB())
            .AddToDB();
    }

    private static FeatureDefinition BuildSpiritBeastAffinityLevel07()
    {
        return FeatureDefinitionSummoningAffinityBuilder
            .Create(FeatureDefinitionSummoningAffinitys.SummoningAffinityKindredSpiritMagicalSpirit,
                "SummoningAffinityWildMasterSummonSpiritBeast07")
            .SetRequiredMonsterTag(SpiritBeastTag)
            .AddToDB();
    }

    private static FeatureDefinition BuildSpiritBeastAffinityLevel11()
    {
        return FeatureDefinitionSummoningAffinityBuilder
            .Create("SummoningAffinityWildMasterSummonSpiritBeast11")
            .SetGuiPresentationNoContent()
            .SetRequiredMonsterTag(SpiritBeastTag)
            .SetAddedConditions(
                ConditionDefinitionBuilder
                    .Create("ConditionWildMasterSummonSpiritBeastSaving")
                    .SetGuiPresentationNoContent()
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ConditionDefinition.OriginOfAmount.SourceSpellAttack)
                    .SetFeatures(
                        FeatureDefinitionSavingThrowAffinityBuilder
                            .Create("SavingThrowAffinityWildMasterSummonSpiritBeast")
                            .SetGuiPresentationNoContent()
                            .SetCustomSubFeatures(new AddPBToSummonCheck(1,
                                AttributeDefinitions.Strength,
                                AttributeDefinitions.Dexterity,
                                AttributeDefinitions.Constitution,
                                AttributeDefinitions.Intelligence,
                                AttributeDefinitions.Wisdom,
                                AttributeDefinitions.Charisma))
                            .AddToDB())
                    .AddToDB())
            .AddToDB();
    }

    private static FeatureDefinition BuildCommandSpiritBeast()
    {
        var condition = ConditionDefinitionBuilder
            .Create(CommandSpiritBeastCondition)
            .SetGuiPresentationNoContent()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetDuration(DurationType.Round, 1)
            .SetSpecialDuration()
            .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
            .AddToDB();

        var power = FeatureDefinitionPowerBuilder
            .Create("PowerWildMasterSpiritBeastCommand")
            .SetGuiPresentation(Category.Feature, Command)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(condition, ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .SetCustomSubFeatures(new ShowInCombatWhenHasSpiritBeast())
            .AddToDB();

        power.AddCustomSubFeatures(new ApplyOnTurnEnd(condition, power));

        return power;
    }

    private class SummonerHasConditionOrKOd : IDefinitionApplicationValidator, ICharacterTurnStartListener
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            // if commanded allow anything
            if (IsCommanded(locationCharacter.RulesetCharacter))
            {
                return;
            }

            // force dodge action if not at level 7 yet
            if (locationCharacter.RulesetCharacter.GetMySummoner()?.RulesetCharacter is RulesetCharacterHero hero
                && hero.ClassesAndLevels[CharacterClassDefinitions.Ranger] < 7)
            {
                ServiceRepository.GetService<ICommandService>()
                    ?.ExecuteAction(new CharacterActionParams(locationCharacter, Id.Dodge), null, false);
            }
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character)
        {
            //Apply limits if not commanded
            return !IsCommanded(character);
        }

        private static bool IsCommanded(RulesetCharacter character)
        {
            //can act freely outside of battle
            if (Gui.Battle == null)
            {
                return true;
            }

            var summoner = character.GetMySummoner()?.RulesetCharacter;

            //shouldn't happen, but consider being commanded in this case
            if (summoner == null)
            {
                return true;
            }

            //can act if summoner is KO
            return summoner.IsUnconscious ||
                   //can act if summoner commanded
                   summoner.HasConditionOfType(CommandSpiritBeastCondition);
        }
    }

    private class ApplyOnTurnEnd : ICharacterTurnEndListener
    {
        private readonly ConditionDefinition condition;
        private readonly FeatureDefinitionPower power;

        public ApplyOnTurnEnd(ConditionDefinition condition, FeatureDefinitionPower power)
        {
            this.condition = condition;
            this.power = power;
        }

        public void OnCharacterTurnEnded(GameLocationCharacter locationCharacter)
        {
            var status = locationCharacter.GetActionStatus(Id.PowerBonus, ActionScope.Battle);

            if (status != ActionStatus.Available)
            {
                return;
            }

            var character = locationCharacter.RulesetCharacter;
            var newCondition = RulesetCondition.CreateActiveCondition(character.Guid, condition, DurationType.Round, 1,
                TurnOccurenceType.StartOfTurn, locationCharacter.Guid, character.CurrentFaction.Name);

            character.AddConditionOfCategory(AttributeDefinitions.TagCombat, newCondition);
            GameConsoleHelper.LogCharacterUsedPower(character, power);
        }
    }

    private class ShowInCombatWhenHasSpiritBeast : IPowerUseValidity
    {
        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower featureDefinitionPower)
        {
            return ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress &&
                   character.powersUsedByMe.Any(p => p.sourceDefinition.Name.StartsWith(SummonSpiritBeastPower));
        }
    }

    private class RetargetSpiritBeast : IRetargetCustomRestPower
    {
        public GameLocationCharacter GetTarget(RulesetCharacter character)
        {
            var spiritBeast = GetSpiritBeast(character);

            return spiritBeast == null ? null : GameLocationCharacter.GetFromActor(spiritBeast);
        }
    }

    private sealed class SpellEffectLevelFromSummonerLevel : ICustomSpellEffectLevel
    {
        public int GetEffectLevel(RulesetActor caster)
        {
            if (caster is RulesetCharacterMonster monster &&
                monster.GetMySummoner().RulesetCharacter is RulesetCharacterHero hero)
            {
                return hero.ClassesAndLevels[CharacterClassDefinitions.Ranger];
            }

            return 1;
        }
    }
}
