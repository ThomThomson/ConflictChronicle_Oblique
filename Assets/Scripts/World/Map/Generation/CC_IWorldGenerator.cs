using System.Collections.Generic;

namespace ConflictChronicle {

    public interface CC_IWorldGenerator {
        MapModel GenerateWorld (SettingsController settings);
    }

}