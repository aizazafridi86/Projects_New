﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA_Visualization_Software.GA
{
    public class BestIndividual
    {

        public int bestIndividual_index;

        //method that returns best particle in the current population
        public int return_best_individual(List<Individual> ind, int pop_size, string optimization_type)
        {
            //Initially assuming that individual[0] has the best fitness
            this.bestIndividual_index = 0;

            //calculating fitness of individual[0] and storing it in bestIndividual_fitness variable
            double bestIndividual_fitness = ind[bestIndividual_index].CalculateFitness();

            //iterating through the individuals to find the index of the individual having the best fitness

            //for minimization problems
            if (optimization_type == "minimize")
            {
                for (int i = 1; i < pop_size; i++)
                {
                    if (ind[i].CalculateFitness() < bestIndividual_fitness)
                    {
                        bestIndividual_fitness = ind[i].CalculateFitness();
                        bestIndividual_index = i;
                    }
                }
            }
            //for maximization problems
            else if(optimization_type=="maximize")
            {
                for(int i=1; i<pop_size; i++)
                {
                    if (ind[i].CalculateFitness() > bestIndividual_fitness)
                    {
                        bestIndividual_fitness = ind[i].CalculateFitness();
                        bestIndividual_index = i;
                    }
                }
            }

            return bestIndividual_index;
        }

    }
}
