using System;
using System.Collections.Generic;
using System.IO;

namespace Drifted.Input
{
    /// <summary>
    /// Multiple styles of input 
    /// </summary>
    public class MultiMapping
    {
        Dictionary<string, AbstractInputDictionary<string>> Inputs;

        public MultiMapping(params KeyValuePair<string, AbstractInputDictionary<string>>[] mappings)
        {
            foreach(var mapping in mappings) Inputs.Add(mapping.Key, mapping.Value);
        }

        private AbstractInputDictionary<string> GetInputByMapping(string mapping)
        {
            if (Inputs.ContainsKey(mapping)) return Inputs[mapping];
            return null;
        }

        public void PushNewMapping(string mappingName, AbstractInputDictionary<string> mapping)
        {
            if (Inputs.ContainsKey(mappingName)) Inputs[mappingName] = mapping;
            else Inputs.Add(mappingName, mapping);
        }

        public bool GetKeyDown(string mapKey, string button)
        {
            var inputMapping = GetInputByMapping(mapKey);

            if(inputMapping != null) return inputMapping.GetKeyDown(button);
            return false;
        }

        public bool GetKey(string mapKey, string button)
        {
            var inputMapping = GetInputByMapping(mapKey);

            if(inputMapping != null) return inputMapping.GetKey(button);
            return false;
        }

        public bool GetKeyUp(string mapKey, string button)
        {
            var inputMapping = GetInputByMapping(mapKey);

            if (inputMapping != null) return inputMapping.GetKeyUp(button);
            return false;
        }

        public bool WriteMappingsToFile(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                // Header
                sw.WriteLine("drifted_mappings");

                // Number of mappings we have saved.
                sw.WriteLine(Inputs.Count);

                foreach(var mapping in Inputs)
                {
                    sw.WriteLine();
                }
            }

            return false;
        }
    }
}
