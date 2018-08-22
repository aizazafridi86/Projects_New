﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA_Visualization_Software.GA
{
    public class Offspring
    {
        public List<double> genes = new List<double>();
        public string inputFunctionForEvaluation;
        public List<string> uniqueGenesFound = new List<string>();
        public int numberOfGenes;
        public string inputFunction;

        //Creating instance of ScriptControl class that would be used for evaluating particle fitness 
        MSScriptControl.ScriptControl sc = new MSScriptControl.ScriptControl();

        //creating instance of Ramdom class that would be used to generate random numbers
        //Random random = new Random();

        //constructor that initializes offspring values
        public Offspring(List<string> unique_genes,string input_function, int number_genes)
        {
            this.uniqueGenesFound = unique_genes;
            this.inputFunction = input_function;
            this.numberOfGenes = number_genes;

            //initializing genes of offspring to 0
            for(int i=0; i<number_genes; i++)
            {
                this.genes.Add(0);
            }
        }

        //Method to calculate fitness of offsrping
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
