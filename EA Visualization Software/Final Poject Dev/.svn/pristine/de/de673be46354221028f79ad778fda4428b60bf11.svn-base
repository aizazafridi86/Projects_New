using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA_Visualization_Software.PSO_Algorithm
{
    public class CalculateAverageFitness
    {

        public double Return_AverageFitness(List<Particle> particle, int numberOfParticles)
        {
            double averageFitness = 0;
            double total = 0;

            //adding all particles fitnesses
            for(int i=0; i<numberOfParticles; i++)
            {
                total = total + particle[i].CalculateFitness();
            }

            //dividing by the total number of particles to get the average fitness
            averageFitness = total / numberOfParticles;

            return averageFitness;
        }
    }
}
