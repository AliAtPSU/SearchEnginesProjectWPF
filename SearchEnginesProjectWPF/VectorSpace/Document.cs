using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace VectorSpaceModel.Components
{
    public class Document : IEnumerable<string>
    {
        protected readonly IDictionary<string, double> _augmentedFrequency = new Dictionary<string, double>();
        protected readonly IDictionary<string, bool> _booleanFrequency = new Dictionary<string, bool>();
        protected readonly IDictionary<string, double> _logaritmicFrequency = new Dictionary<string, double>();
        public IDictionary<string, int> _regularFrequency = new Dictionary<string, int>();
        public IList<string> _terms;
        private double? _maxFrequency;

        private int _size;
        private string _text;
        private string _url;
        private int _viewstateSize;

        public int Size
        {
            get { return _size; }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _size = value.Length;
            }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public int ViewstateSize
        {
            get { return _viewstateSize; }
            set { _viewstateSize = value; }
        }



        public IEnumerator<string> GetEnumerator()
        {
            return _terms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public double BooleanSimilarity(Document query, IList<Document> corpus)
        {
            List<string> vocabulary = corpus.SelectMany(term => term).Distinct().ToList();

            double[] queryWeights = vocabulary.Select(query.BooleanTermFrequency).ToArray();
            double[] documentWeights = vocabulary.Select(BooleanTermFrequency).ToArray();

            var vectorSpaceModel = new VectorSpaceModel(queryWeights, documentWeights);

            return vectorSpaceModel.CalculateCosineSimilarity();
        }

        public double AugumentedSimilarity(Document query, IList<Document> corpus)
        {
            List<string> vocabulary = corpus.SelectMany(term => term).Distinct().ToList();

            double[] queryWeights = vocabulary.Select(query.AugmentedTermFrequency).ToArray();
            double[] documentWeights = vocabulary.Select(AugmentedTermFrequency).ToArray();

            var vectorSpaceModel = new VectorSpaceModel(queryWeights, documentWeights);

            return vectorSpaceModel.CalculateCosineSimilarity();
        }

        public double TFIDFSimilarity(Document query, Corpus corpus)
        {
            double[] queryWeights = corpus.Select(term => query.TFIDF(term, corpus)).ToArray();
            double[] documentWeights = corpus.Select(term => TFIDF(term, corpus)).ToArray();

            var vectorSpaceModel = new VectorSpaceModel(queryWeights, documentWeights);

            return vectorSpaceModel.CalculateCosineSimilarity();
        }

        public double BooleanTFIDFSimilarity(Document query, Corpus corpus)
        {
            double[] queryWeights =
                corpus.Select(term => query.TFIDF(term, corpus, query.BooleanTermFrequency)).ToArray();
            double[] documentWeights = corpus.Select(term => TFIDF(term, corpus, BooleanTermFrequency)).ToArray();

            var vectorSpaceModel = new VectorSpaceModel(queryWeights, documentWeights);

            return vectorSpaceModel.CalculateCosineSimilarity();
        }


        private void CalculateMaxFrequency()
        {
            foreach (KeyValuePair<string, int> pair in _regularFrequency)
            {
                if (pair.Value > _maxFrequency)
                {
                    _maxFrequency = pair.Value;
                }
            }
        }

        public int RegularTermFrequency(string term)
        {
            if (!_regularFrequency.ContainsKey(term))
            {
                _regularFrequency[term] = _terms.Where(dt => dt.Equals(term)).Select(dt => 1).Sum();
            }

            return _regularFrequency[term];
        }

        public double LogaritmicTermFrequency(string term)
        {
            if (!_logaritmicFrequency.ContainsKey(term))
            {
                _logaritmicFrequency[term] = Math.Log10(RegularTermFrequency(term) + 1d);
            }
            return _logaritmicFrequency[term];
        }

        public double AugmentedTermFrequency(string term)
        {
            if (_maxFrequency == null)
            {
                CalculateMaxFrequency();
            }
            Debug.Assert(_maxFrequency != null, "_maxFrequency != null");

            if (!_augmentedFrequency.ContainsKey(term))
            {
                _augmentedFrequency[term] = 0.5d + (0.5d * RegularTermFrequency(term)) / (double)_maxFrequency;
            }

            return _augmentedFrequency[term];
        }

        public double TFIDF(string term, Corpus corpus, Func<string, double> termFrequencyFunction)
        {
            return termFrequencyFunction(term) * corpus.IDF(term);
        }

        public double TFIDF(string term, Corpus corpus)
        {
            return TFIDF(term, corpus, AugmentedTermFrequency);
        }


        public double BooleanTermFrequency(string term)
        {
            if (!_booleanFrequency.ContainsKey(term))
            {
                _booleanFrequency[term] = _terms.Contains(term);
            }
            return _booleanFrequency[term] ? 1d : 0d;
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


        public void CalculateViewstateSize()
        {
            int startingIndex = Text.IndexOf("id=\"__VIEWSTATE\"");
            if (startingIndex > -1)
            {
                int indexOfViewstateValueNode = Text.IndexOf("value=\"", startingIndex);
                int indexOfClosingQuotationMark = Text.IndexOf("\"", indexOfViewstateValueNode + 7);
                string viewstateValue = Text.Substring(indexOfViewstateValueNode + 7, indexOfClosingQuotationMark - (indexOfViewstateValueNode + 7));

                ViewstateSize = viewstateValue.Length;
            }
        }
    }
}
