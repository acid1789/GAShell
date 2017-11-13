using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAShell
{
    public abstract class IChromosome : IComparable
    {
        public abstract float GetFitness();
        public abstract void Mutate(Random rand);
        public abstract IChromosome CrossBreed(IChromosome father, Random rand);

        public int CompareTo(object obj)
        {
            IChromosome other = (IChromosome)obj;
            float diff = other.GetFitness() - GetFitness();
            int ret = diff < 0 ? -1 : diff == 0 ? 0 : 1;
            return ret;
        }
    }
}
