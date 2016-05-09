using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace VectorSpaceModel.Components
{
    public class Document : Vector
    {

        public Document(string id, AssignmentSE.Category category ,params string[] terms)
            : this(id,terms)
        {
            this.category = category;
        }

        public Document(string id, params string[] terms)
            : this(terms)
        {
            ID = id;
        }

        public Document(params string[] terms)
        {
            _terms = terms.ToList();
            foreach (string word in terms)
            {

                if (!wordIsInList(word))
                {
                    _regularFrequency.Add(word, 0);
                }
                else
                {
                    _regularFrequency[word]++;
                }
            }

        }

        public bool wordIsInList(string word)
        {
            for (int i = 0; i < _regularFrequency.Count; i++)
            {
                if (_regularFrequency.ElementAt(i).Key.Equals(word, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;

        }

        public string ID { get; private set; }

        public int Count
        {
            get { return _terms.Count; }
        }

        public string this[int index]
        {
            get { return _terms[index]; }
        }


        
        public override string ToString()
        {
            return string.Format("ID: {0}, Terms: [{1}]", ID,
                string.Join(",", _terms.Select(term => term.ToString()).ToArray()));
        }
    }
}