using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA_Visualization_Software.Common;

namespace EA_Visualization_Software.PSO_Algorithm
{
    public class RosenbrockParticle
    {
        public int lowerRange;
        public int higherRange;
        public int posClampVelocity;
        public int negClampVelocity;
        public int numberOfTimeSteps;

        public List<double> genes = new List<double>();
        public int numberOfGenes;
        public List<double> velocity = new List<double>();
        public List<double> pbest = new List<double>();
        public double bestFitness;

        public RosenbrockParticle(int lowerRange,int higherRange,int posClampVelocity,int negClampVelocity,int numberOfTimeSteps)
        {
            this.lowerRange = lowerRange;
            this.higherRange = higherRange;
            this.posClampVelocity = posClampVelocity;
            this.negClampVelocity = negClampVelocity;
            this.numberOfTimeSteps = numberOfTimeSteps;

            RandomNumGenerator rand = new RandomNumGenerator();
            //initializing genes and velocity values
            for(int i=0; i<10 ; i++)
            {
                double number = rand.Generate_Double(this.lowerRange, this.higherRange);
                this.genes.Add(number);
                this.velocity.Add(0);
            }

            this.numberOfGenes = this.genes.Count;

            //initializing pbest list with the initial genes values
            for (int i = 0; i < this.numberOfGenes; i++)
            {
                this.pbest.Add(this.genes[i]);
            }

            //initializing bestFitness
            this.bestFitness = this.CalculateFitness();
        }

        public double CalculateFitness()
        {
            double factor1;
            double factor2;
            double finalFactor;
            double fitness = 0;

            for(int i=0; i<this.numberOfGenes-1; i++)
            {
                factor1 = (1 - genes[i]);
                factor1 = Math.Pow(factor1, 2);

                factor2 = genes[i + 1] - Math.Pow(genes[i], 2);
                factor2 = Math.Pow(factor2, 2);
                factor2 = 100 * factor2;

                finalFactor = factor1 + factor2;
                
                //finally, calculating fitness
                fitness = fitness + finalFactor;
            }

            return fitness;
        }

        //method that returns fitness in case of Rosenbrock function
        public double ReturnFitness()
        {
            return this.CalculateFitness();
        }

        //method that calculates pbest in case of Rosenbrock function
        public void Calculate_Pbest()
        {
            //calcualting fitness 
            double fitnessInTimeStep = this.CalculateFitness();

            //copying genes values to pbest list if new fitness is less than the previous best fitness
            if (Math.Abs(fitnessInTimeStep) < Math.Abs(this.bestFitness))
            {
                this.bestFitness = fitnessInTimeStep;

                for (int i = 0; i < this.numberOfGenes; i++)
                {
                    this.pbest[i] = this.genes[i];
                }
            }

        }

        public void Update_LinearDecreasingVelocity(Particle gbest, double wt)
        {
            int c1 = 2;
            int c2 = 2;
            double r1, r2;

            for(int i=0; i<this.numberOfGenes; i++)
            {
                RandomNumGenerator generator = new RandomNumGenerator();

                r1 = generator.Generate_Double();
                r2 = generator.Generate_Double();
                this.velocity[i] = this.velocity[i] * wt + ((c1 * r1 * (this.pbest[i] - this.genes[i])) + (c2 * r2 * (gbest.genes[i] - this.genes[i])));

                //clamping positive velocity if it is out of range
                if(this.velocity[i] > this.posClampVelocity)
                {
                    this.velocity[i] = this.posClampVelocity;
                }

                //clamping negative velocity if it is out of range
                if(this.velocity[i] < this.negClampVelocity)
                {
                    this.velocity[i] = this.negClampVelocity;
                }
            }
        }

        //method to update linear increasing velocity
        public void Update_LinearIncreasingVelocity(Particle gbest, double wt)
        {
            double r1, r2, fi1, fi2;

            for(int i=0;i<this.numberOfGenes;i++)
            {
                RandomNumGenerator generator = new RandomNumGenerator();
                r1 = generator.Generate_Double();
                r2 = generator.Generate_Double();
                fi1 = 1.5 * r1 + 0.5;
                fi2 = 1.5 * r2 + 0.5;

                double factor1 = fi1 * (this.pbest[i] - this.genes[i]);
                double factor2 = fi2 * (gbest.genes[i] - this.genes[i]);
                double factor3 = this.velocity[i] * wt;
                this.velocity[i] = factor1 + factor2 + factor3;

                //complete equation for velocity update
                //this.velocity[i] = this.velocity[i] * wt + (fi1 * (this.pbest[i] - this.genes[i]) + fi2 * (gbest.genes[i] - this.genes[i]));

                //clamping positive velocity if it is out of range
                if (this.velocity[i] > this.posClampVelocity)
                {
                    this.velocity[i] = this.posClampVelocity;
                }

                //clamping negative velocity if it is out of range
                if (this.velocity[i] < this.negClampVelocity)
                {
                    this.velocity[i] = this.negClampVelocity;
                }

            }
        }

        //method to update particle position
        public void Update_Position()
        {
            for(int i=0; i<this.numberOfGenes ; i++)
            {
                this.genes[i] = this.genes[i] + this.velocity[i];
            }
        }


        }
    }

