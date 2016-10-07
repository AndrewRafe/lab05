using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab05 {
    public class NPCManager {

        public List<NPC> listOfNPCs { get; private set; } 

        public NPCManager() {
            listOfNPCs = new List<NPC>();
        }

        public void addNPC(NPC npc) {
            listOfNPCs.Add(npc);
        } 

    }
}
