﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA_Visualization_Software.GA
{
    public class Individual
    {
        public double lowerRange;
        public double higherRange;

        public List<double> genes = new List<double>();
        public List<string> uniqueGenesFound = new List<string>();

        public string inputFunction;
        public string inputFunctionForEvaluation;
        public double bestFitness;                                  //variable that would store numerical value of particle best fitness 
        public int numberOfGenes;

        //Creating instance of ScriptControl class that would be used for evaluating particle fitness 
        MSScriptControl.ScriptControl sc = new MSScriptControl.ScriptControl();

        //creating instance of Ramdom class that would be used to generate random numbers
        //Random random = new Random();

        //list that is used to store unique random values in genes
        List<double> uniqueRandomNumbers = new List<double>();

        //For Roulette Wheel Selection
        public double wheel_lowerRange;
        public double wheel_higherRange;

        //constructor that initializes Individual values
        public Individual(List<string> unique_genes,int number_genes, double high_range, double low_range, string input_function,Random random)
        {
            double value;

            this.numberOfGenes = number_genes;
            this.higherRange = high_range;
            this.lowerRange = low_range;
            this.inputFunction = input_function;

            this.uniqueGenesFound = unique_genes;

            //Initializing genes values, adding unique genes values
            for (int i = 0; i < this.numberOfGenes; i++)
            {
                value = random.NextDouble() * (this.higherRange - this.lowerRange) + this.lowerRange;

                while (this.uniqueRandomNumbers.Contains(value))
                {
                    value = random.NextDouble() * (this.higherRange - this.lowerRange) + this.lowerRange;
                }

                this.uniqueRandomNumbers.Add(value);

                this.genes.Add(value);

            }
        }

        //Method to calculate fitness of Individual
        public double CalculateFitness()
        {

            //replacing genes values 
            string replacement = this.inputFunction;
            //string newFunc;

            for (int i = 0; i < this.numberOfGenes; i++)
            {
                if (i == 0)
                {
                    this.inputFunctionForEvaluation = replacement.Replace(uniqueGenesFound[i], this.genes[i].ToString());
                }
                else
                {
                    this.inputFunctionForEvaluation = this.inputFunctionForEvaluation.Replace(uniqueGenesFound[i], this.genes[i].ToString());
                }
            }

            this.sc.Language = "VBScript";

            object result = this.sc.Eval(this.inputFunctionForEvaluation);
            double fitness = Convert.ToDouble(result);

            return fitness;

        }
    }
}
