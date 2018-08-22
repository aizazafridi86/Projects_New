using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using EA_Visualization_Software.Common;

namespace EA_Visualization_Software.PSO_Algorithm
{
    public class PSController
    {

        //Initializing Variables
        string inputFunction;
        double lowerRange;
        double higherRange;
        string updateRule;
        int posClampVelocity;
        int negClampVelocity;
        int numberOfParticles;
        int numberOfTimeSteps;
        int numberOfRuns;
        string optimizationType;

        int best_index;
        double average;
        double meanDispersion;

        int run;

        List<string> uniqueGenesFound = new List<string>();

        //creating instance of CalculateGBest class
        CalculateGBest gbest = new CalculateGBest();

        //creating isntance of Inertia class
        Inertia inertia = new Inertia();

        //creating instance of class CalculateAverageFitness
        CalculateAverageFitness averageFitness = new CalculateAverageFitness();

        //Creatung a new list of type Output
        List<Output> output = new List<Output>();

        //creating instance of CountGenes class
        CountGenes countGenes = new CountGenes();

        Dispersion disp = new Dispersion();

        //This method set values of all variables that are passed by the GUI
        public void Initialize(string inputFunction,double lowerRange,double higherRange,string updateRule,int posClampVelocity,int negClampVelocity,int numberOfParticles,int numberOfTimeSteps,int numberOfRuns, string optimizationType)
        {
            this.inputFunction = inputFunction;
            this.lowerRange = lowerRange;
            this.higherRange = higherRange;
            this.updateRule = updateRule;
            this.posClampVelocity = posClampVelocity;
            this.negClampVelocity = negClampVelocity;
            this.numberOfParticles = numberOfParticles;
            this.numberOfTimeSteps = numberOfTimeSteps;
            this.numberOfRuns = numberOfRuns;
            this.optimizationType = optimizationType;
        }

        //Running PS Algorithm
        public List<Output> RunPSAlgorithm()
        {    
                //clearing output
                this.output.Clear();

                //Initializing list of particles
                List<Particle> particle = new List<Particle>();    

                //counting number of genes in input function
                this.uniqueGenesFound = this.countGenes.Return_NumberOfGenes(this.inputFunction);
                
                //setting run to 0
                this.run = 0;

                while (this.run < this.numberOfRuns)
                {

                    //clearing particle list in each run
                    particle.Clear();

                    //initialiazing dispersion class
                    this.disp.InitializeDispersion(this.numberOfParticles, this.uniqueGenesFound);

                    //creating an instance of output class in each run
                    this.output.Add(new Output());

                    //creating particles and initializing their variables
                    for (int i = 0; i < this.numberOfParticles; i++)
                    {
                        particle.Add(new Particle(this.inputFunction, this.lowerRange, this.higherRange, this.posClampVelocity, this.negClampVelocity, this.numberOfTimeSteps, this.uniqueGenesFound));
                    }

                    //calling method for linear decreasing velocity update rule minimization problem
                    if (this.optimizationType == "Minimize" && this.updateRule == "Linear Decreasing")
                    {
                        this.Decreasing_Minimize(particle);
                    }

                    //calling method for linear increasing velocity update rule minimization problem
                    if (this.optimizationType == "Minimize" && this.updateRule == "Linear Increasing")
                    {
                        this.Increasing_Minimize(particle);
                    }

                    //calling method for linear decreasing velocity update rule maximization problem
                    if (this.optimizationType == "Maximize" && this.updateRule == "Linear Decreasing")
                    {
                        this.Decreasing_Maximize(particle);
                    }

                    //calling method for linear increasing velocity update rule maximization problem
                    if (this.optimizationType == "Maximize" && this.updateRule == "Linear Increasing")
                    {
                        this.Increase_Maximize(particle);
                    }

                    this.run = this.run + 1;
                }

                //returning instance of output class to the GUI
                return this.output;
            
        }

        public void Decreasing_Minimize(List<Particle> particle)
        {
            
            //Initializing linear decreasing inertia 
            this.inertia.Initialize_LinearDecreasingInertia();

            //for timestep=1
            //Finding index of best particle
            this.best_index = this.gbest.Return_gbest_Minimization(particle, this.numberOfParticles);

            //calculating average fitness
            this.average = this.averageFitness.Return_AverageFitness(particle, this.numberOfParticles); 

            //calculating population mean dispersion
            this.meanDispersion = disp.calculateDispersion(particle);

            //adding best particle fitness, average fitness of population and mean population dispersion to output class 
            this.output[this.run].Add_Values(1, this.average, particle[this.best_index].ReturnFitness(), this.meanDispersion);

            //for rest of timesteps
            for (int i = 2; i <= this.numberOfTimeSteps; i++)
            {
                //updating velocity and position
                for (int j = 0; j < this.numberOfParticles; j++)
                {
                    //every particle calculating its own personal best
                    particle[j].Calculate_Pbest_Minimize();

                    //updating velocity
                    particle[j].Update_LinearDecreasingVelocity(particle[this.best_index], this.inertia.wt);

                    //updating position
                    particle[j].Update_Position();
                }

                //updating inertia
                this.inertia.Update_LinearDecreasingInertia(this.numberOfTimeSteps, i);

                //calculating best particle
                this.best_index = this.gbest.Return_gbest_Minimization(particle, this.numberOfParticles);

                //calculating average fitness
                this.average = this.averageFitness.Return_AverageFitness(particle, this.numberOfParticles);

                //calculating population mean dispersion
                this.meanDispersion = disp.calculateDispersion(particle);

                //adding best particle fitness, average fitness of population and mean population dispersion to output class 
                this.output[this.run].Add_Values(i, this.average, particle[this.best_index].ReturnFitness(), this.meanDispersion);          

            }

        }

