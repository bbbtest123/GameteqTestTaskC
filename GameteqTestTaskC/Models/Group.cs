using GameteqTestTaskC.Helpers;

namespace GameteqTestTaskC.Models
{
    // A model class for groups/segments
    internal class Group
    {
        private Connective _connective;
        private List<string> _segmentsValues = new();
        private List<Group> _groups = new();

        public Connective Connective { get => _connective; }
        public List<string> SegmentsValues { get => _segmentsValues; }
        public List<Group> Groups { get => _groups; }

        public Group(Connective connective = Connective.Or, List<string>? segmentsValues = null, List<Group>? groups = null)
        {
            _connective = connective;
            _segmentsValues = segmentsValues ?? new List<string>();
            _groups = groups ?? new List<Group>();
        }

        public void AddSegment(string segmentValue) => _segmentsValues.Add(segmentValue);

        public void ChangeSegmentValue(int index, string segmentValue) => _segmentsValues[index] = segmentValue;

        public void RemoveSegment(int index) => _segmentsValues.RemoveAt(index);

        public void AddGroup(Group group) => _groups.Add(group);

        public void RemoveGroup(int index) => _groups.RemoveAt(index);
    }
}
