using System.Collections.Generic;
using System.Linq;

namespace FaceRec.API.Features.FindPerson
{
    public class SimilarPersonsCollection
    {
        private const int MAX_PERSONS = 3;

        private readonly List<KeyValuePair<double, string>> _similarPersons = new List<KeyValuePair<double, string>>();

        public void Add(double similarity, string name)
        {
            if (_similarPersons.Count == 0)
            {
                _similarPersons.Add(new KeyValuePair<double, string>(similarity, name));
                return;
            }
            
            if (_similarPersons.Count == 1 || _similarPersons.Count == 2)
            {
                _similarPersons.Add(new KeyValuePair<double, string>(similarity, name));
                _similarPersons.Sort(CompareKey);
                return;
            }

            for (var i = 0; i < MAX_PERSONS; i++)
            {
                if (similarity < _similarPersons[i].Key)
                {
                    _similarPersons.Insert(i, new KeyValuePair<double, string>(similarity, name));
                    _similarPersons.RemoveAt(MAX_PERSONS);
                    break;
                }
            }
        }

        public IList<string> GetValues()
        {
            return _similarPersons.Select(kv => kv.Value).Take(3).ToList();
        }

        static int CompareKey(KeyValuePair<double, string> a, KeyValuePair<double, string> b)
        {
            return a.Key.CompareTo(b.Key);
        }
    }
}
