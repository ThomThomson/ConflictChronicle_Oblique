using System;

namespace ConflictChronicle {

    // TODO rename this to CC_Environment_Model and only put variables in here that change with the environment.
    [Serializable]
    public class CC_EnvironmentModel {
        public string SAVE_FILE_LOCATION { get; set; }

        public CC_EnvironmentModel () {
            SAVE_FILE_LOCATION = @"C:\temp\ConflictChronicle\Worlds";
        }
    }
}