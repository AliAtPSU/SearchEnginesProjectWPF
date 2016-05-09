using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VectorSpaceModel.Components
{
    public class Corpus : IEnumerable<string>
    {
        public IList<Document> _documents;
        public IDictionary<string, double> _invertedDocumentFrequency = new Dictionary<string, double>();
        public IList<string> _vocabulary;

        public Corpus(params Document[] documents)
            : this(documents.ToList())
        {
        }

        public Corpus(IEnumerable<Document> documents) : this(documents, true)
        {
        }

        public Corpus(IEnumerable<Document> documents, bool calculateIDF)
        {
            if (documents == null)
            {
                throw new ArgumentException("Error! documents collection is null.");
            }
            _documents = documents.ToList();
            _vocabulary = _documents.SelectMany(term => term).Distinct(StringComparer.InvariantCultureIgnoreCase).ToList();
            if (calculateIDF)
            {
                CalculateInverseDocumentFrequency();
            }
        }


        public IList<Document> Documents
        {
            get { return _documents; }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _vocabulary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void CalculateInverseDocumentFrequency()
        {
            foreach (string term in _vocabulary)
            {
                if (_invertedDocumentFrequency.ContainsKey(term)) continue;
                double termCount = _documents.Select(document => document.BooleanTermFrequency(term)).Sum();
                _invertedDocumentFrequency[term] = ((int)termCount) == 0 ? 0 : Math.Log10(_documents.Count / termCount);
            }
        }

        public double IDF(string index)
        {
            return _invertedDocumentFrequency[index];
        }

        internal List<Centroid> calculateCentroids()
        {
            List<Centroid> toReturn = new List<Centroid>();
            toReturn.Add(calculateCentroid(AssignmentSE.Category.Politics));
            toReturn.Add(calculateCentroid(AssignmentSE.Category.Sports));
            toReturn.Add(calculateCentroid(AssignmentSE.Category.Economy));
            toReturn.Add(calculateCentroid(AssignmentSE.Category.Technology));
            return toReturn;
        }

        Centroid calculateCentroid(AssignmentSE.Category category)
        {
            List<string> listOfAllTerms = new List<string>();
            foreach (Document document in _documents)
            {
                listOfAllTerms.AddRange(document._terms);
            }
            return new Centroid(category, listOfAllTerms.ToArray());
        }
    }
}