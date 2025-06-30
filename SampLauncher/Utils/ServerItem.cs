namespace SAMPLauncher.Utils
{
    public class ServerItem
    {
        public string DisplayName { get; }
        public string Address { get; }

        public ServerItem(string name, string address)
        {
            DisplayName = name;
            Address = address;
        }

        public override string ToString() => DisplayName;
    }
}
