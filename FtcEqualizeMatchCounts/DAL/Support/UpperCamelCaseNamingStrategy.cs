using Newtonsoft.Json.Serialization;

namespace FEMC.DAL.Support
    {
    // See https://futurestud.io/tutorials/gson-builder-basics-naming-policies
    // We seek to mirror FieldNamingPolicy.UPPER_CAMEL_CASE
    public class UpperCamelCaseNamingStrategy : NamingStrategy
        {
        public UpperCamelCaseNamingStrategy(bool processDictionaryKeys, bool overrideSpecifiedNames)
            {
            ProcessDictionaryKeys = processDictionaryKeys;
            OverrideSpecifiedNames = overrideSpecifiedNames;
            }

        public UpperCamelCaseNamingStrategy(bool processDictionaryKeys, bool overrideSpecifiedNames, bool processExtensionDataNames)
            : this(processDictionaryKeys, overrideSpecifiedNames)
            {
            ProcessExtensionDataNames = processExtensionDataNames;
            }

        public UpperCamelCaseNamingStrategy()
            {
            }

        // Make the first letter upper case
        protected override string ResolvePropertyName(string name)
            {
            if (string.IsNullOrEmpty(name))
                return name;

            char[] chars = name.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
                {
                if (char.IsLetter(chars[i]))
                    {
                    chars[i] = char.ToUpperInvariant(chars[i]);
                    break;
                    }
                }

            return new string(chars);
            }
        }
    }
