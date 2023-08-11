namespace GameteqTestTaskC.Models
{
    // Model class for offers
    internal class Offer
    {
        private int _id;
        private bool _forTest;
        private string _name;
        private string _key;
        private string _category;
        private List<string> _networks;
        private string _group;
        private Group _segments;

        public Offer(string name, string key, int id = 0, bool forTest = true, string category = "",
            List<string>? network = null, string group = "", Group? segments = null)
        {
            _id = id;
            _name = name ?? "";
            _key = key ?? "";
            _forTest = forTest;
            _category = category ?? "";
            _networks = network ?? new List<string>();
            _group = group ?? "";
            _segments = segments ?? new Group();
        }

        public int Id { get => _id; set => _id = value; }
        public bool ForTest { get => _forTest; }
        public string Name { get => _name; }
        public string Key { get => _key; }
        public string Category { get => _category; }
        public List<string> Networks { get => _networks; }
        public string Group { get => _group; }
        public Group Segments { get => _segments; }

        public override bool Equals(object? obj)
        {
            return obj is Offer offer &&
                   _id == offer.Id &&
                   _name == offer._name &&
                   _key == offer._key;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_id, _name, _key);
        }
    }
}
