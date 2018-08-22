﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA_Visualization_Software.PSO_Algorithm
{
    public class Inertia
    {
        public double w0;
        public double wn;
        public double wt;
        public double diff;

        double numberOfTimeSteps;
        double currentTimeStep;

        //method to initialize values for linear decreasing inertia velocity update rule
        public void Initialize_LinearDecreasingInertia()
        {
            this.w0 = 0.9;
            this.wn = 0.4;
            this.wt = 0.9;
            this.diff = this.w0 - this.wn;
            
        }
        //method that returns wt value for linear decreasing velocity update rule
        public double Update_LinearDecreasingInertia(int numberOfTimeSteps, int currentTimeStep)
        {
            double sub = numberOfTimeSteps - currentTimeStep;
            double div = sub / numberOfTimeSteps;
            double add = this.diff * div;

            this.wt = this.wn + add;
            return this.wt;
            //this.wt = this.wn + this.diff*((numberOfTimeSteps-currentTimeStep)/numberOfTimeSteps);
        
        }

        //method to initialize values for linear increasing inertial velocity update rule
        public void Initialize_LinearIncreasingInertia()
        {
            this.w0 = 0.4;
            this.wn = 0.9;
            this.wt = 0.4;
            this.diff = this.w0 - this.wn;
        }

        //method that returns wt value for linear increasing velocity update rule
        public double Update_LinearIncreasingInertia(int numberOfTimeSteps, int currentTimeStep)
        {

            this.numberOfTimeSteps = Convert.ToDouble(numberOfTimeSteps);
            this.currentTimeStep = Convert.ToDouble(currentTimeStep);

            double div = this.currentTimeStep / this.numberOfTimeSteps;
            //double div = this.numberOfTimeSteps / this.currentTimeStep;
            double mul = div *this.diff;

            this.wt = this.w0-mul;
            
            return this.wt;
        }
    }
}
