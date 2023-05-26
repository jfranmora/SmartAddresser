using System;
using System.IO;
using UnityEngine.Assertions;

namespace SmartAddresser.Editor.Core.Models.Shared
{
    public enum PartialAssetPathType
    {
        FileName,
        FileNameWithoutExtensions,
        AssetPath
    }

    public static class PartialAssetPathTypeExtensions
    {
        public static string Create(this PartialAssetPathType self, string assetPath)
        {
            Assert.IsFalse(string.IsNullOrEmpty(assetPath));

            switch (self)
            {
                case PartialAssetPathType.FileName:
                    return Path.GetFileName(assetPath);
                case PartialAssetPathType.FileNameWithoutExtensions:
                    return Path.GetFileNameWithoutExtension(assetPath);
                case PartialAssetPathType.AssetPath:
                    return assetPath;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
