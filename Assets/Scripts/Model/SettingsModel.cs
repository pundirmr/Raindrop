using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LumosLabs.Shared.Pillar;

namespace LumosLabs.Raindrops
{
    public class SettingsModel : AssetModel<SettingsAsset>
    {
        public const string Key = "Raindrops.Model.Settings";
        
        protected override string GetStoreKey()
        {
            return Key;
        }
    }
}
