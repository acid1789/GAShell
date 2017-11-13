using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAShell
{
    public class Population
    {
        public delegate IChromosome ChromosomeGenerator(Random r);

        List<IChromosome> _chromosomes;
        ChromosomeGenerator _generator;
        Random _rand;

        int _populationSize;
        float _mutatePercent;
        float _crossBreedPercent;


        public Population(int populationSize, float mutationPercentage, float crossBreedPercentage, ChromosomeGenerator generator, Random rand = null)
        {
            _populationSize = populationSize;
            _mutatePercent = mutationPercentage;
            _crossBreedPercent = crossBreedPercentage;

            _generator = generator;
            _rand = rand == null ? new Random() : rand;
            _chromosomes = new List<IChromosome>(populationSize);

            for (int i = 0; i < populationSize; i++)
            {
                IChromosome chromosome = _generator(_rand);
                _chromosomes.Add(chromosome);
            }
            _chromosomes.Sort();
        }

        public void Breed()
        {
            // Crossbreed to eliminate the worst
            int numToCrossbreed = (int)(_chromosomes.Count() * _crossBreedPercent);
            numToCrossbreed &= 0x7FFFFFFE; // Only even numbers

            // Randomly add the number to crossbreed into mom & dad lists
            List<IChromosome> moms = new List<IChromosome>();
            List<IChromosome> dads = new List<IChromosome>();
            for (int i = 0; i < numToCrossbreed; i++ )
            {
                if (_rand.Next(1) == 0)
                    moms.Add(_chromosomes[i]);
                else
                    dads.Add(_chromosomes[i]);
            }

            // Make sure mom and dad lists are balanced
            while (moms.Count != dads.Count)
            {
                List<IChromosome> largest = moms.Count > dads.Count ? moms : dads;
                List<IChromosome> smallest = moms.Count > dads.Count ? dads : moms;
                int r = _rand.Next(largest.Count);
                IChromosome rc = largest[r];
                largest.RemoveAt(r);
                smallest.Add(rc);
            }

            for (int i = 0; i < moms.Count; i++)
            {
                IChromosome child = moms[i].CrossBreed(dads[i], _rand);
                _chromosomes.Add(child);
            }

            // Sort by fitness
            _chromosomes.Sort();
            
            // Trim down to population level
            while (_chromosomes.Count > _populationSize)
                _chromosomes.RemoveAt(_chromosomes.Count - 1);

            // Mutate the top of the list
            int numToMutate = (int)(_chromosomes.Count() * _mutatePercent);
            for (int i = _chromosomes.Count - 1; i >= _chromosomes.Count - numToMutate; i--)
            {
                _chromosomes[i].Mutate(_rand);
            }

            _chromosomes.Sort();
        }

        public float BestFitness { get { return _chromosomes[0].GetFitness(); } }
        public IChromosome[] CurrentPopulation { get { return _chromosomes.ToArray(); } }
    }
}
