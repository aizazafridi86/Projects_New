using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA_Visualization_Software.Common;

namespace EA_Visualization_Software.GA
{
    public class GAController
    {
        //initializing global variables
        public string input_function;
        public double lower_range;
        public double higher_range;
        public int pop_size;
        public string selection_method;
        public int tournament_size;
        public string recombination_type;
        public int crossover_points;
        public double crossover_rate;
        public string mutation_type;
        public double mutation_rate;
        public int number_generations;
        public int number_runs;
        string survivor_selection_method;
        int q;
        int offspring_size;
        string optimization_type;
        double a;
        double s;

        double average;
        double mean_dispersion;
        int run;
        int best_index;

        public List<string> uniqueGenesFound = new List<string>();
        public int number_genes;

        //creating instance of output class
        List<Output> output = new List<Output>();

        //creating instance of dispersion class
        Dispersion disp = new Dispersion();

        //creating an instance of class that calculates average fitness
        CalculateAverageFitness average_fitness = new CalculateAverageFitness();

        //Creating an instance of Random number generator
        Random random = new Random();

        //creating instance of mating selection class
        MatingSelection mating = new MatingSelection();

        //creating instance of Recombination class
        Recombination recomb = new Recombination();

        //creating an instance of class mutation
        Mutation mutation = new Mutation();

        //creating an instance of class survivor selection
        SurvivorSelection survivor = new SurvivorSelection();

        //creating an instance of class that calculates best particle
        BestIndividual best_individual = new BestIndividual();

        //creating instance of method that would be used to generate random numbers according to normal distribution
        MathNet.Numerics.Distributions.Normal normalDist = new MathNet.Numerics.Distributions.Normal();
        MathNet.Numerics.Distributions.Cauchy cauchyDist = new MathNet.Numerics.Distributions.Cauchy();

        //setting all global variables values
        public void Initialize(string input_f,double l_range,double h_range,int pop_size,string sel_meth,int tour_size,string recom_type,int cross_p,double cross_rate,string mutation_type, double mutation_rate,List<string> uniqueGenesFound,int number_genes,int gen, int runs,string survivor_selection,int off_size, double a, double s, string opt_type)
        {
            this.input_function = input_f;
            this.lower_range = l_range;
            this.higher_range = h_range;
            this.pop_size = pop_size;
            this.offspring_size = off_size;
            this.selection_method = sel_meth;
            this.tournament_size = tour_size;
            this.recombination_type = recom_type;
            this.crossover_points = cross_p;
            this.crossover_rate = cross_rate;
            this.mutation_type = mutation_type;
            this.mutation_rate = mutation_rate;
            this.uniqueGenesFound = uniqueGenesFound;
            this.number_genes = number_genes;
            this.number_generations = gen;
            this.number_runs = runs;
            this.survivor_selection_method = survivor_selection;
            this.optimization_type = opt_type;
            this.a = a;
            this.s = s;
            
        }

