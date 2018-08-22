﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA_Visualization_Software.GA
{
    public class Mutation
    {
        public double generated_number;
        public double gene_value;
       

        //method for performing gaussian distribution mutation
        public List<Offspring> gaussian_mutaion(List<Offspring> offspring, int offspring_size, double mutation_rate, int number_genes, Random random, MathNet.Numerics.Distributions.Normal normalDist, double lower_range, double higher_range)
        {
            for(int i=0; i<offspring_size; i++)
            {
                for (int j=0; j<number_genes; j++)
                {
                    this.generated_number = random.NextDouble();

                    //perform mutation if generated number is less than or equal to mutation rate
                    if (this.generated_number <= mutation_rate)
                    {
                        //calculate new gene value
                        this.gene_value = offspring[i].genes[j] + normalDist.Sample();

                        //update gene value only if it is withing the function upper and lower bounds
                        if(this.gene_value<=higher_range && this.gene_value>=lower_range)
                        {
                            offspring[i].genes[j] = this.gene_value;
                        }
                        //offspring[i].genes[j] = offspring[i].genes[j] + normalDist.Sample();
                    }
                }
            }

            return offspring;
        }//end of method

        //method for performing Cauchy distribution mutation
        public List<Offspring> cauchy_mutation(List<Offspring> offspring, int offspring_size, double mutation_rate, int number_genes, Random random, MathNet.Numerics.Distributions.Cauchy cauchyDist, double lower_range, double higher_range)
        {
            for (int i = 0; i < offspring_size; i++)
            {
                for (int j = 0; j < number_genes; j++)
                {
                    this.generated_number = random.NextDouble();

                    //perform mutation if generated number is less than or equal to mutation rate
                    if (this.generated_number <= mutation_rate)
                    {
                        //calculate new gene value
                        this.gene_value = offspring[i].genes[j] + cauchyDist.Sample();

                        //update gene value only if it is withing the function upper and lower bounds
                        if (this.gene_value <= higher_range && this.gene_value >= lower_range)
                        {
                            offspring[i].genes[j] = this.gene_value;
                        }
                        //offspring[i].genes[j] = offspring[i].genes[j] + normalDist.Sample();
                    }
                }
            }

            return offspring;
        }//end of method

    }
}
