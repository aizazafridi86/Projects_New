using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA_Visualization_Software.Common
{
    public class Output
    {
        //dictionaries to store the average fitness,best fitness and dispersion per time step
        public Dictionary<int, double> averageFitness = new Dictionary<int, double>();
        public Dictionary<int, double> bestFitness = new Dictionary<int, double>();
        public Dictionary<int, double> dispersion = new Dictionary<int, double>();

        //method to add the average fitness,best fitness and dispersion values to the dictionaries
        public void Add_Values(int timeStep, double average, double best, double disp)
        {
            this.averageFitness.Add(timeStep, average);
            this.bestFitness.Add(timeStep, best);
            this.dispersion.Add(timeStep, disp);
        }

        //method to add best particle fitness
        public void Add_Best(int timeStep, double best)
        {
            this.bestFitness.Add(timeStep, best);
        }

        public void Add_Average(int timeStep, double average)
        {
            this.averageFitness.Add(timeStep, average);
        }



    }
}
