﻿
namespace Masa.Blazor.Docs.Examples.components.windows.Usages;

public class Usage : Components.Usage
{
    protected override Type UsageWrapperType => typeof(UsageWrapper);

    public Usage() : base(typeof(MWindow))
    {
    }
}