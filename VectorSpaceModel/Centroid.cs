using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorSpaceModel.Components
{
    public class Centroid:Vector
    {
        public Centroid(AssignmentSE.Category category,params string[] terms)
        {
            this.category = category;
            foreach(string term in terms)
            {
                if(_regularFrequency.Select(t => t.Key).Contains(term, StringComparer.InvariantCultureIgnoreCase)){
                    _regularFrequency[term]++;
                }else
                {
                    _regularFrequency.Add(new KeyValuePair<string, int>(term, 0));
                }
            }
        }
    }
}