        //method that runs GA Algorithm
        public List<Output> RunGA()
        {
            //clearing output list
            this.output.Clear();

            //initializing list of individuals
            List<Individual> ind = new List<Individual>();

            //initializing list of offsprings
            List<Offspring> offspring = new List<Offspring>();

            //variable that stores best individual fitness value
            double best_fitness_in_generation;

            //setting run to 0
            this.run = 0;

            while (this.run < this.number_runs)
            {

                //clear lists of individuals and offsrpings
                ind.Clear();
                offspring.Clear();

                //initializing values in dispersion class
                this.disp.InitializeDispersion(this.pop_size, this.uniqueGenesFound);

                //add an instance of output class to list in each run
                this.output.Add(new Output());

                //initializing list of individuals
                for (int i = 0; i < this.pop_size; i++)
                {
                    //initialize individuals here
                    ind.Add(new Individual(this.uniqueGenesFound, this.number_genes, this.higher_range, this.lower_range, this.input_function, this.random));
                }

                //initializing list of offspring
                for (int i = 0; i < this.offspring_size; i++)
                {
                    offspring.Add(new Offspring(this.uniqueGenesFound, this.input_function, this.number_genes));
                }

                //running GA Algorithnm

                //calculations for time step1
                
                //calculating average fitness of population
                this.average = average_fitness.return_averageFitness(ind, this.pop_size);

                //calculating population mean dispersion
                this.mean_dispersion = disp.calculateDispersion(ind);

                //calculating population best fitness here
                this.best_index = best_individual.return_best_individual(ind, this.pop_size, this.optimization_type);
                best_fitness_in_generation = ind[this.best_index].CalculateFitness();

                //adding generation1 values to output class
                this.output[this.run].Add_Values(1, this.average, best_fitness_in_generation, this.mean_dispersion);

                //for rest of generations
                for (int i = 2; i <= this.number_generations; i++)
                {
                    //performing mating selection
                    switch(this.selection_method)
                    {
                        case "Tournament":
                            offspring = this.mating.tournament_selection(ind, offspring, this.pop_size, this.offspring_size, this.tournament_size, this.number_genes, this.optimization_type, this.random);
                            break;

                        case "Fitness_Proportionate":
                            offspring = this.mating.fitness_proportionate(ind, offspring, this.pop_size, this.offspring_size, this.number_genes, this.optimization_type, random);
                            break;

                        case "Rank":
                            offspring = this.mating.rank_selection(ind, offspring, this.pop_size, this.offspring_size, this.number_genes, this.optimization_type, this.random, this.s);
                            break;
                    }

                    //performing recombination
                    switch(this.recombination_type)
                    {
                        case "One_point_crossover":
                            offspring = this.recomb.onepoint_crossover(offspring, this.offspring_size, this.crossover_rate, this.number_genes, this.random);
                            break;

                        case "Arithmentic_single_crossover":
                            offspring = this.recomb.arithmetic_single_recombination(offspring, this.offspring_size, this.crossover_rate, this.a, this.number_genes, this.random);
                            break;

                        case "Arithmetic_simple_crossover":
                            offspring = this.recomb.arithmetic_simple_recombination(offspring, this.offspring_size, this.crossover_rate, this.a, this.number_genes, this.random);
                            break;

                        case "Arithmetic_whole_crossover":
                            offspring = this.recomb.arithmetic_whole_recombination(offspring, this.offspring_size, this.crossover_rate, this.a, this.number_genes, this.random);
                            break;
                    }

                    //performing mutation
                    switch(this.mutation_type)
                    {
                        case "Normal_Distribution":
                            offspring = mutation.gaussian_mutaion(offspring, this.offspring_size, this.mutation_rate, this.number_genes, this.random, this.normalDist,this.lower_range, this.higher_range);
                            break;

                        case "Cauchy_Distribution":
                            offspring = mutation.cauchy_mutation(offspring, this.offspring_size, this.mutation_rate, this.number_genes, this.random, this.cauchyDist, this.lower_range, this.higher_range);
                            break;
                    }

                    //performing survivor selection
                    switch(this.survivor_selection_method)
                    {
                        case "Truncation":
                            ind = survivor.truncation_selection(ind, offspring, this.pop_size, this.offspring_size, this.number_genes, this.optimization_type, this.random);
                            break;
                    }

                    //calculating average fitness of population
                    this.average = average_fitness.return_averageFitness(ind, this.pop_size);

                    //calculating population mean dispersion
                    this.mean_dispersion = disp.calculateDispersion(ind);

                    //calculating population best fitness here
                    this.best_index = best_individual.return_best_individual(ind, this.pop_size, this.optimization_type);
                    best_fitness_in_generation = ind[this.best_index].CalculateFitness();

                    //adding generation1 values to output class
                    this.output[this.run].Add_Values(i, this.average, best_fitness_in_generation, this.mean_dispersion);

                }

                this.run = this.run + 1;
            }

            //returning instance of output class
            return this.output;               

        }


    }
}
