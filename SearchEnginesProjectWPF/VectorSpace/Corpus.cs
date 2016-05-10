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
        public void addDocument(Document document, bool calculateIDF)
        {
            foreach (Document d in _documents)
            {
                if (d.Url.Equals(document.Url, StringComparison.InvariantCultureIgnoreCase)) ;
                {
                    return;
                }
            }
            _documents.Add(document);
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


    }
}