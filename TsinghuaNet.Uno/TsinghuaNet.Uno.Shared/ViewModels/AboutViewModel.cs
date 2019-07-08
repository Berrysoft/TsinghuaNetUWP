﻿using System.Collections.Generic;
using TsinghuaNet.Models;
using TsinghuaNet.ViewModels;
using Windows.ApplicationModel;

namespace TsinghuaNet.Uno.ViewModels
{
    class AboutViewModel : NetViewModelBase
    {
        public PackageVersion Version { get; } = Package.Current.Id.Version;

        public IEnumerable<PackageBox> Packages { get; set; }
    }
}
