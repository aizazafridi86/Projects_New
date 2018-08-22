using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA_Visualization_Software.GA
{
    public class MatingSelection
    {
        //initialize list of offsprings that would store selected offsprings
        //List<Offspring> offspring;

        //Initialize Random class for random number generation
        //Random random = new Random();

        public bool is_positive = true;

        //Tournament selection method for minimization problems
        public List<Offspring> tournament_selection(List<Individual> ind,List<Offspring>offspring,int pop_size,int offspring_size, int tournament_size, int number_genes,string optimization_type,Random random)
        {
            int numberOfGenes=number_genes;
            List<int> selected_ind = new List<int>();
            List<double> fitness_selected_ind=new List<double>();
            List<double> uniqueRandomNumbers = new List<double>();
            int final_selected_individual;
            double min_fitness;
            int random_number;
   
            //select parents for mating and put them in offspring population that would be later modified by recombination and mutation
            for (int i = 0; i < offspring_size; i++)
            {
                //selecting individuals and calculating their fitness
                for(int j=0; j<tournament_size; j++)
                {
                    //selected_ind[j]=random.Next(0,pop_size);
                    random_number = random.Next(0, pop_size - 1);

                    //making sure that same individual is not selected twice
                    while (uniqueRandomNumbers.Contains(random_number))
                    {
                        random_number = random.Next(0, pop_size - 1);
                    }

                    //adding the value to unique random number list
                    uniqueRandomNumbers.Add(random_number);

                    //finally selecting the individual for tournament 
                    //selected_ind[j] = random_number;
                    selected_ind.Add(random_number);

                    //calculating fitness of selected individual and storing it in a list
                    //fitness_selected_ind[j]= ind[selected_ind[j]].CalculateFitness();
                    fitness_selected_ind.Add(ind[selected_ind[j]].CalculateFitness());
                }

                //selecting individual having the best fitness and copying it to the offspring list
                min_fitness=fitness_selected_ind[0];
                //final_selected_individual=0;
                final_selected_individual = selected_ind[0];

                //if opitmization type if minimize then selecting the individual with lower fitness
                if (optimization_type == "minimize")
                {
                    for (int j = 1; j < tournament_size; j++)
                    {
                        if (fitness_selected_ind[j] < min_fitness)
                        {
                            final_selected_individual = selected_ind[j];
                        }
                    }
                }
                //if optimization type is maximize then selecting the individual with higher fitness
                else if (optimization_type == "maximize")
                {
                    for (int j = 1; j < tournament_size; j++)
                    {
                        if (fitness_selected_ind[j] > min_fitness)
                        {
                            final_selected_individual = selected_ind[j];
                        }
                    }
                }

                //finally, copying selected individual to offspring list
                for(int j=0; j<numberOfGenes; j++)
                {
                    offspring[i].genes[j]=ind[final_selected_individual].genes[j];
                    //offspring[i].genes.Add(ind[final_selected_individual].genes[j]);
                }

                //clearing the list that contains the unique random numbers for the next round of tournament
                uniqueRandomNumbers.Clear();
                selected_ind.Clear();
                fitness_selected_ind.Clear();

            }
            //return list of offsprings
            return offspring;
        }

        /*
        //Tournament selection method for maximization problems
        public List<Offspring> tournament_selection_maximization(List<Individual> ind, List<Offspring> offspring, int pop_size, int offspring_size, int tournament_size, int number_genes,Random random)
        {
            int numberOfGenes = number_genes;
            List<int> selected_ind = new List<int>();
            List<double> fitness_selected_ind = new List<double>();
            List<double> uniqueRandomNumbers = new List<double>();
            int final_selected_individual;
            double max_fitness;
            int random_number;

            //select parents for mating and put them in offspring population that would be later modified by recombination and mutation
            for (int i = 0; i < offspring_size; i++)
            {
                //selecting individuals and calculating their fitness
                for (int j = 0; j < tournament_size; j++)
                {
                    random_number = random.Next(0, pop_size - 1);

                    //making sure that same individual is not selected twice
                    while (uniqueRandomNumbers.Contains(random_number))
                    {
                        random_number = random.Next(0, pop_size - 1);
                    }

                    //adding the value to unique random number list
                    uniqueRandomNumbers.Add(random_number);

                    //finally selecting the individual for tournament 
                    selected_ind[j] = random_number;

                    //calculating fitness of selected individual and storing it in a list
                    fitness_selected_ind[j] = ind[selected_ind[j]].CalculateFitness();
                }

                //selecting individual having the best fitness and copying it to the offspring list
                max_fitness = fitness_selected_ind[0];
                final_selected_individual = 0;

                for (int j = 1; j < tournament_size; j++)
                {
                    if (fitness_selected_ind[j] > max_fitness)
                    {
                        final_selected_individual = j;
                    }
                }

                //finally, copying selected individual to offspring list
                for (int j = 0; j < numberOfGenes; j++)
                {
                    offspring[i].genes[j] = ind[final_selected_individual].genes[j];
                }

                //clearing the list that contains the unique random numbers for the next round of tournament
                uniqueRandomNumbers.Clear();

            }
            //return list of offsprings
            return offspring;
        }*/
        

        //Fitness Proportionate Selection Method (Minimization Problem)
        public List<Offspring> fitness_proportionate(List<Individual> ind, List<Offspring> offspring, int pop_size, int offspring_size, int number_genes, string optimization_type, Random random)
        {
            List<double> fitness = new List<double>();
            List<double> prob = new List<double>();
            double fitness_sum = 0;
            //double number;
            //int selected_individual = 0;
            int constant = 10;

            //calculating fitness of each individual and storing it in a list
            for(int i=0; i<pop_size; i++)
            {
                //fitness[i] = ind[i].CalculateFitness();
                fitness.Add(ind[i].CalculateFitness());

                if(fitness[i]<0)
                {
                    this.is_positive = false;
                }
            }

            //making sure that no fitness value is negative as it wont work when we calculate the probabilities
            //adding a constant to all fitness values if any of the fitness value is negative
            while(this.is_positive == false)
            {
                fitness = this.add_constant(fitness, pop_size, constant);
                constant = constant + 10;
            }

            //for minimizatin problem
            if(optimization_type=="minimize")
            {
                for(int i=0; i<pop_size; i++)
                {
                    fitness[i] = 1 / fitness[i];
                }
            }

            //calculating fitness sum
            for (int i = 0; i < pop_size;i++)
            {
                fitness_sum = fitness_sum + fitness[i];
            }

            //calculating selection probability of each individual
            for (int i = 0; i < pop_size; i++)
            {
                //prob[i] = fitness[i] / fitness_sum;
                prob.Add(fitness[i] / fitness_sum);
            }

            //assigning portion of Roulette Wheel to each individual based on its probability
            //for individual 0
            //lower range of individual 0 starts from 0 and ends in the probability of individual
            ind[0].wheel_lowerRange = 0;
            ind[0].wheel_higherRange = prob[0];

            //for rest of individuals
            for (int i = 1; i < pop_size; i++)
            {
                ind[i].wheel_lowerRange = ind[i - 1].wheel_higherRange;
                ind[i].wheel_higherRange = ind[i - 1].wheel_higherRange + prob[i];
            }

            offspring = this.spin_wheel(ind, offspring, pop_size, offspring_size, number_genes, random);

            //clearing lists
            fitness.Clear();
            prob.Clear();

            //returing list of selected offspring
            return offspring;
        }

        //method for rank selection method for maximization problems
        public List<Offspring> rank_selection(List<Individual> ind, List<Offspring> offspring, int pop_size, int offspring_size, int number_genes, string optimization_type, Random random,double s)
        {
            List<double> fitness = new List<double>();
            List<double> sorted_fitness = new List<double>();
            Dictionary<int, double> prob = new Dictionary<int,double>();
            //List<double>sorted_fitness = new List<double>();
            Dictionary<int, int> ind_info = new Dictionary<int, int>();
            Dictionary<int,int> rank = new Dictionary<int,int>();
            int constant = 10;
            
            //variables that are used in probability calculation
            double probability;
            double factor1;
            double factor2;
            double factor3;
            double factor4;

            //calculating fitness of each individual and storing it in a list
            for (int i = 0; i < pop_size; i++)
            {
                fitness.Add(ind[i].CalculateFitness());

                if (fitness[i] < 0)
                {
                    this.is_positive = false;
                }
            }

            //making sure that no fitness value is negative as it wont work when we calculate the probabilities
            //adding a constant to all fitness values if any of the fitness value is negative
            while (this.is_positive == false)
            {
                fitness = this.add_constant(fitness, pop_size, constant);
                constant = constant + 10;
            }

            //for minimizatin problem
            if (optimization_type == "minimize")
            {
                for (int i = 0; i < pop_size; i++)
                {
                    fitness[i] = 1 / fitness[i];
                }
            }

            //copying fitness values to sorted fitness list and then sorting them
            for (int i=0; i<pop_size; i++)
            {
                sorted_fitness.Add(fitness[i]);
            }

            //sorting list of fitness for rank assignment
            sorted_fitness.Sort();

            //now sorted fitness contains fitness of individuals in ascending order
            //we need to assign individual number to fitness values
            //this for loop keeps track of which individual has what fitness 
            for(int i=0; i<pop_size; i++)
            {
                for (int j=0; j<pop_size; j++)
                {
                    if (sorted_fitness[i] == fitness[j])
                    {
                        //make sure that the given key is not in the dictionary
                        //this would deal with the problem if two individuals have identical fitness values
                        if(!ind_info.ContainsKey(i) && !ind_info.ContainsValue(j))
                        {
                            //ind_info(j->individual number in sorted fitness, i-> individual number in fitness)
                            //means ind in sorted fitness i is individual j
                            ind_info.Add(i, j);
                            break;
                        }
                        
                    }
                }
            }

            //ranking individuals based on their fitness
            //best individual would get rank pop_size-1 and worst individual would get rank 0
            //calculating probability of each individual based on its rank
            for (int i=pop_size-1; i>=0 ; i--)
            {
                //Assigning rank
                rank.Add(ind_info[i], i);

                //calculating probability
                factor1 = (2-s)/pop_size;
                factor2 = 2*rank[ind_info[i]]*(s-1);
                factor3 = pop_size*(pop_size-1);
                factor4 = factor2/factor3;
                probability = factor1 + factor4;

                //adding probability
                prob.Add(ind_info[i],probability);
            }

            //assigning portion of Roulette Wheel to each individual based on its probability
            //for individual 0
            //lower range of individual 0 starts from 0 and ends in the probability of individual
            ind[0].wheel_lowerRange = 0;
            ind[0].wheel_higherRange = prob[0];

            //for rest of individuals
            for (int i = 1; i < pop_size; i++)
            {
                ind[i].wheel_lowerRange = ind[i - 1].wheel_higherRange;
                ind[i].wheel_higherRange = ind[i - 1].wheel_higherRange + prob[i];
            }

            //calling function to rotate the wheel and select offsrpings
            offspring = spin_wheel(ind, offspring, pop_size, offspring_size, number_genes, random);

            //clearing dictionaries and lists
            fitness.Clear();
            sorted_fitness.Clear();
            prob.Clear();
            ind_info.Clear();
            rank.Clear();
             
            //returning offsprings
            return offspring;
        }

        //method that spins the wheel and select individuals
        public List<Offspring> spin_wheel(List<Individual> ind, List<Offspring> offspring, int pop_size, int offspring_size, int number_genes, Random random)
        {
            double number;
            int selected_individual = 0;

            for (int i = 0; i < offspring_size; i++)
            {
                //generating random number between 0 and 1
                number = random.NextDouble();

                //finding out the number lies within which individual portion of the wheel
                //selecting that individual 
                for (int j = 0; j < pop_size; j++)
                {
                    if (number > ind[j].wheel_lowerRange && number <= ind[j].wheel_higherRange)
                    {
                        selected_individual = j;
                        break;
                    }
                }

                //copying selected individual to offspring list
                for (int j = 0; j < number_genes; j++)
                {
                    offspring[i].genes[j] = ind[selected_individual].genes[j];
                }

            }

            //returing list of selected offspring
            return offspring;
        }

        //method that adds constant positive value to fitness list
        public List<double> add_constant(List<double> fitness, int pop_size, int constant)
        {
            int count = 0;
            //adding a constant value to fitness to make it positive
            for(int i=0; i<pop_size; i++)
            {
                fitness[i] = fitness[i] + constant;

                if(fitness[i] <0)
                {
                    count++;
                }
            }

            //if all the fitness values are positive, then count would be 0
            //if count is zero then we need to make is_pisitive true
            if(count ==0)
            {
                this.is_positive = true;
            }

         //return new fitness fitness values   
         return fitness;

        }


    }
}
