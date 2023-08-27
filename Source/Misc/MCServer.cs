namespace WinBot.Misc; 

public class MCServer {
    public string address;
    public string crackedInfo;
    public string dynmap;
    public ulong guildID;
    public string versions;

    public MCServer(ulong guildID, string address, string dynmap, string versions) {
        this.guildID = guildID;
        this.address = address;
        this.dynmap = dynmap;
        this.versions = versions;
        crackedInfo = "No. It never will, just buy the game or stop asking.";
    }

    public MCServer(ulong guildID, string address, string versions) {
        this.guildID = guildID;
        this.address = address;
        this.versions = versions;
        dynmap = null;
        crackedInfo = "No. It never will, just buy the game or stop asking.";
    }

    public MCServer() {
        guildID = 0;
        address = "address";
        dynmap = "dynmap";
        versions = "versions";
        crackedInfo = "No. It never will, just buy the game or stop asking.";
    }
}