        public void Increasing_Minimize(List<Particle> particle)
        {

            //Initializing linear decreasing inertia 
            this.inertia.Initialize_LinearIncreasingInertia();

            //for timestep=1
            //Finding index of best particle
            this.best_index = this.gbest.Return_gbest_Minimization(particle, this.numberOfParticles);

            //calculating average fitness
            this.average = this.averageFitness.Return_AverageFitness(particle, this.numberOfParticles);

            //calculating population mean dispersion
            this.meanDispersion = disp.calculateDispersion(particle);

            //adding best particle fitness, average fitness of population and mean population dispersion to output class 
            this.output[this.run].Add_Values(1, this.average, particle[this.best_index].ReturnFitness(), this.meanDispersion);

            //for rest of timesteps
            for (int i = 2; i <= this.numberOfTimeSteps; i++)
            {
                //updating velocity and position
                for (int j = 0; j < this.numberOfParticles; j++)
                {
                    //every particle calculating its own personal best
                    particle[j].Calculate_Pbest_Minimize();

                    //updating velocity
                    particle[j].Update_LinearIncreasingVelocity(particle[this.best_index], this.inertia.wt);

                    //updating position
                    particle[j].Update_Position();
                }

                //updating inertia
                this.inertia.Update_LinearIncreasingInertia(this.numberOfTimeSteps, i);

                //calculating best particle
                this.best_index = this.gbest.Return_gbest_Minimization(particle, this.numberOfParticles);

                //calculating average fitness
                this.average = this.averageFitness.Return_AverageFitness(particle, this.numberOfParticles);

                //calculating population mean dispersion
                this.meanDispersion = disp.calculateDispersion(particle);

                //adding best particle fitness, average fitness of population and mean population dispersion to output class 
                this.output[this.run].Add_Values(i, this.average, particle[this.best_index].ReturnFitness(), this.meanDispersion);

            }

        }

        public void Decreasing_Maximize(List<Particle> particle)
        {
            //Initializing linear decreasing inertia 
            this.inertia.Initialize_LinearDecreasingInertia();

            //for timestep=1
            //Finding index of best particle
            this.best_index = this.gbest.Return_gbest_Maximization(particle, this.numberOfParticles);

            //calculating average fitness
            this.average = this.averageFitness.Return_AverageFitness(particle, this.numberOfParticles);

            //calculating population mean dispersion
            this.meanDispersion = disp.calculateDispersion(particle);

            //adding best particle fitness, average fitness of population and mean population dispersion to output class 
            this.output[this.run].Add_Values(1, this.average, particle[this.best_index].ReturnFitness(), this.meanDispersion);

            //for rest of timesteps
            for (int i = 2; i <= this.numberOfTimeSteps; i++)
            {
                //updating velocity and position
                for (int j = 0; j < this.numberOfParticles; j++)
                {
                    //every particle calculating its own personal best
                    particle[j].Calculate_Pbest_Maximize();

                    //updating velocity
                    particle[j].Update_LinearDecreasingVelocity(particle[this.best_index], this.inertia.wt);

                    //updating position
                    particle[j].Update_Position();
                }

                //updating inertia
                this.inertia.Update_LinearDecreasingInertia(this.numberOfTimeSteps, i);

                //calculating best particle
                this.best_index = this.gbest.Return_gbest_Maximization(particle, this.numberOfParticles);

                //calculating average fitness
                this.average = this.averageFitness.Return_AverageFitness(particle, this.numberOfParticles);

                //calculating population mean dispersion
                this.meanDispersion = disp.calculateDispersion(particle);

                //adding best particle fitness, average fitness of population and mean population dispersion to output class 
                this.output[this.run].Add_Values(i, this.average, particle[this.best_index].ReturnFitness(), this.meanDispersion);

            }
        }

        public void Increase_Maximize(List<Particle> particle)
        {
            //Initializing linear decreasing inertia 
            this.inertia.Initialize_LinearIncreasingInertia();

            //for timestep=1
            //Finding index of best particle
            this.best_index = this.gbest.Return_gbest_Maximization(particle, this.numberOfParticles);

            //calculating average fitness
            this.average = this.averageFitness.Return_AverageFitness(particle, this.numberOfParticles);

            //calculating population mean dispersion
            this.meanDispersion = disp.calculateDispersion(particle);

            //adding best particle fitness, average fitness of population and mean population dispersion to output class 
            this.output[this.run].Add_Values(1, this.average, particle[this.best_index].ReturnFitness(), this.meanDispersion);

            //for rest of timesteps
            for (int i = 2; i <= this.numberOfTimeSteps; i++)
            {
                //updating velocity and position
                for (int j = 0; j < this.numberOfParticles; j++)
                {
                    //every particle calculating its own personal best
                    particle[j].Calculate_Pbest_Maximize();

                    //updating velocity
                    particle[j].Update_LinearIncreasingVelocity(particle[this.best_index], this.inertia.wt);

                    //updating position
                    particle[j].Update_Position();
                }

                //updating inertia
                this.inertia.Update_LinearIncreasingInertia(this.numberOfTimeSteps, i);

                //calculating best particle
                this.best_index = this.gbest.Return_gbest_Maximization(particle, this.numberOfParticles);

                //calculating average fitness
                this.average = this.averageFitness.Return_AverageFitness(particle, this.numberOfParticles);

                //calculating population mean dispersion
                this.meanDispersion = disp.calculateDispersion(particle);

                //adding best particle fitness, average fitness of population and mean population dispersion to output class 
                this.output[this.run].Add_Values(i, this.average, particle[this.best_index].ReturnFitness(), this.meanDispersion);

            }
        }
    }
}
