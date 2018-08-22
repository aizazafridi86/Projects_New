using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA_Visualization_Software.GA
{
    public class CalculateAverageFitness
    {

        public double return_averageFitness(List<Individual> ind, int pop_size)
        {
            double average_fitness = 0;
            double total = 0;

            //adding all individual fitnesses
            for (int i = 0; i < pop_size; i++)
            {
                total = total + ind[i].CalculateFitness();
            }

            //dividing by the total number of individuals to calcualte the average fitness
            average_fitness = total / pop_size;

            return average_fitness;
        }
    }
}
