namespace Shared
{
    public record EventPayload
    {
        public EventPayload(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
