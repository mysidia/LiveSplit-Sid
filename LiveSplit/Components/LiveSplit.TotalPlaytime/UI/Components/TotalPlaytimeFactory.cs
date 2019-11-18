﻿using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;

[assembly: ComponentFactory(typeof(TotalPlaytimeFactory))]

namespace LiveSplit.UI.Components
{
    public class TotalPlaytimeFactory : IComponentFactory
    {
        public ComponentCategory Category => ComponentCategory.Information;

        public string ComponentName => "Total Playtime";

        public IComponent Create(LiveSplitState state) => new TotalPlaytimeComponent(state);

        public string Description => "Shows the total playtime for running with these splits.";

        public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.TotalPlaytime.xml";

        public string UpdateURL => "http://livesplit.org/update/";

        public Version Version => Version.Parse("1.7.2");

        public string UpdateName => ComponentName;
    }
}
