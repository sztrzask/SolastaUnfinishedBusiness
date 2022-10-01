﻿using System;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Builders;

public abstract class
    MorphotypeElementDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : MorphotypeElementDefinition
    where TBuilder : MorphotypeElementDefinitionBuilder<TDefinition, TBuilder>
{
    public TBuilder SetCategory(MorphotypeElementDefinition.ElementCategory value)
    {
        Definition.category = value;
        return This();
    }

    public TBuilder SetMainColor(Color value)
    {
        Definition.mainColor = value;
        return This();
    }

    public TBuilder SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag value)
    {
        Definition.subClassFilterMask = value;
        return This();
    }

    public TBuilder SetSortOrder(int value)
    {
        Definition.guiPresentation.sortOrder = value;
        return This();
    }

    #region Constructors

    protected MorphotypeElementDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected MorphotypeElementDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected MorphotypeElementDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original,
        name,
        namespaceGuid)
    {
    }

    protected MorphotypeElementDefinitionBuilder(TDefinition original, string name, string definitionGuid) : base(
        original, name,
        definitionGuid)
    {
    }

    #endregion
}

[UsedImplicitly]
internal class MorphotypeElementDefinitionBuilder : MorphotypeElementDefinitionBuilder<MorphotypeElementDefinition,
    MorphotypeElementDefinitionBuilder>
{
    #region Constructors

    protected MorphotypeElementDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected MorphotypeElementDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected MorphotypeElementDefinitionBuilder(MorphotypeElementDefinition original, string name, Guid namespaceGuid)
        : base(original, name,
            namespaceGuid)
    {
    }

    protected MorphotypeElementDefinitionBuilder(MorphotypeElementDefinition original, string name,
        string definitionGuid) : base(original,
        name, definitionGuid)
    {
    }

    #endregion
}
