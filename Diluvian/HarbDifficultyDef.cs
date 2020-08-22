using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace Diluvian
{
    abstract class HarbDifficultyDef
    {
        public string BaseToken { get; protected set; }
        public string Description { get; protected set; }
        public string Name { get; protected set; }

        public string Tag { get; protected set; }

        public Color Color { get; protected set; }

        public float ScalingValue { get; protected set; }
        public float HealthRegenMod { get; protected set; }

        public string IconPath { get; protected set; }

        public string[] StartMessages { get; protected set; }
        public float MonsterRegenMod { get; protected set; }

        public float EliteModifier { get; protected set; }

        public abstract void ApplyHooks();
        public abstract void UndoHooks();

        public abstract Dictionary<string, string> Language {get;}

        protected DifficultyDef thisDef = null;

        public DifficultyDef DifficultyDef
        {
            get
            {
                if (thisDef == null)
                {
                    thisDef = new DifficultyDef(
                                scalingValue: ScalingValue,
                                nameToken: MakeToken("name", Name),
                                iconPath: makeIconPath(),
                                descriptionToken: MakeToken("desc", Description),
                                color: Color,
                                serverTag: Tag,
                                true
                          );
                }
                return thisDef;
            }
        }

        protected string MakeToken(string name, string value)
        {
            string token = BaseToken + "_" + name.ToUpper();
            LanguageAPI.Add(token, value);
            return token;
        }

        private string makeIconPath()
        {
            return DiluvianPlugin.assetString + IconPath;
        }
    }
}
